using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inspectorView;

    [MenuItem("BehaviorTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviorTreeView>();
        inspectorView = root.Q<InspectorView>();
    }

    // private void OnSelectionChange() {
    //     BR_BehaviorTree tree = Selection.activeObject as BR_BehaviorTree;
    //     Debug.Log(tree == null ? "NULL" : "OK");
    //     if (tree) {
    //         treeView.PopulateView(tree);
    //     }
    // }

    private void OnSelectionChange()
    {
        BR_BehaviorTree tree = Selection.activeObject as BR_BehaviorTree;
        if (tree == null)
        {
            if (Selection.activeGameObject)
            {
                BehaviorTreeRunner treeRunner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                if (treeRunner)
                {
                    tree = treeRunner.tree;
                }
            }
        }

        if (tree != null)
        {
            if (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                SerializedObject so = new SerializedObject(tree);
                rootVisualElement.Bind(so);
                if (treeView != null)
                    treeView.PopulateView(tree);

                return;
            }
        }

        rootVisualElement.Unbind();

        TextField textField = rootVisualElement.Q<TextField>("BehaviorTreeName");
        if (textField != null)
        {
            textField.value = string.Empty;
        }
    }
}