using System;
using System.Linq;
using System.Reflection;

namespace Robust.Shared.Reflection;

internal static class ReflectionExtensions
{
	internal static Type GetUnderlyingType(this MemberInfo member)
	{
		return member.MemberType switch
		{
			MemberTypes.Event => ((EventInfo)member).EventHandlerType, 
			MemberTypes.Field => ((FieldInfo)member).FieldType, 
			MemberTypes.Method => ((MethodInfo)member).ReturnType, 
			MemberTypes.Property => ((PropertyInfo)member).PropertyType, 
			_ => throw new ArgumentException("MemberInfo must be one of: EventInfo, FieldInfo, MethodInfo, PropertyInfo"), 
		};
	}

	internal static object? GetValue(this MemberInfo member, object instance)
	{
		if (!(member is FieldInfo fieldInfo))
		{
			if (member is PropertyInfo propertyInfo)
			{
				return propertyInfo.GetValue(instance);
			}
			throw new ArgumentOutOfRangeException("member");
		}
		return fieldInfo.GetValue(instance);
	}

	internal static void SetValue(this MemberInfo member, object instance, object? value)
	{
		if (!(member is FieldInfo fieldInfo))
		{
			if (member is PropertyInfo propertyInfo)
			{
				propertyInfo.SetValue(instance, value);
			}
		}
		else
		{
			fieldInfo.SetValue(instance, value);
		}
	}

	internal static MemberInfo? GetSingleMember(this Type type, string member, Type? declaringType = null)
	{
		MemberInfo[] array = (from m in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where m.Name == member
			select m).ToArray();
		if (array.Length == 0)
		{
			return null;
		}
		if (declaringType != null)
		{
			return array.SingleOrDefault((MemberInfo m) => m.DeclaringType == declaringType);
		}
		if (array.Length <= 1)
		{
			return array[0];
		}
		return array.SingleOrDefault((MemberInfo m) => m.DeclaringType == type);
	}

	internal static PropertyInfo? GetIndexer(this Type type)
	{
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.GetIndexParameters().Length != 0)
			{
				return propertyInfo;
			}
		}
		return null;
	}
}
