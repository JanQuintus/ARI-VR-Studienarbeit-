using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public AudioSource VoiceAudioSource;

    private void Start()
    {
        ARI.Instance.OnLevelFinished += OnLevelFinished;
    }

    private void OnDestroy()
    {
        ARI.Instance.OnLevelFinished -= OnLevelFinished;
    }

    private void OnLevelFinished()
    {
        VoiceAudioSource.Stop();
    }

    public void SayLevelHelp()
    {
        if(LevelLoader.Instance.GetCurrentLevelData().HelpAC != null)
        {
            VoiceAudioSource.Stop();
            VoiceAudioSource.PlayOneShot(LevelLoader.Instance.GetCurrentLevelData().HelpAC);
        }
    }
}
