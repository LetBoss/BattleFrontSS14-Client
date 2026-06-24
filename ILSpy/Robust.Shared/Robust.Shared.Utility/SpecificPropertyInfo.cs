using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Robust.Shared.Utility;

internal sealed class SpecificPropertyInfo : AbstractFieldInfo
{
	public readonly PropertyInfo PropertyInfo;

	public override string Name { get; }

	internal override MemberInfo MemberInfo => PropertyInfo;

	public override Type FieldType => PropertyInfo.PropertyType;

	public override Type? DeclaringType => PropertyInfo.DeclaringType;

	public override Module Module => PropertyInfo.Module;

	public SpecificPropertyInfo(PropertyInfo propertyInfo)
	{
		Name = propertyInfo.Name;
		PropertyInfo = propertyInfo;
	}

	public override object? GetValue(object? obj)
	{
		return PropertyInfo.GetValue(obj);
	}

	public override void SetValue(object? obj, object? value)
	{
		PropertyInfo.SetValue(obj, value);
	}

	public override T? GetAttribute<T>(bool includeBacking = false)
	{
		if (includeBacking && TryGetBackingField(out SpecificFieldInfo field))
		{
			return PropertyInfo.GetCustomAttribute<T>() ?? field.GetAttribute<T>(includeBacking);
		}
		return PropertyInfo.GetCustomAttribute<T>(includeBacking);
	}

	public override IEnumerable<T> GetAttributes<T>(bool includeBacking = false)
	{
		foreach (T customAttribute in PropertyInfo.GetCustomAttributes<T>())
		{
			yield return customAttribute;
		}
		if (!includeBacking || !TryGetBackingField(out SpecificFieldInfo field))
		{
			yield break;
		}
		foreach (T attribute in field.GetAttributes<T>(includeBacking))
		{
			yield return attribute;
		}
	}

	public override bool HasAttribute<T>(bool includeBacking = false)
	{
		if (includeBacking && TryGetBackingField(out SpecificFieldInfo field))
		{
			if (!PropertyInfo.HasCustomAttribute<T>())
			{
				return field.HasAttribute<T>(includeBacking);
			}
			return true;
		}
		return PropertyInfo.HasCustomAttribute<T>();
	}

	public override bool TryGetAttribute<T>([NotNullWhen(true)] out T? attribute, bool includeBacking = false)
	{
		return PropertyInfo.TryGetCustomAttribute<T>(out attribute);
	}

	public override bool TryGetAttribute(Type type, [NotNullWhen(true)] out Attribute? attribute, bool includeBacking = false)
	{
		return PropertyInfo.TryGetCustomAttribute(type, out attribute);
	}

	public override bool IsBackingField()
	{
		return false;
	}

	public override bool HasBackingField()
	{
		return DeclaringType?.HasBackingField(Name) ?? false;
	}

	public override SpecificFieldInfo? GetBackingField()
	{
		return DeclaringType?.GetBackingField(Name);
	}

	public override bool TryGetBackingField([NotNullWhen(true)] out SpecificFieldInfo? field)
	{
		return (field = GetBackingField()) != null;
	}

	public bool IsMostOverridden(Type type)
	{
		if (DeclaringType == type)
		{
			return true;
		}
		MethodInfo methodInfo = PropertyInfo.SetMethod?.GetBaseDefinition();
		MethodInfo methodInfo2 = PropertyInfo.GetMethod?.GetBaseDefinition();
		Type type2 = type;
		List<PropertyInfo> list = (from p in type.GetAllProperties()
			where p.Name == PropertyInfo.Name
			select p).ToList();
		while (type2 != null)
		{
			foreach (PropertyInfo item in list)
			{
				if (!(item.DeclaringType != type2))
				{
					if (methodInfo != null && methodInfo == item.SetMethod?.GetBaseDefinition())
					{
						return item == PropertyInfo;
					}
					if (methodInfo2 != null && methodInfo2 == item.GetMethod?.GetBaseDefinition())
					{
						return item == PropertyInfo;
					}
				}
			}
			type2 = type2.BaseType;
		}
		return false;
	}

	public static implicit operator PropertyInfo(SpecificPropertyInfo f)
	{
		return f.PropertyInfo;
	}

	public static explicit operator SpecificPropertyInfo(PropertyInfo f)
	{
		return new SpecificPropertyInfo(f);
	}

	public override string? ToString()
	{
		return PropertyInfo.ToString();
	}
}
