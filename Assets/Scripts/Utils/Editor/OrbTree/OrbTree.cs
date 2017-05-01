using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

public partial class OrbTree : TreeView
{

	static float ToggleWidth = 16;

	static int idCounter;

	readonly FPInfo NodeOrbProperty = new FPInfo(FastType<Node>.type.GetField(nameof(Node.Orbs)));
	readonly Node rootNode;
	readonly Dictionary<int, TreeViewItem> itemsById = new Dictionary<int, TreeViewItem>();
	static float RowHeights = 20;

	protected override float GetCustomRowHeight(int row, TreeViewItem item) =>
		(item as FPInfoItem)?.height ?? base.GetCustomRowHeight(row, item);

	public OrbTree(TreeViewState state, MultiColumnHeader header, Node root) : base(state, header)
	{
		rowHeight = RowHeights;
		columnIndexForTreeFoldouts = 2;
		showAlternatingRowBackgrounds = true;
		showBorder = true;
		//customFoldoutYOffset = (RowHeights - EditorGUIUtility.singleLineHeight) * 0.5f;
		extraSpaceBeforeIconAndLabel = ToggleWidth;
		rootNode = root;
		Reload();
	}

	protected void BaseRowGUI(RowGUIArgs args)
	{
		base.RowGUI(args);
	}

	protected override void RowGUI(RowGUIArgs args)
	{

		var orbs = rootNode.Orbs.ToArray();
		var ogNodes = orbs.Select(o => o?._node).ToArray();
		foreach (var orb in orbs.Where(o=>o!=null)) { orb._node = rootNode; }
		(args.item as OrbItem)?.RowGUI(this, ref args);
		(args.item as FPInfoItem)?.RowGUI(this, ref args);

		for (var index = 0; index < orbs.Length; index++)
		{
			if (orbs[index] != null) orbs[index]._node = ogNodes[index];
		}
	}

	protected override TreeViewItem BuildRoot()
	{
		var root = new OrbListItem(rootNode, NodeOrbProperty, idCounter=0) {depth = -1};
		// Return root of the tree
		return root;
	}

	protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
	{
		var ids = GetExpanded();
		var ret = new List<TreeViewItem>();
		BuildRecursive(ret, root, ids);
		SetupDepthsFromParentsAndChildren(root);
		return ret;
	}

	void BuildRecursive(List<TreeViewItem> refList, TreeViewItem node, IList<int> expandedIds)
	{
		itemsById[node.id] = node;
		refList.Add(node);
		if (!expandedIds.Contains(node.id)) return;
		var orb = (node as OrbItem)?.orb;
		if (orb != null)
		{
			foreach (var fpInfo in orb.InspectableVariables)
			{
				var child = node.children.FirstOrDefault(item => (item as FPInfoItem)?.Info == fpInfo);
				if (child == null)
				{
					child = FastType<OrbList>.type.IsAssignableFrom(fpInfo.VariableType)
						? new OrbListItem(orb, fpInfo, idCounter++)
						: new FPInfoItem(fpInfo, idCounter++);
					node.AddChild(child);
				}

				BuildRecursive(refList, child, expandedIds);
			}
			return;
		}
		var orblist = (node as OrbListItem)?.list;
		if (orblist != null)
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