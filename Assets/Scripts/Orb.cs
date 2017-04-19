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

    [HideInInspector]
    public override Node Node => _node;

  public override Orb Clone() => Instantiate(this);

    protected void OnDestroy() { /*Called on garbage collect*/ }
    protected void OnDisable() { /*Debug.LogWarning("Don't Call this");*/ }
    protected void OnEnable() { /*Debug.LogWarning("Don't Call this");*/ }
}


public abstract class Orb: BaseScriptableObject, IOrbType
{

    public virtual void OnCreate() { }
    public virtual void OnAttach() { }
    public virtual void OnActivate() { }
    public virtual void OnDeactivate() { }
    public virtual void OnDetach() { }
    public void OnDelete() {  }


    public abstract bool IsActive { get; set; }
    public abstract Type[] Interfaces { get; }
    public abstract Node Node { get; }
    public abstract string Name { get; }
    public abstract Orb Clone();
    public Node _node;

    [HideInInspector] public static readonly Type[] AllOrbTypes;
    [HideInInspector] public static List<Orb> DefaultOrbs;

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


public interface IOrbType
{
  bool IsActive { get; set; }
  Type[] Interfaces { get; }
  Node Node { get; }
  string Name { get; }
  void OnDelete();
  Orb Clone();
}