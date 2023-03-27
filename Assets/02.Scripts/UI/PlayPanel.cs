using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour
{
	[SerializeField] Image flashImage;
	[SerializeField] Text levelText;
	[SerializeField] Text timerText;
	[SerializeField] Text goalText;

	[SerializeField] float flashSpeed = 5;

	Animator anim;

	const string LEVEL_FORMAT = "LEVEL{0}";
	const string GOAL_FORMAT = "{0} {1}초 동안\n떨어트리지 마세요.";
	const string SHOW_ANIMATION_NAME = "PlayPanelStartAnimation";

	private void Start()
	{
		anim = GetComponent<Animator>();
	}
	public void InitStage(int stage, string objectName, int limitTime)
	{
		levelText.text = string.Format(LEVEL_FORMAT, stage);
		goalText.text = string.Format(GOAL_FORMAT, objectName, limitTime);
	}

	public void UpdatePlayTimeUI(float currentTime)
	{
		if (currentTime <= 0)
		{
			timerText.text = "00:00";
		}
		else
			timerText.text = string.Format("{0:00.00}", currentTime);
	}

	public IEnumerator Flash()
	{
		float alpha = 0;
		Color color = flashImage.color;
		while(alpha <1)
		{
			alpha += Time.deltaTime * flashSpeed;
			color.a = alpha;
			flashImage.color = color;
			yield return null;
		}
		while(alpha >0)
		{
			alpha -= Time.deltaTime * flashSpeed;
			color.a = alpha;
			flashImage.color = color;
			yield return null;
		}
	}
	public IEnumerator CountDown()
	{
		anim.SetTrigger("show");
		while (!EndAnimationDone())
		{
			yield return null;
		}
		anim.SetTrigger("end");
	}
	public IEnumerator ShowGoal()
	{
		anim.SetTrigger("show");
		while (!EndAnimationDone())
		{
			yield return null;
		}
		anim.SetTrigger("end");
	}

	bool EndAnimationDone()
	{
		return anim.GetCurrentAnimatorStateInfo(0).IsName(SHOW_ANIMATION_NAME) &&
			anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;
	}
}
