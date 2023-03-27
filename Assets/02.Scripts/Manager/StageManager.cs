using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] Stage[] stages;
    [SerializeField] FallingObjectPhysics fallingObject;
    [SerializeField] Finger finger;

    [SerializeField] Transform startFallingTransform;
    [SerializeField] Transform startFingerTransform;

    [SerializeField] ClearPanel clearPanel;
    [SerializeField] FailPanel failPanel;
    [SerializeField] PlayPanel playPanel;

    [SerializeField] Camera mainCam;
    [SerializeField] Camera captureCam;

    public int StageCount => stages.Length;

    public int SetStage(int stage, out GameObject clearParticle)
	{
        if (stage >= StageCount)
            stage = StageCount - 1;
        Stage currentStage = stages[stage];
        fallingObject.InitStage(startFallingTransform.position, currentStage.fallingObject, currentStage.gravityScale);
        playPanel.InitStage(stage, currentStage.objectName, currentStage.limitTime);
        finger.InitStage(startFingerTransform.position);

        mainCam.backgroundColor = currentStage.backgroundColor;
        captureCam.backgroundColor = currentStage.backgroundColor;
        clearParticle = currentStage.clearParticle;
        return currentStage.limitTime;
    }

    public void SetContinueStage(int stage)
    {
        if (stage >= StageCount)
            stage = StageCount - 1;
        Stage currentStage = stages[stage];
        fallingObject.InitStage(startFallingTransform.position, currentStage.fallingObject, currentStage.gravityScale);
        finger.InitStage(startFingerTransform.position);

    }
    public void SetClearMessage(int stage)
    {
        if (stage >= StageCount)
            stage = StageCount - 1;
        Stage currentStage = stages[stage];
        clearPanel.SetClearMessage(currentStage.clearMessage, currentStage.view);
    }
}
