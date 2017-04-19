using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Bomb))]
public class Bomb : Orb<Bomb>, IActionOrb
{
	public float multiplier = 100f;
	public float deadzone = 0.1f;
	public void OnActionDown()
	{
		foreach (Node n in Node.room.nodes)
		{
			Vector3 diff = n.transform.position - Node.transform.position;
			float dist = diff.sqrMagnitude;
			if (dist < deadzone) dist = deadzone;
			Vector3 force = diff.normalized / dist * multiplier;
			n.RB.AddForce(force);
		}
	}

	public void OnActionHeld()
	{
		throw new NotImplementedException();
	}

	public void OnActionUp()
	{
		throw new NotImplementedException();
	}
}