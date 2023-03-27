using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] Animator menuAnim;
	[SerializeField] Text currentStage;
	CanvasGroup canvasGroup;

	const string SHOW_ANIMATION = "LobbyButtonShowAnimation";
	const string HIDE_ANIMATION = "LobbyButtonHideAnimation";
	const string STAGE_TEXT_FORMAT = "PLAY LEVEL : {0}";

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void SetCurrentStage()
	{
		currentStage.text = string.Format(STAGE_TEXT_FORMAT, GameManager.Instance.currentStage);
	}

	public IEnumerator ShowProcess()
	{
		SetCurrentStage();
		menuAnim.SetTrigger("show");
		float alpha = 0;
		while(alpha < 1)
		{
			alpha += Time.deltaTime / 0.2f;
			canvasGroup.alpha = alpha;
			yield return null;
		}
		canvasGroup.blocksRaycasts = true;
		yield return EndOfAnimation(SHOW_ANIMATION);
	}

	public IEnumerator HideProcess()
	{
		menuAnim.SetTrigger("hide");
		yield return EndOfAnimation(HIDE_ANIMATION);
		float alpha = 1;
		while(alpha > 0)
		{
			alpha -= Time.deltaTime / 0.2f;
			canvasGroup.alpha = alpha;
			yield return null;
		}
		canvasGroup.blocksRaycasts = false;
		menuAnim.SetTrigger("end");
	}

	IEnumerator EndOfAnimation(string animationName)
	{
		while (!EndAnimationDone(animationName))
		{
			yield return null;
		}
	}

	bool EndAnimationDone(string animationName)
	{
		return menuAnim.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
			menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;
	}
}
