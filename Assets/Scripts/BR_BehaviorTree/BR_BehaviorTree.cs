using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class BR_BehaviorTree : ScriptableObject
{
    public BR_Node rootNode;
    public BR_Node.State treeState = BR_Node.State.Running;
    public List<BR_Node> nodes = new List<BR_Node>();

    public BR_Node.State Update() {
        if (rootNode.state == BR_Node.State.Running) {
            treeState = rootNode.Update();
        }
        return treeState;
    }

    public BR_Node CreateNode(System.Type type) {
        BR_Node node = ScriptableObject.CreateInstance(type) as BR_Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BR_Node node) {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
}
