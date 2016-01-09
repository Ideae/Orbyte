using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Types;

public enum WillType { Direction, Target }
public enum AddType { Bar, Hidden }

[DefineCategory("Will", Pattern = "^will")]
[DefineCategory("Items",Pattern = ".*[Ii]tems?")]
public class Entity : BetterBehaviour
{
  
  
  public WillType willType;
  public Vector3 willDirection;
  public Vector3 willTarget;

  public IMovementItem movementItem;
  public IModelItem modelItem;
  public IInspectItem inpsectItem;
  public IToolItem[] toolItems = new IToolItem[3];
  [PerItem, Inline] public Item[] barItems = new Item[7];
  [PerItem, Inline] public List<Item> hiddenItems = new List<Item>();
  [Show]
  public void AddItem([ShowType(typeof(Item))]Type t)
  {
    var x = Item.CreateInstance(t);
    AddItem(x, false);
  }
  [Show]
  public void AddMovementItem([ShowType(typeof(IMovementItem))]Type t)
  {
    var x = Item.CreateInstance(t);
    AddItem(x, false);
  }
  [Show]
  public void AddModelItem([ShowType(typeof(IModelItem))]Type t)
  {
    var x = Item.CreateInstance(t);
    AddItem(x, false);
  }
  [Show]
  public void AddToolItem([ShowType(typeof(IToolItem))]Type t)
  {
    var x = Item.CreateInstance(t);
    AddItem(x, false);
  }

  public T AddItem<T>(bool typeSlot = true, bool overflow = true) where T : Item, new()
  {
    T item = Item.CreateItemInstance<T>();
    return AddItem(item);
  }
  public T AddItem<T>(T item, bool typeSlot = true, bool overflow = true) where T : Item
  {

    item.entity = this;
    if (typeSlot)
    {
      if (movementItem == null && item is IMovementItem)
      {
        movementItem = (IMovementItem) item;
        return item;
      }
      if (modelItem == null && item is IModelItem)
      {
        modelItem = (IModelItem)item; return item;
      }
      if (inpsectItem == null && item is IInspectItem)
      {
        inpsectItem = (IInspectItem)item; return item;
      }
      if (item is IToolItem)
      {
        for (int i = 0; i < toolItems.Length; i++)
        {
          if(toolItems[i] == null) toolItems[i] = (IToolItem)item; return item;
        }
      }
    }
    for (int i = 0; i < barItems.Length; i++)
    {
      if (barItems[i] == null) barItems[i] = item; return item;
    }

    hiddenItems.Add(item);
    return item;
  }
  

  //public IEnumerable<T> GetItems<T>() where T : Item
  //{
  //  return (IEnumerable<T>)items.Where(i => i is T);
  //}

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

  public virtual void Update()
  {
    ((Item) movementItem).Update();
    ((Item) modelItem).Update();
    ((Item) inpsectItem).Update();
    foreach (var toolItem in toolItems)
    {
      ((Item) toolItem).Update();
    }
    foreach (var barItem in barItems)
    {
      barItem.Update();
    }
    foreach (var hiddenItem in hiddenItems)
    {
      hiddenItem.Update();
    }
  }
}