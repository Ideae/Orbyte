using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;
using static UnityEngine.GUILayoutUtility;

[CustomEditor(typeof(Node))]
class NodeEditor :Editor
{

	const float HEIGHT = 200;

	public OrbTree orbTree;
	public TreeViewState state;
	public MultiColumnHeaderState mchState;
	MultiColumnHeader multiColumnHeader;


	static readonly Column[] columns = {
		new Column
		{
			allowToggleVisibility = true,
			contextMenuText = "Icon",
			maxWidth = 20,
			width = 20,
			minWidth = 20,
			autoResize = false,

		},

		new Column
		{
			allowToggleVisibility = false,
			contextMenuText = "Equip",
			headerContent = new GUIContent("Equip"),
			maxWidth = 20,
			width = 20,
			minWidth = 20,
			autoResize = false,

		},

		new Column
		{
			allowToggleVisibility = false,
			autoResize = true,
			contextMenuText = "Elements",
			headerContent = new GUIContent("Elements"), 
		},

		new Column
		{
			allowToggleVisibility = true,
			autoResize = true, 
			contextMenuText = "Values",
			headerContent = new GUIContent("Values"),  
		},
	};

	public void OnEnable()
	{ 
		Setup(true);
	}

	void Setup(bool keepState)
	{
		if (state == null || !keepState) state = new TreeViewState();
		if (mchState == null || !keepState) mchState = new MultiColumnHeaderState(columns);
		multiColumnHeader = new MultiColumnHeader(mchState)
		{
			canSort = false
		};
		orbTree = new OrbTree(state, multiColumnHeader, serializedObject.targetObject as Node);
	}
	public override void OnInspectorGUI()
	{
		
		//serializedObject.Update(); 
		//Node n = serializedObject.targetObject as Node;
		var text = new GUIContent("Reset editor");
		bool butt = GUI.Button(GetRect(text, GUIStyle.none), text);
		if (butt) Setup(false);

		DrawDefaultInspector(); 

		var width = currentViewWidth - 40;
		var rect = GetRect(width, HEIGHT);

		
		multiColumnHeader.ResizeToFit();
		orbTree.OnGUI(rect);
		//serializedObject.ApplyModifiedProperties();

	}
}
