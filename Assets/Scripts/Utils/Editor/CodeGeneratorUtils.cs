using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CodeGeneration
{
	public partial class CodeGenerator
	{
		CodeGenerator(Type[] orbTypes)
		{
			orbTypeNames = orbTypes.Select(o => o.Name).ToArray();
		}

		static string[] tags => InternalEditorUtility.tags;
		static string[] layers => InternalEditorUtility.layers;
		string[] orbTypeNames;

		public static void GenerateCode(Type[] orbTypes)
		{
			var generator = new CodeGenerator(orbTypes);
			var classDefintion = generator.TransformText();
			var outputPath = Path.Combine(Application.dataPath, "Scripts", "Utils", "GeneratedCode.cs");
			try
			{
				if (File.Exists(outputPath + ".bak")) File.Delete(outputPath + ".bak");
				if (File.Exists(outputPath)) File.Copy(outputPath, outputPath + ".bak");
				// Save new class to assets folder.
				File.WriteAllText(outputPath, classDefintion);
				// Refresh assets.
				//AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Debug.Log("An error occurred while saving file: " + e);
			}
		}


	}
}