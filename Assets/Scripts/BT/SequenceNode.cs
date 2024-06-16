using System.Collections.Generic;

public class SequenceNode : BaseNode
{
    private readonly List<BaseNode> _children = new List<BaseNode>();

    public void AddChild(BaseNode child)
    {
        _children.Add(child);
    }

    public override NodeStatus Execute()
    {
        foreach (var child in _children)
        {
            NodeStatus status = child.Execute();

            if (status == NodeStatus.Failure)
            {
                return NodeStatus.Failure;
            }
            else if (status == NodeStatus.Running)
            {
                return NodeStatus.Running;
            }
        }
        return NodeStatus.Success;
    }
}
