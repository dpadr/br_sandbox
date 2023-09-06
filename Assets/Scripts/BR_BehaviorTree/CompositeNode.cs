using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BR_Node
{
    public List<BR_Node> children = new List<BR_Node>();
}
