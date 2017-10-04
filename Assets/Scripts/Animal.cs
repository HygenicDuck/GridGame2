using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

	const float TIME_FOR_EGG_TO_HATCH = 5f;

	[SerializeField]
	SpriteRenderer m_animalSprite;
	[SerializeField]
	SpriteRenderer m_colorSprite;
	[SerializeField]
	SpriteRenderer m_eggSprite;
	[SerializeField]
	Color[] m_colors;
	[SerializeField]
	AnimalDef m_animalDef;
	[SerializeField]
	Vector2 m_touchAreaSize;
	[SerializeField]
	ProgressBar m_progressBar;

	Camera m_camera;
	float m_eggTime;

	// Use this for initialization
	void Start () 
	{
		GameObject camera = GameObject.Find("Main Camera");
		if (camera != null)
		{
			m_camera = camera.GetComponent<Camera>();
		}
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

	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			Vector3 touchPosition = Input.mousePosition;

			Debug.Log("screen size = x="+Screen.width+", y="+Screen.height);
			Debug.Log("m_touchDownPosition = x="+touchPosition.x+", y="+touchPosition.y);

			Vector3 spritePos = m_camera.WorldToScreenPoint(transform.position);
			Vector3 dPos = spritePos - touchPosition;
			Debug.Log("dPos.x "+dPos.x+", dPos.y "+dPos.y);
			if ((Mathf.Abs(dPos.x) < m_touchAreaSize.x/2) && (Mathf.Abs(dPos.y) < m_touchAreaSize.y/2))
			{
				IncEggTime (0.1f);
			}
		}

		IncEggTime (Time.deltaTime / TIME_FOR_EGG_TO_HATCH);
	}

	void IncEggTime(float amount)
	{
		if (m_eggTime < 1f)
		{
			m_eggTime += amount;
			if (m_eggTime >= 1f)
			{
				m_eggTime = 1f;
				m_eggSprite.gameObject.SetActive (false);
				m_progressBar.gameObject.SetActive (false);
			}

			m_progressBar.SetScale (m_eggTime);
		}
	}

	public void EnableEggAndTimer(bool enable)
	{
		m_eggTime = enable ? 0f : 1f;
		m_eggSprite.gameObject.SetActive (enable);
		m_progressBar.gameObject.SetActive (enable);
	}
}
