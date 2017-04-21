using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public static partial class Utils
{
	public static void DestroyChildren(this Transform transform)
	{
		DestroyChildren(transform.gameObject);
	}

	public static void DestroyChildren(this GameObject gameObject)
	{
		foreach (var child in gameObject.transform)
		{
			(child as Transform)?.gameObject.Destroy();
		}
	}

	public static string GetParentName(this GameObject source)
	{
		var parent = source.transform.parent;
		return parent == null ? string.Empty : parent.name;
	}

	public static bool HasComponent(this Transform source, Type componentType) => HasComponent(source.gameObject, componentType);

	public static bool HasComponent(this GameObject source, Type componentType) => source.GetComponent(componentType) != null;

	public static bool HasComponent(this Transform source, string componentName) => HasComponent(source.gameObject, componentName);

	public static bool HasComponent(this GameObject source, string componentName) => source.GetComponent(componentName) != null;

	public static bool HasComponent<T>(this Transform source) where T : Component => HasComponent<T>(source.gameObject);

	public static bool HasComponent<T>(this GameObject source) where T : Component => source.GetComponent<T>() != null;

	/// <summary>
	/// Recursively returns children gameObjects excluding inactive ones
	/// </summary>
	public static IEnumerable<GameObject> GetChildren(this GameObject parent) => GetChildren(parent, false);

	/// <summary>
	/// Recursively returns children gameObjects
	/// </summary>
	public static IEnumerable<GameObject> GetChildren(this GameObject parent, bool includeInactive)
	{
		var transform = parent.transform;
		int count = transform.childCount;
		for (int i = 0; i < count; i++)
		{
			yield return transform.GetChild(i).gameObject;
		}
	}

	/// <summary>
	/// Deactivates (calls SetActive(false)) this gameObject
	/// </summary>
	public static void Deactivate(this GameObject go)
	{
		go.SetActive(false);
	}

	/// <summary>
	/// Activates (calls SetActive(true)) this gameObject
	/// </summary>
	public static void Activate(this GameObject go)
	{
		go.SetActive(true);
	}
	
	/// <summary>
	/// Destroys children objects under this gameObject meeting the specified predicate condition
	/// </summary>
	public static void ClearChildren(this GameObject go)
	{
		var all = go.GetComponentsInChildren<Transform>();
		for (int i = all.Length - 1; i > 0; i--)
		{
			var child = all[i].gameObject;
			child.Destroy(true);
		}
	}

	/// <summary>
	/// Destroys all components in this gameObject
	/// </summary>
	public static void ClearComponents(this GameObject go)
	{
		var all = GetAllComponents(go);
		for (int i = all.Length - 1; i > 0; i--) // Skip Transform
		{
			var c = all[i];
			c.Destroy();
		}
	}

	/// <summary>
	/// Returns an array of all the components attached to this gameObject
	/// </summary>
	public static Component[] GetAllComponents(this GameObject go) => go.GetComponents<Component>();
	

	/// <summary>
	/// Returns a lazy enumerable of the parent gameObjects above this gameObject
	/// Ex: say we had the following hierarchy:
	/// GO 1
	/// --- GO 1.1
	/// --- GO 1.2
	/// ----- GO 1.2.1
	/// Then the parents of GO 1.2.1 are GO 1.2 and GO 1
	/// </summary>
	public static IEnumerable<GameObject> EnumerateParentObjects(this GameObject go)
	{
		var currentParent = go.transform.parent;
		while (currentParent != null)
		{
			yield return currentParent.gameObject;
			currentParent = currentParent.parent;
		}
	}

	public static Component GetOrAddComponent(this GameObject go, Type componentType)
	{
		var result = go.GetComponent(componentType);
		return result == null ? go.AddComponent(componentType) : result;
	}

	public static T GetOrAddComponent<T>(this GameObject go) where T : Component => GetOrAddComponent(go, FastType<T>.type) as T;

	/// <summary>
	/// Gets the child gameObject whose name is specified by 'wanted'
	/// The search is non-recursive by default unless true is passed to 'recursive'
	/// </summary>
	public static GameObject GetChild(this GameObject inside, string wanted, bool recursive = false)
	{
		foreach (Transform child in inside.transform)
		{
			if (child.name == wanted)
				return child.gameObject;

			if (recursive)
			{
				var within = GetChild(child.gameObject, wanted, true);
				if (within != null)
					return within;
			}
		}
		return null;
	}

	/// <summary>
	/// Adds and returns a child gameObject to this gameObject with the specified name and HideFlags
	/// </summary>
	public static GameObject AddChild(this GameObject parent, string name, HideFlags flags = HideFlags.None)
	{

		var relative = new GameObject(name);
		relative.hideFlags = flags;
		relative.transform.parent = parent.transform;
		relative.transform.Reset();
		return relative;
	}
	
	/// <summary>
	/// Gets or adds the child gameObject whose name is 'name'
	/// Pass true to 'recursive' if you want the search to be recursive
	/// Specify HideFlags if you want to add the child using those flags
	/// </summary>
	public static GameObject GetOrAddChild(this GameObject parent, string name, bool recursive = false,
	                                       HideFlags flags = HideFlags.None)
	{
		var child = parent.GetChild(name, recursive);
		return child == null ? parent.AddChild(name, flags) : child;
	}
	
	/// <summary>
	/// Adds and returns a parent gameObject to this gameObject with the specified name and HideFlags
	/// </summary>
	public static GameObject AddParent(this GameObject child, string name, HideFlags flags = HideFlags.None)
	{
		var relative = new GameObject(name);
		relative.hideFlags = flags;
		child.transform.parent = relative.transform;
		child.transform.Reset();
		return relative;
	}

	/// <summary>
	/// Gets the parent whose name is wanted above this gameObject
	/// </summary>
	public static GameObject GetParent(this GameObject child, string wanted)
	{
		Transform currentParent = child.transform.parent;
		while (currentParent != null)
		{
			if (currentParent.name == wanted)
				return currentParent.gameObject;
			currentParent = currentParent.parent;
		}
		return null;
	}

	/// <summary>
	/// Gets or add the specified parent to this gameObject
	/// </summary>
	public static GameObject GetOrAddParent(this GameObject child, string name, HideFlags flags = HideFlags.None)
	{
		var parent = child.GetParent(name);
		return parent == null ? child.AddParent(name, flags) : parent;
	}
	
	public static void ToggleActive(this GameObject go)
	{
		go.SetActive(!go.activeSelf);
	}

	public static void SetActiveIfNot(this GameObject go, bool to)
	{
		if (go.activeSelf != to)
			go.SetActive(to);
	}
	public static Component GetOrAddComponent(this Component c, Type componentType) => c.gameObject.GetOrAddComponent(componentType);

	public static T GetOrAddComponent<T>(this Component c) where T : Component => c.gameObject.GetOrAddComponent<T>();

	public static T InstantiateNew<T>(this T source, Vector3 pos, Quaternion rot) where T : UnityObject => UnityObject.Instantiate(source, pos, rot) as T;

	public static T InstantiateNew<T>(this T source, Vector3 pos) where T : UnityObject => UnityObject.Instantiate(source, pos, Quaternion.identity) as T;

	public static T InstantiateNew<T>(this T source) where T : UnityObject => UnityObject.Instantiate(source, Vector3.zero, Quaternion.identity) as T;

	/// <summary>
	/// Calls Destroy on this object if we're in playmode, otherwise (edit-time) DestroyImmediate
	/// </summary>
	public static void Destroy(this UnityObject obj, bool allowDestroyingAssets = false)
	{
		if (obj == null) return;

		if (Application.isPlaying)
		{
			if (obj is GameObject)
			{
				GameObject gameObject = obj as GameObject;
				gameObject.transform.SetParent(null, false);
			}
			UnityObject.Destroy(obj);
		}
		else
		{
			UnityObject.DestroyImmediate(obj, allowDestroyingAssets);
		}
	}

	public static void Activate(this Transform t)
	{
		t.gameObject.Activate();
	}

	public static void Deactivate(this Transform t)
	{
		t.gameObject.Deactivate();
	}

	/// <summary>
	/// Sets localPosition to Vector3.zero, localRotation to Quaternion.identity, and localScale to Vector3.one
	/// </summary>
	public static void Reset(this Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}
}
