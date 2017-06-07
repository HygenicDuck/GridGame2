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

	void Start()
	{
		m_slotContents = new int[m_slots.Length];
		for (int i=0; i<m_slotContents.Length; i++)
		{
			m_slotContents[i] = -1;
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
//			return false;
//		}

		if (m_slotContents[animalColor] != -1)
		{
			// already have this color
			return false;
		}

		m_slotContents[animalColor] = 1;

		GameObject animal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		animal.transform.SetParent(m_slots[animalColor]);
		animal.transform.localPosition = Vector3.zero;

		Animal animalController = animal.GetComponent<Animal>();
		animalController.SetDef(animalDef);

		return true;
	}

}
