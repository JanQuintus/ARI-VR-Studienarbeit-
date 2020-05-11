using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    private int _pauseCounter = 0;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool IsPaused() => _pauseCounter > 0;

    public void Pause()
    {
        _pauseCounter++;
    }

    public void Unpause()
    {
        _pauseCounter--;
        if (_pauseCounter < 0) _pauseCounter = 0;
    }

    public void ForceUnpause()
    {
        _pauseCounter = 0;
    }
}
