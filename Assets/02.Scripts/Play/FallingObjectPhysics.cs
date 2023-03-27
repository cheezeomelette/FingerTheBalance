using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectPhysics : MonoBehaviour
{
	[SerializeField] GameObject connectedAncor;
	[SerializeField] Transform centerOfMass;
	[SerializeField] Transform fallingObjectTransform;

	public float limitAngle = 60;
	public float gravity = 30f;
	public float deadLine = -5;

	GameManager gameManager;
	Rigidbody2D rigid;
	HingeJoint2D joint;

	float startAngle;
	float currentAngle => transform.rotation.eulerAngles.z;
	float gravityScale = 1f;
	bool isConnected = false;

	private void Start()
	{
		rigid = GetComponent<Rigidbody2D>();
		joint = GetComponent<HingeJoint2D>();
		gameManager = GameManager.Instance;
		startAngle = currentAngle;
		joint.enabled = false;
		centerOfMass.localPosition = rigid.centerOfMass;
		rigid.gravityScale = 0;
	}

	private void FixedUpdate()
	{
		if (joint == null || !joint.enabled)
			return;
		float speed = centerOfMass.position.x - connectedAncor.transform.position.x;
		rigid.angularVelocity = -speed * gravity * gravityScale;
	}

	private void Update()
	{
		if (!gameManager.IsPlaying)
			return;

		if(!isConnected && transform.position.y < deadLine)
		{
			gameManager.FailStage();
		}

		float minAngle = Mathf.Min(AddAngle(startAngle, limitAngle), AddAngle(startAngle, -limitAngle));
		float maxAngle = Mathf.Max(AddAngle(startAngle, limitAngle), AddAngle(startAngle, -limitAngle));
		if (currentAngle > minAngle && currentAngle < maxAngle)
		{
			isConnected = false;
			joint.enabled = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Vector2 contactPoint = collision.GetContact(0).point;
		connectedAncor.transform.position = contactPoint;
		joint.anchor = connectedAncor.transform.localPosition;
		joint.enabled = true;
		isConnected = true;

		SoundManager.Instance.Play("Tup");
		Finger finger = collision.gameObject.GetComponent<Finger>();
		finger.ConnectPivot(contactPoint);
		joint.connectedAnchor = finger.GetConnectedAncor();
	}

	public void InitStage(Vector3 fallingObjectStartPosition, GameObject fallingObject, float gravityScale)
	{
		isConnected = false;
		joint.enabled = false;
		rigid.gravityScale = 0;
		transform.rotation = Quaternion.identity;

		this.gravityScale = gravityScale;
		transform.position = fallingObjectStartPosition;
		foreach (Transform child in fallingObjectTransform)
		{
			Destroy(child.gameObject);
		}
		Instantiate(fallingObject, transform.position, fallingObject.transform.rotation, fallingObjectTransform);
		centerOfMass.localPosition = rigid.centerOfMass;
	}

	public void StartStage()
	{
		rigid.gravityScale = 0.5f;
	}

	public void FinishStage()
	{
		isConnected = false;
		joint.enabled = false;
		rigid.velocity = Vector2.zero;
		rigid.gravityScale = 0f;
		rigid.angularVelocity = 0f;
	}

	private float AddAngle(float a, float b)
	{
		float result = a + b;
		return result < 0 ? result + 360 : result;
	}
}
