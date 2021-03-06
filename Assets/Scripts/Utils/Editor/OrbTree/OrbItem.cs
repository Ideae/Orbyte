﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;
using Object = UnityEngine.Object;

public partial class OrbTree : TreeView
{
	class OrbItem : TreeViewItem
	{
		static Texture2D defaultIcon;
		public readonly Orb orb;

		Dictionary<Type, Object> setterCache = new Dictionary<Type, Object>();
		static Texture2D DefaultIcon => defaultIcon ?? (defaultIcon = FindTexture("Material Icon"));

		public override bool hasChildren => (orb != null) && (orb.InspectableVariables.Count > 0);

		public OrbItem(Orb orb, int id)
		{
			this.id = id;
			children = new List<TreeViewItem>();
			this.orb = orb;
			displayName = orb?.name ?? "Empty";
		}


		public virtual void RowGUI(OrbTree orbTree, ref RowGUIArgs args)
		{
			var colNum = args.GetNumVisibleColumns();
			for (var colIndex = 0; colIndex < colNum; colIndex++)
			{
				CellGUI(orbTree, ref args, colIndex);
			}
		}

		private void CellGUI(OrbTree orbTree, ref RowGUIArgs args, int colIndex)
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
					var eq = orb as IEquippable;
					var orblist = (parent as OrbListItem)?.list;
					if ((eq == null) || (depth != 0) || (orblist == null) ) break;
					var index = orblist.IndexOf(orb);
					var oldEquip = orblist.IsEquipped(index);

					var toggleRect = cellRect;
					toggleRect.width = ToggleWidth;
					var equip = EditorGUI.Toggle(cellRect, oldEquip);
					if (oldEquip != equip)
					{
						Undo.RecordObject(orbTree.rootNode, "Equip");

						orblist.SetEquipped(orb.EquipSlots,index,equip);
					}
					break;

				case 2:
					if (orb == null) break;
					// Make a toggle button to the left of the label text
					toggleRect = cellRect;
					toggleRect.x += orbTree.GetContentIndent(this);
					toggleRect.width = ToggleWidth;
					if (toggleRect.xMax < cellRect.xMax)
					{

						var oldActive = orb.IsActive;
						var active = EditorGUI.Toggle(toggleRect, oldActive);
						if (active != oldActive)
						{

							Undo.RecordObject(orbTree.rootNode, "Toggle");
							orb.IsActive = active;
						}
					}

					// Default icon and label
					args.rowRect = cellRect;
					orbTree.BaseRowGUI(args);
					break;

				case 3:


					var writerObj = WriterObject<Orb>.Get();
					if (writerObj != null)
					{
						writerObj.value = orb;
						writerObj.prop.serializedObject.Update();
						EditorGUI.PropertyField(cellRect, writerObj.prop, GUIContent.none, true);
						writerObj.prop.serializedObject.ApplyModifiedPropertiesWithoutUndo();
						if (writerObj.value != orb)
						{
							var newOrb = writerObj.value;
							Undo.RecordObject(orbTree.rootNode, $"Swap {orb?.name} for {newOrb?.name} on {orb?._node?.name}");
							orblist = ((OrbListItem)parent).list;
							var i = orblist.IndexOf(orb);
							if (!Application.isPlaying)
							{
								orblist[i] = newOrb;
								if(newOrb!= null) newOrb._node = orb?._node;
								if(orb!=null)orb._node = null;
								this.children.Clear();

							}
							else
							{
								if((orb!= null) && (orb._node!=null))
									orb._node.Orbs[i] = newOrb?.Clone();
							}
							orbTree.Reload();
						}
					}
					break;
			}
			
		}
	}
}