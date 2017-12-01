using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LAYER : int
{
	BUILDING = 8,
	PLAYER = 9,
}

[RequireComponent(typeof(People))]
public class PeopleAI : MonoBehaviour {
	public People m_obj = null;

	public SphereCollider m_col_alert = null;
	public SphereCollider m_col_safe = null;

	private PeopleFSMStateManager m_state_manager = null;

	// Use this for initialization
	void Awake () {
		if(m_obj == null)
		{
			m_obj = GetComponent<People>();
		}

		if (m_col_alert == null)
		{
			m_col_alert = gameObject.AddComponent<SphereCollider>();
			m_col_alert.isTrigger = true;
			m_col_alert.radius = m_obj.m_data.alert_dist;
		}

		if (m_col_safe == null)
		{
			m_col_safe = gameObject.AddComponent<SphereCollider>();
			m_col_safe.isTrigger = true;
			m_col_safe.radius = m_obj.m_data.safe_dist;
		}

		m_state_manager = new PeopleFSMStateManager(m_obj);
		m_state_manager.Awake();
	}

	public void RefreshData()
	{
		if (m_col_alert == null)
		{
			m_col_alert = gameObject.AddComponent<SphereCollider>();
			m_col_alert.isTrigger = true;
		}
		m_col_alert.radius = m_obj.m_data.alert_dist;

		if (m_col_safe == null)
		{
			m_col_safe = gameObject.AddComponent<SphereCollider>();
			m_col_safe.isTrigger = true;
		}
		m_col_safe.radius = m_obj.m_data.safe_dist;
	}
	
	// Update is called once per frame
	void Update () {
		m_state_manager.Update();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == (int)LAYER.PLAYER)
		{
			m_state_manager.OnTriggerEnter(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == (int)LAYER.PLAYER)
		{
			m_state_manager.OnTriggerExit(other.gameObject);
		}
	}
}
