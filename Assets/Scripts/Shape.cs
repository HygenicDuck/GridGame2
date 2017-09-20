using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour {

	[SerializeField]
	SpriteRenderer m_animalSprite;
	[SerializeField]
	Transform m_spritesRoot;
	[SerializeField]
	SpriteRenderer m_colorSprite;
	[SerializeField]
	GameObject m_shapeBlock;
	[SerializeField]
	Color[] m_colors;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetColor(int colorNum)
	{
		m_colorSprite.color = m_colors[colorNum];
	}

//	void SetAnimalTexture(Texture2D tex)
//	{
//		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f,0.5f));
//		m_animalSprite.sprite = sprite;
//	}

	public void SetDef(ShapeDef def)
	{
		string defString = ShapeAssetManager.Instance.GetShapeString(def.m_shapeType);
		ClearShape ();

		char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
		string[] lines = defString.Split (delimiterChars);

		Vector2 gridPitch = GameController.Instance.GridPitch ();

		int numLines = lines.Length;
		int minLine = -((numLines-1)/2);
		//float blockSize = 0.8f;
		for (int y = 0; y < numLines; y++)
		{
			string s = lines [y];
			int numChars = s.Length;
			int minChar = -((numChars-1)/2);
			for (int x = 0; x < numChars; x++)
			{
				float xCoord = -(x + minChar) * gridPitch.x;
				float yCoord = -(y + minLine) * gridPitch.y;
				Vector3 pos = new Vector3(xCoord, yCoord, 0f);
				//pos = new Vector3(0f, 0f, 0f);
				if (s [x] == '1')
				{
					GameObject cell = Instantiate (m_shapeBlock, pos, Quaternion.identity);
					cell.transform.SetParent (m_spritesRoot);
					cell.transform.localPosition = pos;
					cell.GetComponent<SpriteRenderer> ().color = m_colors [(int)def.m_colour];
				}
			}
		}

		//SetColor((int)def.m_colour);
	}

	public void ClearShape()
	{
		if (m_spritesRoot != null)
		{
			Transform location = m_spritesRoot;
			for (int j = location.childCount-1; j>=0; --j)
			{
				GameObject child = location.GetChild(j).gameObject;
				Destroy( child );
			}
		}
	}
}
