using System;

public class ActionNode : BaseNode
{
    private readonly Func<NodeStatus> _action;

    public ActionNode(Func<NodeStatus> action)
    {
        _action = action;
    }

    public override NodeStatus Execute()
    {
        return _action.Invoke();
    }
}
