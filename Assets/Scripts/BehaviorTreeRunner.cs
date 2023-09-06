using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    public BR_BehaviorTree tree;
    // Start is called before the first frame update
    void Start()
    {
        tree = ScriptableObject.CreateInstance<BR_BehaviorTree>();

        var log = ScriptableObject.CreateInstance<DebugLogNode>();
        log.message = "Testing";

        tree.rootNode = log;
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}
