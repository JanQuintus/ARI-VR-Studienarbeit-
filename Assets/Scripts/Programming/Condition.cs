public class Condition {
    public enum ConditionType
    {
        NONE,
        NO_WALL_IN_FRONT,
        WALL_IN_FRONT,
        WALL_ABOVE,
        WALL_BELOW,
        NO_WALL_BELOW,
        GOAL_NOT_REACHED
    }

    public static bool CheckCondition(ConditionType conditionType)
    {
        switch (conditionType)
        {
            case ConditionType.NONE:
                return true;
            case ConditionType.NO_WALL_IN_FRONT:
                return !ARI.Instance.IsWallInDirection(ARI.Instance.transform.forward);
            case ConditionType.WALL_IN_FRONT:
                return ARI.Instance.IsWallInDirection(ARI.Instance.transform.forward);
            case ConditionType.WALL_ABOVE:
                return ARI.Instance.IsWallInDirection(ARI.Instance.transform.up);
            case ConditionType.WALL_BELOW:
                return ARI.Instance.IsWallInDirection(-ARI.Instance.transform.up);
            case ConditionType.NO_WALL_BELOW:
                return !ARI.Instance.IsWallInDirection(-ARI.Instance.transform.up);
            case ConditionType.GOAL_NOT_REACHED:
                return !ARI.Instance.GoalReached();
            default:
                return false;
        }
    }
}
