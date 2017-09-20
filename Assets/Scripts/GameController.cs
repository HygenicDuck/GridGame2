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
	public Transform[] m_animalQueueLocations;
	[SerializeField]
	Transform m_topLeftOfGrid;
	[SerializeField]
	Transform m_bottomRightOfGrid;
	[SerializeField]
	GameObject m_setPrefab;
	[SerializeField]
	GameObject m_emptyCellPrefab;
	[SerializeField]
	GameObject m_animalPrefab;
	[SerializeField]
	GameObject m_shapePrefab;
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
	ShapeQueue m_shapeQueue;


	// Use this for initialization
	void Start () 
	{
		m_shapeQueue = new ShapeQueue();
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
		IntVec2 gridPos = GetGridPosFromTouchPosition(touchPos);
		if (!gridPos.IsInvalid ())
		{
			TryToPlacePiece (gridPos, 0);
		}
	}

	bool TryToPlacePiece(IntVec2 gridPos, int queuePosition)
	{
		ShapeDef nextShape = m_shapeQueue.QueuePosition(queuePosition);
		if (OKToDropShapeHere (gridPos, nextShape))
		{
			DropShapeOnGrid (gridPos, nextShape);

			m_shapeQueue.PopFromQueue(queuePosition);
			Transform piecePos = m_grid[gridPos.x,gridPos.y].transform;
			MovePieceIntoPlace(piecePos, queuePosition);
			StartCoroutine(ScrollAnimalQueueCoRoutine(queuePosition));

			return true;
		}

//		CellController cellController = m_grid[gridPos.x,gridPos.y].GetComponent<CellController>();
//		AnimalDef nextAnimal = m_animalQueue.QueuePosition(queuePosition);
//		cellController.AddAnimal(m_animalPrefab, nextAnimal);

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


	void DropShapeOnGrid(IntVec2 gridPos, ShapeDef shapeDef)
	{
		Debug.Log ("DropShapeOnGrid "+gridPos.x+", "+gridPos.y);

		string defString = ShapeAssetManager.Instance.GetShapeString(shapeDef.m_shapeType);

		char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
		string[] lines = defString.Split (delimiterChars);

		int numLines = lines.Length;
		int minLine = -((numLines-1)/2);
		for (int y = 0; y < numLines; y++)
		{
			string s = lines [y];
			int numChars = s.Length;
			int minChar = -((numChars-1)/2);
			for (int x = 0; x < numChars; x++)
			{
				if (s [x] == '1')
				{
					IntVec2 cellGridPos = new IntVec2(x + minChar + gridPos.x, y + minLine + gridPos.y);
					if ((cellGridPos.x >= 0) && (cellGridPos.y >= 0) && (cellGridPos.x < m_gridXDim) && (cellGridPos.y < m_gridYDim))
					{
						CellController cell = m_grid [cellGridPos.x, cellGridPos.y].GetComponent<CellController> ();
						cell.SetColor (Color.red);
						cell.SetFilledStatus (true);
					}
				}
			}
		}
	}

	bool OKToDropShapeHere(IntVec2 gridPos, ShapeDef shapeDef)
	{
		string defString = ShapeAssetManager.Instance.GetShapeString(shapeDef.m_shapeType);

		char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
		string[] lines = defString.Split (delimiterChars);

		Vector2 gridPitch = GameController.Instance.GridPitch ();

		int numLines = lines.Length;
		int minLine = -((numLines-1)/2);
		for (int y = 0; y < numLines; y++)
		{
			string s = lines [y];
			int numChars = s.Length;
			int minChar = -((numChars-1)/2);
			for (int x = 0; x < numChars; x++)
			{
				if (s [x] == '1')
				{
					IntVec2 cellGridPos = new IntVec2(x + minChar + gridPos.x, y + minLine + gridPos.y);
					if ((cellGridPos.x < 0) || (cellGridPos.y < 0) || (cellGridPos.x >= m_gridXDim) || (cellGridPos.y >= m_gridYDim))
					{
						return false;
					}

					CellController cell = m_grid [cellGridPos.x, cellGridPos.y].GetComponent<CellController> ();
					if (cell.GetFilledStatus () == true)
					{
						return false;
					}
				}
			}
		}

		return true;
	}



	IntVec2 GetGridPosFromTouchPosition(Vector3 touchPos)
	{
		IntVec2 gridPos;

		Vector3 screenPos = m_camera.WorldToScreenPoint(m_topLeftOfGrid.position);
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
		piece.transform.localPosition = Vector3.zero;
		int queueItemIndex = QueuePositionFromGameObject(piece);

		return TryToPlacePiece(gridPos, queueItemIndex);
	}

//	SetController.PlaceAnimalResult ChooseSet(int setNum, int queueItem = 0)
//	{
//		if ((setNum < 0) || (setNum >= m_numberOfSets))
//		{
//			return SetController.PlaceAnimalResult.NOT_PLACED;
//		}
//		
//		SetController setController = m_sets[setNum].GetComponent<SetController>();
//
//		AnimalDef nextAnimal = m_animalQueue.QueuePosition(queueItem);
//		return setController.AddAnimal(m_animalPrefab, nextAnimal);
//	}


	public Vector2 GridPitch()
	{
		Vector2 pitch = new Vector2 ((m_topLeftOfGrid.localPosition.x - m_bottomRightOfGrid.localPosition.x)/(m_gridXDim-1), (m_topLeftOfGrid.localPosition.y - m_bottomRightOfGrid.localPosition.y)/(m_gridYDim-1));
		return pitch;
	}


	void BuildScene()
	{
		m_grid = new GameObject[m_gridXDim, m_gridYDim];

		for(int y=0; y<m_gridYDim; y++)
		{
			for(int x=0; x<m_gridXDim; x++)
			{
				float posX = Utils.Lerp (m_topLeftOfGrid.localPosition.x, m_bottomRightOfGrid.localPosition.x, ((float)x) / (m_gridXDim - 1));
				float posY = Utils.Lerp (m_topLeftOfGrid.localPosition.y, m_bottomRightOfGrid.localPosition.y, ((float)y) / (m_gridYDim - 1));
				Vector3 pos = new Vector3(posX, posY, 0f);
				GameObject cell = Instantiate(m_emptyCellPrefab, pos, Quaternion.identity);
				cell.transform.SetParent(m_topLeftOfGrid.parent);
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

		ShowCurrentShapes();
	}





	void ShowCurrentShapes()
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

			GameObject shape = Instantiate(m_shapePrefab, Vector3.zero, Quaternion.identity);
			shape.transform.SetParent(location);
			shape.transform.localPosition = Vector3.zero;
			shape.GetComponent<Shape>().SetDef(m_shapeQueue.QueuePosition(queuePos));
		}
	}

//	void ShowCurrentAnimals()
//	{
//		for(int queuePos = 0; queuePos < m_animalQueueLocations.Length; queuePos++)
//		{
//			Transform location = m_animalQueueLocations[queuePos];
//
//			// destroy any existing children
//			for ( int i = location.childCount-1; i>=0; --i )
//			{
//				GameObject child = location.GetChild(i).gameObject;
//				Destroy( child );
//			}
//
//			GameObject animal = Instantiate(m_animalPrefab, Vector3.zero, Quaternion.identity);
//			animal.transform.SetParent(location);
//			animal.transform.localPosition = Vector3.zero;
//			animal.GetComponent<Animal>().SetDef(m_animalQueue.QueuePosition(queuePos));
//		}
//	}
		
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
		ShowCurrentShapes();
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
