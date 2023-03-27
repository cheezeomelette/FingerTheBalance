using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stage
{
	public GameObject fallingObject;
	public Color backgroundColor;
	public string objectName;
	public string clearMessage;
	public float gravityScale;
	public int limitTime;
	public int view;
	public GameObject clearParticle;
}
