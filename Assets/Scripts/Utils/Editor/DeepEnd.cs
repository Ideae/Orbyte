using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

static class DeepEnd
{
	static Dictionary<Type, SerializedProperty> cache = new Dictionary<Type, SerializedProperty>();
	public static UnityEngine.Object GetTypeWithField(Type parent)
	{

		AssemblyName assName = new AssemblyName("UnitySucks");
		AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("UnitySucksModule");
		TypeBuilder typeBuilder = moduleBuilder.DefineType("UnitySucks" + parent.Name
														   , TypeAttributes.Public |
		                                                   TypeAttributes.Class |
		                                                   TypeAttributes.AutoClass |
		                                                   TypeAttributes.AnsiClass |
		                                                   TypeAttributes.AutoLayout
		                                                   , typeof(UnityEngine.Object));
		//FieldBuilder fieldBuilder = typeBuilder.DefineField("___" + parent.Name, parent, FieldAttributes.Private);
		//var ctx = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
		//                                     MethodAttributes.RTSpecialName);
		//
		Type t = typeBuilder.CreateType();
		
		var typeWithField = Activator.CreateInstance(t, true);
		return (Object)typeWithField;


	}

	public static SerializedProperty GetSerializedProperty(Type type)
	{
		if (!cache.ContainsKey(type))
		{
			SerializedObject o = new SerializedObject(GetTypeWithField(type));
			cache[type] = o.FindProperty("___" + type.Name);
		}
		return cache[type];

	}

	static bool pendingAdd = false;
	
}

