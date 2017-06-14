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
}
