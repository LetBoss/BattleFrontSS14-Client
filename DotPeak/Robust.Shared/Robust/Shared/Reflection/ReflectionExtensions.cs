// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Reflection.ReflectionExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Reflection;

internal static class ReflectionExtensions
{
  internal static Type GetUnderlyingType(this MemberInfo member)
  {
    switch (member.MemberType)
    {
      case MemberTypes.Event:
        return ((EventInfo) member).EventHandlerType;
      case MemberTypes.Field:
        return ((FieldInfo) member).FieldType;
      case MemberTypes.Method:
        return ((MethodInfo) member).ReturnType;
      case MemberTypes.Property:
        return ((PropertyInfo) member).PropertyType;
      default:
        throw new ArgumentException("MemberInfo must be one of: EventInfo, FieldInfo, MethodInfo, PropertyInfo");
    }
  }

  internal static object? GetValue(this MemberInfo member, object instance)
  {
    FieldInfo fieldInfo = member as FieldInfo;
    if ((object) fieldInfo != null)
      return fieldInfo.GetValue(instance);
    return (member as PropertyInfo ?? throw new ArgumentOutOfRangeException(nameof (member))).GetValue(instance);
  }

  internal static void SetValue(this MemberInfo member, object instance, object? value)
  {
    FieldInfo fieldInfo = member as FieldInfo;
    if ((object) fieldInfo == null)
      (member as PropertyInfo)?.SetValue(instance, value);
    else
      fieldInfo.SetValue(instance, value);
  }

  internal static MemberInfo? GetSingleMember(this Type type, string member, Type? declaringType = null)
  {
    MemberInfo[] array = ((IEnumerable<MemberInfo>) type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Where<MemberInfo>((Func<MemberInfo, bool>) (m => m.Name == member)).ToArray<MemberInfo>();
    if (array.Length == 0)
      return (MemberInfo) null;
    if (declaringType != (Type) null)
      return ((IEnumerable<MemberInfo>) array).SingleOrDefault<MemberInfo>((Func<MemberInfo, bool>) (m => m.DeclaringType == declaringType));
    return array.Length <= 1 ? array[0] : ((IEnumerable<MemberInfo>) array).SingleOrDefault<MemberInfo>((Func<MemberInfo, bool>) (m => m.DeclaringType == type));
  }

  internal static PropertyInfo? GetIndexer(this Type type)
  {
    foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      if (property.GetIndexParameters().Length != 0)
        return property;
    }
    return (PropertyInfo) null;
  }
}
