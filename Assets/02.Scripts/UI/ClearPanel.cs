using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanel : MonoBehaviour
{
    [SerializeField] Text viewText;
    [SerializeField] Text ClearText;
    RectTransform rect;
    Animator anim;

	const string VIEW_FORMAT = "조회 {0}";
    const string SHOW_ANIMATION = "PanelUpShowAnimation";
    const string HIDE_ANIMATION = "PanelHideAnimation";

    Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
        startPos = rect.anchoredPosition;
    }

    public void SetClearMessage(string message, int view)
	{
        ClearText.text = message;
        viewText.text = string.Format(VIEW_FORMAT, view);
	}

    public IEnumerator ShowPanel()
	{
        anim.SetTrigger("show");
        yield return EndOfAnimation(SHOW_ANIMATION);
    }

    public IEnumerator HidePanel()
	{
        anim.SetTrigger("hide");
        yield return EndOfAnimation(HIDE_ANIMATION);
        anim.SetTrigger("end");
    }

    public void GoNextStage()
	{
        GameManager.Instance.ClearStartStage();
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
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;
    }



}
