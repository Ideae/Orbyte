﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Global

abstract class WriterObject : ScriptableObject
{

	public SerializedProperty prop { get; set; }
	PropertyInfo _value;
	public object objValue
	{
		get { return _value.GetValue(this); }
		set { _value.SetValue(this, value); }
	}

	public WriterObject()
	{
		_value = GetType().GetProperty(nameof(WriterObject<int>.value));
	}

	protected virtual void OnEnable()
	{

		SerializedObject o = new SerializedObject(this);
		this.prop = o.FindProperty(this.GetType().Name.Substring(1));
	}
	protected static readonly HashSet<Type> pendingAdd = new HashSet<Type>();
	public static readonly Dictionary<Type, WriterObject> objCache = new Dictionary<Type, WriterObject>();

	protected static readonly Dictionary<Type, Func<WriterObject>> funcCache =
		new Dictionary<Type, Func<WriterObject>>();


	static string PrettyTypeName(Type t)
	{
		if (t.IsGenericType)
		{
			return string.Format(
			                     "{0}<{1}>",
			                     t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.InvariantCulture)),
			                     string.Join(", ", t.GetGenericArguments().Select(PrettyTypeName)));
		}

		return t.Name;
	}

	static int c = 0;
	static string SafeTypeName(Type t)
	{
		if (t.IsGenericType)
		{

			c++;
			var r = string.Format(
				"{0}_of{1}{2}_",
				t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.InvariantCulture)),
				string.Join(", ", t.GetGenericArguments().Select(PrettyTypeName)),c);
			c--;
			return r;
		}
		if (t.IsArray)
		{
			var r = t.GetElementType().Name + $"_{new string('A',t.GetArrayRank())}_";
			return r;
		}
		return t.Name;
	}

	public static WriterObject Get(Type type)
	{
		{

			if (objCache.ContainsKey(type)) return objCache[type];

			var fieldname = "_" + SafeTypeName(type);
			var classname = "_" + fieldname;
			var typename = PrettyTypeName(type);
			var t = Type.GetType(classname);

			if (t != null)
			{
				t.BaseType.GetMethod("AddToCache", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
				//t.TypeInitializer?.Invoke(null);
				var obj = funcCache[type]();
				objCache[type] = obj;
				return objCache[type];
			}
			if (pendingAdd.Contains(type)) return null;
			var outputPath = Path.Combine(Application.dataPath, "Scripts", "Utils", "Editor", "WriterObject.cs");

			var lines = new[]
			{
				"class # : WriterObject<*, #> { public * %; public override * value { get { return %; } set { % = value; } } }"
					.Replace("*", typename).Replace("#", classname).Replace("%", fieldname),
				""
			};
			File.AppendAllLines(outputPath, lines);
			if (pendingAdd.Count == 1) EditorApplication.delayCall += AssetDatabase.Refresh;
			pendingAdd.Add(type);
			return null;
		}

	}
}

abstract class WriterObject<T>: WriterObject
{
	public abstract T value { get; set; } 
	
	public static WriterObject<T> Get() => (WriterObject< T>)Get(FastType<T>.type);
}
abstract class WriterObject<T, W> : WriterObject<T> where W:WriterObject<T,W>
{
	public static void AddToCache()
	{
		if (!funcCache.ContainsKey(FastType<T>.type)) funcCache[FastType<T>.type] = CreateInstance<W>;
	}

}

//------------------------------------------------------------------------------
// <auto-generated>
// The rest of this file was generated by the file itself.
// </auto-generated>
//------------------------------------------------------------------------------

class __Orb : WriterObject<Orb, __Orb> { public Orb _Orb; public override Orb value { get { return _Orb; } set { _Orb = value; } } }

class __Boolean : WriterObject<Boolean, __Boolean> { public Boolean _Boolean; public override Boolean value { get { return _Boolean; } set { _Boolean = value; } } }

class __Single : WriterObject<Single, __Single> { public Single _Single; public override Single value { get { return _Single; } set { _Single = value; } } }

class __Node : WriterObject<Node, __Node> { public Node _Node; public override Node value { get { return _Node; } set { _Node = value; } } }

class __String_A_ : WriterObject<String[], __String_A_> { public String[] _String_A_; public override String[] value { get { return _String_A_; } set { _String_A_ = value; } } }


class __IReadOnlyList_ofOrb1_ : WriterObject<IReadOnlyList<Orb>, __IReadOnlyList_ofOrb1_> { public IReadOnlyList<Orb> _IReadOnlyList_ofOrb1_; public override IReadOnlyList<Orb> value { get { return _IReadOnlyList_ofOrb1_; } set { _IReadOnlyList_ofOrb1_ = value; } } }

class __List_ofOrb1_ : WriterObject<List<Orb>, __List_ofOrb1_> { public List<Orb> _List_ofOrb1_; public override List<Orb> value { get { return _List_ofOrb1_; } set { _List_ofOrb1_ = value; } } }

class __Color : WriterObject<Color, __Color> { public Color _Color; public override Color value { get { return _Color; } set { _Color = value; } } }

class __Material : WriterObject<Material, __Material> { public Material _Material; public override Material value { get { return _Material; } set { _Material = value; } } }

class __OrbList : WriterObject<OrbList, __OrbList> { public OrbList _OrbList; public override OrbList value { get { return _OrbList; } set { _OrbList = value; } } }

class __LayerMask : WriterObject<LayerMask, __LayerMask> { public LayerMask _LayerMask; public override LayerMask value { get { return _LayerMask; } set { _LayerMask = value; } } }
