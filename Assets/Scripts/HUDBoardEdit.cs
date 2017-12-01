using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDBoardEdit : MonoBehaviour {
	private UILabel m_talk;
	private UILabel m_name;

	private GameObject m_talkBoard;

	// Use this for initialization
	void Awake () {
		m_talkBoard = gameObject.transform.Find("Widget/talk").gameObject;
		m_talk = m_talkBoard.GetComponent<UILabel>();

		m_name = gameObject.transform.Find("Widget/name").GetComponent<UILabel>();
	}

	public void SetTalk(string str)
	{
		m_talk.text = str;
	}

	public void SetName(string str)
	{
		m_name.text = str;
	}

	public void ShowTalk(bool bShow)
	{
		m_talkBoard.SetActive(bShow);
	}
}
