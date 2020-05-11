using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgramBlockSlot : MonoBehaviour
{
    public Color DefaultColor;
    public Color AssignedColor;
    public Color ErrorColor;
    public Color ExecutingColor;
    public Color DisabledColor;


    private AProgramBlock _assignedPB;
    private Renderer[] _renderer;
    private bool _hasError = false;
    private bool _disabled = false;

    private void Awake()
    {
        _renderer = GetComponentsInChildren<Renderer>();
    }

    public bool HasPB() => _assignedPB != null;
    public AProgramBlock GetPB() => _assignedPB;

    public void AssignBlock(AProgramBlock programBlock)
    {
        if(programBlock == null)
        {
            foreach (Renderer renderer in _renderer)
                renderer.material.DOColor(DefaultColor, "_BaseColor", 0.2f);
            
            _assignedPB = null;
            return;
        }

        programBlock.transform.SetParent(transform);

        foreach(Renderer renderer in _renderer)
            renderer.material.DOColor(AssignedColor, "_BaseColor", 0.2f);

        programBlock.transform.DOMove(transform.position, 0.2f);
        programBlock.transform.rotation = transform.rotation;
        programBlock.SetState(AProgramBlock.BlockState.IN_SLOT);
        _assignedPB = programBlock;
    }

    public void Execute()
    {
        if (HasPB())
        {
            _assignedPB.Execute();
            _assignedPB.SetGrabbable(false);
            StartCoroutine(ExecuteCor());
        }
    }

    public void Stop()
    {
        if (HasPB())
        {
            _assignedPB.CancelExecution();
            _assignedPB.SetGrabbable(true);
            StopAllCoroutines();
        }
    }

    private IEnumerator ExecuteCor()
    {
        foreach (Renderer renderer in _renderer)
            renderer.material.DOColor(ExecutingColor, "_BaseColor", 0.2f);
        yield return new WaitUntil(() => _assignedPB.IsExecuting() == false);
        foreach (Renderer renderer in _renderer)
            renderer.material.DOColor(AssignedColor, "_BaseColor", 0.2f);
        _assignedPB.SetGrabbable(true);
    }

    public void SetHasError(bool hasError)
    {
        _hasError = hasError;
        UpdateColor();
    }

    public void SetDisabled(bool disabled)
    {
        _disabled = disabled;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_disabled)
        {
            foreach (Renderer renderer in _renderer)
                renderer.material.DOColor(DisabledColor, "_BaseColor", 0.2f);
        }
        else
        {
            if (_hasError)
            {
                foreach (Renderer renderer in _renderer)
                    renderer.material.DOColor(ErrorColor, "_BaseColor", 0.2f);
            }
            else
            {
                if (HasPB())
                {
                    foreach (Renderer renderer in _renderer)
                        renderer.material.DOColor(AssignedColor, "_BaseColor", 0.2f);
                }
                else
                {
                    foreach (Renderer renderer in _renderer)
                        renderer.material.DOColor(DefaultColor, "_BaseColor", 0.2f);
                }
            }
        }
    }
}
