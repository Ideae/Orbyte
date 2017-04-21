using System;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public class SerializedPropertyAttribute : CustomPropertyAttribute {
	public SerializedPropertyAttribute(string fieldName) : base(fieldName) {}
}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public class CustomPropertyAttribute : PropertyAttribute
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