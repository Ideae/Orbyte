using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public static class EditorUtils {
	[MenuItem("Generate/Code")]
	public static void GenerateCode()
	{
		CodeGeneration.CodeGenerator.GenerateCode(Orb.AllOrbTypes);
	}
	[MenuItem("Generate/OrbDefaults")]
	public static void GenerateOrbDefaults()
	{
		var defaultOrbs = Resources.LoadAll("DefaultOrbs");
		var types = Orb.AllOrbTypes;
		foreach (var t in types.Where(t => !t.IsAbstract))
		{
			if (defaultOrbs.Any(o => o.GetType() == t)) continue;
			var ass = ScriptableObject.CreateInstance(t);

			var nodeDefaultPath = $"Assets/Resources/DefaultOrbs/Default{t.Name}.asset";

			AssetDatabase.CreateAsset(ass, nodeDefaultPath);
		}
	}
}
