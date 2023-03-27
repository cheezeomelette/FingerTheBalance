using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour
{
	[SerializeField] GameObject connectedPivot;

	new Collider2D collider;

	Vector2 beforePoint = Vector2.zero;
	Vector2 currentPoint = Vector2.zero;
	GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.Instance;
	}

	private void Update()
	{
		currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (gameManager.IsPlaying && Input.GetMouseButton(0))
		{
			transform.position += (Vector3)(currentPoint - beforePoint);
		}

		beforePoint = currentPoint;
	}

	public void InitStage(Vector3 fingerStartPosition)
	{
		collider = GetComponentInChildren<Collider2D>();
		transform.position = fingerStartPosition;
		collider.enabled = true;
	}

	public void ConnectPivot(Vector2 position)
	{
		connectedPivot.transform.position = position;
		collider.enabled = false;
	}

	public Vector3 GetConnectedAncor()
	{
		return connectedPivot.transform.localPosition;
	}
}
