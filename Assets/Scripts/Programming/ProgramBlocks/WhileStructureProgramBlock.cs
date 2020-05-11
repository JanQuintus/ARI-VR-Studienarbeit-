using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhileStructureProgramBlock : AStructureProgramBlock
{
    public static readonly int MAX_ITERATIONS = 100;

    public SpriteRenderer ConditionSprite;

    [Header("Sprites")]
    public Sprite ConditionNoWallInFront;
    public Sprite ConditionWallInFront;
    public Sprite ConditionWallAbove;
    public Sprite ConditionWallBelow;
    public Sprite ConditionNoWallBelow;
    public Sprite ConditionGoalNotReached;

    private bool _inLoop = false;

    private void Start()
    {
        base.SetConditionType(Condition.ConditionType.GOAL_NOT_REACHED);
        ConditionSprite.sprite = ConditionGoalNotReached;
    }

    public override void SetConditionType(Condition.ConditionType conditionType)
    {
        if (conditionType == Condition.ConditionType.NONE)
            return;

        base.SetConditionType(conditionType);
        switch (conditionType) {
            case Condition.ConditionType.NO_WALL_IN_FRONT:
                ConditionSprite.sprite = ConditionNoWallInFront;
                break;
            case Condition.ConditionType.WALL_IN_FRONT:
                ConditionSprite.sprite = ConditionWallInFront;
                break;
            case Condition.ConditionType.WALL_ABOVE:
                ConditionSprite.sprite = ConditionWallAbove;
                break;
            case Condition.ConditionType.WALL_BELOW:
                ConditionSprite.sprite = ConditionWallBelow;
                break;
            case Condition.ConditionType.NO_WALL_BELOW:
                ConditionSprite.sprite = ConditionNoWallBelow;
                break;
            case Condition.ConditionType.GOAL_NOT_REACHED:
                ConditionSprite.sprite = ConditionGoalNotReached;
                break;
        }
    }

    public override void Execute()
    {
        StartCoroutine(ExecuteLoop());
    }

    private IEnumerator ExecuteLoop()
    {
        _inLoop = true;
        int iteration = 0;
        
        while(Condition.CheckCondition(ConditionType))
        {
            base.Execute();
            yield return new WaitUntil(() => base.IsExecuting() == false);
            iteration++;
            if (iteration >= MAX_ITERATIONS)
                break;
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
