using UnityEngine;
using System.Collections;
[ItemInfo, CreateAssetMenu(fileName="Cube", menuName = "OrbyteItems/Cube")]
public class Cube : Item, IModelItem
{
  public Mesh cube;
  public override void OnAttatch()
  {
    base.OnAttatch();
    gameObject.GetComponent<MeshFilter>().sharedMesh = cube;
    var bc = gameObject.GetComponent<Collider>();
    if (bc) Destroy(bc);
    gameObject.AddComponent<BoxCollider>();
  }
}