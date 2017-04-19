using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Types;

public class Node : BaseBehaviour
{
  public Room room;
    [PerItem, Inline, SerializeField] public List<Orb> orbs = new List<Orb>();
    public Rigidbody2D rb => GetComponent<Rigidbody2D>();
  public MeshRenderer mr => GetComponent<MeshRenderer>();
  
  public event Action<Node> OnOrbsChanged;

    IAimedActionOrb _aimedActionOrb;
    public IAimedActionOrb AimedActionOrb
    {
        get { return _aimedActionOrb; }
        set
        {
          
            if (_aimedActionOrb != null) _aimedActionOrb.IsActive = false;
            if (value != null) value.IsActive = true;
            _aimedActionOrb = value;
        }
    }
    IActionOrb _actionOrb;
    public IActionOrb ActionOrb
    {
        get { return _actionOrb; }
        set
        {
            if(_actionOrb != null) _actionOrb.IsActive = false;
            if (value != null) value.IsActive = true;
            _actionOrb = value;
        }
    }
    IMovementOrb _movementOrb;
    public IMovementOrb MovementOrb
    {
        get { return _movementOrb; }
        set
        {
            if (_movementOrb != null) _movementOrb.IsActive = false;
            if (value != null) value.IsActive = true;
            _movementOrb = value;
        }
    }
    
  public Core core;

  public void Awake()
  {
    for (var i = 0; i < orbs.Count; i++)
    {
      Orb o = orbs[i];
      orbs.RemoveAt(i);
      AddOrb(o.Clone(), i);
    }

    if (core == null)
      {
          core = Core.CreateOrb();
          this.AddOrb(core);
      }
  }
    public T GetOrb<T>() where T:class
    {
        foreach (var orb in orbs)
        {
            if (orb is T) return orb as T;
        }
        return null;
    }

    List<T> GetActiveOrbs<T>() where T : IOrbType
    {
        if (activeOrbsbyType.ContainsKey(typeof(T))) return (List<T>)activeOrbsbyType[typeof(T)];
        var ret = new List<T>();
        activeOrbsbyType[typeof(T)] = ret;
        return ret;
    }
    
  
    public void AddOrb(Orb orb, int? index = null)
  {

      orb.Node?.RemoveOrb(orb);
    if (index.HasValue) orbs.Insert(index.Value, orb);
    else orbs.Add(orb);

    orb._node = this;
      orb.OnAttach();

    if (orb.IsActive)
    {
      orb.OnActivate();
      OnActivateOrb(orb);
    }

    OnOrbsChanged?.Invoke(this);
  }



  public void RemoveOrb(Orb orb)
  {

    if (orb.IsActive)
    {
      orb.OnDeactivate();
      orb.Node.OnDeactivateOrb(orb);
    }
    orb.OnDetach();
    orb._node = null;


    orbs.Remove(orb);

    OnOrbsChanged?.Invoke(this);

  }

  public void Update()
  {
      foreach (var orb in GetActiveOrbs<IAffectSelfOrb>()) orb.AffectSelf();
      foreach(IDrawOrb o in GetActiveOrbs<IDrawOrb>()) o.Draw();
      foreach (var o in GetActiveOrbs<IAffectOtherOrb>())
      {
        
          foreach (var other in room.nodes)
          {
                //This if condition might justify turning room.nodes into a hashset :(
              if(other == this) continue;
              o.AffectOther(other);
          }
      }
    }

    public void FixedUpdate()
    {
        foreach (var orb in GetActiveOrbs<IFixedAffectSelf>()) orb.FixedAffectSelf();

        foreach (var o in GetActiveOrbs<IFixedAffectOther>())
        {
            foreach (var other in room.nodes)
            {
                //This if condition might justify turning room.nodes into a hashset :(
                if (other == this) continue;
                o.FixedAffectOther(other);
            }
        }
        MovementOrb?.ProcessMovement();
    }
    public void DeleteNode()
  {
        DeleteAllOrbs();
        Destroy(gameObject);
  }

    public void AimedActionDown(Vector2 worldPos)
    {
        AimedActionOrb?.OnAimedActionDown(worldPos);
    }
    public void AimedActionHeld(Vector2 worldPos)
    {
        AimedActionOrb.OnAimedActionDown(worldPos);
    }
    public void AimedActionUp(Vector2 worldPos)
    {
        AimedActionOrb.OnAimedActionDown(worldPos);
    }

    public void DeleteAllOrbs(bool skipCore = false)
    {
        foreach (var o in orbs)
        {
            if(skipCore && ReferenceEquals(o, core)) continue;
            RemoveOrb(o);
            o.OnDelete();
        }
        orbs.Clear();
    }

    public Vector2 position { get { return transform.position; } set { transform.position = value; } }
    Vector2 _movementTarget;
    bool _isPositionTarget;

    public float rotationTarget;
    readonly Dictionary<Type, IList> activeOrbsbyType = new Dictionary<Type, IList>();

    public Vector2 movementDirectionTarget
    {
        get { return _isPositionTarget ? _movementTarget - position : _movementTarget; }
        set
        {
            _isPositionTarget = false;
            _movementTarget = value;

        }
    }


    public Vector2? movementPositionTarget
    {
        get { return _isPositionTarget ? (Vector2?) _movementTarget : null; }
        set
        {
            _isPositionTarget = value.HasValue;
            _movementTarget = value??Vector2.zero;
        }
    }


    public void OnActivateOrb(Orb orb)
    {
        foreach (var orbType in orb.Interfaces)
        {
            var list = activeOrbsbyType.ContainsKey(orbType)
                ? activeOrbsbyType[orbType]
                : activeOrbsbyType[orbType] = OrbLists.Create(orbType);


            list.Add(orb);
        }
        if (orb is IMovementOrb) MovementOrb = (IMovementOrb)orb;
        if (orb is IActionOrb) ActionOrb = (IActionOrb)orb;
        if (orb is IAimedActionOrb) AimedActionOrb = (IAimedActionOrb)orb;
    }

    public void OnDeactivateOrb(Orb orb)
    {

        foreach (var orbType in orb.Interfaces)
        {
            activeOrbsbyType[orbType].Remove(orb);
        }
        if (ReferenceEquals(orb, MovementOrb)) MovementOrb = null;
        if (ReferenceEquals(orb, ActionOrb)) ActionOrb = null; 
        if (ReferenceEquals(orb, AimedActionOrb)) AimedActionOrb = null;
    }

    public void AddOrbs(List<Orb> list, bool clone = false)
    {
        
        foreach (var o in list)
        {
            var oo = clone ? o.Clone():o;
            AddOrb(oo);
        }
    }

    public void LookTowards(Vector2 v, bool immediately = false)
    {
        v.Normalize();
        rotationTarget = Mathf.Atan2(v.y, v.x);
        if (immediately) rb.rotation = rotationTarget;
    }

    public void LookAt(Vector2 v, bool immediately = false) => LookTowards(v - position, immediately);
}

