using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptablePeopleAll : ScriptableObject {
	[UnityEngine.SerializeField]
	public List<PeopleData> m_data = new List<PeopleData>();

	[MenuItem("JJ/MakePeopleData")]
	public static void MakePeopleData()
	{
		ScriptablePeopleAll obj = ScriptableObject.CreateInstance<ScriptablePeopleAll>();
		UnityEditor.AssetDatabase.CreateAsset(obj, "Assets/Resources/Data/d_people_all.asset");
		
		for(int i = 0; i < 20; ++i)
		{
			PeopleData p = new PeopleData();
			p.m_staticStatistic.name = "name" + i;
			p.m_staticStatistic.id = i;
			p.m_staticStatistic.MoveSpeed = 10;

			p.CalStatistic();
			obj.m_data.Add(p);
		}
		UnityEditor.AssetDatabase.SaveAssets();
		UnityEditor.AssetDatabase.Refresh();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
