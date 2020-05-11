using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LevelLoader : MonoBehaviour
{
    public System.Action<LevelData> OnLevelLoaded;

    [System.Serializable]
    public struct LevelData
    {
        public string DisplayName;
        public string LevelName;
        public string[] EnabledDropperOptions;
        [Range(1, 9)]
        public int AvailableSlots;
        public AudioClip HelpAC;
        public AudioClip LevelCompletedAC;
    }

    public static LevelLoader Instance;
    
    public ParticleSystem LoadLevelParticles;
    public GameObject CollectablePrefab;
    public AudioClip LevelLoadStartAC;
    public AudioClip LevelLoadLoopAC;
    public AudioClip LevelLoadEndAC;
    [SerializeField]
    private LevelData[] LevelDatas;

    private bool _isLoading = false;
    private int _currentLevel = 0;
    private List<Vector3> _collectablePositions = new List<Vector3>();
    private AudioSource _audioSource;

#if UNITY_EDITOR
    [Header("Debug")]
    public bool SkipLevel = false;
    public int LevelIndex = 0;
    public bool SkipToLevel = false;

    private void Update()
    {
        if (SkipLevel)
        {
            SkipLevel = false;
            LoadNextLevel();
        }
        if (SkipToLevel)
        {
            SkipToLevel = false;
            StartCoroutine(LoadLevel(LevelIndex - 1));
        }
    }

#endif

    private void Start()
    {
        StartCoroutine(LoadLevel(
            PlayerPrefs.GetInt($"{PlayerPrefs.GetInt("current_game", 0)}_current_level", 0)));
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = LevelLoadLoopAC;
        _audioSource.loop = true;
    }

    public void LoadNextLevel()
    {
        if (_isLoading)
            return;

        if (_currentLevel + 1 >= LevelDatas.Length)
            return;
        
        StartCoroutine(LoadLevel(_currentLevel + 1));
    }

    public void ResetLevel()
    {
        foreach (GameObject collectable in GameObject.FindGameObjectsWithTag("Collectable"))
        {
            Destroy(collectable);
        }
        foreach (Vector3 collectablePosition in _collectablePositions)
        {
            Instantiate(CollectablePrefab, collectablePosition, Quaternion.identity);
        }
        ARI.Instance.Collected = 0;
        ARI.Instance.ResetTransform();
    }

    public LevelData GetCurrentLevelData() => LevelDatas[_currentLevel];

    private IEnumerator LoadLevel(int index)
    {
        if (index >= 0 && index < LevelDatas.Length)
        {
            GameState.Instance.Pause();
            _isLoading = true;
            LoadLevelParticles.Play();
            _audioSource.PlayOneShot(LevelLoadStartAC);
            yield return new WaitForSeconds(LevelLoadStartAC.length);
            _audioSource.Play();


            if (SceneManager.GetSceneByName(LevelDatas[_currentLevel].LevelName).isLoaded)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(LevelDatas[_currentLevel].LevelName);

                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }

            _currentLevel = index;

            if(PlayerPrefs.GetInt($"{PlayerPrefs.GetInt("current_game", 0)}_current_level", 0) <= index){
                PlayerPrefs.SetInt($"{PlayerPrefs.GetInt("current_game", 0)}_current_level", index);
            }

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(LevelDatas[_currentLevel].LevelName, LoadSceneMode.Additive);
            DropperController.Instance.SetOptions(LevelDatas[_currentLevel].EnabledDropperOptions);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            _collectablePositions.Clear();
            foreach(GameObject collectable in GameObject.FindGameObjectsWithTag("Collectable"))
            {
                _collectablePositions.Add(collectable.transform.position);
            }

            OnLevelLoaded?.Invoke(LevelDatas[_currentLevel]);

            ARI.Instance.Init();
            ARI.Instance.MainProgramSpace.Clear();
            ARI.Instance.MainProgramSpace.SetAvailableSlots(LevelDatas[_currentLevel].AvailableSlots);

            yield return new WaitForSeconds(LevelLoadLoopAC.length);
            _audioSource.Stop();
            _audioSource.PlayOneShot(LevelLoadEndAC);
            LoadLevelParticles.Stop();
            yield return new WaitForSeconds(LevelLoadEndAC.length);

            GameState.Instance.ForceUnpause();
            _isLoading = false;

        }
    }
}
