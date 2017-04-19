using System;
using UnityEngine;

public class CustomProperty : Attribute
{
	public readonly string fieldName;

	public CustomProperty(string fieldName)
	{
		this.fieldName = fieldName;
	}
}