using UnityEngine;
using UnityEngine.UI;

public abstract class AProgramBlock : MonoBehaviour
{
    public enum BlockState
    {
        DEFAULT,
        GRABBING,
        IN_SLOT
    }

    public string ID;
    public string DisplayName;
    public string Description;
    public Sprite Icon;

    [HideInInspector]
    public ProgramSpace CurrentProgramSpace = null;

    public OVRGrabbable Grabbable { get; private set; }

    public BlockState State { get; private set; }

    [SerializeField]
    protected Condition.ConditionType ConditionType = Condition.ConditionType.NONE;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Grabbable = GetComponent<OVRGrabbable>();
        Grabbable.OnGrabBegin += BeginGrab;
        Grabbable.OnGrabEnd += EndGrab;
    }

    private void BeginGrab() => SetState(BlockState.GRABBING);
    private void EndGrab() => SetState(BlockState.DEFAULT);

    public abstract void Execute();
    public abstract bool IsExecuting();
    public abstract bool HasError();
    public abstract void CancelExecution();

    public virtual void SetConditionType(Condition.ConditionType conditionType)
    {
        ConditionType = conditionType;
    }

    public virtual Condition.ConditionType GetConditionType() => ConditionType;

    public virtual void SetState(BlockState state, bool expand = true)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        switch (state)
        {
            case BlockState.DEFAULT:
                rb.isKinematic = false;
                rb.useGravity = true;
                gameObject.layer = 0;
                CurrentProgramSpace = null;
                transform.SetParent(null);
                break;
            case BlockState.GRABBING:
                rb.isKinematic = true;
                rb.useGravity = false;
                gameObject.layer = 8;
                transform.SetParent(null);
                if (CurrentProgramSpace != null)
                {
                    CurrentProgramSpace.RemoveFromProgramSpace(this);
                    CurrentProgramSpace = null;
                }
                break;
            case BlockState.IN_SLOT:
                rb.isKinematic = true;
                rb.useGravity = false;
                gameObject.layer = 8;
                break;
        }

        State = state;
    }

    public void SetGrabbable(bool state)
    {
        Grabbable.enabled = state;
    }

    private void OnDestroy()
    {
        Grabbable.OnGrabBegin -= BeginGrab;
        Grabbable.OnGrabEnd -= EndGrab;
    }
}
