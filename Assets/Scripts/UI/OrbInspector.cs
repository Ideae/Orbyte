using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class OrbInspector : MonoBehaviour
{
	public Text orbText;
	public GameObject propertiesPanel, inputPrefab, boolPrefab;

	Orb orb;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetOrb(Orb orb)
	{
		this.orb = orb;

		FPInfo fp = orb.InspectableVariables[0];


	}
}
