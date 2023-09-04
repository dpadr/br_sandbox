using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public BR_Node node;
    public NodeView(BR_Node node) {
        this.node = node;
        this.title = node.name;
    }
}
