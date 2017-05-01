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
			var column = args.GetColumn(colIndex);
			var cellRect = args.GetCellRect(colIndex);
			switch (column)
			{
				case 3:
					var lRect = cellRect;
					lRect.width /= 5;

					var clear = GUI.Button(lRect, "C");
					lRect.x += lRect.width;
					if(clear) list.Clear();
					var add = GUI.Button(lRect, "+");
					lRect.x += lRect.width;
					if(add) list.Add(null);
					var selectedIDs = orbTree.GetSelection();
					if (selectedIDs.Count != 1) Debug.LogWarning("Select ONE orb");
					else
					{
						var selectedItem = orbTree.itemsById[selectedIDs[0]];
						var thisNum = orbTree.GetRows().IndexOf(this);
						var selectedNum = orbTree.GetRows().IndexOf(selectedItem);
						var pos = selectedNum - thisNum - 1;
						if ((pos >= 0) && (pos < list.Count))
						{
							var remove = GUI.Button(lRect, "-");
							lRect.x += lRect.width;
							if(remove) list.RemoveAt(pos);

							var up = GUI.Button(lRect, "^");
							if (up && pos != 0)
							{
								var s = list.states[pos];
								var o = list[pos];
								list.RemoveAt(pos);
								list.Insert(pos-1,o, s, false);
							}

							lRect.x += lRect.width;
							var down = GUI.Button(lRect, "v");

							if (down && pos != list.Count-1)
							{
								var s = list.states[pos];
								var o = list[pos];
								list.RemoveAt(pos);
								list.Insert(pos + 1, o, s, false);
							}
						}
					}
					break;
				default:
					base.CellGUI(orbTree, ref args, colIndex);
					break;
			}
		}
	}
}