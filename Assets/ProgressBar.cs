using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

	[SerializeField]
	GameObject m_bar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetScale(float scale)
	{
		m_bar.transform.localScale = new Vector3 (scale, 1f, 1f);
	}
}
