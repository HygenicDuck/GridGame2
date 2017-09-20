using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeQueue
{
	static ShapeQueue m_instance = null;

	List<ShapeDef> m_shapeDeck = new List<ShapeDef>();

	public ShapeQueue()
	{
		m_instance = this;
		FillQueue();
	}

	public static ShapeQueue Instance
	{
		get
		{
			return m_instance;
		}
	}



	const int QUEUE_LENGTH = 4;
	const int NUM_OF_PACKS = 5;
	ShapeDef[] m_shapeQueue = new ShapeDef[QUEUE_LENGTH];

	// Use this for initialization
	void Start () 
	{
		InitShapeDeck();
	}


	void InitShapeDeck()
	{
		ShapeDef.ColorTypes[] colorTypesArray = Enum.GetValues(typeof(ShapeDef.ColorTypes)) as ShapeDef.ColorTypes[];
		int numberOfShapeTypes = ShapeAssetManager.Instance.NumberOfShapeTypes ();

		m_shapeDeck = new List<ShapeDef>();

		for(int packNum=0; packNum<NUM_OF_PACKS; packNum++)
		{
			for(int colorNum=0; colorNum<colorTypesArray.Length; colorNum++)
			{
				for(int shapeNum=0; shapeNum<numberOfShapeTypes; shapeNum++)
				{
					m_shapeDeck.Add(new ShapeDef(shapeNum, (ShapeDef.ColorTypes)colorNum));
				}
			}
		}

		m_shapeDeck.Shuffle();
	}

	ShapeDef PullAnimalFromDeck()
	{
		if (m_shapeDeck.Count == 0)
		{
			InitShapeDeck();
		}

		ShapeDef ad = m_shapeDeck[0];
		m_shapeDeck.RemoveAt(0);
		return ad;
	}


	ShapeDef RandomShape()
	{
		ShapeDef.ColorTypes[] colorTypesArray = Enum.GetValues(typeof(ShapeDef.ColorTypes)) as ShapeDef.ColorTypes[];

		int numberOfShapeTypes = ShapeAssetManager.Instance.NumberOfShapeTypes ();
		int type = UnityEngine.Random.Range(0, numberOfShapeTypes);
		ShapeDef.ColorTypes colour = colorTypesArray[UnityEngine.Random.Range(0, colorTypesArray.Length)];

		ShapeDef ad = new ShapeDef(type, colour);

		return ad;
	}

	void FillQueue()
	{
		for(int i=0; i<QUEUE_LENGTH; i++)
		{
			m_shapeQueue[i] = PullAnimalFromDeck();
		}
	}

	public ShapeDef PopFromQueue(int usedPieceIndex)
	{
		ShapeDef nextAnimal = m_shapeQueue[usedPieceIndex];
		for(int i=usedPieceIndex; i<QUEUE_LENGTH-1; i++)
		{
			m_shapeQueue[i] = m_shapeQueue[i+1];
		}
		m_shapeQueue[QUEUE_LENGTH-1] = PullAnimalFromDeck();

		return nextAnimal;
	}

	public ShapeDef HeadOfQueue()
	{
		return m_shapeQueue[0];
	}

	public ShapeDef QueuePosition(int pos)
	{
		return m_shapeQueue[pos];
	}

//	public void AddNewSpeciesToDeck(AnimalDef currentAnimal)
//	{
//		// evolve from current animal
//		AnimalDef ad = currentAnimal.Evolved();
//		//m_animalDeck.Insert(0,ad);
//		m_shapeQueue[QUEUE_LENGTH-1] = ad;
//	}


//
//	public enum AnimalTypes
//	{
//		FISH,
//		RABBIT,
//		DUCK,
//		DOG,
//		WHALE,
//		ELEPHANT,
//		BEE,
//		SHEEP
//	}
//
//	public enum ColorTypes
//	{
//		BLUE,
//		RED,
//		GREEN,
//		YELLOW
//	}
//
//	public AnimalTypes animalType;
//	public ColorTypes colour;
}
