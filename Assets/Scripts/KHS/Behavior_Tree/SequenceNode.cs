using System.Collections.Generic;
public class SequenceNode : INode
{
    List<INode> childs;

    public SequenceNode(List<INode> _childs)
    {
        childs = _childs;
    }

    public INode.ENodeState Evaluate()
    {
        if (childs == null || childs.Count == 0)
            return INode.ENodeState.ENS_Failure;

        foreach(INode child in childs)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                case INode.ENodeState.ENS_Success:
                    continue;
                case INode.ENodeState.ENS_Failure:
                    return INode.ENodeState.ENS_Failure;
            }
        }

        return INode.ENodeState.ENS_Success;
    }

}
