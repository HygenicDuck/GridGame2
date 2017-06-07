using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalDef 
{
	public enum AnimalTypes
	{
		FISH = 0,
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
		BLUE = 0,
		RED,
		GREEN,
		YELLOW
	}

	public AnimalTypes animalType;
	public ColorTypes colour;
}
