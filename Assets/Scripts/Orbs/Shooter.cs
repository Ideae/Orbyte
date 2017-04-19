using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Shooter))]
public class Shooter : Orb<Shooter>, IAimedActionOrb
{
	public List<Orb> orbs;
	public float shootForce = 1000f;

	public void OnAimedActionDown(Vector2 target)
	{
		var dir = new Vector3(target.x - Node.transform.position.x, target.y - Node.transform.position.y, 0f).normalized;

		var n = Node.room.SpawnNode(Vector2.zero, orbs, true);
		n.transform.position = Node.transform.position + dir * (Node.core.Radius + n.core.Radius);
		n.RB.AddForce(dir * shootForce);
	}

	public void OnAimedActionHeld(Vector2 target) {}

	public void OnAimedActionUp(Vector2 target) {}
}