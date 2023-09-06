using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;

public class BehaviorTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits>{};
    BR_BehaviorTree tree;

    public BehaviorTreeView() {

        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach(var type in types) {
                Debug.Log("here");
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach(var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach(var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }
        
    }

    internal void PopulateView(BR_BehaviorTree tree) {
        this.tree = tree;

        DeleteElements(graphElements);

        tree.nodes.ForEach(n => CreateNodeView(n));
    }


    void CreateNode(System.Type type) {
        Debug.Log(tree == null ? "True" : "False");
        BR_Node node = tree.CreateNode(type);
        Debug.Log(node);
        CreateNodeView(node);
    }

    void CreateNodeView(BR_Node node) {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }
}
