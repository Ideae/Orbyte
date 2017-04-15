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
  public float boost = 10;
  public override void AffectSelf()
  {
    float x = Input.GetAxis("Horizontal");
    float y = Input.GetAxis("Vertical");
    float b = Input.GetKey(KeyCode.LeftShift) ? boost : 0f;
    node.rb.AddForce(new Vector2(x, y) * (speed+b));
    //print(x + " : " + y);
    
  }
}
