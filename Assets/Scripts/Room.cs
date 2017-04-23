using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	public Node NodePrefab;
	public List<Node> nodes = new List<Node>();

	public GameObject SceneNodes;

	// Use this for initialization
	void Start()
	{
		UIManager.Instance.RegisterRoom(this);
		var ns = FindObjectsOfType<Node>();
		foreach (var n in ns)
		{
			n.room = this;
			if (!nodes.Contains(n))
				nodes.Add(n);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Delete))
			DeleteAllNodes();
	}

	public Node SpawnNode(Vector2 pos, OrbList orbs = null, bool cloneOrbs = false)
	{
		var node = Instantiate(NodePrefab, SceneNodes.transform);

		node.room = this;
		node.transform.position = new Vector3(pos.x, pos.y, gameObject.transform.position.z);
		if (orbs != null)
			node.Orbs.AddAll(orbs, cloneOrbs);
		nodes.Add(node);
		return node;
	}

	void DeleteAllNodes()
	{
		for (var i = 0; i < nodes.Count; i++)
		{ 
			var n = nodes[i];
			if (n.Orbs.Get<Player>() != null) continue;
			n.DeleteNode();
			nodes.RemoveAt(i);
			i--;
		}
	}
}