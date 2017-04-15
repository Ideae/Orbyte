using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
  public Node NodePrefab;
  public GameObject SceneNodes;
  public List<Node> nodes = new List<Node>();
	// Use this for initialization
	void Start ()
	{
	    UIManager.Instance.RegisterRoom(this);
    var ns = FindObjectsOfType<Node>();
    foreach(Node n in ns)
    {
      n.room = this;
      if (!nodes.Contains(n))
      {
        nodes.Add(n);

      }
    }
	}

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Delete))
    {
      DeleteAllNodes();
    }

    for (int i = 0; i < nodes.Count; i++)
    {
      Node n1 = nodes[i];
      for (int j = i + 1; j < nodes.Count; j++)
      {
        Node n2 = nodes[j];
        n1.AffectOther(n2);
        n2.AffectOther(n1);
      }
    }

  }

  void FixedUpdate()
  {
    for (int i = 0; i < nodes.Count; i++)
    {
      Node n1 = nodes[i];
      for (int j = i + 1; j < nodes.Count; j++)
      {
        Node n2 = nodes[j];
        n1.FixedAffectOther(n2);
        n2.FixedAffectOther(n1);
      }
    }
  }

  public Node SpawnNode(Vector2 pos, List<Orb> orbs = null)
  {
    Node node = Instantiate(NodePrefab, SceneNodes.transform);
    node.transform.position = new Vector3(pos.x, pos.y, gameObject.transform.position.z);
    if (orbs != null)
    {
      node.orbs = orbs;
      
    }
    nodes.Add(node);
    return node;
  }
  void DeleteAllNodes()
  {
    for(int i = 0; i < nodes.Count; i++)
    {
      var n = nodes[i];
      if (n.GetOrb<Player>() != null) continue;
      n.DeleteNode();
      nodes.RemoveAt(i);
      i--;
    }
  }
}
