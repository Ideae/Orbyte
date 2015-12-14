using UnityEngine;
using System.Collections;
[ItemInfo]
public class Cube : Item
{
  public Mesh cube;
  public override void OnAttatch()
  {
    base.OnAttatch();
    GetComponent<MeshFilter>().sharedMesh = cube;
    var bc = entity.GetComponent<Collider>();
    if (bc) Destroy(bc);
    gameObject.AddComponent<BoxCollider>();
  }
}