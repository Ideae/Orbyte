using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

public partial class OrbTree : TreeView
{
	class OrbListItem : FPInfoItem
	{
		static Texture2D defaultIcon;
		public readonly object owner;
		static Texture2D DefaultIcon => defaultIcon ?? (defaultIcon = FindTexture("UnityEditor.HierarchyWindow"));
		public List<Orb> list => Info.GetValue(owner) as List<Orb>;

		public override bool hasChildren => true;

		public OrbListItem(object owner, FPInfo info, int id) : base(info, id)
		{
			this.owner = owner;
		}

		//public override void RowGUI(OrbTree orbTree, ref RowGUIArgs args) {}
	}
}