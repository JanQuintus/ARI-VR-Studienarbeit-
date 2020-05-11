using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ARIMover), typeof(AudioSource), typeof(Animator))]
public class ARI : MonoBehaviour
{
    public static ARI Instance;


    #region Public Variables
    public ProgramSpace MainProgramSpace;
    public LayerMask ObstacleLM;
    [Header("Audio")]
    public AudioClip CollectCoinAC;
    public AudioClip GoalAC;
    public AudioClip RespawnAC;
    public AudioSource CrashAudioSource;
    public AudioClip CrashAC;
    #endregion
    public ARIMover Mover { get; private set; }
    public Collider GoalCheckerCollider;
    public GameObject FlameEmissionGO;
    public AudioSource VoiceAS;
    [HideInInspector]
    public int Collected = 0;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private int _toCollect = 0;
    private AudioSource _as;
    private Animator _anim;
    private Rigidbody _rb;
    private bool _goalReached = false;


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        Mover = GetComponent<ARIMover>();
        _as = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        Init();
    }

    public void Init()
    {
        GameObject spawn = GameObject.Find("SPAWN");

        if(spawn != null)
        {
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;
        }

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        Collected = 0;
        _toCollect = FindObjectsOfType<Collectable>().Length;
        _goalReached = false;
    }

    public void ResetTransform()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }

    public void Collect()
    {
        Collected++;
        Debug.Log($"Collected {Collected} / {_toCollect}");
        _as.PlayOneShot(CollectCoinAC);
        ScanLight.Instance.Scan();
    }

    public void EnterGoal()
    {
        if (GameState.Instance.IsPaused())
            return;

        print("Goal Reached");
        if(CollectedAllRoomdetectionPoints())
        {
            _goalReached = true;
            StartCoroutine(FinishLevel());
        }
    }

    public bool CollectedAllRoomdetectionPoints() => Collected >= _toCollect;
    public bool GoalReached() => _goalReached;

    public void Die()
    {
        if (GameState.Instance.IsPaused())
            return;

        StartCoroutine(DieCor());
    }

    private IEnumerator FinishLevel()
    {
        GameState.Instance.Pause();
        _anim.SetTrigger("Goal");
        _as.PlayOneShot(GoalAC);
        if (LevelLoader.Instance.GetCurrentLevelData().LevelCompletedAC != null)
        {
            VoiceAS.PlayOneShot(LevelLoader.Instance.GetCurrentLevelData().LevelCompletedAC);
            yield return new WaitForSeconds(LevelLoader.Instance.GetCurrentLevelData().LevelCompletedAC.length + 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }


        while (MainProgramSpace.IsExecuting())
            yield return null;

        LevelLoader.Instance.LoadNextLevel();
    }

    private IEnumerator DieCor()
    {
        GameState.Instance.Pause();
        _rb.isKinematic = false;
        _rb.useGravity = true;
        GoalCheckerCollider.isTrigger = true;
        FlameEmissionGO.SetActive(false);
        MainProgramSpace.Stop();
        Mover.StopAction();

        CrashAudioSource.pitch = Random.Range(1f, 3f);
        CrashAudioSource.PlayOneShot(CrashAC);

        _rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), ForceMode.Impulse);

        yield return new WaitForSeconds(2f);

        FlameEmissionGO.SetActive(true);
        _rb.isKinematic = true;
        _rb.useGravity = false;
        GoalCheckerCollider.isTrigger = false;
        LevelLoader.Instance.ResetLevel();

        _as.PlayOneShot(RespawnAC);

        GameState.Instance.Unpause();
    }

    public bool IsWallInDirection(Vector3 direction)
    {
        Debug.DrawRay(transform.position + new Vector3(0, 0.25f, 0), direction * 0.5f, Color.red, 2f);
        return Physics.Raycast(transform.position + new Vector3(0, 0.25f, 0), direction, .5f, ObstacleLM);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Obstacle")
        {
            Die();
        }
    }
}
