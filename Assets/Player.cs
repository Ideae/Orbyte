using UnityEngine;
using System.Collections;

public class Player : Entity {
  void Update()
  {
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");
    //print(h + "  :  " + v);
    willDirection = new Vector3(h,v,0);
  }
}
