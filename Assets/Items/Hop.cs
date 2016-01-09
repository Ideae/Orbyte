using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class Hop : Item, IMovementItem
{
  public Trait<bool> hop = true;
  public void Move()
  {
    throw new System.NotImplementedException();
  }
}
