using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostPlayerControl : MonoBehaviour
{
	public Transform cameraTrans;

	private float h = 0.0f;
	private float v = 0.0f;

	private Rigidbody rb;
	private People m_obj;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		m_obj = GetComponent<People>();
	}
	
	// Update is called once per frame
	void Update()
	{
		JoystickControl();
	}

	void JoystickControl()
	{
		if (cameraTrans)
		{
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
			Vector3 old_vel = rb.velocity;
			Vector3 new_vel = (cameraTrans.forward * v + cameraTrans.right * h) * m_obj.m_data.cur_speed;
			old_vel.x = new_vel.x;
			old_vel.z = new_vel.z;
			rb.velocity = old_vel;
		}

		if (Input.GetButton("Jump"))
		{
			rb.velocity = new Vector3(rb.velocity.x, m_obj.m_data.cur_speed, rb.velocity.z);
		}
	}
}
