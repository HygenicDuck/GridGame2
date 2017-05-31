using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

	[SerializeField]
	SpriteRenderer m_animalSprite;
	[SerializeField]
	SpriteRenderer m_colorSprite;
	[SerializeField]
	Color[] m_colors;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetColor(int colorNum)
	{
		m_colorSprite.color = m_colors[colorNum];
	}

	public void SetAnimalTexture(Texture2D tex)
	{
		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f,0.5f));
		m_animalSprite.sprite = sprite;
	}
}
