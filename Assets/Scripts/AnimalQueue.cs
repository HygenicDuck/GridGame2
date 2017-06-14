using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalQueue
{
	static AnimalQueue m_instance = null;

	List<AnimalDef> m_animalDeck = new List<AnimalDef>();

	public AnimalQueue()
	{
		m_instance = this;
		FillQueue();
	}

	public static AnimalQueue Instance
	{
		get
		{
			return m_instance;
		}
	}



	const int QUEUE_LENGTH = 4;
	const int INITIAL_NUM_ANIMAL_TYPES = 2;
	const int NUM_OF_PACKS = 5;
	AnimalDef[] m_animalQueue = new AnimalDef[QUEUE_LENGTH];

	// Use this for initialization
	void Start () 
	{
		InitAnimalDeck();
	}


	void InitAnimalDeck()
	{
		AnimalDef.ColorTypes[] colorTypesArray = Enum.GetValues(typeof(AnimalDef.ColorTypes)) as AnimalDef.ColorTypes[];

		m_animalDeck = new List<AnimalDef>();

		for(int packNum=0; packNum<NUM_OF_PACKS; packNum++)
		{
			for(int colorNum=0; colorNum<colorTypesArray.Length; colorNum++)
			{
				for(int animalNum=0; animalNum<INITIAL_NUM_ANIMAL_TYPES; animalNum++)
				{
					m_animalDeck.Add(new AnimalDef((AnimalDef.AnimalTypes)animalNum, (AnimalDef.ColorTypes)colorNum));
				}
			}
		}

		m_animalDeck.Shuffle();
	}

	AnimalDef PullAnimalFromDeck()
	{
		if (m_animalDeck.Count == 0)
		{
			InitAnimalDeck();
		}

		AnimalDef ad = m_animalDeck[0];
		m_animalDeck.RemoveAt(0);
		return ad;
	}


	AnimalDef RandomAnimal()
	{
		AnimalDef.AnimalTypes[] animalTypesArray = Enum.GetValues(typeof(AnimalDef.AnimalTypes)) as AnimalDef.AnimalTypes[];
		AnimalDef.ColorTypes[] colorTypesArray = Enum.GetValues(typeof(AnimalDef.ColorTypes)) as AnimalDef.ColorTypes[];

		AnimalDef.AnimalTypes type = animalTypesArray[UnityEngine.Random.Range(0, INITIAL_NUM_ANIMAL_TYPES)];	//animalTypesArray.Length)];
		AnimalDef.ColorTypes colour = colorTypesArray[UnityEngine.Random.Range(0, colorTypesArray.Length)];

		AnimalDef ad = new AnimalDef(type, colour);

		return ad;
	}

	void FillQueue()
	{
		for(int i=0; i<QUEUE_LENGTH; i++)
		{
			m_animalQueue[i] = PullAnimalFromDeck();
		}
	}

	public AnimalDef PopFromQueue(int usedPieceIndex)
	{
		AnimalDef nextAnimal = m_animalQueue[usedPieceIndex];
		for(int i=usedPieceIndex; i<QUEUE_LENGTH-1; i++)
		{
			m_animalQueue[i] = m_animalQueue[i+1];
		}
		m_animalQueue[QUEUE_LENGTH-1] = PullAnimalFromDeck();

		return nextAnimal;
	}

	public AnimalDef HeadOfQueue()
	{
		return m_animalQueue[0];
	}

	public AnimalDef QueuePosition(int pos)
	{
		return m_animalQueue[pos];
	}

	public void AddNewSpeciesToDeck(AnimalDef currentAnimal)
	{
		// evolve from current animal
		AnimalDef ad = new AnimalDef(AnimalDef.AnimalTypes.ELEPHANT, currentAnimal.m_colour);
		m_animalDeck.Insert(0,ad);
	}


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
