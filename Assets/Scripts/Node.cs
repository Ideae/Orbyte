using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Types;

public class Node : BaseBehaviour
{
	readonly Dictionary<Type, IList> _activeOrbsbyType = new Dictionary<Type, IList>();
	bool _isPositionTarget;
	Vector2 _movementTarget;

	public Core core;
	[PerItem] [Inline] [SerializeField] public List<Orb> orbs = new List<Orb>();
	public Room room;

	public float rotationTarget;
	public Rigidbody2D RB { get; private set; }
	
	public MeshRenderer MR { get; private set; }

	public IAimedActionOrb aimedActionOrb;
	public IActionOrb actionOrb;
	public IMovementOrb movementOrb;

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
		RB = GetComponent<Rigidbody2D>();
		MR = GetComponent<MeshRenderer>();

		var orbPrefabs = orbs;
		orbs = new List<Orb>();
		AddOrbs(orbPrefabs, clone:true);
			

		if (core == null)
		{

			core = Core.GetDefault();
			core.IsActive = true;
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
		movementOrb?.ProcessMovement();
	}

	public void DeleteNode()
	{
		DeleteAllOrbs();
		Destroy(gameObject);
	}

	public void AimedActionDown(Vector2 worldPos)
	{
		aimedActionOrb?.OnAimedActionDown(worldPos);
	}

	public void AimedActionHeld(Vector2 worldPos)
	{
		aimedActionOrb.OnAimedActionDown(worldPos);
	}

	public void AimedActionUp(Vector2 worldPos)
	{
		aimedActionOrb.OnAimedActionDown(worldPos); 
	}

	public void DeleteAllOrbs(bool skipCore = false)
	{
		foreach (var o in new List<Orb>(orbs))
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
		if (orb is IMovementOrb) movementOrb = (IMovementOrb)orb;
		if (orb is IActionOrb) actionOrb = (IActionOrb)orb;
		if (orb is IAimedActionOrb) aimedActionOrb = (IAimedActionOrb)orb;
		if (orb is Core) core = (Core)orb;
	}

	public void OnDeactivateOrb(Orb orb)
	{
		foreach (var orbType in orb.Interfaces)
			_activeOrbsbyType[orbType].Remove(orb);
		if (ReferenceEquals(orb, movementOrb)) movementOrb = null;
		if (ReferenceEquals(orb, actionOrb)) actionOrb = null;
		if (ReferenceEquals(orb, aimedActionOrb)) aimedActionOrb = null;
		
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


	[AttributeUsage(AttributeTargets.Method)]
	public class ApplyPropsAttribute : DrawnAttribute { }
	[ApplyPropsAttribute] public void ApplyProps()
	{
		//Called by vexe
	}
}