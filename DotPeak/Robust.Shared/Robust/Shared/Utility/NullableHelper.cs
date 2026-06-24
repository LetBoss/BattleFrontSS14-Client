// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.NullableHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Utility;

public static class NullableHelper
{
  private const int NotAnnotatedNullableFlag = 1;
  private static readonly Type? BclNullableCache;
  private static readonly Type? BclNullableContextCache;
  private static readonly Dictionary<Assembly, (Type AttributeType, FieldInfo NullableFlagsField)?> _nullableAttributeTypeCache = new Dictionary<Assembly, (Type, FieldInfo)?>();
  private static readonly Dictionary<Assembly, (Type AttributeType, FieldInfo FlagsField)?> _nullableContextAttributeTypeCache = new Dictionary<Assembly, (Type, FieldInfo)?>();

  static NullableHelper()
  {
    NullableHelper.BclNullableCache = Type.GetType("System.Runtime.CompilerServices.NullableAttribute");
    NullableHelper.BclNullableContextCache = Type.GetType("System.Runtime.CompilerServices.NullableContextAttribute");
  }

  public static Type? GetUnderlyingType(this Type type)
  {
    Type underlyingType = Nullable.GetUnderlyingType(type);
    if (underlyingType != (Type) null)
      return underlyingType;
    return !type.IsValueType ? type : (Type) null;
  }

  public static Type EnsureNullableType(this Type type)
  {
    if (!type.IsValueType)
      return type;
    return typeof (Nullable<>).MakeGenericType(type);
  }

  public static Type EnsureNotNullableType(this Type type)
  {
    Type underlyingType = type.GetUnderlyingType();
    return (object) underlyingType != null ? underlyingType : type;
  }

  internal static bool IsMarkedAsNullable(AbstractFieldInfo field)
  {
    if (Nullable.GetUnderlyingType(field.FieldType) != (Type) null)
      return true;
    byte[] nullableFlags = NullableHelper.GetNullableFlags(field);
    if (nullableFlags.Length != 0 && nullableFlags[0] != (byte) 1)
      return true;
    return !(field.DeclaringType == (Type) null) && !field.FieldType.IsValueType && NullableHelper.GetNullableContextFlag(field.DeclaringType) != (byte) 1;
  }

  public static bool IsNullable(this Type type) => type.IsNullable(out Type _);

  public static bool IsNullable(this Type type, [NotNullWhen(true)] out Type? underlyingType)
  {
    underlyingType = type.GetUnderlyingType();
    return !(underlyingType == (Type) null);
  }

  private static byte[] GetNullableFlags(AbstractFieldInfo field)
  {
    lock (NullableHelper._nullableAttributeTypeCache)
    {
      Assembly assembly = field.Module.Assembly;
      if (!NullableHelper._nullableAttributeTypeCache.TryGetValue(assembly, out (Type, FieldInfo)? _))
        NullableHelper.CacheNullableFieldInfo(assembly);
      (Type AttributeType, FieldInfo NullableFlagsField)? nullable = NullableHelper._nullableAttributeTypeCache[assembly];
      if (!nullable.HasValue)
        return new byte[1];
      Attribute attribute;
      if (!field.TryGetAttribute(nullable.Value.AttributeType, out attribute))
        return new byte[1]{ (byte) 1 };
      if (!(nullable.Value.NullableFlagsField.GetValue((object) attribute) is byte[] nullableFlags))
        nullableFlags = new byte[1]{ (byte) 1 };
      return nullableFlags;
    }
  }

  private static byte GetNullableContextFlag(Type type)
  {
    lock (NullableHelper._nullableContextAttributeTypeCache)
    {
      Assembly assembly = type.Assembly;
      if (!NullableHelper._nullableContextAttributeTypeCache.TryGetValue(assembly, out (Type, FieldInfo)? _))
        NullableHelper.CacheNullableContextFieldInfo(assembly);
      (Type AttributeType, FieldInfo FlagsField)? nullable = NullableHelper._nullableContextAttributeTypeCache[assembly];
      if (!nullable.HasValue)
        return 0;
      Attribute customAttribute = type.GetCustomAttribute(nullable.Value.AttributeType);
      return customAttribute == null ? (byte) 1 : (byte) (nullable.Value.FlagsField.GetValue((object) customAttribute) ?? (object) 1);
    }
  }

  private static void CacheNullableFieldInfo(Assembly assembly)
  {
    Type type = assembly.GetType("System.Runtime.CompilerServices.NullableAttribute");
    if ((object) type == null)
      type = NullableHelper.BclNullableCache;
    if (type == (Type) null)
    {
      NullableHelper._nullableAttributeTypeCache.Add(assembly, new (Type, FieldInfo)?());
    }
    else
    {
      FieldInfo field = type.GetField("NullableFlags");
      if (field == (FieldInfo) null)
        NullableHelper._nullableAttributeTypeCache.Add(assembly, new (Type, FieldInfo)?());
      else
        NullableHelper._nullableAttributeTypeCache.Add(assembly, new (Type, FieldInfo)?((type, field)));
    }
  }

  private static void CacheNullableContextFieldInfo(Assembly assembly)
  {
    Type type = assembly.GetType("System.Runtime.CompilerServices.NullableContextAttribute");
    if ((object) type == null)
      type = NullableHelper.BclNullableContextCache;
    if (type == (Type) null)
    {
      NullableHelper._nullableContextAttributeTypeCache.Add(assembly, new (Type, FieldInfo)?());
    }
    else
    {
      FieldInfo field = type.GetField("Flag");
      if (field == (FieldInfo) null)
        NullableHelper._nullableContextAttributeTypeCache.Add(assembly, new (Type, FieldInfo)?());
      else
        NullableHelper._nullableContextAttributeTypeCache.Add(assembly, new (Type, FieldInfo)?((type, field)));
    }
  }
}
