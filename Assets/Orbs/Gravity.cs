using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Gravity))]
public class Gravity : Orb
{
  public float multiplier = 1.0f;
  public float deadzone = 0.1f;
  public override void FixedAffectOther(Node other)
  {
    Vector3 dir = node.transform.position - other.transform.position;
    float distSquared = dir.sqrMagnitude;
    if (distSquared > deadzone)
    {
      float force = multiplier * other.rb.mass * node.rb.mass / distSquared;
      Vector3 f = dir.normalized * force;
      other.rb.AddForce(f);
    }

  }
}