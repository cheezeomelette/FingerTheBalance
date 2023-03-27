using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	[SerializeField] GameObject connectedAncor;
	[SerializeField] Transform centerOfMass;
	[SerializeField] Transform fallingObjectTransform;

	Rigidbody2D rigid;
	HingeJoint2D joint;

	float startAngle;
	float currentAngle => transform.rotation.eulerAngles.z;

	bool isConnected = false;
	private void Start()
	{
		rigid = GetComponent<Rigidbody2D>();
		joint = GetComponent<HingeJoint2D>();
		centerOfMass.localPosition = rigid.centerOfMass;
		startAngle = currentAngle;
	}
	private void FixedUpdate()
	{
		float speed = centerOfMass.position.x - connectedAncor.transform.position.x;
		rigid.angularVelocity = -speed *20;
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
			centerOfMass.localPosition = rigid.centerOfMass;

	}
	private float AddAngle(float a, float b)
	{
		float result = a + b;
		return result < 0 ? result + 360 : result;
	}
}
