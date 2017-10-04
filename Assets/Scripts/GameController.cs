﻿using System.Collections;
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
	public Transform[] m_animalQueueLocations;
	[SerializeField]
	Transform m_topSetLocation;
	[SerializeField]
	Transform m_bottomSetLocation;
	[SerializeField]
	GameObject m_setPrefab;
	[SerializeField]
	GameObject m_emptyCellPrefab;
	[SerializeField]
	GameObject m_animalPrefab;
	[SerializeField]
	int m_gridXDim = 5;
	[SerializeField]
	int m_gridYDim = 5;
	[SerializeField]
	int m_numberOfSets = 4;
	[SerializeField]
	Camera m_camera;

	bool m_touching = false;
	float m_touchTime = 0f;
	Vector3 m_touchPosition;
	GameObject[] m_sets;
	GameObject[,] m_grid;
	AnimalQueue m_animalQueue;


	// Use this for initialization
	void Start () 
	{
		m_animalQueue = new AnimalQueue();
		InitQueueTimers ();
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
		// TEMP - I've disabled this. No tapping on the grid to drop the animal at the head of the queue.
//		IntVec2 gridPos = new IntVec2(-1, -1);	//GetGridPosFromTouchPosition(touchPos);
//		AnimalDef droppedAnimalDef;
//		droppedAnimalDef = m_animalQueue.QueuePosition (0);
//		TryToPlacePiece(gridPos, 0, droppedAnimalDef);
	}





	const int QUEUE_LENGTH = 4;
	float[] m_queueTimers = new float[QUEUE_LENGTH];

	void InitQueueTimers()
	{
		for (int i = 0; i < QUEUE_LENGTH; i++)
		{
			m_queueTimers[i] = 0f;
		}
	}

	void SetQueueTimers()
	{
		GameObject animal;

		for (int i = 0; i < QUEUE_LENGTH; i++)
		{
			animal = m_animalQueueLocations [i].GetChild (0).gameObject;
			animal.GetComponent<Animal> ().SetEggTime (m_queueTimers [i]);
		}
	}

	void GetQueueTimers(int usedPieceIndex)
	{
		GameObject animal;

		for (int i = 0; i < usedPieceIndex; i++)
		{
			animal = m_animalQueueLocations [i].GetChild (0).gameObject;
			m_queueTimers [i] = animal.GetComponent<Animal> ().GetEggTime ();
		}

		for(int i=usedPieceIndex; i<QUEUE_LENGTH-1; i++)
		{
			animal = m_animalQueueLocations [i+1].GetChild (0).gameObject;
			m_queueTimers [i] = animal.GetComponent<Animal> ().GetEggTime ();
		}

		m_queueTimers[QUEUE_LENGTH-1] = 0f;
	}




	bool TryToPlacePiece(IntVec2 gridPos, IntVec2 previousGridPos, int queuePosition, AnimalDef animalDef)
	{
		if (!gridPos.IsInvalid ())
		{
			CellController cellController = m_grid [gridPos.x, gridPos.y].GetComponent<CellController> ();
			AnimalDef existingDef = cellController.GetAnimalDef ();
			//AnimalDef nextAnimal = m_animalQueue.QueuePosition(queuePosition);

			if ((existingDef != null) && !cellController.CanEvolve(animalDef))
			{
				CellController cellController2 = m_grid [previousGridPos.x, previousGridPos.y].GetComponent<CellController> ();
				cellController2.AddAnimal (m_animalPrefab, existingDef);
			}

			cellController.AddAnimal (m_animalPrefab, animalDef);

			if (queuePosition >= 0)
			{
				// dropped an animal from the queue. Remove it from the queue and scroll items behind it along one.
				GetQueueTimers(queuePosition);
				m_animalQueue.PopFromQueue (queuePosition);
				Transform piecePos = m_grid [gridPos.x, gridPos.y].transform;
				MovePieceIntoPlace (piecePos, queuePosition);
				StartCoroutine (ScrollAnimalQueueCoRoutine (queuePosition));

				return true;
			}
		}

//		SetController.PlaceAnimalResult placeResult = ChooseSet(setNum, queuePosition);
//		if (placeResult != SetController.PlaceAnimalResult.NOT_PLACED)
//		{
//			if (placeResult == SetController.PlaceAnimalResult.MATCHING_SET_COMPLETED)
//			{
//				m_animalQueue.AddNewSpeciesToDeck(nextAnimal);
//			}
//
//			m_animalQueue.PopFromQueue(queuePosition);
//			Transform piecePos = m_sets[setNum].GetComponent<SetController>().GetPlacedPiecePosition();
//			MovePieceIntoPlace(piecePos, queuePosition);
//			StartCoroutine(ScrollAnimalQueueCoRoutine(queuePosition));
//
//			return true;
//		}

		return false;
	}



	IntVec2 FindAnimalInGrid(Animal animal)
	{
		// tries to find the gameobject in the grid
		for(int y=0; y<m_gridYDim; y++)
		{
			for(int x=0; x<m_gridXDim; x++)
			{
				Animal cellAnimal = m_grid [x, y].GetComponentInChildren<Animal> ();
				if (cellAnimal == animal)
				{
					return new IntVec2 (x, y);
				}
			}
		}

		return new IntVec2 (-1, -1);
	}

	IntVec2 GetGridPosFromTouchPosition(Vector3 touchPos)
	{
		IntVec2 gridPos;

		Vector3 screenPos = m_camera.WorldToScreenPoint(m_topSetLocation.position);
		Vector3 pos0 = m_camera.WorldToScreenPoint(m_grid[0,0].transform.position);
		Vector3 pos1 = m_camera.WorldToScreenPoint(m_grid[1,1].transform.position);
		float yPitch = pos0.y - pos1.y;
		float xPitch = pos0.x - pos1.x;
		Debug.Log("yPitch = "+yPitch);
		Debug.Log("xPitch = "+xPitch);

		float dy = pos0.y + yPitch/2 - touchPos.y;
		gridPos.y = (int)(dy / yPitch);
		float dx = pos0.x + xPitch/2 - touchPos.x;
		gridPos.x = (int)(dx / xPitch);

		if ((gridPos.x >= m_gridXDim) || (gridPos.x < 0) || (gridPos.y >= m_gridYDim) || (gridPos.y < 0))
			gridPos = new IntVec2(-1, -1);

		Debug.Log("gridpos = "+gridPos.x+", "+gridPos.y);

		return gridPos;
	}

	// **** to be deleted

//	int GetSetFromTouchPosition(Vector3 touchPos)
//	{
//		Vector3 screenPos = m_camera.WorldToScreenPoint(m_topSetLocation.position);
//		Vector3 pos0 = m_camera.WorldToScreenPoint(m_sets[0].transform.position);
//		Vector3 pos1 = m_camera.WorldToScreenPoint(m_sets[1].transform.position);
//		float setWidth = pos0.y - pos1.y;
//		Debug.Log("setWidth = "+setWidth);
//
//		float dy = pos0.y + setWidth/2 - touchPos.y;
//		int setNum = (int)(dy / setWidth);
//
//		if ((setNum >= m_numberOfSets) || (setNum < 0)) 
//			setNum = -1;
//		
//		return setNum;
//	}

	public bool PieceDropped(GameObject piece)
	{
		// returns false if it wasn't dropped in a valid location
		Vector3 touchPos = m_camera.WorldToScreenPoint(piece.transform.position);
		IntVec2 gridPos = GetGridPosFromTouchPosition(touchPos);
		IntVec2 previousGridPos = FindAnimalInGrid (piece.GetComponent<Animal> ());
		piece.transform.localPosition = Vector3.zero;
		int queueItemIndex = QueuePositionFromGameObject(piece);

		AnimalDef droppedAnimalDef;
		if (queueItemIndex >= 0)
		{
			droppedAnimalDef = m_animalQueue.QueuePosition (queueItemIndex);
		} else
		{
			Animal droppedAnimal = piece.GetComponent<Animal> ();
			droppedAnimalDef = droppedAnimal.GetDef ();
		}

		if (!gridPos.IsInvalid ())
		{
			// destroy the piece that you were carrying.
			piece.transform.parent = null;
			Destroy (piece);
		}

		return TryToPlacePiece(gridPos, previousGridPos, queueItemIndex, droppedAnimalDef);
	}

	SetController.PlaceAnimalResult ChooseSet(int setNum, int queueItem = 0)
	{
		if ((setNum < 0) || (setNum >= m_numberOfSets))
		{
			return SetController.PlaceAnimalResult.NOT_PLACED;
		}
		
		SetController setController = m_sets[setNum].GetComponent<SetController>();

		AnimalDef nextAnimal = m_animalQueue.QueuePosition(queueItem);
		return setController.AddAnimal(m_animalPrefab, nextAnimal);
	}



	void BuildScene()
	{
		m_grid = new GameObject[m_gridXDim, m_gridYDim];

		for(int y=0; y<m_gridYDim; y++)
		{
			for(int x=0; x<m_gridXDim; x++)
			{
				float posX = Utils.Lerp (m_topSetLocation.localPosition.x, m_bottomSetLocation.localPosition.x, ((float)x) / (m_gridXDim - 1));
				float posY = Utils.Lerp (m_topSetLocation.localPosition.y, m_bottomSetLocation.localPosition.y, ((float)y) / (m_gridYDim - 1));
				Vector3 pos = new Vector3(posX, posY, 0f);
				GameObject cell = Instantiate(m_emptyCellPrefab, pos, Quaternion.identity);
				cell.transform.SetParent(m_topSetLocation.parent);
				m_grid[x,y] = cell;
			}
		}
				
		// to be deleted
//		m_sets = new GameObject[m_numberOfSets];
//
//		for(int i=0; i<m_numberOfSets; i++)
//		{
//			Vector3 pos = Utils.LerpVec3(m_topSetLocation.localPosition, m_bottomSetLocation.localPosition, ((float)i)/(m_numberOfSets-1));
//			GameObject set = Instantiate(m_setPrefab, pos, Quaternion.identity);
//			set.transform.SetParent(m_topSetLocation.parent);
//			m_sets[i] = set;
//		}

		ShowCurrentAnimals();
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
			animal.GetComponent<Animal>().EnableEggAndTimer (true, m_queueTimers[queuePos]);
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

		return -1;
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
