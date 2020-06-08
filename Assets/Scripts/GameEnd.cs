using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    public AudioClip Clip;
    public GameObject LoadingAnimation;

    private AudioSource _as;

    private void Awake()
    {
        _as = GetComponent<AudioSource>();
        StartCoroutine(SpeakAndLoadLevel());

        LoadingAnimation.SetActive(false);
    }

    private void Start()
    {
        ARI.Instance.PlayEndAnimation();
    }

    IEnumerator SpeakAndLoadLevel()
    {
        _as.PlayOneShot(Clip);
        yield return new WaitForSeconds(Clip.length);

        LoadingAnimation.SetActive(true);

        yield return new WaitForSeconds(1);


        StartCoroutine(LoadLevel("MainMenu"));
    }

    private IEnumerator LoadLevel(string level)
    {

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);

        while (!loadOperation.isDone)
            yield return null;

        LoadingAnimation.SetActive(false);

        SceneManager.UnloadSceneAsync("Level26_EndScene");
        SceneManager.UnloadSceneAsync("Main");
    }
}
