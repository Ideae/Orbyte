using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WillType { Direction, Target}

public class Entity : MonoBehaviour
{
  [HideInInspector]
  public Dictionary<Type, Item> items = new Dictionary<Type, Item>();
  public IMovementItem movementItem;
  IModelItem modelItem;
  public WillType willType;
  public Vector3 willDirection;
  public Vector3 willTarget;

  public T GetItem<T> () where T: Item
  {
    return (T) items[typeof (T)];
  }

  public virtual void FixedUpdate()
  {
    if(movementItem!= null) movementItem.Move();
  }

  public virtual void Awake()
  {
    if (!gameObject.GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();
    var mr = gameObject.GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
    mr.material = new Material(Shader.Find("Diffuse"));
  }
  public static Entity Create(List<Type> items = null)
  {
    var go = new GameObject();
    var res = go.AddComponent<Entity>();
    if (items != null)
    {
      foreach (var item in items)
      {
        go.AddComponent(item);
      }
    }
    return res;
  }
}