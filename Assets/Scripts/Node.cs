using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vexe.Runtime.Types;

public class Node : BaseBehaviour
{
  public Room room;
  [PerItem, Inline] public List<Orb> orbs = new List<Orb>();
  public Rigidbody2D rb => GetComponent<Rigidbody2D>();
  public MeshRenderer mr => GetComponent<MeshRenderer>();
  
  public event Action<Node> OnOrbsChanged;
  public Orb AimedActionOrb;
  public Core core;
  
  public void Awake()
  {
    var orbPrefabs = orbs;
    orbs = new List<Orb>();
    foreach (Orb o in orbPrefabs)
    {
      Orb oo = Instantiate(o);
      AddOrb(oo);
      if (o.GetType() == typeof(Core)) core = (Core)oo;
    }
    if (core == null)
    {
      core = Orb.CreateOrb<Core>(this);
      orbs.Insert(0, core);
    }
  }
public T GetOrb<T>() where T : Orb
  {
    foreach(Orb o in orbs)
    {
      if (o.GetType() == typeof(T)) return (T)o;
    }
    return null;
  }
  public void AddOrb(Orb o, int? index = null)
  {
    o.node = this;
    o.OnAttach();
    if(index.HasValue && orbs.Count > index.Value) orbs.Insert(index.Value, o); 
    else orbs.Add(o);
    OnOrbsChanged?.Invoke(this);
  }

  public void RemoveOrb(Orb o)
  {
    o.OnDetach();
    orbs.Remove(o);
    o.node = null;
    OnOrbsChanged?.Invoke(this);
  }

  public void Update()
  {
    foreach(Orb o in orbs)
    {
      o.AffectSelf();
      o.Draw();
    }
  }
  public void AffectOther(Node other)
  {
    foreach(var o in orbs)
    {
      o.AffectOther(other);
    }
  }

  public void FixedAffectOther(Node other)
  {
    foreach (var o in orbs)
    {
      o.FixedAffectOther(other);
    }
  }

  public void DeleteNode()
  {
    foreach(var o in orbs)
    {
      o.OnDelete();
    }
    Destroy(gameObject);
  }
}
