using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperController : MonoBehaviour
{
    public static DropperController Instance;

    [System.Serializable]
    private struct DropButtonOption
    {
        public string Name;
        public Button3D DropButton;
        public GameObject EnabledGO;
        public GameObject DisabledGO;
    }

    [SerializeField]
    private DropButtonOption[] DropButtonOptions;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetOptions(string[] enabledOptions)
    {
        List<string> enabledOptionList = new List<string>(enabledOptions);
        foreach(DropButtonOption option in DropButtonOptions)
        {
            bool active = enabledOptionList.Contains(option.Name);
            option.DropButton.Enabled = active;
            option.EnabledGO.SetActive(active);
            option.DisabledGO.SetActive(!active);
        }
    }
}
