using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Types;


public abstract class Orb<T> :  Orb where T : Orb<T>
{
    static readonly List<Tuple<PropertyInfo, FieldInfo>> customProperties;
    static Orb()
    {
        customProperties = new List<Tuple<PropertyInfo, FieldInfo>>();
        var t = typeof(T);
        foreach (var p in t.GetProperties(BindingFlags.Instance))
        {
            CustomProperty cp = p.GetCustomAttribute<CustomProperty>();
            if (cp != null) customProperties.Add(new Tuple<PropertyInfo, FieldInfo>(p, t.GetField(cp.fieldName, BindingFlags.Instance)));
        }
        _interfaces = Orb.AllOrbTypes.Where(i => t.GetInterfaces().Contains(i)).ToArray();
    }

    static readonly Type[] _interfaces;

    public static T CreateOrb()
  {
    return CreateInstance<T>();
  }

    public override Type[] Interfaces => _interfaces;

    void Awake()
    {
        //sets all CustomProperties that need to have their setters called
        foreach (var tuple in customProperties)
        {
            tuple.Item1.SetValue(this, tuple.Item2.GetValue(this));
        }

        OnCreate();
    }

    string _Name;
    public override string Name => string.IsNullOrEmpty(_Name) ? (_Name = this.GetType().Name) : _Name;

    [SerializeField, HideInInspector] bool _isActive;
    [Show]
    public override bool IsActive
    {
        get { return _isActive; }
        set
        {
            if (_isActive == value) return;
            if (value)
            {

                _isActive = true;
                if(Node == null)return;
                this.OnActivate();
                Node.OnActivateOrb(this);
            }
            else
            {

                _isActive = false;
                if (Node == null) return;
                this.OnDeactivate();
                Node.OnDeactivateOrb(this);
            }
        }
    }

    [HideInInspector] Node _node;
    public override Node Node
    {
        get { return _node; }
        set
        {
            if (_node == value) return;
            if (IsActive)
            {
                this.OnDeactivate();
                _node?.OnDeactivateOrb(this);
            }
            if (_node) this.OnDetach();
            var n = _node;
            _node = value;
            n?.OnOrbRemoved(this);
            if (value == null) return;
            _node.OnOrbAdded(this);
            this.OnAttach();
            if (!IsActive) return;
            this.OnActivate();
            _node.OnActivateOrb(this);
        }
    }
    protected virtual void OnCreate() { }
    protected virtual void OnAttach() { }
    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }
    protected virtual void OnDetach() { }
    public override void OnDelete() { }

    public override Orb MakeCopy() => Instantiate(this);

    protected void OnDestroy() { /*Called on garbage collect*/ }
    protected void OnDisable() { /*Debug.LogWarning("Don't Call this");*/ }
    protected void OnEnable() { /*Debug.LogWarning("Don't Call this");*/ }
}


public abstract class Orb: BaseScriptableObject
{
    public abstract bool IsActive { get; set; }
    public abstract Type[] Interfaces { get; }
    public abstract Node Node { get; set; }
    public abstract string Name { get; }
    public abstract void OnDelete();
    public abstract Orb MakeCopy();

    public static readonly Type[] AllOrbTypes;
    public static List<Orb> DefaultOrbs;

    static Orb()
    {
        AllOrbTypes = typeof(IOrbType).Assembly.GetTypes()
            .Where(type => type.GetInterfaces().Contains(typeof(IOrbType)) && !type.IsGenericTypeDefinition)
            .ToArray();

    }

    public static void Initialize()
    {

        DefaultOrbs = Resources.LoadAll("Defaults").Cast<Orb>().ToList();
    }
}