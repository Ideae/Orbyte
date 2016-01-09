using UnityEngine;
using System.Collections;

public class Player : Entity
{

  private GameObject cameraTarget;
  public override void Awake()
  {
    base.Awake();
    cameraTarget = GameObject.Find("CameraTarget");
  }

  public override void Update()
  {
    base.Update();
    cameraTarget.transform.position = gameObject.transform.position;
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");
    //print(h + "  :  " + v);
    willDirection = new Vector3(h,v,0);
  }
}
