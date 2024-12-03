public class BehaviorTreeRunner
{
    INode rootNode;
    public BehaviorTreeRunner(INode _rootNode)
    {
        rootNode = _rootNode;
    }

    public void Operate()
    {
        rootNode.Evaluate();
    }
}
