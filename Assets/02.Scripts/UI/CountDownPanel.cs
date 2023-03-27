using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownPanel : MonoBehaviour
{
    [SerializeField] Text countText;

    Animator anim;
    CanvasGroup canvasGroup;

    const string COUNT_ANIMATION = "CountDownAnimation";

    void Start()
    {
        anim = GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator CountDownProcess()
	{
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        countText.text = "3";
        yield return ShowCountDown();
        countText.text = "2";
        yield return ShowCountDown();
        countText.text = "1";
        yield return ShowCountDown();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    public IEnumerator ShowCountDown()
    {
        anim.SetTrigger("count");
        yield return EndOfAnimation(COUNT_ANIMATION);
        anim.SetTrigger("end");
    }

    IEnumerator EndOfAnimation(string animationName)
    {
        yield return null;
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
