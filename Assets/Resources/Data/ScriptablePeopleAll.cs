using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptablePeopleAll : ScriptableObject {
	[UnityEngine.SerializeField]
	public List<PeopleData> m_data = new List<PeopleData>();

	[MenuItem("JJ/MakePeopleData")]
	public static ScriptablePeopleAll MakePeopleData(int count)
	{
		ScriptablePeopleAll obj = ScriptableObject.CreateInstance<ScriptablePeopleAll>();
		
		for(int i = 0; i < count; ++i)
		{
			PeopleData p = new PeopleData();
			p.m_staticStatistic.name = "name" + i;
			p.m_staticStatistic.id = i;
			p.m_staticStatistic.MoveSpeed = 3;
			p.m_staticStatistic.Power = 0.01f;

			p.CalStatistic();
			obj.m_data.Add(p);
		}

#if UnityEditor
		UnityEditor.AssetDatabase.CreateAsset(obj, "Assets/Resources/Data/d_people_all.asset");
		UnityEditor.AssetDatabase.SaveAssets();
		UnityEditor.AssetDatabase.Refresh();
#endif
		return obj;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
