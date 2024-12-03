using System.Collections.Generic;


public class SelectorNode : INode
{
    List<INode> childs;

    public SelectorNode(List<INode> _childs)
    {
        childs = _childs;
    }

    public INode.ENodeState Evaluate()
    {
        if (childs == null)
            return INode.ENodeState.ENS_Failure;

        foreach(var child in childs)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                case INode.ENodeState.ENS_Success:
                    return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
}
