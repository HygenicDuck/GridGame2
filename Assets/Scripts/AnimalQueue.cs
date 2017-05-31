using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalQueue
{
	AnimalDef[] m_animalQueue = new AnimalDef[3];

	// Use this for initialization
	void Start () {

	}

	AnimalDef RandomAnimal()
	{
		AnimalDef ad = new AnimalDef();

		I'M HERE
	}

	public enum AnimalTypes
	{
		FISH,
		RABBIT,
		DUCK,
		DOG,
		WHALE,
		ELEPHANT,
		BEE,
		SHEEP
	}

	public enum ColorTypes
	{
		BLUE,
		RED,
		GREEN,
		YELLOW
	}

	public AnimalTypes animalType;
	public ColorTypes colour;
}
