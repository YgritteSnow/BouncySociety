using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using UnityEditor;

#region 缓存池
public interface PoolData<T>
{
	void InitData(T info);
	void ClearData();
}
public class Pool<T, TInfo> where T:PoolData<TInfo>, new()
{
	private List<T> m_pool = new List<T>();
	private List<int> m_active_idx = new List<int>();
	public int Count { get{return m_pool.Count - m_active_idx.Count;} }
	public int Capacity { get { return m_pool.Count; } }

	private void MallocElement(TInfo info)
	{
		T data = new T();
		data.InitData(info);
		m_pool.Add(data);
		m_active_idx.Add(m_pool.Count-1);
	}
	public T CreateElement(TInfo info)
	{
		if(m_active_idx.Count == 0)
		{
			MallocElement(info);
		}
		int idx = m_active_idx[0];
		m_active_idx.RemoveAt(0);
		return m_pool[idx];
	}
	public void DestroyElement(T obj)
	{
		int idx = m_pool.IndexOf(obj);
		T data = m_pool[idx];
		data.ClearData();
		m_active_idx.Add(idx);
	}
}

/**
 * 人物和人物数据
 **/
public class PeopleObject : PoolData<PeopleData>
{
	public PeopleObject() { obj = null; }
	public GameObject obj;

	public void InitData(PeopleData info)
	{
		GameObject peoplePrefab = GameManager.Instance.peoplePrefab;
		float GameRectRadiusX = GameManager.Instance.GameRectRadiusX;
		float GameRectRadiusZ = GameManager.Instance.GameRectRadiusZ;
		float GameRectRadiusY = GameManager.Instance.GameRectRadiusY;
		Vector3 pos = new Vector3(Random.Range(-GameRectRadiusX, GameRectRadiusX), GameRectRadiusY, Random.Range(-GameRectRadiusZ, GameRectRadiusZ));

		if (obj == null)
		{
			obj = GameObject.Instantiate(peoplePrefab, pos, Quaternion.identity);
		}
		else
		{
			obj.transform.position = pos;
			obj.SetActive(true);
		}

		People pl = obj.GetComponent<People>();
		pl.m_data = info;
		PeopleAI ai = obj.GetComponent<PeopleAI>();
		ai.RefreshData();
	}
	public void ClearData()
	{
		obj.SetActive(false);
	}
}
#endregion

public class GameManager : MonoBehaviour {

	public float GameTime = 0;
	public float GameRectRadiusX = 250;
	public float GameRectRadiusZ = 250;
	public float GameRectRadiusY = 10;
	public float GameMaxPeople = 100;
	public bool pause = false;
	public GameObject peoplePrefab;

	public List<PeopleData> PeopleStock = new List<PeopleData>();
	public Pool<PeopleObject, PeopleData> PeoplePool = new Pool<PeopleObject, PeopleData>();

	public static GameManager s_instance = null;
	public static GameManager Instance { get { return s_instance; } }
	void Awake()
	{
		s_instance = this;
	}

	// Use this for initialization
	void Start () {
		LoadPeopleStock();
		StartCoroutine(this.CreatePeople());
	}

	void LoadPeopleStock()
	{
		ScriptablePeopleAll.MakePeopleData();
		ScriptablePeopleAll ass = Resources.Load<ScriptablePeopleAll>("Data/d_people_all") as ScriptablePeopleAll;
		PeopleStock = ass.m_data;
	}

	IEnumerator CreatePeople()
	{
		while(!pause){
			yield return new WaitForSeconds(0.1f);

			if(PeoplePool.Count < 2 && PeoplePool.Count < PeopleStock.Count)
			{
				PeoplePool.CreateElement(PeopleStock[PeoplePool.Count]);
			}
		}
	}
}
