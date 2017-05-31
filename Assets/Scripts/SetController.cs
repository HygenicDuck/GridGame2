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


	public void AddAnimal(GameObject prefab)
	{
		int animalID = 2;
		int animalColor = 1;

		if (m_setAnimalID == -1)
		{
			m_setAnimalID = animalID;
		}

		if (m_setAnimalID != animalID)
		{
			// can't add this animal to this set
			return;
		}

		if (m_slotContents[animalColor] != -1)
		{
			// already have this color
			return;
		}

		m_slotContents[animalColor] = 1;

		GameObject animal = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		animal.transform.SetParent(m_slots[animalColor]);
		animal.transform.localPosition = Vector3.zero;
		Texture2D tex = AnimalAssetManager.Instance.GetAnimalTexture((AnimalAssetManager.AnimalIDs)animalID) as Texture2D;
		Animal animalController = animal.GetComponent<Animal>();
		animalController.SetAnimalTexture(tex);
		animalController.SetColor(animalColor);
//		SpriteRenderer sr = animal.GetComponentInChildren<SpriteRenderer>();
//		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f,0.5f));
//		sr.sprite = sprite;
	}

}
