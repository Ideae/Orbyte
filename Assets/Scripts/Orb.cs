using System;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Types;

public abstract class Orb : BaseScriptableObject
{
  private string _Name = null;
  public string Name => string.IsNullOrEmpty(_Name) ? (_Name = this.GetType().Name) : _Name;

  internal Node node;
  public virtual void Draw()
  {

  }
  public virtual void AffectSelf()
  {

  }
  public virtual void AffectOther(Node other)
  {

  }

  public virtual void FixedAffectOther(Node other)
  {

  }

  public virtual void AimedAction(Vector2 target)
  {

  }

  public void Print(object message)
  {

  }

  protected virtual void OnCreate()
  {
    
  }

  public virtual void OnAttach()
  {
    
  }

  public virtual void OnDetach()
  {

  }

  public virtual void OnDelete()
  {

  }

  private void Awake()
  {
    node = createOrbNode;
    createOrbNode = null;
    //sets all CustomProperties that need to have their setters called
    Type t = GetType();
    foreach(PropertyInfo pi in t.GetProperties())
    {
      var cp = pi.GetCustomAttribute<CustomProperty>();
      if (cp != null)
      {
        var fi = t.GetField(cp.fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        pi.SetValue(this, fi.GetValue(this));
      }
    }

    OnCreate();
  }
  private void OnDestroy() { /*Called on garbage collect*/ }
  private void OnDisable() { }
  private void OnEnable() { }

  private static Node createOrbNode;
  public static T CreateOrb<T>(Node n) where T : Orb
  {
    createOrbNode = n;
    return ScriptableObject.CreateInstance<T>();
  }
}
