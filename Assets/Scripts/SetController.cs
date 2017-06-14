using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class SetController : MonoBehaviour 
{
	[SerializeField]
	Transform[] m_slots;
	[SerializeField]
	GameObject m_newAnimalPrefab;

	int[] m_slotContents;
	int m_setAnimalID = -1;
	Transform m_placedPiecePosition = null;

	public enum PlaceAnimalResult
	{
		NOT_PLACED = 0,
		PLACED,
		SET_COMPLETED,
		MATCHING_SET_COMPLETED
	}




	void Start()
	{
		m_slotContents = new int[m_slots.Length];
		ClearSet();
	}
		
	void ClearSet()
	{
		for (int i=0; i<m_slotContents.Length; i++)
		{
			m_slotContents[i] = -1;
		}

		for (int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slots[i] != null)
			{
				Transform location = m_slots[i];
				for (int j = location.childCount-1; j>=0; --j)
				{
					GameObject child = location.GetChild(j).gameObject;
					Destroy( child );
				}
			}
		}

		m_setAnimalID = -1;
	}


	public PlaceAnimalResult AddAnimal(GameObject prefab, AnimalDef animalDef)
	{
		int animalID = (int)animalDef.m_animalType;
		int animalColor = (int)animalDef.m_colour;

		if (m_setAnimalID == -1)
		{
			m_setAnimalID = animalID;
		}

//		if (m_setAnimalID != animalID)
//		{
//			// can't add this animal to this set
//			m_placedPiecePosition = null;
//			return false;
//		}

		if (m_slotContents[animalColor] != -1)
		{
			// already have this color
			m_placedPiecePosition = null;
			return PlaceAnimalResult.NOT_PLACED;
		}

		m_slotContents[animalColor] = animalID;

		GameObject animal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		animal.transform.SetParent(m_slots[animalColor]);
		animal.transform.localPosition = Vector3.zero;

		m_placedPiecePosition = m_slots[animalColor];

		Animal animalController = animal.GetComponent<Animal>();
		animalController.SetDef(animalDef);

		return CheckForCompletedSet();
	}


	GameObject ShowEvolvedAnimal(AnimalDef animalDef)
	{
		GameObject animal = Instantiate(m_newAnimalPrefab, Vector3.zero, Quaternion.identity);
		animal.transform.SetParent(transform);
		animal.transform.localPosition = new Vector3(0f,0f,-1.5f);
		animal.transform.localScale = new Vector3(1f,1f);

		Animal animalController = animal.GetComponent<Animal>();
		animalController.SetDef(animalDef);

		return animal;
	}

	PlaceAnimalResult CheckForCompletedSet()
	{
		for (int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slotContents[i] == -1)
			{
				return PlaceAnimalResult.PLACED;
			}
		}

		// yes - it is finished
		bool matchingSet = true;
		for(int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slotContents[i] != m_setAnimalID)
				matchingSet = false;
		}

		if (matchingSet)
		{
			StartCoroutine(SetCompleteSequenceMatchingSet());
			return PlaceAnimalResult.MATCHING_SET_COMPLETED;
		}

		StartCoroutine(SetCompleteSequence());
		return PlaceAnimalResult.SET_COMPLETED;
	}

	IEnumerator SetCompleteSequenceMatchingSet()
	{
		const float MOVE_TOGETHER_TIME = 0.5f;
		const float SCALE_UP_TIME = 0.2f;
		const float SCALE_FACTOR = 2.5f;
		const float WOBBLE_TIME = 0.2f;
		const float WOBBLE_COUNT = 2;
		const float WOBBLE_FACTOR = 0.15f;
		const float MOVE_TO_QUEUE_TIME = 0.2f;
		ScaleAll(0.1f, MOVE_TOGETHER_TIME, true);
		MoveTogether(MOVE_TOGETHER_TIME,false);

		yield return new WaitForSeconds(MOVE_TOGETHER_TIME-0.1f);

		GameObject evolvedAnimal = ShowEvolvedAnimal(new AnimalDef(AnimalDef.AnimalTypes.WHALE, AnimalDef.ColorTypes.BLUE));
		Scaler scaler = evolvedAnimal.GetComponent<Scaler>();
		scaler.ScaleBy(new Vector3(SCALE_FACTOR,SCALE_FACTOR,0f),SCALE_UP_TIME);
		for(int i=0; i<WOBBLE_COUNT; i++)
		{
			yield return new WaitForSeconds(WOBBLE_TIME);
			scaler.ScaleBy(new Vector3(-WOBBLE_FACTOR,-WOBBLE_FACTOR,0f),WOBBLE_TIME);
			yield return new WaitForSeconds(WOBBLE_TIME);
			scaler.ScaleBy(new Vector3(WOBBLE_FACTOR,WOBBLE_FACTOR,0f),WOBBLE_TIME);
		}

//		ScaleAll(0.5f, 0.1f, true);
//		yield return new WaitForSeconds(0.55f);
//		ScaleAll(-0.5f, 0.1f, true);
//
//		yield return new WaitForSeconds(0.3f);
//		ScaleAll(1f, 0.5f, false);
//		//MoveAll(new Vector3(-100f,0f,0f), 1f, false);

		yield return new WaitForSeconds(1.0f);

		Mover mov = evolvedAnimal.GetComponent<Mover>();
		mov.MoveTo(GameController.Instance.m_animalQueueLocations[2].position, MOVE_TO_QUEUE_TIME);
		scaler.ScaleTo(new Vector3(1f,1f),MOVE_TO_QUEUE_TIME);

		yield return new WaitForSeconds(0.5f);

		Destroy(evolvedAnimal);

		ClearSet();
	}

	IEnumerator SetCompleteSequence()
	{
		ScaleAll(0.1f, 0.1f, true);
		yield return new WaitForSeconds(0.15f);
		ScaleAll(-0.1f, 0.1f, true);

		yield return new WaitForSeconds(0.3f);
		ScaleAll(1f, 0.3f, false);
		MoveAll(new Vector3(-100f,0f,0f), 1f, false);

		yield return new WaitForSeconds(1.0f);
		ClearSet();
	}

	void ScaleAll(float dScale, float duration, bool linear)
	{
		for (int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slots[i] != null)
			{
				Transform location = m_slots[i];
				for (int j = location.childCount-1; j>=0; --j)
				{
					GameObject child = location.GetChild(j).gameObject;
					Scaler scaler = child.GetComponent<Scaler>();
					if (scaler != null)
					{
						scaler.ScaleBy(new Vector3(dScale, dScale, 0f), duration, linear);
					}
				}
			}
		}
	}

	void MoveAll(Vector3 dPos, float duration, bool linear)
	{
		for (int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slots[i] != null)
			{
				Transform location = m_slots[i];
				for (int j = location.childCount-1; j>=0; --j)
				{
					GameObject child = location.GetChild(j).gameObject;
					Mover mover = child.GetComponent<Mover>();
					if (mover != null)
					{
						mover.MoveBy(dPos, duration, linear);
					}
				}
			}
		}
	}

	void MoveTogether(float duration, bool linear)
	{
		for (int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slots[i] != null)
			{
				Transform location = m_slots[i];
				for (int j = location.childCount-1; j>=0; --j)
				{
					GameObject child = location.GetChild(j).gameObject;
					Mover mover = child.GetComponent<Mover>();
					if (mover != null)
					{
						mover.MoveTo(transform.localPosition, duration, linear);
					}
				}
			}
		}
	}

	public Transform GetPlacedPiecePosition()
	{
		return m_placedPiecePosition;
	}


}
