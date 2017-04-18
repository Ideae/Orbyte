using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class UIManager : Singleton<UIManager> {
  
  public OrbListPanel playerOrbList;
  public OrbListPanel otherOrbList;
  public Node SelectedNode { get; private set; }
  public Player Player { get; private set; }
  public Room CurrentRoom { get; private set; }

  
  void Awake()
  {

      Orb.Initialize();
  }

  public void SelectNode(Node n)
  {
    SelectedNode = n;
    otherOrbList.SetNode(n);
  }


  public void RegisterPlayer(Player player)
  {
    Player = player;
    playerOrbList.SetNode(player.Node);
  }

  public void Update()
  {
    CheckMouse();
  }

  void CheckMouse()
  {
    if (Input.GetMouseButtonDown(0))
    {

        var worldPos = ScreenToWorldPoint();
        if (worldPos.HasValue)
        {
            Player.Node.AimedActionDown(worldPos.Value);
        }
    }
    }

    public Vector2? ScreenToWorldPoint()
    {
        //Todo: refactor checkMouse to use this method;
        if (EventSystem.current.IsPointerOverGameObject()) return null;
        Plane p = new Plane(CurrentRoom.transform.forward, CurrentRoom.transform.position);
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        if (p.Raycast(r, out enter))
        {
            Vector2 v = r.GetPoint(enter);
            return v;
        }
        return null;
    }

    public void OnOrbDropped(ReorderableList.ReorderableListEventStruct args)
    {
        var targetList = args.ToList?.GetComponentInParent<OrbListPanel>();
        if (targetList == null)
        {
            var point = ScreenToWorldPoint();
            if (point.HasValue)
            {
                //Todo: Spawn Item node
                Destroy(args.DroppedObject.gameObject);
            }
            else
            {
                args.Cancel();
            }
        }
        else
        {
            //don't add at the beggining of list(before orbs)
            //if (args.ToIndex == 0) args.Cancel();
        }

    }

    public void OnOrbGrabbed(ReorderableList.ReorderableListEventStruct args)
    {
        //Noop?
    }
    public void OnOrbRemoved(ReorderableList.ReorderableListEventStruct args)
    {

        var removedOrb = args.SourceObject.GetComponent<OrbButton>().orb;
        removedOrb.Node.RemoveOrb(removedOrb);
    }
    public void OnOrbAdded(ReorderableList.ReorderableListEventStruct args)
    {
        var removedOrb = args.SourceObject.GetComponent<OrbButton>().orb;
        var targetList = args.ToList?.GetComponentInParent<OrbListPanel>();
        if (targetList != null) targetList.node.AddOrb(removedOrb, args.ToIndex);
    }

    public void RegisterRoom(Room room)
    {
        this.CurrentRoom = room;
    }
}
