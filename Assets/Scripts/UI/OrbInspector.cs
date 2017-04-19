using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Vexe.Runtime.Extensions;

public class OrbInspector : MonoBehaviour
{
	public Text orbText;
	public GameObject propertiesPanel, inputPrefab, boolPrefab;

	Orb orb;
	// Use this for initialization
	void Start () {
		SetOrb(UIManager.Instance.Player.Node.core);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetOrb(Orb orb)
	{
		bool sameType = orb.GetType() == this.orb?.GetType();
		this.orb = orb;
		if (sameType) return;
		propertiesPanel.transform.DestroyChildren();
		foreach (FPInfo fp in this.orb.InspectableVariables)
		{
			Type t = fp.Type;
			GameObject elem = null;
			if (t == typeof(string) || t == typeof(float) || t == typeof(int))
			{
				elem = (GameObject)Instantiate(inputPrefab, propertiesPanel.transform);
				InputField inputField = elem.transform.GetComponentInChildren<InputField>();
				inputField.text = fp.GetValue(orb).ToString();
				inputField.onEndEdit.AddListener((s) =>
				{
					if (t == typeof(string))
					{
						fp.SetValue(orb, s);
					}
					else if (t == typeof(float))
					{
						float f = 0f;
						if (float.TryParse(s, out f))
						{
							fp.SetValue(orb, f);
						}
					}
					else if (t == typeof(int))
					{
						int i = 0;
						if (int.TryParse(s, out i))
						{
							fp.SetValue(orb, i);
						}
					}
				});
			}
			else if (t == typeof(bool))
			{
				elem = (GameObject)Instantiate(boolPrefab, propertiesPanel.transform);
				Toggle toggle = elem.transform.GetComponentInChildren<Toggle>();
				toggle.onValueChanged.AddListener((b) =>
				{
					fp.SetValue(orb, b);
				});

			}
			//Debug.Log("b");
			if (elem != null)
			{
				Text elemLabel = elem.transform.Find("LabelText").gameObject.GetComponent<Text>();
				elemLabel.text = fp.Name;
			}
			
		}


	}
}
