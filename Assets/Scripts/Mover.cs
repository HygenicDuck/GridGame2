
using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	float m_timer;
	float m_moveDuration;
	Vector3 m_startPos;
	Vector3 m_destPos;
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
			if (m_timer >= m_moveDuration)
			{
				m_timer = m_moveDuration;
				m_moving = false;
				ReachedDestination();
			}

			float p = m_timer / m_moveDuration;
			if (!m_isLinear)
				p = p*p * (3f - 2f*p);
			Vector3 pos = m_startPos + (m_destPos - m_startPos)*p;
			transform.position = pos;
		}
	}

	public void MoveBy(Vector3 vec, float duration, bool linear = false)
	{
		MoveTo(transform.position + vec, duration, linear);
	}

	public void MoveTo(Vector3 vec, float duration, bool linear = false)
	{
		m_timer = 0f;
		m_moveDuration = duration;
		m_moving = true;
		m_startPos = transform.position;
		m_destPos = vec;
		//Rigidbody2D rb = GetComponent<Rigidbody2D>();
		//rb.isKinematic = true;
		m_isLinear = linear;
	}

	void ReachedDestination()
	{
		//Rigidbody2D rb = GetComponent<Rigidbody2D>();
		//rb.isKinematic = false;
	}
}

