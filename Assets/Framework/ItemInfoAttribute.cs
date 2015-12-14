using System;
using UnityEngine;
using System.Collections;

//[Flags]
//public enum ItemType { Generic, Model, Movement }

[AttributeUsage(AttributeTargets.Class)]
public class ItemInfoAttribute : Attribute
{
  //public ItemType type;
  //
  //public ItemInfoAttribute(ItemType type)
  //{
  //  this.type = type;
  //}
}
