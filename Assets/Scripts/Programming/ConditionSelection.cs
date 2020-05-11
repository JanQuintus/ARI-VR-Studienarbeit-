using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSelection : MonoBehaviour
{
    [System.Serializable]
    private struct ConditionOption {
        public string Name;
        public Sprite Icon;
        public Condition.ConditionType ConditionType;
    }

    [SerializeField]
    private ConditionOption[] Options;

    public SpriteRenderer ConditionSprite;
    public TMPro.TMP_Text ConditionName;

    private int _currentIndex = 0;
    private ProgramBlockConfigurator _pbConfigurator;

    public void Init(Condition.ConditionType conditionType)
    {
        _pbConfigurator = GetComponentInParent<ProgramBlockConfigurator>();
        if (_pbConfigurator == null)
        {
            Debug.LogError("ConditionSelection must be child of ProgramBlockConfigurator!", this);
            return;
        }

        if (Options.Length == 0)
        {
            Debug.LogError("No ConditionOptions set!", this);
            return;
        }

        SetSelectedOption(conditionType);
    }

    public void SetSelectedOption(Condition.ConditionType conditionType)
    {
        for(int i = 0; i < Options.Length; i++)
        {
            if(Options[i].ConditionType == conditionType)
            {
                ShowConditionOption(i);
                return;
            }
        }
        ShowConditionOption(0);
    }

    public void NextCondition()
    {
        if(++_currentIndex >= Options.Length) _currentIndex = 0;
        ShowConditionOption(_currentIndex);
    }

    public void PreviousCondition()
    {
        if (--_currentIndex < 0) _currentIndex = Options.Length - 1;
        ShowConditionOption(_currentIndex);
    }

    private void ShowConditionOption(int index)
    {
        ConditionSprite.sprite = Options[index].Icon;
        ConditionName.text = Options[index].Name;
        _pbConfigurator.SetCondition(Options[index].ConditionType);
        _currentIndex = index;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(30, 80, 30, 30), "<"))
        {
            PreviousCondition();
        }
        if (GUI.Button(new Rect(65, 80, 30, 30), ">"))
        {
            NextCondition();
        }
    }
}
