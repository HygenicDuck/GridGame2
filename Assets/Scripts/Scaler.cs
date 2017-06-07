
using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour
{
	float m_timer;
	float m_duration;
	Vector3 m_startScale;
	Vector3 m_destScale;
	bool m_moving;
	bool m_isLinear;



	void Awake()
	{
		m_moving = false;
		m_timer = 0f;
	}

	// Update
	void Update ()
	{
		if (m_moving)
		{
			m_timer += Time.deltaTime;
			if (m_timer >= m_duration)
			{
				m_timer = m_duration;
				m_moving = false;
				ReachedDestination();
			}

			float p = m_timer / m_duration;
			if (!m_isLinear)
				p = p*p * (3f - 2f*p);
			Vector3 scale = m_startScale + (m_destScale - m_startScale)*p;
			transform.localScale = scale;
		}
	}

	public void ScaleBy(Vector3 vec, float duration, bool linear = false)
	{
		ScaleTo(transform.localScale + vec, duration, linear);
	}

	public void ScaleTo(Vector3 vec, float duration, bool linear = false)
	{
		m_timer = 0f;
		m_duration = duration;
		m_moving = true;
		m_startScale = transform.localScale;
		m_destScale = vec;
		m_isLinear = linear;
	}

	void ReachedDestination()
	{
	}
}

