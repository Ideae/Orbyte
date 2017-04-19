using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Player))]
public class Player : Orb<Player>, IAimedActionOrb, IAffectSelfOrb
{
	bool _isInspecting;

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
			Node.MovementDirectionTarget = new Vector2(x, y);
		}
		if (Node.MovementDirectionTarget.magnitude > .3f) Node.LookTowards(Node.MovementDirectionTarget);
	}

	bool IAimedActionOrb.IsActive
	{
		get { return base.IsActive && _isInspecting; }
		set { _isInspecting = value; }
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