using BehaviorTree;

public class BoarBT : BehaviourTree
{

    public static float speed = 2f;

    protected override Node SetupTree()
    {
        Node root = new TaskWander(transform);
        /*Node root = new Sequence(new List<Node> //This isn't working, need to figure out how to reping IsHost until it runs
            {
                new IsHost(),
                new TaskWander(transform),
            });*/
        return root;
    }
}