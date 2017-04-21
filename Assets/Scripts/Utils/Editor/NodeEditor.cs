using System.Collections.Generic;
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
			autoResize = true,
			canSort = false,
			contextMenuText = "CMenu",
			headerContent = new GUIContent("wot"),
			headerTextAlignment = TextAlignment.Right,
			maxWidth = 100,
			minWidth = 0,
			sortedAscending = false
		},

		new Column
		{
			allowToggleVisibility = true,
			autoResize = true,
			canSort = false,
			contextMenuText = "CMenuss",
			headerContent = new GUIContent("woasdat"),
			headerTextAlignment = TextAlignment.Right,
			sortedAscending = false
		},
	};

	public void OnEnable()
	{ 
		if (state == null) state = new TreeViewState();
		if (mchState == null) mchState = new MultiColumnHeaderState(columns); 
		multiColumnHeader = new MultiColumnHeader(mchState);
		orbTree = new OrbTree(state,multiColumnHeader, serializedObject.targetObject as Node);
	}

	void ResetState()
	{
		state = new TreeViewState();
		mchState = new MultiColumnHeaderState(columns);
		multiColumnHeader = new MultiColumnHeader(mchState);
		orbTree = new OrbTree(state, multiColumnHeader, serializedObject.targetObject as Node);
	}
	public override void OnInspectorGUI()
	{
		//serializedObject.Update(); 
		//Node n = serializedObject.targetObject as Node;
		var text = new GUIContent("Reset editor");
		bool butt = GUI.Button(GetRect(text, GUIStyle.none), text);
		if (butt) ResetState();
		

		DrawDefaultInspector(); 

		var width = currentViewWidth - 40;
		var rect = GetRect(width, HEIGHT);
		orbTree.OnGUI(rect);
		//serializedObject.ApplyModifiedProperties();

	}
}
