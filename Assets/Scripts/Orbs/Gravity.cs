using UnityEngine;

[CreateAssetMenu(menuName = "Orbs/" + nameof(Gravity))]
public class Gravity : Orb<Gravity>, IFixedAffectOther, IDrawOrb
{
  public float multiplier = 1.0f;
  public float deadzone = 0.1f;

  public void FixedAffectOther(Node other)
  {
    Vector3 dir = Node.transform.position - other.transform.position;
    float distSquared = dir.sqrMagnitude;
    if (distSquared > deadzone)
    {
      float force = multiplier * other.rb.mass * Node.rb.mass / distSquared;
      Vector3 f = dir.normalized * force;
      other.rb.AddForce(f);
    }
  }
    public void Draw()
  {
    Node.mr.material.SetFloat("_Frequency", multiplier/5f);
  }

  public override void OnAttach()
  {
    Node.mr.material.SetFloat("_Gravity", 1);
  }
  public override void OnDetach()
  {
    Node.mr.material.SetFloat("_Gravity", 0);
  }

}