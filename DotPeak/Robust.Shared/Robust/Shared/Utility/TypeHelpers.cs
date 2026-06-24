// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.TypeHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Utility;

public static class TypeHelpers
{
  internal static readonly IComparer<Type> TypeInheritanceComparer = (IComparer<Type>) new TypeHelpers.TypeInheritanceComparerImpl();

  public static IEnumerable<FieldInfo> GetAllFields(this Type t)
  {
    foreach (Type type in t.GetClassHierarchy())
    {
      FieldInfo[] fieldInfoArray = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      for (int index = 0; index < fieldInfoArray.Length; ++index)
        yield return fieldInfoArray[index];
      fieldInfoArray = (FieldInfo[]) null;
    }
  }

  public static IEnumerable<PropertyInfo> GetAllProperties(this Type t)
  {
    return t.GetClassHierarchy().SelectMany<Type, PropertyInfo>((Func<Type, IEnumerable<PropertyInfo>>) (p => (IEnumerable<PropertyInfo>) p.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)));
  }

  public static bool IsBasePropertyDefinition(this PropertyInfo propertyInfo)
  {
    foreach (MethodInfo accessor in propertyInfo.GetAccessors())
    {
      if (accessor.IsVirtual && accessor.GetBaseDefinition() != accessor)
        return false;
    }
    return true;
  }

  public static IEnumerable<Type> GetClassHierarchy(this Type t)
  {
    yield return t;
    while (t.BaseType != (Type) null)
    {
      t = t.BaseType;
      yield return t;
    }
  }

  public static IEnumerable<Type> GetAllNestedTypes(this Type t)
  {
    foreach (Type type in t.GetClassHierarchy())
    {
      Type[] typeArray = type.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      for (int index = 0; index < typeArray.Length; ++index)
        yield return typeArray[index];
      typeArray = (Type[]) null;
    }
  }

  internal static IEnumerable<AbstractFieldInfo> GetAllPropertiesAndFields(this Type type)
  {
    foreach (FieldInfo allField in type.GetAllFields())
      yield return (AbstractFieldInfo) new SpecificFieldInfo(allField);
    foreach (PropertyInfo allProperty in type.GetAllProperties())
      yield return (AbstractFieldInfo) new SpecificPropertyInfo(allProperty);
  }

  public static bool TrySelectCommonType(Type type1, Type type2, [NotNullWhen(true)] out Type? commonType)
  {
    commonType = (Type) null;
    if (type1.IsAssignableFrom(type2))
      commonType = type1;
    else if (type2.IsAssignableFrom(type1))
      commonType = type2;
    return commonType != (Type) null;
  }

  internal static SpecificFieldInfo? GetBackingField(this Type type, string propertyName)
  {
    foreach (Type type1 in type.GetClassHierarchy())
    {
      FieldInfo field = type1.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
      if (field != (FieldInfo) null)
        return new SpecificFieldInfo(field);
    }
    return (SpecificFieldInfo) null;
  }

  public static bool HasBackingField(this Type type, string propertyName)
  {
    return type.GetBackingField(propertyName) != null;
  }

  internal static bool TryGetBackingField(
    this Type type,
    string propertyName,
    [NotNullWhen(true)] out SpecificFieldInfo? field)
  {
    return (field = type.GetBackingField(propertyName)) != null;
  }

  public static bool IsBackingField(this MemberInfo memberInfo)
  {
    return memberInfo.HasCustomAttribute<CompilerGeneratedAttribute>() && memberInfo.Name.StartsWith("<") && memberInfo.Name.EndsWith(">k__BackingField");
  }

  public static bool HasCustomAttribute<T>(this MemberInfo memberInfo) where T : Attribute
  {
    return (object) memberInfo.GetCustomAttribute<T>() != null;
  }

  public static bool HasCustomAttribute<T>(this ParameterInfo memberInfo) where T : Attribute
  {
    return (object) memberInfo.GetCustomAttribute<T>() != null;
  }

  public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, [NotNullWhen(true)] out T? attribute) where T : Attribute
  {
    return (object) (attribute = memberInfo.GetCustomAttribute<T>()) != null;
  }

  public static bool TryGetCustomAttribute(
    this MemberInfo memberInfo,
    Type type,
    [NotNullWhen(true)] out Attribute? attribute)
  {
    return (attribute = memberInfo.GetCustomAttribute(type)) != null;
  }

  internal static bool HasParameterlessConstructor(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
  {
    return ((IEnumerable<ConstructorInfo>) type.GetConstructors(flags)).Any<ConstructorInfo>((Func<ConstructorInfo, bool>) (m => m.GetParameters().Length == 0));
  }

  internal static bool IsAutogeneratedRecordMember(this AbstractFieldInfo info)
  {
    return !(info.DeclaringType == (Type) null) && info.Name == "EqualityContract" && info.FieldType == typeof (Type);
  }

  public sealed class TypeInheritanceComparerImpl : IComparer<Type>
  {
    public int Compare(Type? x, Type? y)
    {
      if (x == (Type) null || y == (Type) null || x == y)
        return 0;
      if (x.IsAssignableFrom(y))
        return -1;
      return y.IsAssignableFrom(x) ? 1 : 0;
    }
  }
}
