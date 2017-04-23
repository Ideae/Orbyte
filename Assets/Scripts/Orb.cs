using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class Orb<T> : Orb where T : Orb<T>
{
	static readonly List<FPInfo> inspectableVariables;
	static readonly EquipSlot _equipSlots;


	
	public override EquipSlot EquipSlots => _equipSlots;
	public override List<FPInfo> InspectableVariables => inspectableVariables;
	public override string OrbName => string.IsNullOrEmpty(_orbName) ? (_orbName = GetType().Name) : _orbName;
	string _orbName;



	static Orb()
	{
		var customProperties = new List<CustomProperty>();
		inspectableVariables = new List<FPInfo>();
		var type = FastType<T>.type;
		foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			var cp = p.GetCustomAttribute<CustomPropertyAttribute>();
			if (cp != null)
			{
				customProperties.Add(new CustomProperty(p, type.GetField(cp.fieldName, BindingFlags.Instance)));
				inspectableVariables.Add(new FPInfo(p));
			}
		}
		foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
		{
			if (f.GetCustomAttribute<HideInInspector>() != null) continue;
			inspectableVariables.Add(new FPInfo(f));
		}
		var subtypes = AllOrbTypes.Where(i => i.IsAssignableFrom(type)).ToArray();
		_equipSlots = OrbList.EquipTypes.Select((t, i) => new {t, i})
		                     .Where(a => subtypes.Contains(a.t))
		                     .Select(a => OrbList.EquipSlots[a.i])
		                     .Aggregate((a,b)=>a|b);
	}

	public static T CreateOrb() => CreateInstance<T>();

	public static T GetDefault()
	{
		if (DefaultOrbs == null) DefaultOrbs = Resources.LoadAll("DefaultOrbs").Cast<Orb>().ToList();
		foreach (Orb o in DefaultOrbs)
		{
			if (o.GetType() == FastType<T>.type)
			{
				return (T)o.Clone();
			}
		}
		return null;
	}

}


public abstract class Orb : ScriptableObject, IOrbType
{
	[HideInInspector] public static readonly Type[] AllOrbTypes;
	[HideInInspector] public static List<Orb> DefaultOrbs;

	static Orb()
	{
		AllOrbTypes = FastType<IOrbType>.type.Assembly.GetTypes()
		                              .Where(type => type.GetInterfaces().Contains(FastType<IOrbType>.type) &&
			                                     !type.IsGenericTypeDefinition)
		                              .ToArray();
	}
	

	[NonSerialized] public Node _node;
	[HideInInspector] public Node Node => _node;

	int _cachedIndex;
	public int Index
	{
		get
		{
			if (Node == null) return -1;
			return Node.Orbs[_cachedIndex] == this ? _cachedIndex : (_cachedIndex = Node.Orbs.IndexOf(this));
		}
	}

	public virtual bool IsActive
	{
		get { return (Node != null) && Node.Orbs.IsActive(Index); }
		set { Node?.Orbs.SetActive(Index, value); }
	}


	public abstract List<FPInfo> InspectableVariables { get; }
	public abstract string OrbName { get; }
	public abstract EquipSlot EquipSlots { get; }

	public Orb Clone() => Instantiate(this);

	protected virtual void OnCreate() {}
	protected virtual void OnAttach() {}
	protected virtual void OnActivate() {}
	protected virtual void OnDeactivate() {}
	protected virtual void OnDetach() {}
	protected virtual void OnEquip(){}
	protected virtual void OnUnequip() {}
	protected virtual void OnDelete() { }

	public virtual void OnClick()
	{
		if (this is IEquippable)
		{
			var index = Node.Orbs.IndexOf(this);
			if(index == -1) return;
			var isEquipped = Node.Orbs.IsEquipped(index);
			Node.Orbs.SetEquipped(EquipSlots,index, isEquipped);
		}
	}

	void Awake()
	{
		//sets all CustomProperties that need to have their setters called
		//foreach (var cp in customProperties)
		//	cp.property.SetValue(this, cp.field.GetValue(this));

		OnCreate();
	}


	protected void OnDestroy()
	{
		/*Called on garbage collect*/
	}

	protected void OnDisable()
	{
		/*Debug.LogWarning("Don't Call this");*/
	}

	protected void OnEnable()
	{
		/*Debug.LogWarning("Don't Call this");*/
	}
	public void OnStateChanged(OrbList.EventArgs args)
	{
		switch (args.eventType)
		{
			case OrbList.Event.Removed:
				this.OnDetach();
				Debug.Assert(Node == null);
				break;
			case OrbList.Event.Added:
				this.OnAttach();
				Debug.Assert(Node != null);
				break;
			case OrbList.Event.Equipped:
				OnEquip();
				Debug.Assert(Node != null);
				Debug.Assert(Node.Orbs.IsEquipped(args.index));
				break;
			case OrbList.Event.UnEquipped:
				OnUnequip();
				Debug.Assert(Node != null);
				Debug.Assert(!Node.Orbs.IsEquipped(args.index));
				break;
			case OrbList.Event.Locked:
				Debug.Assert(Node != null);
				Debug.Assert(Node.Orbs.IsLocked(args.index));
				break;
			case OrbList.Event.Unlocked:
				Debug.Assert(Node != null);
				Debug.Assert(!Node.Orbs.IsLocked(args.index));
				break;
			case OrbList.Event.Activated:
				OnActivate();
				Debug.Assert(Node != null);
				Debug.Assert(Node.Orbs.IsActive(args.index));
				break;
			case OrbList.Event.Deactivated:
				OnDeactivate();
				Debug.Assert(Node != null);
				Debug.Assert(!Node.Orbs.IsActive(args.index));
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}

