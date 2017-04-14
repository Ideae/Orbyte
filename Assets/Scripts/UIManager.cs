using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager> {



  public OrbList playerOrbList;
  public OrbList otherOrbList;
  public Node SelectedNode { get; private set; }
  public Player Player { get; private set; }

  void Start()
  {
    
  }

  public void SelectNode(Node n)
  {
    SelectedNode = n;
    otherOrbList.SetNode(n);
  }


  public void RegisterPlayer(Player player)
  {
    Player = player;
    playerOrbList.SetNode(player.node);
  }

  public void Update()
  {
    CheckMouse();
  }

  void CheckMouse()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if (EventSystem.current.IsPointerOverGameObject()) return;
      Plane p = new Plane(Vector3.forward, Vector3.zero);
      Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
      float enter = 0f;
      if (p.Raycast(r, out enter))
      {
        Vector2 v = r.GetPoint(enter);
        var coll = Physics2D.OverlapPoint(v);
        SelectNode(coll?.gameObject.GetComponent<Node>());
      }
    }
  }
}
