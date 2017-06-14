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

	public AnimalTypes m_animalType;
	public ColorTypes m_colour;

	public AnimalDef(AnimalTypes animalType, ColorTypes colour)
	{
		m_animalType = animalType;
		m_colour = colour;
	}

	public AnimalDef Evolved()
	{
		AnimalDef evolved = new AnimalDef(EvolvedType(m_animalType), m_colour);
		return evolved;
	}

	AnimalTypes EvolvedType(AnimalTypes inType)
	{
		switch(inType)
		{
		case AnimalTypes.FISH:
			return AnimalTypes.WHALE;
			break;
		case AnimalTypes.RABBIT:
			return AnimalTypes.SHEEP;
			break;
		case AnimalTypes.DUCK:
			return AnimalTypes.WHALE;
			break;
		case AnimalTypes.DOG:
			return AnimalTypes.ELEPHANT;
			break;
		case AnimalTypes.WHALE:
			return AnimalTypes.BEE;
			break;
		case AnimalTypes.ELEPHANT:
			return AnimalTypes.BEE;
			break;
		case AnimalTypes.BEE:
			return AnimalTypes.DUCK;
			break;
		case AnimalTypes.SHEEP:
			return AnimalTypes.DOG;
			break;
		}

		return AnimalTypes.FISH;
	}
}
