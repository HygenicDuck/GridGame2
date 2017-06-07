using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour 
{
	static GameController m_instance = null;

	GameController()
	{
		m_instance = this;
	}

	public static GameController Instance
	{
		get
		{
			return m_instance;
		}
	}




	[SerializeField]
	Transform[] m_animalQueueLocations;
//	[SerializeField]
//	Transform m_currentAnimalLocation;
//	[SerializeField]
//	Transform m_nextAnimalLocation;
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
	[SerializeField]
	Camera m_camera;

	bool m_touching = false;
	float m_touchTime = 0f;
	Vector3 m_touchPosition;
	GameObject[] m_sets;
	AnimalQueue m_animalQueue;


	// Use this for initialization
	void Start () 
	{
		m_animalQueue = new AnimalQueue();
		BuildScene();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			m_touching = true;
			m_touchPosition = Input.mousePosition;

			Debug.Log("screen size = x="+Screen.width+", y="+Screen.height);
			Debug.Log("m_touchDownPosition = x="+m_touchPosition.x+", y="+m_touchPosition.y);

//			Vector3 screenPos = m_camera.WorldToScreenPoint(m_topSetLocation.position);
//			Debug.Log("screenPos = x="+screenPos.x+", y="+screenPos.y);
			RespondToTouch(m_touchPosition);

			m_touchTime = Time.timeSinceLevelLoad;
		}
	}

	void RespondToTouch(Vector3 touchPos)
	{
		int setNum = GetSetFromTouchPosition(touchPos);
//		Vector3 screenPos = m_camera.WorldToScreenPoint(m_topSetLocation.position);
//		Vector3 pos0 = m_camera.WorldToScreenPoint(m_sets[0].transform.position);
//		Vector3 pos1 = m_camera.WorldToScreenPoint(m_sets[1].transform.position);
//		float setWidth = pos0.y - pos1.y;
//		Debug.Log("setWidth = "+setWidth);
//
//		float dy = pos0.y + setWidth/2 - touchPos.y;
//		int setNum = (int)(dy / setWidth);
//		Debug.Log("dy = "+dy+", setNum = "+setNum);

		if (ChooseSet(setNum))
		{
			m_animalQueue.PopFromQueue(0);
			Transform piecePos = m_sets[setNum].GetComponent<SetController>().GetPlacedPiecePosition();
			MovePieceIntoPlace(piecePos, 0);
			StartCoroutine(ScrollAnimalQueueCoRoutine(0));
		}
	}

	int GetSetFromTouchPosition(Vector3 touchPos)
	{
		Vector3 screenPos = m_camera.WorldToScreenPoint(m_topSetLocation.position);
		Vector3 pos0 = m_camera.WorldToScreenPoint(m_sets[0].transform.position);
		Vector3 pos1 = m_camera.WorldToScreenPoint(m_sets[1].transform.position);
		float setWidth = pos0.y - pos1.y;
		Debug.Log("setWidth = "+setWidth);

		float dy = pos0.y + setWidth/2 - touchPos.y;
		int setNum = (int)(dy / setWidth);

		if ((setNum >= m_numberOfSets) || (setNum < 0)) 
			setNum = -1;
		
		return setNum;
	}

	public bool PieceDropped(GameObject piece)
	{
		// returns false if it wasn't dropped in a valid location
		Vector3 touchPos = m_camera.WorldToScreenPoint(piece.transform.position);
		int setNum = GetSetFromTouchPosition(touchPos);
		piece.transform.localPosition = Vector3.zero;
		int queueItemIndex = QueuePositionFromGameObject(piece);
		if (ChooseSet(setNum, queueItemIndex))
		{
			m_animalQueue.PopFromQueue(queueItemIndex);
			Transform piecePos = m_sets[setNum].GetComponent<SetController>().GetPlacedPiecePosition();
			MovePieceIntoPlace(piecePos, queueItemIndex);
			StartCoroutine(ScrollAnimalQueueCoRoutine(queueItemIndex));
			return true;
		}
		return false;
	}

	bool ChooseSet(int setNum, int queueItem = 0)
	{
		if ((setNum < 0) || (setNum >= m_numberOfSets))
			return false;
		
		SetController setController = m_sets[setNum].GetComponent<SetController>();

		AnimalDef nextAnimal = m_animalQueue.QueuePosition(queueItem);
		return setController.AddAnimal(m_animalPrefab, nextAnimal);
	}

	void BuildScene()
	{
		m_sets = new GameObject[m_numberOfSets];

		for(int i=0; i<m_numberOfSets; i++)
		{
			Vector3 pos = LerpVec3(m_topSetLocation.localPosition, m_bottomSetLocation.localPosition, ((float)i)/(m_numberOfSets-1));
			GameObject set = Instantiate(m_setPrefab, pos, Quaternion.identity);
			set.transform.SetParent(m_topSetLocation.parent);
			m_sets[i] = set;
		}

		ShowCurrentAnimals();
	}

	Vector3 LerpVec3(Vector3 a, Vector3 b, float p)
	{
		return new Vector3(Lerp(a.x,b.x,p), Lerp(a.y,b.y,p), Lerp(a.z,b.z,p));
	}

	float Lerp(float a, float b, float p)
	{
		return a + (b-a)*p;
	}

	void ShowCurrentAnimals()
	{
		for(int queuePos = 0; queuePos < m_animalQueueLocations.Length; queuePos++)
		{
			Transform location = m_animalQueueLocations[queuePos];

			// destroy any existing children
			for ( int i = location.childCount-1; i>=0; --i )
			{
				GameObject child = location.GetChild(i).gameObject;
				Destroy( child );
			}

			GameObject animal = Instantiate(m_animalPrefab, Vector3.zero, Quaternion.identity);
			animal.transform.SetParent(location);
			animal.transform.localPosition = Vector3.zero;
			animal.GetComponent<Animal>().SetDef(m_animalQueue.QueuePosition(queuePos));
		}
	}
		
	void ScrollAnimalQueue(float duration, int usedPieceIndex)
	{
		// graphically moves each member of the queue forward
		for(int queuePos = usedPieceIndex+1; queuePos < m_animalQueueLocations.Length; queuePos++)
		{
			Vector3 nextLocation = m_animalQueueLocations[queuePos-1].position;
			Vector3 currentLocation = m_animalQueueLocations[queuePos].position;
			Vector3 dPos = nextLocation - currentLocation;

			Transform location = m_animalQueueLocations[queuePos];

			for ( int i = location.childCount-1; i>=0; --i )
			{
				GameObject child = location.GetChild(i).gameObject;
				Mover mover = child.GetComponent<Mover>();
				if (mover != null)
				{
					mover.MoveBy(dPos, duration);
				}
			}
		}
	}

	public int QueuePositionFromGameObject(GameObject gameObject)
	{
		for(int queuePos = 0; queuePos < m_animalQueueLocations.Length; queuePos++)
		{
			Transform location = m_animalQueueLocations[queuePos];

			for ( int i = location.childCount-1; i>=0; --i )
			{
				GameObject child = location.GetChild(i).gameObject;
				if (child == gameObject)
					return queuePos;
			}
		}

		return 0;
	}		



	IEnumerator ScrollAnimalQueueCoRoutine(int usedPieceIndex)
	{
		ScrollAnimalQueue(0.5f, usedPieceIndex);
		yield return new WaitForSeconds(0.5f);
		ShowCurrentAnimals();
	}

	void MovePieceIntoPlace(Transform piecePos, int usedPieceIndex)
	{
		Transform location = m_animalQueueLocations[usedPieceIndex];
		Vector3 nextLocation = piecePos.position;
		Vector3 currentLocation = location.position;
		Vector3 dPos = nextLocation - currentLocation;

		for ( int i = location.childCount-1; i>=0; --i )
		{
			GameObject child = location.GetChild(i).gameObject;
			Mover mover = child.GetComponent<Mover>();
			if (mover != null)
			{
				mover.MoveBy(dPos, 0.2f);
			}
		}
	}
}
