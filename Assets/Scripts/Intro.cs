using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Intro : MonoBehaviour
{
    public AudioClip[] ClipsToSay;

    private AudioSource _as;

    private void Awake()
    {
        _as = GetComponent<AudioSource>();
        StartCoroutine(SpeakAndLoadLevel());
    }

    IEnumerator SpeakAndLoadLevel()
    {
        foreach(AudioClip ac in ClipsToSay)
        {
            _as.PlayOneShot(ac);
            yield return new WaitForSeconds(ac.length + 0.5f);
        }


        PlayerPrefs.SetInt("first_start", 1);
        PlayerPrefs.SetInt("current_game", PlayerPrefs.GetInt("games", 0));
        PlayerPrefs.SetInt("games", PlayerPrefs.GetInt("games", 0) + 1);

        SceneManager.LoadScene("Main");
    }
}
