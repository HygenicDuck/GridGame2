using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAssetManager : MonoBehaviour {

	static AnimalAssetManager m_instance = null;

	AnimalAssetManager()
	{
		m_instance = this;
	}

	public static AnimalAssetManager Instance
	{
		get
		{
			return m_instance;
		}
	}



	public enum AnimalIDs
	{
		FISH,
		RABBIT,
		DUCK,
		DOG,
		WHALE,
		ELEPHANT,
		BEE,
		SHEEP,
		NUM_ANIMALS
	}

	[SerializeField]
	Texture[] m_animalGraphics;



	// Use this for initialization
	void Start () 
	{
		//m_animalGraphics = new Texture[AnimalIDs.GetNames(typeof(MyEnum)).Length];
	}
	
	public Texture GetAnimalTexture(AnimalDef.AnimalTypes id)
	{
		return m_animalGraphics[(int)id];
	}

}
