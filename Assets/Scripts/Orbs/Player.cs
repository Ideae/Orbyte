using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Player))]
public class Player : Orb<Player>, IAimedActionOrb, IAffectSelfOrb
{

	public override bool IsActive
	{
		get { return true; }
		set { }
	}
	
	public void AffectSelf()
	{
		if (Input.GetMouseButton(1))
		{
			var v = UIManager.Instance.ScreenToWorldPoint();
			Node.MovementPositionTarget = v ?? Vector2.zero;
		}
		else if (!Node.MovementPositionTarget.HasValue)
		{
			var x = Input.GetAxis("Horizontal");
			var y = Input.GetAxis("Vertical");
			Node.MovementDirection = new Vector2(x, y);
		}
		if (Node.MovementDirection.magnitude > .3f) Node.LookTowards(Node.MovementDirection);
	}
	
	public void OnAimedActionDown(Vector2 target)
	{
		var coll = Physics2D.OverlapPoint(target);
		UIManager.Instance.SelectNode(coll?.gameObject.GetComponent<Node>());
	}

	public void OnAimedActionHeld(Vector2 target) {}

	public void OnAimedActionUp(Vector2 target) {}

	public override void OnAttach()
	{
		UIManager.Instance.RegisterPlayer(this);
	}
}