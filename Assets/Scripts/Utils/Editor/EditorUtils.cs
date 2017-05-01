using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public static class EditorUtils {
	[MenuItem("Generate/Code")]
	public static void GenerateCode()
	{
		CodeGenerator.GenerateCode(Orb.AllOrbTypes);
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

			var orbDefaultPath = $"Assets/Resources/DefaultOrbs/Default{t.Name}.asset";

			AssetDatabase.CreateAsset(ass, orbDefaultPath);
		}
	}
	// Attaches core so that edits to the core's props take effect at designtime
	[MenuItem("CONTEXT/Node/Attach Core")]
	static void AttachCorePrefab(MenuCommand command)
	{
		bool confirm = EditorUtility.DisplayDialog("Editing prefab",
			"Heads up, you're editing a shared core from many objects, Are you sure you want to do this?",
			"Yah Man");
		if(!confirm) return;
		
		Node n = (Node)command.context;
		var orb = n.Orbs.FirstOrDefault(o=> o is Core && o.IsActive);
		if (orb == null)
		{
			Debug.LogError("Eh? There's no active core on this node!");
			return;
		} 
		else orb._node = n;
	}

	[MenuItem("CONTEXT/Node/Attach Core Copy")]
	static void AttachCoreCopy(MenuCommand command)
	{
		Node n = (Node)command.context;

		Core orb = null;
		// Warning:directly modifying orbs list;
		var orbs = n.Orbs;
		for (int i = 0; i < orbs.Count; i++)
		{
			var o = n.Orbs[i];
			if (o is Core && o.IsActive)
			{
				orb = (Core)o.Clone();
				orbs[i] = orb;
				break;
			}
		}
		if (orb == null)
		{
			Debug.LogError("Eh? There's no active core on this node!");
			return;
		}
		

		var orbPath = $"Assets/Resources/Generated/{n.name}_{orb.GetType().Name}.asset";

		AssetDatabase.CreateAsset(orb, orbPath);
		orb._node = n;
	}

	[MenuItem("CONTEXT/Node/Create Material Instance")]
	static void CreateMaterialInstance(MenuCommand command)
	{
		Node n = (Node)command.context;
		var origMat = n.GetComponent<MeshRenderer>().sharedMaterial;
		var mat = Object.Instantiate(origMat);
		n.GetComponent<MeshRenderer>().sharedMaterial = mat;
		var materialPath = $"Assets/Materials/Generated/{n.name}_{origMat.name}.mat";
		AssetDatabase.CreateAsset(mat, materialPath);

	}



	[MenuItem("CONTEXT/Node/Serialize All Orbs")]
	static void SerializeAllOrbs(MenuCommand command)
	{
		Node n = (Node)command.context;
		var savedNodesPath = "Assets/SavedNodes";
		AssetDatabase.CreateFolder(savedNodesPath, n.name);
		var nodepath = savedNodesPath + "/"+n.name;
		AssetDatabase.CreateFolder(nodepath, "Orbs");
		var orbsPath = nodepath + "/Orbs";
		// Warning:directly modifying orbs list;
		var orbs = n.Orbs;
		for (int i = 0; i < orbs.Count; i++)
		{
			orbs[i] = n.Orbs[i].Clone();

			AssetDatabase.CreateAsset(n.Orbs[i], orbsPath +$"/{n.name}_{n.Orbs[i].GetType().Name}.asset");

		}
		PrefabUtility.CreatePrefab(nodepath+"/"+n.name + ".prefab", n.gameObject);


	}

}
