using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Shooter))]
public class Shooter : Orb
{
  public float shootForce = 1000f;
  public List<Orb> orbs;
  public override void AimedAction(Vector2 mousePos)
  {
    Vector3 dir = new Vector3(mousePos.x - node.transform.position.x, mousePos.y - node.transform.position.y, 0f).normalized;

    Node n = node.room.SpawnNode(Vector2.zero, orbs);
    n.transform.position = node.transform.position + dir * (node.core.Radius + n.core.Radius);

    n.rb.AddForce(dir * shootForce);
  }
  public override void OnAttach()
  {
    node.AimedActionOrb = this;
  }
  public override void OnDetach()
  {
    node.AimedActionOrb = null;
  }
}
