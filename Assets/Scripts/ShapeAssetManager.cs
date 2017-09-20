using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeAssetManager : MonoBehaviour {

	static ShapeAssetManager m_instance = null;

	ShapeAssetManager()
	{
		m_instance = this;
	}

	public static ShapeAssetManager Instance
	{
		get
		{
			return m_instance;
		}
	}


	[SerializeField]
	string[] m_shapes;
	[SerializeField]
	Color[] m_colors;


	// Use this for initialization
	void Start () 
	{
		//m_animalGraphics = new Texture[AnimalIDs.GetNames(typeof(MyEnum)).Length];
	}

	public string GetShapeString(int id)
	{
		return m_shapes[id];
	}

	public int NumberOfShapeTypes()
	{
		return m_shapes.Length;
	}

	public Color GetColorFromCellColorType(ShapeDef.ColorTypes colour)
	{
		return m_colors [(int)colour];
	}

}
