using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	public float followDist = 4.0f;
	public float followSpeed = 10f;
	public float rotateSpeed = 1f;
	public Transform followTrans;

	public bool useSmooth = false;

	private Transform selfTrans;
	// Use this for initialization
	void Start () {
		selfTrans = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		if (Input.GetMouseButton(1))
		{
			selfTrans.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * rotateSpeed) * Time.deltaTime, Space.World);
			selfTrans.Rotate(Vector3.left * (Input.GetAxis("Mouse Y") * rotateSpeed) * Time.deltaTime, Space.Self);
		}

		if (followTrans)
		{
			float newDist = 0.0f;
			if(useSmooth)
			{
				float oldDist = (selfTrans.position - followTrans.position).magnitude;
				newDist = Mathf.Lerp(oldDist, followDist, 0.1f);
			}
			else
			{
				newDist = followDist;
			}
			selfTrans.position = followTrans.position - selfTrans.forward * newDist;
		}
	}
}
