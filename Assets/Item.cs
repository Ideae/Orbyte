using System;
using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour
{
  private static Color color;
  public bool visible;
  public bool editable;
  [HideInInspector]
  public Entity entity;
  public float affectRange;

  public virtual void Start()
  {
    entity = GetComponent<Entity>();
    if (entity)
    {
      entity.items[this.GetType()] = this;
    }
    else
    {
      Debug.LogError("Item was attached to gameobject without an Entity component.");
    }

    OnAttatch();
  }

  public virtual void OnAttatch() {}
  public virtual void OnDetatch() {}
  public virtual void AffectOther(Entity n) {}


}
