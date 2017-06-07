using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class SetController : MonoBehaviour 
{
	[SerializeField]
	Transform[] m_slots;

	int[] m_slotContents;
	int m_setAnimalID = -1;
	Transform m_placedPiecePosition = null;

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


	public bool AddAnimal(GameObject prefab, AnimalDef animalDef)
	{
		int animalID = (int)animalDef.animalType;
		int animalColor = (int)animalDef.colour;

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
			return false;
		}

		m_slotContents[animalColor] = animalID;

		GameObject animal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		animal.transform.SetParent(m_slots[animalColor]);
		animal.transform.localPosition = Vector3.zero;

		m_placedPiecePosition = m_slots[animalColor];

		Animal animalController = animal.GetComponent<Animal>();
		animalController.SetDef(animalDef);

		CheckForCompletedSet();

		return true;
	}

	bool CheckForCompletedSet()
	{
		for (int i=0; i<m_slotContents.Length; i++)
		{
			if (m_slotContents[i] == -1)
				return false;
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
		}
		else
		{
			StartCoroutine(SetCompleteSequence());
		}

		return true;
	}

	IEnumerator SetCompleteSequenceMatchingSet()
	{
		ScaleAll(0.5f, 0.1f, true);
		yield return new WaitForSeconds(0.55f);
		ScaleAll(-0.5f, 0.1f, true);

		yield return new WaitForSeconds(0.3f);
		ScaleAll(1f, 0.5f, false);
		//MoveAll(new Vector3(-100f,0f,0f), 1f, false);

		yield return new WaitForSeconds(1.0f);
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

	public Transform GetPlacedPiecePosition()
	{
		return m_placedPiecePosition;
	}


}
