using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ScriptUtility {

	public static Stack<PropertyDrawer> DrawerStack
	{
		get { throw new NotImplementedException(); }
		set { throw new NotImplementedException(); }
	}

	static Dictionary<FPInfo, PropertyHandler> propertyHandlerCache = new Dictionary<FPInfo, PropertyHandler>();

	static PropertyHandler nullHandler = null;
	static PropertyHandler GetHandler(FPInfo property, UnityEngine.Object targetObject)
	{
		PropertyHandler handler;
		propertyHandlerCache.TryGetValue(property, out handler);
		if (handler != null) return handler;

		Type type = property.memberType;
		List<PropertyAttribute> list = null;
		FPInfo field = property;
		if (targetObject is MonoBehaviour || targetObject is ScriptableObject)
		{
			list = field.GetCustomAttributes<PropertyAttribute>().ToList();
		}
		handler = new PropertyHandler();
		if (list != null)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				handler.HandleAttribute(list[i], field, type);
			}
		}
		if (!handler.hasPropertyDrawer && type != null)
		{
			handler.HandleDrawnType(type, type, field, null);
		}
		if (handler.empty)
		{
			propertyHandlerCache[property] = nullHandler;
			handler = nullHandler;
		}
		else
		{
			propertyHandlerCache[property] = handler;
			//ScriptAttributeUtility.s_NextHandler = new PropertyHandler();
		}
		return handler;
	}

	public struct DrawerKeySet
	{
		public Type drawer;

		public Type type;
	}
	internal static Type[] GetTypesFromAssembly(Assembly assembly)
	{
		Type[] result;
		if (assembly == null)
		{
			result = new Type[0];
		}
		else
		{
			try
			{
				result = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException)
			{
				result = new Type[0];
			}
		}
		return result;
	}
	static IEnumerable<Type> SubclassesOf(Type parent)
	{
		return from klass in loadedTypes
		       where klass.IsSubclassOf(parent)
		       select klass;
	}
	internal static IEnumerable<Type> loadedTypes
	{
		get
		{
			var t = Type.GetType("UnityEditor.EditorAssemblies");
			var loadedAssemblies = (Assembly[])t.GetProperty("loadedAssemblies", BindingFlags.Static | BindingFlags.NonPublic)
			                                    .GetValue(null);
			return loadedAssemblies.SelectMany(GetTypesFromAssembly);
		}
	}
	private static void BuildDrawerTypeForTypeDictionary()
	{
		DrawerTypeForType = new Dictionary<Type, DrawerKeySet>();
		Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetTypesFromAssembly).ToArray();
		foreach (Type current in SubclassesOf(typeof(GUIDrawer)))
		{
			object[] customAttributes = current.GetCustomAttributes(typeof(CustomPropertyDrawer), true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				CustomPropertyDrawer editor = (CustomPropertyDrawer)customAttributes[i];
				var editType = editor.GetType().GetField("m_Type").GetValue(editor) as Type;
				var UseForChildren = (bool)editor.GetType().GetField("m_UseForChildren").GetValue(editor);
				DrawerTypeForType[editType] = new DrawerKeySet
				{
					drawer = current,
					type = editType
				};
				if (UseForChildren)
				{
					IEnumerable<Type> enumerable = from x in source
					                               where x.IsSubclassOf(editType)
					                               select x;
					foreach (Type current2 in enumerable)
					{
						if (!DrawerTypeForType.ContainsKey(current2) || !editType.IsAssignableFrom(DrawerTypeForType[current2].type))
						{
							DrawerTypeForType[current2] = new DrawerKeySet
							{
								drawer = current,
								type = editType
							};
						}
					}
				}
			}
		}
	}

	public static Dictionary<Type, DrawerKeySet> DrawerTypeForType;

	internal static Type GetDrawerTypeForType(Type type)
	{
		if (DrawerTypeForType == null)
		{
			BuildDrawerTypeForTypeDictionary();
		}
		DrawerKeySet drawerKeySet;
		DrawerTypeForType.TryGetValue(type, out drawerKeySet);
		Type drawer;
		if (drawerKeySet.drawer != null)
		{
			drawer = drawerKeySet.drawer;
		}
		else
		{
			if (type.IsGenericType)
			{
				DrawerTypeForType.TryGetValue(type.GetGenericTypeDefinition(), out drawerKeySet);
			}
			drawer = drawerKeySet.drawer;
		}
		return drawer;
	}

}