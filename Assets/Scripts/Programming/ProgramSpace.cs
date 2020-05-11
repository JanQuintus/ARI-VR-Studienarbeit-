using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProgramSpace : MonoBehaviour
{
    public System.Action<AProgramBlock, int> OnBlockAdded;
    public System.Action<AProgramBlock> OnBlockRemoved;
    public System.Action OnExecute;

    public ProgramBlockSlot[] Slots;
    public bool AllowStructuredProgramBlocks = true;

    public AudioClip AddPBAC;
    public AudioClip RemovePBAC;
    public AudioClip PBNotAcceptedAC;

    private int _currentPC = 0;
    private bool _isExecuting = false;
    private int _availableSlots = 9;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetAvailableSlots(Slots.Length);
    }

    public bool IsExecuting() => _isExecuting;

    public void RemoveFromProgramSpace(AProgramBlock pb)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].GetPB() == pb)
            {
                RemovePB(i);
                _audioSource.PlayOneShot(RemovePBAC);
            }
        }
    }

    public void Execute()
    {
        ResetProgram();
        if (!HasError() && !_isExecuting)
        {
            StartCoroutine(ExecuteAll());
            OnExecute?.Invoke();
        }
    }

    public void Stop()
    {
        if (_isExecuting)
            Slots[_currentPC].Stop();
        ResetProgram();
    }

    private IEnumerator ExecuteAll()
    {
        _isExecuting = true;
        for(int i = _currentPC; i < Slots.Length; i++)
        {
            _currentPC = i;
            if (Slots[i].HasPB())
            {
                Slots[i].Execute();
                yield return new WaitUntil(() => Slots[i].GetPB().IsExecuting() == false);
            }
        }
        ResetProgram();
    }

    private void ResetProgram()
    {
        StopAllCoroutines();
        _currentPC = 0;
        _isExecuting = false;
    }

    public void SetAvailableSlots(int count)
    {
        _availableSlots = count;
        for(int i = 0; i < Slots.Length; i++)
        {
            if (i <= count - 1)
                Slots[i].SetDisabled(false);
            else
                Slots[i].SetDisabled(true);
        }
    }

    public void Clear()
    {
        Stop();
        foreach(ProgramBlockSlot slot in Slots)
        {
            if (slot.HasPB())
                Destroy(slot.GetPB().gameObject);
            slot.AssignBlock(null);
        }
        HasError();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ProgramBlock"))
        {
            if (!AllowStructuredProgramBlocks && other.GetComponent<AStructureProgramBlock>() != null)
                return;

            if (other.GetComponent<Rigidbody>().useGravity)
            {
                int nextSlotIndex = FindNextSlotIndex(other.transform.position);
                if (nextSlotIndex != -1)
                    InsertPB(other.GetComponent<AProgramBlock>(), nextSlotIndex);
            }
        }
    }

    private int FindNextSlotIndex(Vector3 position)
    {
        int nextPBSIndex = -1;
        float minDist = float.MaxValue;
        for (int i = 0; i < _availableSlots; i++)
        {
            float dist = Vector3.Distance(Slots[i].transform.position, position);
            if (dist <= minDist)
            {
                minDist = dist;
                nextPBSIndex = i;
            }
        }

        return nextPBSIndex;
    }

    private void RemovePB(int slot)
    {
        Stop();

        if (Slots[slot].HasPB())
        {
            OnBlockRemoved?.Invoke(Slots[slot].GetPB());
            Slots[slot].GetPB().CurrentProgramSpace = null;
        }

        Slots[slot].AssignBlock(null);
        HasError();
    }

    private void InsertPB(AProgramBlock pb, int slot)
    {
        bool assigend = false;
        if (!Slots[slot].HasPB())
        {
            Stop();
            Slots[slot].AssignBlock(pb);
            pb.CurrentProgramSpace = this;
            assigend = true;
        }
        else
        {
            if (TryShiftRight(slot))
            {
                Stop();
                Slots[slot].AssignBlock(pb);
                pb.CurrentProgramSpace = this;
                assigend = true;
            }
            else if (TryShiftLeft(slot))
            {
                Stop();
                Slots[slot].AssignBlock(pb);
                pb.CurrentProgramSpace = this;
                assigend = true;
            }
        }
        if (!assigend)
        {
            Vector3 force = OVRManager.instance.transform.position - pb.transform.position;
            force = force.normalized * 1f;
            force.y = 2f;
            pb.GetComponent<Rigidbody>().velocity = Vector3.zero;
            pb.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            _audioSource.PlayOneShot(PBNotAcceptedAC);
        }
        else
        {
            HasError();
            _audioSource.PlayOneShot(AddPBAC);
            OnBlockAdded?.Invoke(pb, slot);
        }
    }

    private bool TryShiftRight(int from)
    {
        for (int i = from; i < _availableSlots; i++)
        {
            if (!Slots[i].HasPB())
            {
                for (int j = i-1; j > from - 1; j--)
                {
                    AProgramBlock toShift = Slots[j].GetPB();
                    Slots[j + 1].AssignBlock(toShift);
                    Slots[j].AssignBlock(null);
                }
                return true;
            }
        }
        return false;
    }

    private bool TryShiftLeft(int from)
    {
        for (int i = from; i > -1; i--)
        {
            if (!Slots[i].HasPB())
            {
                for (int j = i + 1; j < from + 1; j++)
                {
                    AProgramBlock toShift = Slots[j].GetPB();
                    Slots[j - 1].AssignBlock(toShift);
                    Slots[j].AssignBlock(null);
                }
                return true;
            }
        }
        return false;
    }

    public bool HasError()
    {
        bool hasError = false;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].HasPB() && Slots[i].GetPB().HasError())
                hasError = true;

            if (!Slots[i].HasPB())
            {
                bool foundLaterPB = false;
                for (int j = i + 1; j < Slots.Length; j++)
                {
                    if (Slots[j].HasPB())
                    {
                        hasError = true;
                        foundLaterPB = true;
                    }
                }
                Slots[i].SetHasError(foundLaterPB);
            }
            else
            {
                Slots[i].SetHasError(false);
            }
        }
        return hasError;
    }
}
