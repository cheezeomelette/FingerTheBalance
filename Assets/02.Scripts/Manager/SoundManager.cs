using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] clips;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioSource timerAudioSource;

    private static SoundManager instance;
    public static SoundManager Instance => instance;

    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

	private void Awake()
	{
        instance = this;
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		foreach (AudioClip clip in clips)
		{
			audioClips.Add(clip.name, clip);
		}
	}

	public void ToggleSound(bool isOn)
	{
		audioSource.volume = isOn ? 0.3f : 0f;
		timerAudioSource.volume = isOn ? 0.3f : 0f;
	}

	public void StopAndPlay(string clipName)
	{
		if (audioSource.isPlaying)
			audioSource.Stop();

		audioSource.clip = audioClips[clipName];
		audioSource.Play();
	}

	public void Play(string clipName)
	{
		audioSource.PlayOneShot(audioClips[clipName]);
	}

	public void StartTimer()
	{
		timerAudioSource.Play();
	}
	public void StopTimer()
	{
		timerAudioSource.Stop();
	}
}
