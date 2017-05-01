using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

public partial class OrbTree : TreeView
{
	class FPInfoItem : TreeViewItem
	{
		static Texture2D defaultIcon;
		protected virtual Texture2D DefaultIcon => AssetPreview.GetMiniTypeThumbnail(Info.VariableType)??defaultIcon ?? (defaultIcon = FindTexture("FilterByType"));
		public FPInfo Info { get; }
		public float? height { get; set; }
		public override bool hasChildren => false;

		public FPInfoItem(FPInfo info, int id)
		{
			this.id = id;
			children = new List<TreeViewItem>();
			Info = info;
			displayName = info.Name;
		}


		public virtual void RowGUI(OrbTree orbTree, ref RowGUIArgs args)
		{
			var colNum = args.GetNumVisibleColumns();
			for (var colIndex = 0; colIndex < colNum; colIndex++)
			{
				CellGUI(orbTree, ref args, colIndex);
			}
		}

		protected virtual void CellGUI(OrbTree orbTree, ref RowGUIArgs args, int colIndex)
		{
			var column = args.GetColumn(colIndex);
			var cellRect = args.GetCellRect(colIndex);
			switch (column)
			{
				case 0:
					// Draw custom texture
					GUI.DrawTexture(cellRect, DefaultIcon, ScaleMode.ScaleToFit);
					break;
				case 1: 
					break;

				case 2:

					// Make a toggle button to the left of the label text

					var toggleRect = cellRect;
					toggleRect.x += orbTree.GetContentIndent(this);
					toggleRect.width = ToggleWidth;
					if (toggleRect.xMax < cellRect.xMax)
					{
						//Todo:
						EditorGUI.Toggle(toggleRect, false);
					}

					// Default icon and label
					args.rowRect = cellRect;
					orbTree.BaseRowGUI(args);
					break;

				case 3:


					var writerObj = WriterObject.Get(Info.VariableType);
					if (writerObj != null)
					{
						var parentItem = (parent as OrbItem);
						if(parentItem == null)break;
						var parentobj = parentItem.orb;
						var obj = Info.GetValue(parentobj);
						writerObj.objValue = obj;
						writerObj.prop.serializedObject.Update();
						var h = height;
						var exp = EditorGUI.PropertyField(cellRect, writerObj.prop, GUIContent.none, true);
						height = EditorGUI.GetPropertyHeight(writerObj.prop, true);
						if (h!= height)orbTree.RefreshCustomRowHeights();
						writerObj.prop.serializedObject.ApplyModifiedPropertiesWithoutUndo();
						if (writerObj.objValue != obj)
						{
							var newObj = writerObj.objValue;
							Undo.RecordObject(parentobj, $"Update {this.displayName} on {parent.displayName}");
							Info.SetValue(parentobj, newObj);
						}
					}
					break;
			}

		}
	}
}