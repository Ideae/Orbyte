using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class Orb<T> : Orb where T : Orb<T>
{
	static readonly List<CustomProperty> customProperties;
	static readonly List<FPInfo> inspectableVariables;
	static readonly Type[] _interfaces;


	string _Name;

	public override Type[] Interfaces => _interfaces;
	public override List<FPInfo> InspectableVariables => inspectableVariables;
	public override string Name => string.IsNullOrEmpty(_Name) ? (_Name = GetType().Name) : _Name;




	static Orb()
	{
		customProperties = new List<CustomProperty>();
		inspectableVariables = new List<FPInfo>();
		var t = FastType<T>.type;
		foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			var cp = p.GetCustomAttribute<CustomPropertyAttribute>();
			if (cp != null)
			{
				customProperties.Add(new CustomProperty(p, t.GetField(cp.fieldName, BindingFlags.Instance)));
				inspectableVariables.Add(new FPInfo(p));
			}
		}
		foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
		{
			if (f.GetCustomAttribute<HideInInspector>() != null) continue;
			inspectableVariables.Add(new FPInfo(f));
		}
		_interfaces = AllOrbTypes.Where(i => t.GetInterfaces().Contains(i)).ToArray();
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

	void Awake()
	{
		//sets all CustomProperties that need to have their setters called
		//foreach (var cp in customProperties)
		//	cp.property.SetValue(this, cp.field.GetValue(this));

		OnCreate();
	}

	public override Orb Clone() => Instantiate(this);

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



	[SerializeField] [HideInInspector] bool _isActive;
	[SerializedProperty(nameof(_isActive))]
	public virtual bool IsActive
	{
		get { return _isActive; }
		set
		{
			if (_isActive == value) return;
			_isActive = value;
			if((Node == null) || !Application.isPlaying) return;
			if (value)
			{
				OnActivate();
				Node.OnActivateOrb(this);
			}
			else
			{
				OnDeactivate();
				Node.OnDeactivateOrb(this);
			}
		}
	}

	[NonSerialized] public Node _node;

	[HideInInspector]
	public Node Node => _node;

	public void OnDelete() {}
	public abstract Type[] Interfaces { get; }
	public abstract List<FPInfo> InspectableVariables { get; }
	public abstract string Name { get; }
	public abstract Orb Clone();

	public virtual void OnCreate() {}
	public virtual void OnAttach() {}
	public virtual void OnActivate() {}
	public virtual void OnDeactivate() {}
	public virtual void OnDetach() {}

	public virtual void OnEquip()
	{
		IsActive = true;
	}
	public virtual void OnUnequip()
	{
		IsActive = false;
	}

	public virtual void OnClick()
	{
		if (this is IEquippable)
		{
			IEquippable ie = (IEquippable)this;
			ie.SetEquipped(true);
		}
	}

	public void OnStateChanged(OrbList.EventArgs args)
	{
		throw new NotImplementedException();
	}
}

