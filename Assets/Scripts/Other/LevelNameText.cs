using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LevelNameText : MonoBehaviour
{
    private TMPro.TMP_Text text;

    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
        LevelLoader.Instance.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnDestroy()
    {
        LevelLoader.Instance.OnLevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(LevelLoader.LevelData data)
    {
        text.SetText(data.DisplayName);
    }
}
