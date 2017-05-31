using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	[SerializeField]
	Transform m_currentAnimalLocation;
	[SerializeField]
	Transform m_nextAnimalLocation;
	[SerializeField]
	Transform m_topSetLocation;
	[SerializeField]
	Transform m_bottomSetLocation;
	[SerializeField]
	GameObject m_setPrefab;
	[SerializeField]
	GameObject m_animalPrefab;
	[SerializeField]
	int m_numberOfSets = 4;



	// Use this for initialization
	void Start () 
	{
		BuildScene();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void BuildScene()
	{
		for(int i=0; i<m_numberOfSets; i++)
		{
			Vector3 pos = LerpVec3(m_topSetLocation.localPosition, m_bottomSetLocation.localPosition, ((float)i)/(m_numberOfSets-1));
			GameObject set = Instantiate(m_setPrefab, pos, Quaternion.identity);
			set.transform.SetParent(m_topSetLocation.parent);
		}

		ShowCurrentAnimal();
	}

	Vector3 LerpVec3(Vector3 a, Vector3 b, float p)
	{
		return new Vector3(Lerp(a.x,b.x,p), Lerp(a.y,b.y,p), Lerp(a.z,b.z,p));
	}

	float Lerp(float a, float b, float p)
	{
		return a + (b-a)*p;
	}

	void ShowCurrentAnimal()
	{
		GameObject animal = Instantiate(m_animalPrefab, Vector3.zero, Quaternion.identity);
		animal.transform.SetParent(m_currentAnimalLocation);
		animal.transform.localPosition = Vector3.zero;
	}

}
