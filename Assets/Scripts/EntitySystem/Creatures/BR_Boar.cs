using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
public class BR_Boar : BR_Animal
{
    [SerializeField]
    private int maxHealth = 10;

    BoarBT boarBT;
    protected void Start()
    {
        base.Start(); //calls the start function of the parent class (in this case, bc Animal has no start, it calls Creature's)
        _healthComponent.SetMaxHealth(maxHealth);
        boarBT = new BoarBT();
    }
    protected override void OnUpdate()
    {



    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        _healthComponent = GetComponent<BR_HealthComponent>();
        _healthComponent.SetMaxHealth(maxHealth);
#endif
    }

}

public class BoarBT : BehaviorTree.Tree
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