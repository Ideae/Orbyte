using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Lifetime))]
public class Lifetime : Orb<Lifetime>
{
  public float MaxLifeTime = 100;
  public float CurrentLifetime = 100;

}
