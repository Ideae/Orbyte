using UnityEngine;
using System.Collections;

public class Sphere : Item {
  public Mesh sphere;
  public override void OnAttatch()
  {
    base.OnAttatch();
    GetComponent<MeshFilter>().sharedMesh = sphere;
    var bc = entity.GetComponent<Collider>();
    if (bc) Destroy(bc);
    gameObject.AddComponent<SphereCollider>();
  }
}
