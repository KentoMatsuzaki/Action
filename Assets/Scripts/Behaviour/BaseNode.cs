public abstract class BaseNode
{
    public abstract NodeStatus Execute();
}

public enum NodeStatus
{
    Success,
    Failure,
    Running
}
