using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Move))]
public class Move : Orb
{
  public float speed = 5;
  public override void AffectSelf()
  {
    float x = Input.GetAxis("Horizontal");
    float y = Input.GetAxis("Vertical");
    node.rb.AddForce(new Vector2(x, y) * speed);
    //print(x + " : " + y);
    
  }
}
