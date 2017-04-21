using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;
using static UnityEngine.GUILayoutUtility;

partial class OrbTree : TreeView
{
	static float ToggleWidth;
	readonly Node rootNode;
	public OrbTree(TreeViewState state, MultiColumnHeader header, Node root) : base(state, header)
	{
		rowHeight = 20;
		columnIndexForTreeFoldouts = 0;
		showAlternatingRowBackgrounds = true;
		showBorder = true;
		//customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f;
		//extraSpaceBeforeIconAndLabel = kToggleWidth;
		this.rootNode = root;
		Reload();
		
	}

	protected override void RowGUI(RowGUIArgs args)
	 {
		 (args.item as OrbItem)?.RowGUI(this, args);
		 (args.item as FPInfoItem)?.RowGUI(this, args);
		
	 }

	readonly FPInfo NodeOrbProperty = new FPInfo(FastType<Node>.type.GetProperty(nameof(Node.Orbs)));

	protected override TreeViewItem BuildRoot()
	{
		// BuildRoot is called every time Reload is called to ensure that TreeViewItems 
		// are created from data. Here we create a fixed set of items. In a real world example,
		// a data model should be passed into the TreeView and the items created from the model.

		// This section illustrates that IDs should be unique. The root item is required to 
		// have a depth of -1, and the rest of the items increment from that.

		var root = new OrbListItem(rootNode, NodeOrbProperty, idCounter++) {depth = -1};
		// Return root of the tree
		return root;
	}

	static int idCounter = 0;

	protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
	{
		var ids = GetExpanded();
		var ret = new List<TreeViewItem>(); 
		BuildRecursive(ret, root, ids);
		SetupDepthsFromParentsAndChildren(root);
		return ret;
	}
	Dictionary<int, TreeViewItem> itemsById = new Dictionary<int, TreeViewItem>();
	private void BuildRecursive(List<TreeViewItem> refList, TreeViewItem node, IList<int> expandedIds)
	{
		itemsById[node.id] = node;
		refList.Add(node);
		if(!expandedIds.Contains(node.id) && node.depth!=-1) return;
		var orb = (node as OrbItem)?.orb;
		if (orb != null)
		{
			foreach (var fpInfo in orb.InspectableVariables)
			{
				var child = node.children.FirstOrDefault(item => (item as FPInfoItem)?.Info == fpInfo);
				if (child == null)
				{
					child = FastType<IList<Orb>>.type.IsAssignableFrom(fpInfo.memberType) ?
						new OrbListItem(orb, fpInfo, idCounter++) :
						new FPInfoItem(fpInfo, idCounter++);
					node.AddChild(child);
				}

				BuildRecursive(refList, child, expandedIds);
			}
			return;
		}
		var orblist = (node as OrbListItem)?.list;
		if (orblist != null)
		{
			foreach (var o in orblist)
			{
				var child = node.children.FirstOrDefault(item => (item as OrbItem)?.orb == o);
				if (child == null)
				{
					child = new OrbItem(o, idCounter++);
					node.AddChild(child);
				}
				BuildRecursive(refList, child, expandedIds);
			}
		}

	}

	class FPInfoItem : TreeViewItem
	{

		static Texture2D defaultIcon;
		public FPInfo Info { get; }

		public FPInfoItem(FPInfo info, int id)
		{
			this.id = id;
			children = new List<TreeViewItem>();
			this.Info = info;
			displayName = info.Name;
			icon = defaultIcon ?? (defaultIcon = FindTexture("FilterByType"));
		}

		public override bool hasChildren => false;

		public virtual void RowGUI(OrbTree orbTree, RowGUIArgs args)
		{
			
		}
	}
	class OrbListItem : FPInfoItem
	{

		static Texture2D defaultIcon;
		public readonly object owner;
		public List<Orb> list => Info.GetValue(owner) as List<Orb>;
		public OrbListItem(object owner, FPInfo info, int id) : base(info, id)
		{
			this.owner = owner;
			this.icon = defaultIcon ?? (defaultIcon = FindTexture("UnityEditor.HierarchyWindow"));
		}

		public override bool hasChildren => true;

		public override void RowGUI(OrbTree orbTree, RowGUIArgs args)
		{

		}
	}

	class OrbItem : TreeViewItem
	{

		static Texture2D defaultIcon;
		public readonly Orb orb;
		public OrbItem(Orb orb, int id)
		{
			this.id = id;
			children = new List<TreeViewItem>();
			this.orb = orb;
			this.displayName = orb?.name ?? "Empty";
			this.icon = defaultIcon ?? (defaultIcon = FindTexture("Material Icon"));
		}

		public override bool hasChildren => (orb != null) && (orb.InspectableVariables.Count > 0);

		public virtual void RowGUI(OrbTree orbTree, RowGUIArgs args)
		{
			var colNum = args.GetNumVisibleColumns();
			for (int colIndex = 0; colIndex < colIndex; colIndex++)
			{
				var column = args.GetColumn(colIndex);
				var cellRect = args.GetCellRect(colIndex);
				switch (column)
				{

					case 1:

						// Draw custom texture
						GUI.DrawTexture(cellRect, icon, ScaleMode.ScaleToFit);
						break;

					case 2:
						var eq = orb as IEquippable; 
						if (eq == null || depth!= 0) break;
						bool oldEquip = eq.IsEquipped();
						bool equip = EditorGUI.Toggle(cellRect, oldEquip, "");
						if(oldEquip!= equip) eq.SetEquipped(equip);
						break;

					case 3:

						// Make a toggle button to the left of the label text
						Rect toggleRect = cellRect;
						toggleRect.x += orbTree.GetContentIndent(this);
						toggleRect.width = ToggleWidth;
						if (toggleRect.xMax < cellRect.xMax)
						{
							bool oldActive = orb.IsActive;
							bool active = EditorGUI.Toggle(toggleRect, oldActive);
							if (active != oldActive) orb.IsActive = active;
						}

						// Default icon and label
						args.rowRect = cellRect;
						orbTree.RowGUI(args);
						break;

					case 4:

						EditorGUI.PropertyField(position, property, label, true);

						// Show a Slider control for value 1
						item.data.floatValue1 = EditorGUI.Slider(cellRect, GUIContent.none, item.data.floatValue1, 0f, 1f);
						break;

					case MyColumns.Value2:

						// Show an ObjectField for materials
						item.data.material = (Material)EditorGUI.ObjectField(cellRect, GUIContent.none, item.data.material,
						                                                     typeof(Material), false);
						break;

					case MyColumns.Value3:

						// Show a TextField for the data text string
						item.data.text = GUI.TextField(cellRect, item.data.text);
						break;
				}
			}
			

		}
	}



}