using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentUI
{
	LOBBY,
	PLAY,
	CLEAR,
	FAIL,
	COUNTDOWN,
	CHANGING,
}
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [SerializeField] FallingObjectPhysics fallingObject;
	[SerializeField] GameObject[] celebrateParticles;
	[SerializeField] GameObject[] dustParticles;
	[SerializeField] Transform particleTransform;

	[SerializeField] StageManager stageManager;

	[SerializeField] LobbyPanel lobbyPanel;
    [SerializeField] ClearPanel clearPanel;
	[SerializeField] PlayPanel playPanel;
    [SerializeField] FailPanel failPanel;
	[SerializeField] CountDownPanel countDownPanel;

	[SerializeField] RenderTexture camTexture;
	[SerializeField] RenderTexture copyTexture;

	GameObject clearParticle;

	public float panelSpeed = 2f;
	public int currentStage = 0;

	public bool IsPlaying => isPlaying;
	bool isPlaying = false;

	bool isTimerStarted = false;
	float currentTime;
	CurrentUI currentUI;


	private void Awake()
	{
        instance = this;
	}

	private void Start()
	{
		Application.targetFrameRate = 60;

		StartCoroutine(lobbyPanel.ShowProcess());
		currentUI = CurrentUI.LOBBY;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && currentUI == CurrentUI.LOBBY)
			Application.Quit();	
		// 플레이중이 아니라면 리턴
		if (!isPlaying)
			return;
		CheckTime();
	}

	private void CheckTime()
	{
		currentTime -= Time.deltaTime;

		if (isPlaying && currentTime < 0)
		{
			currentTime = 0;
			playPanel.UpdatePlayTimeUI(currentTime);
			ClearStage();
			return;
		}
		else if (isPlaying && !isTimerStarted && currentTime <= 5)
		{
			isTimerStarted = true;
			SoundManager.Instance.StartTimer();
		}

		playPanel.UpdatePlayTimeUI(currentTime);
	}

	private void SetStage()
	{
		isTimerStarted = false;
		currentTime = stageManager.SetStage(currentStage, out clearParticle);
		playPanel.UpdatePlayTimeUI(currentTime);
	}

	private void StartStage()
	{
		isPlaying = true;
		fallingObject.StartStage();
	}
	public void LobbyStartStage()
	{
		SetStage();
		StartCoroutine(LobbyStageStartProcess());
	}
	public void ClearStartStage()
	{
		SetStage();
		StartCoroutine(ClearStageStartProcess());
	}
	public void FailStartStage()
	{
		SetStage();
		StartCoroutine(FailStageStartProcess());
	}
	public void ContinueStage()
	{
		isTimerStarted = false;
		stageManager.SetContinueStage(currentStage);
		StartCoroutine(FailStageContinueProcess());
	}

	public void ClearStage()
	{
		StartCoroutine(ClearProcess());
	}
	public void FailStage()
	{
		StartCoroutine(FailProcess());
	}

	IEnumerator ClearProcess()
	{
		currentUI = CurrentUI.CHANGING;
		isPlaying = false;
		fallingObject.FinishStage();
		stageManager.SetClearMessage(currentStage);
		currentStage += 1;

		SoundManager.Instance.StopTimer();
		SoundManager.Instance.Play("CameraShutter");
		yield return new WaitForSeconds(0.3f);
		Graphics.CopyTexture(camTexture, copyTexture);
		yield return playPanel.Flash();
		SoundManager.Instance.Play("Success");
		yield return new WaitForSeconds(0.3f);
		SoundManager.Instance.Play("Yoohoo");
		foreach(GameObject particle in celebrateParticles)
		{
			particle.SetActive(true);
		}
		Instantiate(clearParticle, particleTransform.position, clearParticle.transform.rotation);
		yield return new WaitForSeconds(1f);
		yield return clearPanel.ShowPanel();
		Debug.Log("Arrive");
		currentUI = CurrentUI.CLEAR;
	}

	IEnumerator FailProcess()
	{
		currentUI = CurrentUI.CHANGING;
		isPlaying = false;
		//0.2초동안 떨어지고
		SoundManager.Instance.StopTimer();
		SoundManager.Instance.StopAndPlay("Scream");
		// 스크림 사운드 재생
		yield return new WaitForSeconds(0.5f);
		fallingObject.FinishStage();

		// 펑 소리 재생
		SoundManager.Instance.Play("Explosion");
		foreach (GameObject particle in dustParticles)
		{
			particle.SetActive(true);
		}
		yield return new WaitForSeconds(1f);
		yield return failPanel.ShowPanel();
		currentUI = CurrentUI.FAIL;
	}

	IEnumerator FailStageContinueProcess()
	{
		currentUI = CurrentUI.COUNTDOWN;
		failPanel.ContinueStage();
		yield return countDownPanel.CountDownProcess();
		StartStage();
		currentUI = CurrentUI.PLAY;
	}

	IEnumerator FailStageStartProcess()
	{
		currentUI = CurrentUI.CHANGING;
		yield return failPanel.HidePanel();
		yield return StartProcess();
	}
	IEnumerator ClearStageStartProcess()
	{
		currentUI = CurrentUI.CHANGING;
		yield return clearPanel.HidePanel();
		yield return StartProcess();

	}
	IEnumerator LobbyStageStartProcess()
	{
		currentUI = CurrentUI.CHANGING;
		yield return lobbyPanel.HideProcess();
		yield return StartProcess();
	}
	IEnumerator StartProcess()
	{
		currentUI = CurrentUI.CHANGING;
		yield return playPanel.ShowGoal();
		StartStage();
		currentUI = CurrentUI.PLAY;
	}

	public void GoBack()
	{
		switch(currentUI)
		{
			case CurrentUI.CHANGING:
				return;
			case CurrentUI.PLAY:
				isPlaying = false;
				currentUI = CurrentUI.CHANGING;
				SoundManager.Instance.StopTimer();
				fallingObject.FinishStage();
				StartCoroutine(lobbyPanel.ShowProcess());
				currentUI = CurrentUI.LOBBY;
				break;
			case CurrentUI.CLEAR:
				currentUI = CurrentUI.CHANGING;
				fallingObject.FinishStage();
				StartCoroutine(clearPanel.HidePanel());
				StartCoroutine(lobbyPanel.ShowProcess());
				currentUI = CurrentUI.LOBBY;
				break;
			case CurrentUI.FAIL:
				currentUI = CurrentUI.CHANGING;
				fallingObject.FinishStage();
				StartCoroutine(failPanel.HidePanel());
				StartCoroutine(lobbyPanel.ShowProcess());
				currentUI = CurrentUI.LOBBY;
				break;
		}
	}
}
