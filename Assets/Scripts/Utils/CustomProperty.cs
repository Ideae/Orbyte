using System;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class CustomPropertyAttribute : Attribute
{
	public readonly string fieldName;
	public CustomPropertyAttribute(string fieldName)
	{
		this.fieldName = fieldName;
		
	}
}

public struct CustomProperty
{
	public CustomProperty(PropertyInfo property, FieldInfo field)
	{
		this.property = property;
		this.field = field;
	}
	public PropertyInfo property;
	public FieldInfo field;
}