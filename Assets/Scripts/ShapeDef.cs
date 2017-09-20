using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShapeDef 
{
	public enum ColorTypes
	{
		BLUE = 0,
		RED,
		GREEN,
		YELLOW
	}

	public int m_shapeType;
	public ColorTypes m_colour;

	public ShapeDef(int shapeType, ColorTypes colour)
	{
		m_shapeType = shapeType;
		m_colour = colour;
	}

}
