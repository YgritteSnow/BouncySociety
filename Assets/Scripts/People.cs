using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using normtype = System.Single;
using disttype = System.Single;
using timetype = System.Single;

/**
 * 玩家数据
 */
[System.Serializable]
public class PeopleData
{
	public PeopleData()
	{
		m_staticStatistic = new StaticStatistic();
		m_dynamicStatistic = new DynamicStatistic();
		m_calculatedStatistic = new CalculatedStatistic();
	}
	// 固有属性
	[System.Serializable]
	public struct StaticStatistic
	{
		public string name; // 姓名
		public int id; // 编号

		public disttype MoveSpeed;	// 运动速度：物理运动速度
		public normtype Intellect;	// 智力程度：在最佳选择与最差选择之间的插值
		public timetype EmotionSpeed;	// 反应速度：情感反应速度
		public normtype Steadiness;	// 稳定程度：进行选择时的随机程度（方差）
	};

	// 某时刻的状态
	[System.Serializable]
	public struct DynamicStatistic
	{
		public normtype Happyness;	// 悲伤程度
		public normtype Madness;	// 愤怒程度
	};

	// 游戏所用内部状态
	[System.Serializable]
	public struct CalculatedStatistic
	{
		public disttype alert_distance; // 警戒距离，超过此距离将引起玩家注意
		public disttype safe_distance; // 安全距离，超过此距离将要进入攻击或者逃跑状态
		public normtype fear_limit; // 好奇界限，超出此数值将会逃跑而非攻击
		public normtype curious_limit; // 好奇界限，超过此数值将会巡逻而非静止
		public disttype curious_distance; // 好奇距离，巡逻范围
	}

	//[Header("固有性格")]
	[HideInInspector]
	public StaticStatistic m_staticStatistic;
	//[Header("当前状态")]
	private DynamicStatistic m_dynamicStatistic;
	//[Header("计算结果")]
	private CalculatedStatistic m_calculatedStatistic;

	public void CalStatistic()
	{
		m_calculatedStatistic.alert_distance = 4;
		m_calculatedStatistic.safe_distance = 2;
		m_calculatedStatistic.fear_limit = 2;
		m_calculatedStatistic.curious_limit = 2;
		m_calculatedStatistic.curious_distance = 2;
	}

	public timetype cur_madness { get{return m_dynamicStatistic.Madness;} set{m_dynamicStatistic.Madness = value;} }
	public Color cur_color { get { return Color.green * (1 - m_dynamicStatistic.Madness) + Color.red * m_dynamicStatistic.Madness; } }
	public disttype cur_speed { get { return m_staticStatistic.MoveSpeed; } }
	public disttype alert_dist { get { return m_calculatedStatistic.alert_distance; } }
	public disttype safe_dist { get { return m_calculatedStatistic.safe_distance; } }
}

#region 状态机
/**
 * 状态机
 */
public abstract class PeopleFSMState_base
{
	public PeopleFSMState_base(People obj) {
		m_obj = obj;
		m_vec_target = new List<GameObject>();
	}
	public PeopleFSMState_base(People obj, List<GameObject> target)
	{
		m_obj = obj;
		m_vec_target = target;
	}
	public People m_obj;
	public List<GameObject> m_vec_target = new List<GameObject>();
	
	public virtual void Update() {}
	public virtual void HandlerInput() {}
	public abstract PeopleFSMState_base CheckNextState();
	public virtual void OnEnterState() {}
	public virtual void OnLeaveState() {}

	public virtual void OnTriggerEnter(GameObject target)
	{
		m_vec_target.Add(target);
	}

	public virtual void OnTriggerExit(GameObject target)
	{
		m_vec_target.Remove(target);

		string s = "";
		foreach(var o in m_vec_target)
		{
				  s += o.name + ",";
		}
		Debug.Log(s);
	}
}

public class PeopleFMSState_stand : PeopleFSMState_base
{
	public PeopleFMSState_stand(People obj) : base(obj) {}
	public PeopleFMSState_stand(People obj, List<GameObject> target) : base(obj, target) {}

	public override PeopleFSMState_base CheckNextState()
	{
		if (m_vec_target.Count > 0)
		{
			return new PeopleFSMState_flee(m_obj, m_vec_target);
		}
		else
		{
			return null;
		}
	}

	public override void OnEnterState()
	{
		m_obj.m_data.cur_madness = Time.fixedTime % 1;
	}
}

public class PeopleFSMState_flee : PeopleFSMState_base
{
	public PeopleFSMState_flee(People obj, List<GameObject> target) : base(obj, target) { }

	public override void Update()
	{
		//m_obj.rb.velocity = new Vector3(m_obj.rb.velocity.x, m_obj.m_data.cur_speed, m_obj.rb.velocity.z);
	}

	public override PeopleFSMState_base CheckNextState()
	{
		if (m_vec_target.Count == 0)
		{
			return new PeopleFMSState_stand(m_obj);
		}
		else
		{
			return null;
		}
	}

	public override void OnEnterState()
	{
		m_obj.m_data.cur_madness = 1;
	}
}

public class PeopleFSMStateManager
{
	public PeopleFSMStateManager(People obj)
	{
		m_obj = obj;
	}
	
	public People m_obj;
	public List<PeopleFSMState_base> m_states = new List<PeopleFSMState_base>();
	public PeopleFSMState_base m_cur_state = null;

	public void OnTriggerEnter(GameObject target)
	{
		m_cur_state.OnTriggerEnter(target);
	}

	public void OnTriggerExit(GameObject target)
	{
		m_cur_state.OnTriggerExit(target);
	}

	public void Awake()
	{
		m_cur_state = new PeopleFMSState_stand(m_obj);
		m_cur_state.OnEnterState();
	}

	public void Update()
	{
		m_cur_state.Update();

		PeopleFSMState_base new_state = m_cur_state.CheckNextState();
		if(new_state != null)
		{
			m_cur_state.OnLeaveState();
			m_cur_state = new_state;
			m_cur_state.OnEnterState();
		}
	}
}
#endregion

[RequireComponent(typeof(Rigidbody), typeof(Transform), typeof(Renderer))]
public class People : MonoBehaviour {

	[UnityEngine.SerializeField]
	public PeopleData m_data;

	public Renderer rd;
	public Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rd = GetComponent<Renderer>();
	}

	void Awake()
	{
	}
	
	// Update is called once per frame
	void Update () {
		ResetColor();
	}

	void ResetColor()
	{
		// 重设物体颜色
		Color c = m_data.cur_color;
		rd.material.SetColor("_Color", c);
	}
}
