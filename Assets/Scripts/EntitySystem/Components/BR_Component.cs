using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BR_Component : MonoBehaviour
{
    public abstract void OnUpdate();

    private BR_Entity _parent;
    public BR_Entity Parent
    {
        set { _parent = value; }
        get { return _parent; }
    }
}
