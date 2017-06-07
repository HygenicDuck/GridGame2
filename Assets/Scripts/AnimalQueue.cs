using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalQueue
{
	static AnimalQueue m_instance = null;

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


	const int QUEUE_LENGTH = 3;
	AnimalDef[] m_animalQueue = new AnimalDef[QUEUE_LENGTH];

//	// Use this for initialization
//	void Start () {
//
//	}
//


	AnimalDef RandomAnimal()
	{
		AnimalDef ad = new AnimalDef();

		AnimalDef.AnimalTypes[] animalTypesArray = Enum.GetValues(typeof(AnimalDef.AnimalTypes)) as AnimalDef.AnimalTypes[];
		AnimalDef.ColorTypes[] colorTypesArray = Enum.GetValues(typeof(AnimalDef.ColorTypes)) as AnimalDef.ColorTypes[];

		ad.animalType = animalTypesArray[UnityEngine.Random.Range(0, 3)];	//animalTypesArray.Length)];
		ad.colour = colorTypesArray[UnityEngine.Random.Range(0, colorTypesArray.Length)];

		return ad;
	}

	void FillQueue()
	{
		for(int i=0; i<QUEUE_LENGTH; i++)
		{
			m_animalQueue[i] = RandomAnimal();
		}
	}

	public AnimalDef PopFromQueue()
	{
		AnimalDef nextAnimal = m_animalQueue[0];
		for(int i=0; i<QUEUE_LENGTH-1; i++)
		{
			m_animalQueue[i] = m_animalQueue[i+1];
		}
		m_animalQueue[QUEUE_LENGTH-1] = RandomAnimal();

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
