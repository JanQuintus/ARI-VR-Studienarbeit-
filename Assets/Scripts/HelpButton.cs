using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public AudioSource VoiceAudioSource;
    
    public void SayLevelHelp()
    {
        if(LevelLoader.Instance.GetCurrentLevelData().HelpAC != null)
        {
            VoiceAudioSource.Stop();
            VoiceAudioSource.PlayOneShot(LevelLoader.Instance.GetCurrentLevelData().HelpAC);
        }
    }
}
