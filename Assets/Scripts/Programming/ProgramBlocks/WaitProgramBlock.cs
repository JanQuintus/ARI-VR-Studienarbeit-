using UnityEngine;
using System.Collections;

public class WaitProgramBlock : AProgramBlock
{
    private bool _isExecuting = false;

    public override void Execute()
    {
        if (!Condition.CheckCondition(ConditionType)) return;
        StartCoroutine(WaitForConditionFalse());
        _isExecuting = true;
    }

    private IEnumerator WaitForConditionFalse()
    {
        while (Condition.CheckCondition(ConditionType))
            yield return null;

        _isExecuting = false;
    }


    public override bool IsExecuting()
    {
        return _isExecuting;
    }

    public override void CancelExecution()
    {
        _isExecuting = false;
    }

    public override bool HasError()
    {
        return false;
    }
}
