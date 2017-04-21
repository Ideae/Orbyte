using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(menuName = "Orbs/" + nameof(ForceMove))]
public class ForceMove : Orb<ForceMove>, IMovementOrb
{
	public float boost = 10;
	public float force = 5;
	public bool stopSuddenly;
	public float torque = 1;

	public void ProcessMovement()
	{
		var b = Input.GetKey(KeyCode.LeftShift) ? boost : 0f;
		var direction = Vector2.ClampMagnitude(Node.MovementDirection, 1);

		if (direction.sqrMagnitude.Approx(0) && stopSuddenly) Node.RB.velocity = Vector2.zero;
		else Node.RB.AddForce(direction * (force + b));

		var t = Clamp(DeltaAngle(Node.RB.rotation, Node.rotationGoal), -torque, torque);
		if (t.Approx(0) && stopSuddenly) Node.RB.angularVelocity = 0;
		else Node.RB.AddTorque(t);
	}

	public bool ReachedRotationTarget(float target) => (Node.RB.rotation - target).Approx(0);

	public bool ReachedPositionTarget(Vector2 target) => Node.RB.velocity.sqrMagnitude >
		(target - Node.Position).sqrMagnitude;
}