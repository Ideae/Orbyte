using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIManager : Singleton<UIManager>
{
	public OrbListPanel otherOrbList;
	public OrbListPanel playerOrbList;
	public Text nodeCounterText;
	public Node SelectedNode { get; private set; }
	public Player Player { get; private set; }
	public Room CurrentRoom { get; private set; }


	void Awake()
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
		playerOrbList.SetNode(player.Node);
	}

	public void Update()
	{
		CheckMouse();
		nodeCounterText.text = CurrentRoom.nodes.Count.ToString();
	}

	void CheckMouse()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var worldPos = ScreenToWorldPoint();
			if (worldPos.HasValue)
				Player.Node.AimedActionDown(worldPos.Value);
		}
	}

	public Vector2? ScreenToWorldPoint()
	{
		//Todo: refactor checkMouse to use this method;
		if (EventSystem.current.IsPointerOverGameObject()) return null;
		var p = new Plane(CurrentRoom.transform.forward, CurrentRoom.transform.position);
		var r = Camera.main.ScreenPointToRay(Input.mousePosition);
		var enter = 0f;
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
				Destroy(args.DroppedObject.gameObject);
			else
				args.Cancel();
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
		CurrentRoom = room;
	}
}