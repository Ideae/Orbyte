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

  

  public event Action<Node> OnOrbsChanged;
  
  public void Start()
  {

    var orbPrefabs = orbs;
    orbs = new List<Orb>();
    foreach (Orb o in orbPrefabs)
    {
      AddOrb(Instantiate(o));
    }
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
  
}
