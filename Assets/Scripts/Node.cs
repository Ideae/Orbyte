using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

public class Node : BaseBehaviour
{
	readonly Dictionary<Type, IList> _activeOrbsbyType = new Dictionary<Type, IList>();
	IActionOrb _actionOrb;

	IAimedActionOrb _aimedActionOrb;
	bool _isPositionTarget;
	IMovementOrb _movementOrb;
	Vector2 _movementTarget;

	public Core core;
	[PerItem] [Inline] [SerializeField] public List<Orb> orbs = new List<Orb>();
	public Room room;

	public float rotationTarget;
	public Rigidbody2D RB => GetComponent<Rigidbody2D>();
	public MeshRenderer MR => GetComponent<MeshRenderer>();

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

	public IActionOrb ActionOrb
	{
		get { return _actionOrb; }
		set
		{
			if (_actionOrb != null) _actionOrb.IsActive = false;
			if (value != null) value.IsActive = true;
			_actionOrb = value;
		}
	}

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

	public Vector2 Position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	public Vector2 MovementDirectionTarget
	{
		get { return _isPositionTarget ? _movementTarget - Position : _movementTarget; }
		set
		{
			_isPositionTarget = false;
			_movementTarget = value;
		}
	}


	public Vector2? MovementPositionTarget
	{
		get { return _isPositionTarget ? (Vector2?)_movementTarget : null; }
		set
		{
			_isPositionTarget = value.HasValue;
			_movementTarget = value ?? Vector2.zero;
		}
	}

	public event Action<Node> OnOrbsChanged;

	public void Awake()
	{
		for (var i = 0; i < orbs.Count; i++)
		{
			var o = orbs[i];
			orbs.RemoveAt(i);
			AddOrb(o.Clone(), i);
		}

		if (core == null)
		{
			core = Core.CreateOrb();
			AddOrb(core);
		}
	}

	public T GetOrb<T>() where T : class
	{
		foreach (var orb in orbs)
			if (orb is T) return orb as T;
		return null;
	}

	List<T> GetActiveOrbs<T>() where T : IOrbType
	{
		if (_activeOrbsbyType.ContainsKey(typeof(T))) return (List<T>)_activeOrbsbyType[typeof(T)];
		var ret = new List<T>();
		_activeOrbsbyType[typeof(T)] = ret;
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
		foreach (var o in GetActiveOrbs<IDrawOrb>()) o.Draw();
		foreach (var o in GetActiveOrbs<IAffectOtherOrb>())
		{
			foreach (var other in room.nodes)
			{
				//This if condition might justify turning room.nodes into a hashset :(
				if (other == this) continue;
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
			if (skipCore && ReferenceEquals(o, core)) continue;
			RemoveOrb(o);
			o.OnDelete();
		}
		orbs.Clear();
	}


	public void OnActivateOrb(Orb orb)
	{
		foreach (var orbType in orb.Interfaces)
		{
			var list = _activeOrbsbyType.ContainsKey(orbType)
				? _activeOrbsbyType[orbType]
				: _activeOrbsbyType[orbType] = OrbLists.Create(orbType);


			list.Add(orb);
		}
		if (orb is IMovementOrb) MovementOrb = (IMovementOrb)orb;
		if (orb is IActionOrb) ActionOrb = (IActionOrb)orb;
		if (orb is IAimedActionOrb) AimedActionOrb = (IAimedActionOrb)orb;
	}

	public void OnDeactivateOrb(Orb orb)
	{
		foreach (var orbType in orb.Interfaces)
			_activeOrbsbyType[orbType].Remove(orb);
		if (ReferenceEquals(orb, MovementOrb)) MovementOrb = null;
		if (ReferenceEquals(orb, ActionOrb)) ActionOrb = null;
		if (ReferenceEquals(orb, AimedActionOrb)) AimedActionOrb = null;
	}

	public void AddOrbs(List<Orb> list, bool clone = false)
	{
		foreach (var o in list)
		{
			var oo = clone ? o.Clone() : o;
			AddOrb(oo);
		}
	}

	public void LookTowards(Vector2 v, bool immediately = false)
	{
		v.Normalize();
		rotationTarget = Mathf.Atan2(v.y, v.x);
		if (immediately) RB.rotation = rotationTarget;
	}

	public void LookAt(Vector2 v, bool immediately = false) => LookTowards(v - Position, immediately);
}