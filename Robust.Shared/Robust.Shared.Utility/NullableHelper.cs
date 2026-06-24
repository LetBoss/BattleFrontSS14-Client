using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Robust.Shared.Utility;

public static class NullableHelper
{
	private const int NotAnnotatedNullableFlag = 1;

	private static readonly Type? BclNullableCache;

	private static readonly Type? BclNullableContextCache;

	private static readonly Dictionary<Assembly, (Type AttributeType, FieldInfo NullableFlagsField)?> _nullableAttributeTypeCache;

	private static readonly Dictionary<Assembly, (Type AttributeType, FieldInfo FlagsField)?> _nullableContextAttributeTypeCache;

	static NullableHelper()
	{
		_nullableAttributeTypeCache = new Dictionary<Assembly, (Type, FieldInfo)?>();
		_nullableContextAttributeTypeCache = new Dictionary<Assembly, (Type, FieldInfo)?>();
		BclNullableCache = Type.GetType("System.Runtime.CompilerServices.NullableAttribute");
		BclNullableContextCache = Type.GetType("System.Runtime.CompilerServices.NullableContextAttribute");
	}

	public static Type? GetUnderlyingType(this Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if (underlyingType != null)
		{
			return underlyingType;
		}
		if (!type.IsValueType)
		{
			return type;
		}
		return null;
	}

	public static Type EnsureNullableType(this Type type)
	{
		if (type.IsValueType)
		{
			return typeof(Nullable<>).MakeGenericType(type);
		}
		return type;
	}

	public static Type EnsureNotNullableType(this Type type)
	{
		return type.GetUnderlyingType() ?? type;
	}

	internal static bool IsMarkedAsNullable(AbstractFieldInfo field)
	{
		if (Nullable.GetUnderlyingType(field.FieldType) != null)
		{
			return true;
		}
		byte[] nullableFlags = GetNullableFlags(field);
		if (nullableFlags.Length != 0 && nullableFlags[0] != 1)
		{
			return true;
		}
		if (field.DeclaringType == null || field.FieldType.IsValueType)
		{
			return false;
		}
		return GetNullableContextFlag(field.DeclaringType) != 1;
	}

	public static bool IsNullable(this Type type)
	{
		Type underlyingType;
		return type.IsNullable(out underlyingType);
	}

	public static bool IsNullable(this Type type, [NotNullWhen(true)] out Type? underlyingType)
	{
		underlyingType = type.GetUnderlyingType();
		if (underlyingType == null)
		{
			return false;
		}
		return true;
	}

	private static byte[] GetNullableFlags(AbstractFieldInfo field)
	{
		lock (_nullableAttributeTypeCache)
		{
			Assembly assembly = field.Module.Assembly;
			if (!_nullableAttributeTypeCache.TryGetValue(assembly, out (Type, FieldInfo)? value))
			{
				CacheNullableFieldInfo(assembly);
			}
			value = _nullableAttributeTypeCache[assembly];
			if (!value.HasValue)
			{
				return new byte[1];
			}
			if (!field.TryGetAttribute(value.Value.Item1, out Attribute attribute))
			{
				return new byte[1] { 1 };
			}
			return (value.Value.Item2.GetValue(attribute) as byte[]) ?? new byte[1] { 1 };
		}
	}

	private static byte GetNullableContextFlag(Type type)
	{
		lock (_nullableContextAttributeTypeCache)
		{
			Assembly assembly = type.Assembly;
			if (!_nullableContextAttributeTypeCache.TryGetValue(assembly, out (Type, FieldInfo)? value))
			{
				CacheNullableContextFieldInfo(assembly);
			}
			value = _nullableContextAttributeTypeCache[assembly];
			if (!value.HasValue)
			{
				return 0;
			}
			Attribute customAttribute = type.GetCustomAttribute(value.Value.Item1);
			if (customAttribute == null)
			{
				return 1;
			}
			return (byte)(value.Value.Item2.GetValue(customAttribute) ?? ((object)1));
		}
	}

	private static void CacheNullableFieldInfo(Assembly assembly)
	{
		Type type = assembly.GetType("System.Runtime.CompilerServices.NullableAttribute");
		if ((object)type == null)
		{
			type = BclNullableCache;
		}
		if (type == null)
		{
			_nullableAttributeTypeCache.Add(assembly, null);
			return;
		}
		FieldInfo field = type.GetField("NullableFlags");
		if (field == null)
		{
			_nullableAttributeTypeCache.Add(assembly, null);
		}
		else
		{
			_nullableAttributeTypeCache.Add(assembly, (type, field));
		}
	}

	private static void CacheNullableContextFieldInfo(Assembly assembly)
	{
		Type type = assembly.GetType("System.Runtime.CompilerServices.NullableContextAttribute");
		if ((object)type == null)
		{
			type = BclNullableContextCache;
		}
		if (type == null)
		{
			_nullableContextAttributeTypeCache.Add(assembly, null);
			return;
		}
		FieldInfo field = type.GetField("Flag");
		if (field == null)
		{
			_nullableContextAttributeTypeCache.Add(assembly, null);
		}
		else
		{
			_nullableContextAttributeTypeCache.Add(assembly, (type, field));
		}
	}
}
