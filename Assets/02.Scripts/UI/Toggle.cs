using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toggle : MonoBehaviour
{
	[SerializeField] RectTransform onTransform;
	[SerializeField] RectTransform offTransform;
	[SerializeField] RectTransform toggleTransform;
	[SerializeField] Image onImage;
	[SerializeField] Image offImage;

	Coroutine coroutine;

	private void Start()
	{
		onImage.gameObject.SetActive(true);
		offImage.gameObject.SetActive(false);
	}
	public void OnChangeValue(bool isOn)
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
		if (isOn)
			coroutine = StartCoroutine(ToggleOn());
		else
			coroutine = StartCoroutine(ToggleOff());

	}

	IEnumerator ToggleOn()
	{
		onImage.gameObject.SetActive(true);
		offImage.gameObject.SetActive(false);
		float distance = Vector2.Distance(toggleTransform.anchoredPosition, onTransform.anchoredPosition);
		while (toggleTransform.anchoredPosition != onTransform.anchoredPosition)
		{
			toggleTransform.anchoredPosition = Vector2.MoveTowards(toggleTransform.anchoredPosition, onTransform.anchoredPosition, distance * Time.deltaTime / 0.2f);
			yield return null;
		}
	}

	IEnumerator ToggleOff()
	{
		onImage.gameObject.SetActive(false);
		offImage.gameObject.SetActive(true);
		float distance = Vector2.Distance(toggleTransform.anchoredPosition, offTransform.anchoredPosition);
		while (toggleTransform.anchoredPosition != offTransform.anchoredPosition)
		{
			toggleTransform.anchoredPosition = Vector2.MoveTowards(toggleTransform.anchoredPosition, offTransform.anchoredPosition, distance * Time.deltaTime / 0.2f);
			yield return null;
		}
	}
}
