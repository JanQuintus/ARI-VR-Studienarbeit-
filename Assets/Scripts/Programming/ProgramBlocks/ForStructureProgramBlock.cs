using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForStructureProgramBlock : AStructureProgramBlock
{
    public static readonly int MAX_COUNT = 10;

    public TMPro.TMP_Text CountText;

    [SerializeField]
    private int _count = 1;
    private bool _inLoop = false;

    private void Start()
    {
        CountText.SetText(_count.ToString());
    }

    public void IncreaseCount()
    {
        _count++;
        if (_count > MAX_COUNT) _count = MAX_COUNT;
        CountText.SetText(_count.ToString());
    }

    public void DecreaseCount()
    {
        _count--;
        if (_count < 0) _count = 0;
        CountText.SetText(_count.ToString());
    }

    public int GetCount() => _count;

    public override void Execute()
    {
        StartCoroutine(ExecuteLoop());
    }

    private IEnumerator ExecuteLoop()
    {
        _inLoop = true;
        for(int i = 0; i < _count; i++)
        {
            base.Execute();
            yield return new WaitUntil(() => base.IsExecuting() == false);
        }
        _inLoop = false;
    }

    public override void CancelExecution()
    {
        base.CancelExecution();
        StopAllCoroutines();
    }

    public override bool IsExecuting()
    {
        return _inLoop;
    }
}
