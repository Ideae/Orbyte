using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Item), true)]
public class ItemEditor : Editor
{

  public override void OnInspectorGUI()
  {
    Item myTarget = (Item)target;
    EditorGUILayout.LabelField("Editing:", myTarget.GetType().Name);
    base.OnInspectorGUI();

  }
}