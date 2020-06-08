using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Tutorial : MonoBehaviour
{
    [System.Serializable]
    private struct TutorialSection
    {
        public string Title;
        public GameObject[] ActivateObjects;
        [Header("Options")]
        public AudioClip SpeakAC;
        public bool ProceedAfterAudio;
        [Tooltip("id of pb")]
        public string ProceedAfterDropPB;
        [Tooltip("id of pb")]
        public string ProceedAfterGrabPB;
        [Tooltip("id of pb")]
        public string ProceedAfterReleasePB;
        [Tooltip("id of pb or id:slot")]
        public string ProceedAfterAddToMainPS;
        [Tooltip("id of pb")]
        public string ProceedAfterRemoveFromMainPS;
        public bool ProceedAfterMainPSExecute;
        [Tooltip("id of pb")]
        public string ProceedOnCofigurationStart;
        [Tooltip("id of pb")]
        public string ProceedOnCofigurationEnd;
        [Tooltip("id of pb")]
        public string ProceedOnShowDescription;
        [Tooltip("id of pb")]
        public string ProceedOnShowSettings;
        [Tooltip("final value")]
        public int ProceedOnForValue;
        [Tooltip("pbid:CONDITION")]
        public string ProceedSetCondition;
        [Tooltip("id of pb od id:slot")]
        public string ProceedAfterAddToForPS;
        [Tooltip("id of pb od id:slot")]
        public string ProceedAfterAddToWhilePS;
    }

    public bool StartWithLevelLoad = false;
    public float AfterLevelLoadDelay = 1f; 

    [SerializeField]
    private TutorialSection[] Sections;
    
    private AudioSource _audioSource;
    private int _currentSection = -1;


    private void Awake()
    {

        _audioSource = GetComponent<AudioSource>();

        foreach(TutorialSection section in Sections)
        {
            foreach (GameObject activeObject in section.ActivateObjects)
                activeObject.SetActive(false);
        }

    }

    private void Start()
    {
        Dropper.Instance.OnDrop += OnDrop;
        foreach (OVRGrabber grabber in FindObjectsOfType<OVRGrabber>())
        {
            grabber.OnGrabBegin += OnGrabBegin;
            grabber.OnGrabEnd += OnGrabEnd;
        }
        ARI.Instance.MainProgramSpace.OnBlockAdded += OnPBAddedToMainPS;
        ARI.Instance.MainProgramSpace.OnBlockRemoved += OnPBRemovedFromMainPS;
        ARI.Instance.MainProgramSpace.OnExecute += OnMainPSExecute;
        ARI.Instance.OnLevelFinished += OnLevelFinished;
        ProgramBlockConfigurator.Instance.OnConfigurationStart += OnConfigurationStart;
        ProgramBlockConfigurator.Instance.OnConfigurationEnd += OnConfigurationEnd;
        ProgramBlockConfigurator.Instance.OnShowDescription += OnShowDescription;
        ProgramBlockConfigurator.Instance.OnShowSettings += OnShowSettings;
        ProgramBlockConfigurator.Instance.OnChangeForValue += OnChangeForValue;
        ProgramBlockConfigurator.Instance.OnChangeCondition += OnChangeCondition;

        if (StartWithLevelLoad)
            StartCoroutine(NextSectionAfterDelay(AfterLevelLoadDelay));
    }

    private IEnumerator NextSectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextSection();
    }

    private void OnDestroy()
    {
        Dropper.Instance.OnDrop -= OnDrop;
        foreach (OVRGrabber grabber in FindObjectsOfType<OVRGrabber>())
        {
            grabber.OnGrabBegin -= OnGrabBegin;
            grabber.OnGrabEnd -= OnGrabEnd;
        }
        ARI.Instance.MainProgramSpace.OnBlockAdded -= OnPBAddedToMainPS;
        ARI.Instance.MainProgramSpace.OnBlockRemoved -= OnPBRemovedFromMainPS;
        ARI.Instance.MainProgramSpace.OnExecute -= OnMainPSExecute;
        ARI.Instance.OnLevelFinished -= OnLevelFinished;
        ProgramBlockConfigurator.Instance.OnConfigurationStart -= OnConfigurationStart;
        ProgramBlockConfigurator.Instance.OnConfigurationEnd -= OnConfigurationEnd;
        ProgramBlockConfigurator.Instance.OnShowDescription -= OnShowDescription;
        ProgramBlockConfigurator.Instance.OnShowSettings -= OnShowSettings;
        ProgramBlockConfigurator.Instance.OnChangeForValue -= OnChangeForValue;
        ProgramBlockConfigurator.Instance.OnChangeCondition -= OnChangeCondition;

        foreach (ForStructureProgramBlock fpb in FindObjectsOfType<ForStructureProgramBlock>())
            fpb.PS.OnBlockAdded -= OnPBAddedToForPS;

        foreach (WhileStructureProgramBlock wpb in FindObjectsOfType<WhileStructureProgramBlock>())
            wpb.PS.OnBlockAdded -= OnPBAddedToWhilePS;
    }



    private void NextSection()
    {
        if (_currentSection == Sections.Length)
            return;

        if(_currentSection >= 0)
        {
            foreach (GameObject activeObject in Sections[_currentSection].ActivateObjects)
                activeObject.SetActive(false);

            StopAllCoroutines();
        }
        if (++_currentSection < Sections.Length)
        {
            foreach (GameObject activeObject in Sections[_currentSection].ActivateObjects)
                activeObject.SetActive(true);
            if (Sections[_currentSection].SpeakAC != null)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(Sections[_currentSection].SpeakAC);
                if (Sections[_currentSection].ProceedAfterAudio)
                    StartCoroutine(ProceedAfterAudio(Sections[_currentSection].SpeakAC));
            }
        }
    }

#region Events
    private void OnDrop(AProgramBlock pb)
    {
        if(_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if(Sections[_currentSection].ProceedAfterDropPB != null && Sections[_currentSection].ProceedAfterDropPB.Length > 0)
            {
                if (pb.ID == Sections[_currentSection].ProceedAfterDropPB)
                    NextSection();
            }
        }

        if (pb is ForStructureProgramBlock)
            StartCoroutine(RegisterForEvents((ForStructureProgramBlock)pb));

        if (pb is WhileStructureProgramBlock)
            StartCoroutine(RegisterWhileEvents((WhileStructureProgramBlock)pb));
    }

    private void OnGrabBegin(OVRGrabbable grabbable)
    {
        if(grabbable.GetComponent<AProgramBlock>() != null)
        {
            if (_currentSection >= 0 && _currentSection < Sections.Length)
            {
                if (Sections[_currentSection].ProceedAfterGrabPB != null && Sections[_currentSection].ProceedAfterGrabPB.Length > 0)
                {
                    if (grabbable.GetComponent<AProgramBlock>().ID == Sections[_currentSection].ProceedAfterGrabPB)
                        NextSection();
                }
            }
        }
    }

    private void OnGrabEnd(OVRGrabbable grabbable)
    {
        if (grabbable.GetComponent<AProgramBlock>() != null)
        {
            if (_currentSection >= 0 && _currentSection < Sections.Length)
            {
                if (Sections[_currentSection].ProceedAfterReleasePB != null && Sections[_currentSection].ProceedAfterReleasePB.Length > 0)
                {
                    if (grabbable.GetComponent<AProgramBlock>().ID == Sections[_currentSection].ProceedAfterReleasePB)
                        NextSection();
                }
            }
        }
    }

    private void OnPBAddedToMainPS(AProgramBlock pb, int slot)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedAfterAddToMainPS != null && Sections[_currentSection].ProceedAfterAddToMainPS.Length > 0)
            {
                if(Sections[_currentSection].ProceedAfterAddToMainPS.Split(':').Length >= 2)
                {
                    if (pb.ID == Sections[_currentSection].ProceedAfterAddToMainPS.Split(':')[0] &&
                        int.TryParse(Sections[_currentSection].ProceedAfterAddToMainPS.Split(':')[1], out int result) &&
                        result == slot)
                    {
                        NextSection();
                    }
                }
                else
                {
                    if (pb.ID == Sections[_currentSection].ProceedAfterAddToMainPS)
                        NextSection();
                }
            }
        }
    }

    private void OnPBRemovedFromMainPS(AProgramBlock pb)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedAfterRemoveFromMainPS != null && Sections[_currentSection].ProceedAfterRemoveFromMainPS.Length > 0)
            {
                if (pb.ID == Sections[_currentSection].ProceedAfterRemoveFromMainPS)
                    NextSection();
            }
        }
    }

    private void OnMainPSExecute()
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedAfterMainPSExecute)
                NextSection();
        }
    }

    private IEnumerator ProceedAfterAudio(AudioClip playingClip)
    {
        yield return new WaitForSeconds(playingClip.length + .5f);
        NextSection();
    }

    private void OnConfigurationStart(AProgramBlock pb)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedOnCofigurationStart != null && Sections[_currentSection].ProceedOnCofigurationStart.Length > 0)
            {
                if (pb.ID == Sections[_currentSection].ProceedOnCofigurationStart)
                    NextSection();
            }
        }
    }

    private void OnConfigurationEnd(AProgramBlock pb)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedOnCofigurationEnd != null && Sections[_currentSection].ProceedOnCofigurationEnd.Length > 0)
            {
                if (pb.ID == Sections[_currentSection].ProceedOnCofigurationEnd)
                    NextSection();
            }
        }
    }

    private void OnShowDescription(AProgramBlock pb)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedOnShowDescription != null && Sections[_currentSection].ProceedOnShowDescription.Length > 0)
            {
                if (pb.ID == Sections[_currentSection].ProceedOnShowDescription)
                    NextSection();
            }
        }
    }

    private void OnShowSettings(AProgramBlock pb)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedOnShowSettings != null && Sections[_currentSection].ProceedOnShowSettings.Length > 0)
            {
                if (pb.ID == Sections[_currentSection].ProceedOnShowSettings)
                    NextSection();
            }
        }
    }

    private void OnChangeForValue(int value)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedOnForValue > 0)
            {
                if (value == Sections[_currentSection].ProceedOnForValue)
                    NextSection();
            }
        }
    }

    private void OnChangeCondition(AProgramBlock pb, Condition.ConditionType conditionType)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedSetCondition != null && Sections[_currentSection].ProceedSetCondition.Length > 0)
            {
                if (Sections[_currentSection].ProceedSetCondition.Split(':').Length >= 2)
                {
                    if (pb.ID == Sections[_currentSection].ProceedSetCondition.Split(':')[0] &&
                        conditionType.ToString() == Sections[_currentSection].ProceedSetCondition.Split(':')[1])
                    {
                        NextSection();
                    }
                }
            }
        }
    }

    private void OnPBAddedToForPS(AProgramBlock pb, int slot)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedAfterAddToForPS != null && Sections[_currentSection].ProceedAfterAddToForPS.Length > 0)
            {
                if (Sections[_currentSection].ProceedAfterAddToForPS.Split(':').Length >= 2)
                {
                    if (pb.ID == Sections[_currentSection].ProceedAfterAddToForPS.Split(':')[0] &&
                        int.TryParse(Sections[_currentSection].ProceedAfterAddToForPS.Split(':')[1], out int result) &&
                        result == slot)
                    {
                        NextSection();
                    }
                }
                else
                {
                    if (pb.ID == Sections[_currentSection].ProceedAfterAddToForPS)
                        NextSection();
                }
            }
        }
    }

    private void OnPBAddedToWhilePS(AProgramBlock pb, int slot)
    {
        if (_currentSection >= 0 && _currentSection < Sections.Length)
        {
            if (Sections[_currentSection].ProceedAfterAddToWhilePS != null && Sections[_currentSection].ProceedAfterAddToWhilePS.Length > 0)
            {
                if (Sections[_currentSection].ProceedAfterAddToWhilePS.Split(':').Length >= 2)
                {
                    if (pb.ID == Sections[_currentSection].ProceedAfterAddToWhilePS.Split(':')[0] &&
                        int.TryParse(Sections[_currentSection].ProceedAfterAddToWhilePS.Split(':')[1], out int result) &&
                        result == slot)
                    {
                        NextSection();
                    }
                }
                else
                {
                    if (pb.ID == Sections[_currentSection].ProceedAfterAddToWhilePS)
                        NextSection();
                }
            }
        }
    }

    private void OnLevelFinished()
    {
        StopAllCoroutines();
        _audioSource.Stop();
    }

    #endregion

    private IEnumerator RegisterForEvents(ForStructureProgramBlock fpb)
    {
        while (!fpb.IsInitialized)
            yield return null;

        fpb.PS.OnBlockAdded += OnPBAddedToForPS;
    }

    private IEnumerator RegisterWhileEvents(WhileStructureProgramBlock wpb)
    {
        while (!wpb.IsInitialized)
            yield return null;

        wpb.PS.OnBlockAdded += OnPBAddedToWhilePS;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(110, 10, 30, 30), "T"))
        {
            NextSection();
        }
    }
}
