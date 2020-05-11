using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramSpaceController : MonoBehaviour
{
    private Animator _animator;
    public static ProgramSpaceController Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    public void Execute()
    {
        if (GameState.Instance.IsPaused()) return;

        _animator.SetBool("IsPlaying", true);
        ARI.Instance.MainProgramSpace.Stop();
        ARI.Instance.Mover.StopAction();
        LevelLoader.Instance.ResetLevel();
        ARI.Instance.MainProgramSpace.Execute();
        StartCoroutine(WaitForExecutionDone());
    }

    private IEnumerator WaitForExecutionDone()
    {
        yield return new WaitUntil(() => ARI.Instance.MainProgramSpace.IsExecuting() == false);
        _animator.SetBool("IsPlaying", false);
    }

    public void Stop()
    {
        _animator.SetBool("IsPlaying", false);
        _animator.SetTrigger("Stop");
        ARI.Instance.MainProgramSpace.Stop();
        ARI.Instance.Mover.StopAction();
        LevelLoader.Instance.ResetLevel();
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(30, 10, 30, 30), "P"))
        {
            Execute();
            Debug.Log("GO!");
        }
        if (GUI.Button(new Rect(65, 10, 30, 30), "S"))
        {
            Stop();
        }
    }
}
