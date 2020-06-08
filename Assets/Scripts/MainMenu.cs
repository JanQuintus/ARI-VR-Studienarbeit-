using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuGO;
    public GameObject LoadGameMenuGO;
    public GameObject DeleteQuestionGO;
    public GameObject LoadingAnimation;

    [Header("Level selection")]
    public GameObject PrevGO;
    public TMPro.TMP_Text Slot0Text;
    public TMPro.TMP_Text Slot1Text;
    public TMPro.TMP_Text Slot2Text;


    private int _page = 0;
    private SortedList<int, int> _savedGames = new SortedList<int, int>();

    private bool _isLoadingLevel = false;

    private void Awake()
    {

        if (PlayerPrefs.GetInt("first_start", 0) == 0)
        {
            SceneManager.LoadScene("Intro");
            return;
        }

        LoadingAnimation.SetActive(false);

        for (int i = 0; i < PlayerPrefs.GetInt("games", 0); i++)
        {
            if (PlayerPrefs.HasKey($"{i}_current_level"))
            {
                _savedGames.Add(i, PlayerPrefs.GetInt($"{i}_current_level"));
            }
        }
    }

    public void NewGame()
    {
        if (_isLoadingLevel) return;
        StartCoroutine(LoadLevel("Intro"));
    }

    public void ShowLoadGameMenu()
    {
        if (_isLoadingLevel) return;

        MainMenuGO.SetActive(false);
        LoadGameMenuGO.SetActive(true);

        ShowPage(0);
    }

    public void ShowMainMenu()
    {
        if (_isLoadingLevel) return;

        LoadGameMenuGO.SetActive(false);
        MainMenuGO.SetActive(true);
    }

    public void LoadGame(int game)
    {
        if (_isLoadingLevel) return;

        PlayerPrefs.SetInt("current_game", game);
        StartCoroutine(LoadLevel("Main"));
    }

    public void QuitGame()
    {
        if (_isLoadingLevel) return;

        Application.Quit();
    }

    public void NextPage()
    {
        if (_isLoadingLevel) return;

        ShowPage(_page + 1);
    }

    public void PrevPage()
    {
        if (_isLoadingLevel) return;

        ShowPage(_page - 1);
    }


    private void ShowPage(int page)
    {
        if (_isLoadingLevel) return;

        if (page < 0)
            return;

        if (_savedGames.Count - 1 < page * 3)
            return;

        Slot0Text.GetComponent<Button3D>().OnButtonPressed.RemoveAllListeners();
        Slot1Text.GetComponent<Button3D>().OnButtonPressed.RemoveAllListeners();
        Slot2Text.GetComponent<Button3D>().OnButtonPressed.RemoveAllListeners();


        _page = page;
        int slot = 0;
        for (int i = page * 3; i < _savedGames.Count; i++)
        {
            switch (slot)
            {
                case 0:
                    Slot0Text.SetText($"Spielstand {i + 1}, Versuch {_savedGames.Values[i] + 1}");
                    Slot0Text.GetComponent<Button3D>().OnButtonPressed.AddListener(() => LoadGame(_savedGames.Keys[page * 3]));
                    break;
                case 1:
                    Slot1Text.SetText($"Spielstand {i + 1}, Versuch {_savedGames.Values[i] + 1}");
                    Slot1Text.GetComponent<Button3D>().OnButtonPressed.AddListener(() => LoadGame(_savedGames.Keys[page * 3 + 1]));
                    break;
                case 2:
                    Slot2Text.SetText($"Spielstand {i + 1}, Versuch {_savedGames.Values[i] + 1}");
                    Slot2Text.GetComponent<Button3D>().OnButtonPressed.AddListener(() => LoadGame(_savedGames.Keys[page * 3 + 2]));
                    break;
            }

            slot++;
            if (slot == 3) break;
        }

        if (slot == 1)
        {
            Slot1Text.SetText("");
            Slot2Text.SetText("");
        }
        if (slot == 2)
        {
            Slot2Text.SetText("");
        }
    }

    private IEnumerator LoadLevel(string level)
    {
        _isLoadingLevel = true;
        LoadingAnimation.SetActive(true);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);

        while (!loadOperation.isDone)
            yield return null;

        _isLoadingLevel = false;
        LoadingAnimation.SetActive(false);

        SceneManager.UnloadSceneAsync("MainMenu");
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, 100, 30), "Reset PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
