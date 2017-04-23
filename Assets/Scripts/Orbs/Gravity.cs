using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Gravity))]
public class Gravity : Orb<Gravity>, IFixedAffectOther, IDrawOrb
{
	public float deadzone = 0.1f;
	public float multiplier = 1.0f;

	public void Draw()
	{
		Node.MR.material.SetFloat("_Frequency", multiplier / 5f);
	}

	public void FixedAffectOther(Node other)
	{
		var dir = Node.transform.position - other.transform.position;
		var distSquared = dir.sqrMagnitude;
		if (distSquared > deadzone)
		{
			var force = multiplier * other.RB.mass * Node.RB.mass / distSquared;
			var f = dir.normalized * force;
			other.RB.AddForce(f);
		}
	}

	protected override void OnAttach()
	{
		Node.MR.material.SetFloat("_Gravity", 1);
	}

	protected override void OnDetach()
	{
		Node.MR.material.SetFloat("_Gravity", 0);
	}
}