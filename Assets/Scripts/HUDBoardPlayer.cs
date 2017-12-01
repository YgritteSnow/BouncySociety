using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDBoardPlayer : MonoBehaviour
{
	/// <summary>
	/// 3D target that this object will be positioned above.
	/// </summary>

	public Transform target;

	/// <summary>
	/// Game camera to use.
	/// </summary>

	public static Camera gameCamera;

	/// <summary>
	/// UI camera to use.
	/// </summary>

	public static Camera uiCamera;

	/// <summary>
	/// Whether the children will be disabled when this object is no longer visible.
	/// </summary>

	public bool disableIfInvisible = true;

	public bool enableAll = true;
	public static float cullDistance = 0f;
	public static float cullZDepth = 6f;

	public Vector3 offset = Vector3.zero;

	Transform mTrans;
	bool mIsVisible = false;
	Transform mRootTrans;

	static int frameCount = 0;
	static int maxCountActivateInOnFrame = 2;
	static int currentCountActivateInOnFrame = 0;
	static Matrix4x4 cameraMatrix;
	static Matrix4x4 projectMatrix;

	/// <summary>
	/// Cache the transform;
	/// </summary>

	void Awake() { mTrans = transform; }

	/// <summary>
	/// Find both the UI camera and the game camera so they can be used for the position calculations
	/// </summary>

	void Start()
	{
		if (target != null)
		{
			//if (gameCamera == null) gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
			//if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
			SetVisible(false);
		}
		else
		{
			//Debug.LogError("Expected to have 'target' set to a valid transform", this);
			//enabled = false;
		}
		UIRoot root = NGUITools.FindInParents<UIRoot>(transform);
		if (root == null)
			return;
		mRootTrans = root.gameObject.transform;
	}

	/// <summary>
	/// Enable or disable child objects.
	/// </summary>

	public void SetVisible(bool val)
	{
		mIsVisible = val;

		for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
		{
			NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
		}
	}

	/// <summary>
	/// Update the position of the HUD object every frame such that is position correctly over top of its real world object.
	/// </summary>

	void LateUpdate()
	{
		if (mRootTrans == null)
			return;
		if (target == null)
			return;
		if (gameCamera == null)
			return;

		Vector3 tpos = target.position;
		tpos += offset;

		Vector3 pos = gameCamera.WorldToViewportPoint(tpos);

		// Determine the visibility and the target alpha
		bool isVisible = (gameCamera.orthographic || pos.z > 0) && (!disableIfInvisible || (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f));
		isVisible = enableAll && isVisible;

		if (isVisible && cullDistance > 0f)
		{
			Vector3 dist = gameCamera.transform.position - target.position;
			if (dist.sqrMagnitude > cullDistance * cullDistance)
				isVisible = false;
		}

		// Update the visibility flag
		if (mIsVisible != isVisible && isVisible == false)
			SetVisible(isVisible);

		if (isVisible)
		{
			if (frameCount != Time.frameCount)
			{
				frameCount = Time.frameCount;
				cameraMatrix = gameCamera.worldToCameraMatrix;
				projectMatrix = gameCamera.projectionMatrix;
				currentCountActivateInOnFrame = 0;
			}

			if (!mIsVisible && ++currentCountActivateInOnFrame <= maxCountActivateInOnFrame)
			{
				SetVisible(true);
			}

			if (mIsVisible)
			{
				Vector3 camp = cameraMatrix.MultiplyPoint3x4(tpos);
				Vector3 projpos = projectMatrix.MultiplyPoint(camp);
				
				mTrans.position = uiCamera.ViewportToWorldPoint(pos);
				pos = mTrans.localPosition;
				pos.x = Mathf.FloorToInt(pos.x + 0.5f);
				pos.y = Mathf.FloorToInt(pos.y + 0.5f);
				pos.z = projpos.z / mRootTrans.localScale.z; // mTrans.parent.localScale.z;
				mTrans.localPosition = pos;
				mTrans.localScale = Vector3.one;
			}
		}
	}

	/// <summary>
	/// Custom update function.
	/// </summary>

	protected virtual void OnUpdate(bool isVisible) { }
}
