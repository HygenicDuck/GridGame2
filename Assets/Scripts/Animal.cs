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
	[SerializeField]
	AnimalDef m_animalDef;

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

	void SetAnimalTexture(Texture2D tex)
	{
		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f,0.5f));
		m_animalSprite.sprite = sprite;
	}

	public void SetDef(AnimalDef def)
	{
		Texture2D tex = AnimalAssetManager.Instance.GetAnimalTexture(def.m_animalType) as Texture2D;
		//Animal animalController = animal.GetComponent<Animal>();
		SetAnimalTexture(tex);
		SetColor((int)def.m_colour);
		m_animalDef = def;
	}

	public AnimalDef GetDef()
	{
		return m_animalDef;
	}
}
