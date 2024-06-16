using System;

public class ConditionNode : BaseNode
{
    private readonly Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override NodeStatus Execute()
    {
        return _condition.Invoke() ? NodeStatus.Success : NodeStatus.Failure;
    }
}
