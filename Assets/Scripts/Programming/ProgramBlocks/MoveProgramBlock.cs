using UnityEngine;

public class MoveProgramBlock : AProgramBlock
{
    public Vector3 Direction = Vector3.forward;

    private bool _isExecuting = false;

    private void Update()
    {
        if (_isExecuting)
            _isExecuting = ARI.Instance.Mover.InAction();
    }

    public override void Execute()
    {
        if (!Condition.CheckCondition(ConditionType)) return;
        ARI.Instance.Mover.Move(Direction);
        _isExecuting = true;
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
