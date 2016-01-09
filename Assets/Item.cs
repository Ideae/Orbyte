using System;
using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public abstract class Item : BaseScriptableObject
{
  private static Color color;
  public bool visible;
  public bool editable;
  [Inline]public Entity entity;
  public float affectRange;
  public GameObject gameObject { get { if (entity != null) return entity.gameObject; return null; } }
  public Transform transform { get { if (entity != null) return entity.gameObject.transform; return null; } }
  public T GetComponent<T>() where T: Component { if (entity != null) return entity.GetComponent<T>(); return null; }

  protected Item()
  {
  }
  protected Item(Entity parent) : this()
  {
    entity = parent;
  }
  public static T CreateItemInstance<T>() where T : Item
  {
    return (T)CreateInstance(typeof (T));
  }
  public new static Item CreateInstance(Type type)
  {
    var i = (Item) ScriptableObject.CreateInstance(type);
    i.name = type.Name;
    return i;
  }

  public virtual void Start() { }
  public virtual void Update() { }
  public virtual void OnEnable() { }
  public virtual void OnDisable() { }
  public virtual void FixedUpdate() { }
  public virtual void Awake() { }

  public virtual void OnAttatch() { }
  public virtual void OnDetatch() { }
  public virtual void AffectOther(Entity n) { }


}
