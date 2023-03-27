using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailPanel : MonoBehaviour
{
    Animator anim;

    const string SHOW_ANIMATION = "FailShowAnimation";
    const string HIDE_ANIMATION = "FailHideAnimation";

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public IEnumerator ShowPanel()
    {
        anim.SetTrigger("show");
        Debug.Log("Show");
        yield return EndOfAnimation(SHOW_ANIMATION);
    }

    public IEnumerator HidePanel()
    {
        anim.SetTrigger("hide");
        yield return EndOfAnimation(HIDE_ANIMATION);
        anim.SetTrigger("end");
    }
    public void ContinueStage()
	{
        anim.SetTrigger("continue");
    }

    public void ReTryStage()
    {
        GameManager.Instance.FailStartStage();
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
