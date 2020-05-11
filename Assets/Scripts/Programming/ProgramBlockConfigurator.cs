using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramBlockConfigurator : MonoBehaviour
{
    public static ProgramBlockConfigurator Instance;

    public GameObject PBConfiguratorContainer;
    public TMPro.TMP_Text PBNameText;
    public TMPro.TMP_Text PBDescText;
    public SpriteRenderer PBIconSR;
    public Sprite SettingsButtonIcon;
    public Sprite DescButtonIcon;
    public GameObject SettingsButton;
    public SpriteRenderer SettingsButtonSR;

    [Header("Settings")]
    public GameObject ConditionSettings;
    [Space]
    public GameObject ForSettings;
    public TMPro.TMP_Text ForCountText;
    [Space]
    public GameObject WhileSettings;

    private AProgramBlock _editingPB;
    private bool _settingsVisible = false;

    public System.Action<AProgramBlock> OnConfigurationStart;
    public System.Action<AProgramBlock> OnConfigurationEnd;
    public System.Action<AProgramBlock> OnShowDescription;
    public System.Action<AProgramBlock> OnShowSettings;
    public System.Action<int> OnChangeForValue;
    public System.Action<AProgramBlock, Condition.ConditionType> OnChangeCondition;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        PBConfiguratorContainer.SetActive(false);
    }

    public void Show(AProgramBlock pb)
    {
        PBConfiguratorContainer.SetActive(true);
        PBNameText.text = pb.DisplayName;
        PBIconSR.sprite = pb.Icon;
        _editingPB = pb;

        ShowDescription();

        OnConfigurationStart?.Invoke(pb);
    }

    public void Hide()
    {
        PBConfiguratorContainer.SetActive(false);
        OnConfigurationEnd?.Invoke(_editingPB);
    }

    public void ToggleSettings()
    {
        if (_settingsVisible)
        {
            ShowDescription();
        }
        else
        {
            PBDescText.gameObject.SetActive(false);
            SettingsButtonSR.sprite = DescButtonIcon;
            if (_editingPB is ForStructureProgramBlock)
            {
                ForSettings.SetActive(true);
                ForCountText.SetText(((ForStructureProgramBlock)_editingPB).GetCount().ToString());
            } else if (_editingPB is WhileStructureProgramBlock || _editingPB is WaitProgramBlock)
            {
                WhileSettings.SetActive(true);
                WhileSettings.GetComponent<ConditionSelection>().Init(_editingPB.GetConditionType());
            } else
            {
                ConditionSettings.SetActive(true);
                ConditionSettings.GetComponent<ConditionSelection>().Init(_editingPB.GetConditionType());
            }
            _settingsVisible = true;

            OnShowSettings?.Invoke(_editingPB);
        }
    }

    public void ShowDescription()
    {
        ForSettings.SetActive(false);
        WhileSettings.SetActive(false);
        ConditionSettings.SetActive(false);

        SettingsButtonSR.sprite = SettingsButtonIcon;
        PBDescText.text = _editingPB.Description;
        PBDescText.gameObject.SetActive(true);
        _settingsVisible = false;

        OnShowDescription?.Invoke(_editingPB);
    }

    public void IncreaseForSPBCount()
    {
        if (_editingPB == null) return;
        if(_editingPB is ForStructureProgramBlock)
        {
            ((ForStructureProgramBlock)_editingPB).IncreaseCount();
            ForCountText.SetText(((ForStructureProgramBlock)_editingPB).GetCount().ToString());
            OnChangeForValue?.Invoke(((ForStructureProgramBlock)_editingPB).GetCount());
        }
    }

    public void DecreaseForSPBCount()
    {
        if (_editingPB == null) return;
        if (_editingPB is ForStructureProgramBlock)
        {
            ((ForStructureProgramBlock)_editingPB).DecreaseCount();
            ForCountText.SetText(((ForStructureProgramBlock)_editingPB).GetCount().ToString());
            OnChangeForValue?.Invoke(((ForStructureProgramBlock)_editingPB).GetCount());
        }
    }

    public void SetCondition(Condition.ConditionType conditionType)
    {
        if (_editingPB == null) return;
        _editingPB.SetConditionType(conditionType);
        OnChangeCondition?.Invoke(_editingPB, conditionType);
    }

    private void OnGUI()
    {
        if (_editingPB == null) return;

        if(GUI.Button(new Rect(30, 40, 200, 30), "Toggle Settings"))
        {
            ToggleSettings();
        }
    }
}
