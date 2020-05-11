using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(ProgramSpace))]
public abstract class AStructureProgramBlock : AProgramBlock
{

    private Animator _anim;
    [HideInInspector]
    public ProgramSpace PS;
    public bool IsInitialized = false;

    protected override void Init()
    {
        base.Init();
        _anim = GetComponent<Animator>();
        PS = GetComponent<ProgramSpace>();
        IsInitialized = true;
    }

    public override void SetState(BlockState state, bool expand = true)
    {
        base.SetState(state);
        if(expand)
            _anim.SetBool("Show", state == BlockState.IN_SLOT);
    }

    public override void CancelExecution()
    {
        PS.Stop();
    }

    public override bool HasError()
    {
        return PS.HasError();
    }

    public override void Execute()
    {
        PS.Execute();
    }

    public override bool IsExecuting()
    {
        return PS.IsExecuting();
    }
}
