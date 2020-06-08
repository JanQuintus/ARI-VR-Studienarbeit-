using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Intro : MonoBehaviour
{
    public AudioClip[] ClipsToSay;
    public GameObject LoadingAnimation;

    private AudioSource _as;

    private void Awake()
    {
        _as = GetComponent<AudioSource>();
        StartCoroutine(SpeakAndLoadLevel());

        LoadingAnimation.SetActive(false);
    }

    IEnumerator SpeakAndLoadLevel()
    {
        foreach(AudioClip ac in ClipsToSay)
        {
            _as.PlayOneShot(ac);
            yield return new WaitForSeconds(ac.length - 0.5f);
        }


        PlayerPrefs.SetInt("first_start", 1);
        PlayerPrefs.SetInt("current_game", PlayerPrefs.GetInt("games", 0));
        PlayerPrefs.SetInt("games", PlayerPrefs.GetInt("games", 0) + 1);

        StartCoroutine(LoadLevel("Main"));
    }

    private IEnumerator LoadLevel(string level)
    {
        LoadingAnimation.SetActive(true);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);

        while (!loadOperation.isDone)
            yield return null;

        LoadingAnimation.SetActive(false);

        SceneManager.UnloadSceneAsync("Intro");
    }
}
