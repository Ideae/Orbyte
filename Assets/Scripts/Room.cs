using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
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
}
