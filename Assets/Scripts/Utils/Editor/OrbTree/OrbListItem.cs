using UnityEngine;
using static UnityEditor.EditorGUIUtility;

public partial class OrbTree
{
	class OrbListItem : FPInfoItem
	{
		static Texture2D defaultIcon;
		readonly object owner;
		protected override Texture2D DefaultIcon => defaultIcon ?? (defaultIcon = FindTexture("UnityEditor.HierarchyWindow"));
		public OrbList list => Info.GetValue(owner) as OrbList;

		public override bool hasChildren => list?.Count>0;

		public OrbListItem(object owner, FPInfo info, int id) : base(info, id)
		{
			this.owner = owner;
		}

		protected override void CellGUI(OrbTree orbTree, ref RowGUIArgs args, int colIndex)
		{
			base.CellGUI(orbTree, ref args, colIndex);
		}
	}
}