using System;
using UnityEngine;
using Vexe.Runtime.Types;

public abstract class Orb : BaseScriptableObject
{
  private string _Name = null;
  public string Name => string.IsNullOrEmpty(_Name) ? (_Name = this.GetType().Name) : _Name;

  internal Node node;

  public virtual void AffectSelf()
  {

  }
  public virtual void AffectOther(Node other)
  {

  }

  public virtual void FixedAffectOther(Node other)
  {

  }

  public void Print(object message)
  {
    Debug.Log(message);
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

  private void Awake() { OnCreate(); }
  private void OnDestroy() { /*Called on garbage collect*/ }
  private void OnDisable() { }
  private void OnEnable() { }

}
