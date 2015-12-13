using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
  [HideInInspector]
  public Dictionary<Type, Item> items = new Dictionary<Type, Item>();
  Item movementItem;
  Item modelItem;

  public T GetItem<T> () where T: Item
  {
    return (T) items[typeof (T)];
  }

  void Awake()
  {
    gameObject.AddComponent<MeshFilter>();
    var mr = gameObject.AddComponent<MeshRenderer>();
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
