using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour 
{
	[SerializeField]
	Camera m_camera;
	[SerializeField]
	Vector2 m_touchAreaSize;

	bool m_touching = false;
	bool m_dragging = false;
	float m_dragStartTime = 0f;
	Vector3 m_touchPosition;

	// Use this for initialization
	void Start () 
	{
		GameObject camera = GameObject.Find("Main Camera");
		if (camera != null)
		{
			m_camera = camera.GetComponent<Camera>();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			m_touching = true;
			m_touchPosition = Input.mousePosition;

			Debug.Log("screen size = x="+Screen.width+", y="+Screen.height);
			Debug.Log("m_touchDownPosition = x="+m_touchPosition.x+", y="+m_touchPosition.y);

			Vector3 spritePos = m_camera.WorldToScreenPoint(transform.position);
			Vector3 dPos = spritePos - m_touchPosition;
			Debug.Log("dPos.x "+dPos.x+", dPos.y "+dPos.y);
			if ((Mathf.Abs(dPos.x) < m_touchAreaSize.x/2) && (Mathf.Abs(dPos.y) < m_touchAreaSize.y/2))
			{
				m_dragging = true;
				m_dragStartTime = Time.timeSinceLevelLoad;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (m_dragging)
			{
				m_dragging = false;
				GameController.Instance.PieceDropped(gameObject);
			}
		}
		else if (Input.GetMouseButton(0) && m_dragging)
		{
			Vector3 worldTouchPos = m_camera.ScreenToWorldPoint(m_touchPosition);
			Vector3 worldMousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 dPos = worldMousePos - worldTouchPos;
			m_touchPosition = Input.mousePosition;
			Vector3 pos = transform.position;
			pos += dPos;
			transform.position = pos;
		}
	}

//	void RespondToTouch(Vector3 touchPos)
//	{
//		Vector3 dPos = transform.position - touchPos;
//		if ((Mathf.Abs(dPos.x) < m_touchAreaSize.x/2) && (Mathf.Abs(dPos.y) < m_touchAreaSize.y/2))
//		{
//			m_dragging = true;
//		}
//
//		Vector3 screenPos = m_camera.WorldToScreenPoint(m_topSetLocation.position);
//		Vector3 pos0 = m_camera.WorldToScreenPoint(m_sets[0].transform.position);
//		Vector3 pos1 = m_camera.WorldToScreenPoint(m_sets[1].transform.position);
//		float setWidth = pos0.y - pos1.y;
//		Debug.Log("setWidth = "+setWidth);
//
//		float dy = pos0.y + setWidth/2 - touchPos.y;
//		int setNum = (int)(dy / setWidth);
//		Debug.Log("dy = "+dy+", setNum = "+setNum);
//
//		if (ChooseSet(setNum))
//		{
//			m_animalQueue.PopFromQueue();
//			//ShowCurrentAnimals();
//			Transform piecePos = m_sets[setNum].GetComponent<SetController>().GetPlacedPiecePosition();
//			MovePieceIntoPlace(piecePos);
//			StartCoroutine(ScrollAnimalQueueCoRoutine());
//		}
//	}
}
