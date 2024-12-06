using System;

public class ActionNode : INode
{
    public delegate INode.ENodeState ActionNodeCallback();

    private ActionNodeCallback actionNodeCallback;

    public ActionNode(ActionNodeCallback _actionNodeCallback)
    {
        actionNodeCallback = _actionNodeCallback;
    }

    public INode.ENodeState Evaluate() => actionNodeCallback?.Invoke() ?? INode.ENodeState.ENS_Failure;
}
