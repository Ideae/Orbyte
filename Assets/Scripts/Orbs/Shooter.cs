using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Shooter))]
public class Shooter : Orb<Shooter>, IAimedActionOrb
{
  public float shootForce = 1000f;
  public List<Orb> orbs;

    public void OnAimedActionDown(Vector2 target)
    {
        Vector3 dir = new Vector3(target.x - Node.transform.position.x, target.y - Node.transform.position.y, 0f).normalized;
        
        Node n = Node.room.SpawnNode(Vector2.zero, orbs, cloneOrbs:true);
        n.transform.position = Node.transform.position + dir * (Node.core.Radius + n.core.Radius);
        n.rb.AddForce(dir * shootForce);
    }

    public void OnAimedActionHeld(Vector2 target)
    {
        
    }

    public void OnAimedActionUp(Vector2 target)
    {
    }
}
