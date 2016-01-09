using UnityEngine;
using System.Collections;
[ItemInfo, CreateAssetMenu(menuName = "OrbyteItems/Sphere")]
public class Sphere : Item, IModelItem
{
  public Mesh sphere;
  public override void OnAttatch()
  {
    base.OnAttatch();
    gameObject.GetComponent<MeshFilter>().sharedMesh = sphere;
    var bc = gameObject.GetComponent<Collider>();
    if (bc) Destroy(bc);
    gameObject.AddComponent<SphereCollider>();
  }
}
