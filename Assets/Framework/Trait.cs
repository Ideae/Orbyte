using System;
using UnityEngine;
using System.Collections;
[Serializable]
public class Trait<T> where T: new()
{
  public bool enabled;
  public bool visible;
  public static int Priority;
  public T value;

  public Trait()
  {
    value = new T();
  }

  public Trait(T val)
  {
    value = val;
  }

  public static implicit operator T(Trait<T> trait)
  {
    return trait.value;
  }
  public static implicit operator Trait<T>(T val)
  {
    return new Trait<T>(val);
  }

}
