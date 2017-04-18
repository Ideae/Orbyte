using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(menuName = "Orbs/" + nameof(ForceMove))]
public class ForceMove : Orb<ForceMove>, IMovementOrb
{
  public float force = 5;
  public float boost = 10;
    public float torque = 1;
    public bool stopSuddenly = false;
    public void ProcessMovement()
    {
        
        float b = Input.GetKey(KeyCode.LeftShift) ? boost : 0f;
        var direction = Vector2.ClampMagnitude(Node.movementDirectionTarget, 1);

        if (direction.sqrMagnitude.Approx(0) && stopSuddenly) Node.rb.velocity = Vector2.zero; 
        else Node.rb.AddForce(direction* (force + b));

        var t = Clamp(DeltaAngle(Node.rb.rotation, Node.rotationTarget),-torque, torque);
        if (t.Approx(0) && stopSuddenly) Node.rb.angularVelocity = 0; 
        else Node.rb.AddTorque(t);
    }

    public bool ReachedRotationTarget(float target) => (Node.rb.rotation - target).Approx(0);
    
    public bool ReachedPositionTarget(Vector2 target) => Node.rb.velocity.sqrMagnitude > (target-Node.position).sqrMagnitude;
}
