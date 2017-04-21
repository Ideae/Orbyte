using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

public struct FPInfo
{
	public readonly MemberInfo memberInfo;
	public Type DeclaringType => memberInfo.DeclaringType;
	public Type ReflectedType => memberInfo.ReflectedType;

	public IEnumerable<CustomAttributeData> CustomAttributes => memberInfo.CustomAttributes;
	public Module Module => memberInfo.Module;
	public int MetadataToken => memberInfo.MetadataToken;

	public MemberTypes MemberType => memberInfo.MemberType;

	public string Name => memberInfo.Name;

	public Type memberType =>
		(memberInfo as PropertyInfo)?.PropertyType ??
		(memberInfo as FieldInfo)?.FieldType;

	public FPInfo(FieldInfo fieldInfo)
	{
		memberInfo = fieldInfo;
	}

	public FPInfo(PropertyInfo propertyInfo)
	{
		memberInfo = propertyInfo;
	}

	public override string ToString() => memberInfo.ToString();
	public override bool Equals(object obj) => obj is FPInfo && (((FPInfo)obj).memberInfo == memberInfo);
	public override int GetHashCode() => memberInfo.GetHashCode();
	public IList<CustomAttributeData> GetCustomAttributesData() => memberInfo.GetCustomAttributesData();

	public T GetCustomAttribute<T>() where T : Attribute => memberInfo.GetCustomAttribute<T>();
	public Attribute GetCustomAttribute(Type attrType) => memberInfo.GetCustomAttribute(attrType);
	public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute => memberInfo.GetCustomAttributes<T>();
	public IEnumerable<Attribute> GetCustomAttributes(Type attrType) => memberInfo.GetCustomAttributes(attrType);

	public object[] GetCustomAttributes(Type attributeType, bool inherit) => memberInfo.GetCustomAttributes(attributeType,
	                                                                                                        inherit);

	public object[] GetCustomAttributes(bool inherit) => memberInfo.GetCustomAttributes(inherit);

	public bool IsDefined(Type attributeType, bool inherit) => memberInfo.IsDefined(attributeType, inherit);
	[Pure]
	public object GetValue(object obj) =>
		(memberInfo as PropertyInfo)?.GetValue(obj) ??
		(memberInfo as FieldInfo)?.GetValue(obj);

	public object GetValue<T>(object obj) =>
		(T)((memberInfo as PropertyInfo)?.GetValue(obj) ??
			(memberInfo as FieldInfo)?.GetValue(obj));


	public void SetValue(object obj, object value)
	{
		(memberInfo as PropertyInfo)?.SetValue(obj, value);
		(memberInfo as FieldInfo)?.SetValue(obj, value);
	}

	public static bool operator ==(FPInfo left, FPInfo right) => left.memberInfo == right.memberInfo;
	public static bool operator !=(FPInfo left, FPInfo right) => left.memberInfo != right.memberInfo;
}