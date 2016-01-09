using UnityEngine;
using System.Collections;

public class Walk : Item, IMovementItem
{
  public Trait<bool> run = false;
  public Trait<float> walkSpeed = 0.3f;
  public Trait<float> runSpeed = 0.5f;
  public override void OnAttatch()
  {
    base.OnAttatch();
    if (entity.movementItem == null)
    {
      entity.movementItem = this;
    }
    if (entity.GetComponent<Rigidbody>() == null) entity.gameObject.AddComponent<Rigidbody>();
  }

  public void Move()
  {
    Vector3 v = Vector3.zero;
    if (entity.willType == WillType.Direction)
    {
      v = entity.willDirection;
    }
    else if(entity.willType == WillType.Target)
    {
      v = entity.willTarget - entity.transform.position;
    }
    float speed = run ? runSpeed : walkSpeed;
    //print(speed);
    gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + Vector3.ClampMagnitude(v, speed));
    

  }
}
