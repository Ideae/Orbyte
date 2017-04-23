using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
	[NonSerialized] public Room room;
	[SerializeField, HideInInspector] private List<Orb> orbs = new List<Orb>();
	public IReadOnlyList<Orb> Orbs => orbs;

	readonly Dictionary<Type, IList> _activeOrbsbyType = new Dictionary<Type, IList>();
	
	public Rigidbody2D RB { get; private set; }
	public MeshRenderer MR { get; private set; }

	[SerializeField, HideInInspector] int _aimedIndex, _actionIndex, _movementIndex, _coreIndex;
	public Core core => orbs.ElementAtOrDefault(_coreIndex) as Core;
	public IAimedActionOrb AimedActionOrb => orbs.ElementAtOrDefault(_aimedIndex) as IAimedActionOrb;
	public IActionOrb ActionOrb => orbs.ElementAtOrDefault(_actionIndex) as IActionOrb;
	public IMovementOrb MovementOrb => orbs.ElementAtOrDefault(_movementIndex) as IMovementOrb;

	[NonSerialized] public bool HasPositionTarget = false;
	//This could be either a target position or direction depending on HasPositionTarget
	Vector2 _movementGoal; 
	public Vector2 MovementDirection
	{
		get { return HasPositionTarget ? _movementGoal - Position : _movementGoal; }
		set
		{
			HasPositionTarget = false;
			_movementGoal = value;
		}
	}

	public Vector2? MovementPositionTarget
	{
		get { return HasPositionTarget ? (Vector2?)_movementGoal : null; }
		set
		{
			HasPositionTarget = value.HasValue;
			_movementGoal = value ?? Vector2.zero;
		}
	}

	public float rotationGoal { get; set; }

	public Vector2 Position
	{
		get { return transform.position; }
		set { transform.position = value; }
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
			var newCore = Core.GetDefault();
			newCore.IsActive = true;
			AddOrb(newCore);
			Equip(newCore);
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
		if (_activeOrbsbyType.ContainsKey(FastType<T>.type)) return (List<T>)_activeOrbsbyType[FastType<T>.type];
		var ret = new List<T>();
		_activeOrbsbyType[FastType<T>.type] = ret;
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
		if (orb is IEquippable) Equip((IEquippable)orb);
	}

	public void OnDeactivateOrb(Orb orb)
	{
		foreach (var orbType in orb.Interfaces)
			_activeOrbsbyType[orbType].Remove(orb);

		if (orb is IEquippable) UnEquip((IEquippable)orb);
		
	}

	public void AddOrbs(List<Orb> list, bool clone = false)
	{
		if (list == null) return;;
		foreach (var o in list)
		{
			var oo = clone ? o.Clone() : o;
			AddOrb(oo);
		}
	}

	public void LookTowards(Vector2 v, bool immediately = false)
	{
		v.Normalize();
		rotationGoal = Mathf.Atan2(v.y, v.x);
		if (immediately) RB.rotation = rotationGoal;
	}

	public void LookAt(Vector2 v, bool immediately = false) => LookTowards(v - Position, immediately);

	public void UnEquip(IEquippable equippableOrb)
	{
		if(equippableOrb == null) return;
		if (equippableOrb == AimedActionOrb) _aimedIndex = -1;
		else if(equippableOrb == ActionOrb) _actionIndex = -1;
		else if(equippableOrb == MovementOrb) _movementIndex = -1;
		else if(ReferenceEquals(equippableOrb, core)) _coreIndex = -1;
		else return;
		equippableOrb.OnUnequip();
	}

	public void Equip(IEquippable equippableOrb)
	{
		var orb = equippableOrb as Orb;
		int index = orbs.IndexOf(orb);
		if (orb is IAimedActionOrb && !ReferenceEquals(orb, AimedActionOrb))
		{
			UnEquip(AimedActionOrb);
			_aimedIndex = index;
			AimedActionOrb?.OnEquip();
		}
		if (orb is IActionOrb && !ReferenceEquals(orb, ActionOrb))
		{
			UnEquip(ActionOrb);
			_actionIndex = index;
			ActionOrb?.OnEquip();
		}
		if (orb is IMovementOrb && !ReferenceEquals(orb, AimedActionOrb))
		{
			UnEquip(AimedActionOrb);
			_movementIndex = index;
			AimedActionOrb?.OnEquip();
		}
		if (orb is Core && !ReferenceEquals(orb, core))
		{
			UnEquip(core);
			_coreIndex = index;
			core?.OnEquip();
		}
	}
}