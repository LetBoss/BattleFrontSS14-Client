// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SpecificPropertyInfo
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class SpecificPropertyInfo : AbstractFieldInfo
{
  public readonly PropertyInfo PropertyInfo;

  public override string Name { get; }

  internal override MemberInfo MemberInfo => (MemberInfo) this.PropertyInfo;

  public override Type FieldType => this.PropertyInfo.PropertyType;

  public override Type? DeclaringType => this.PropertyInfo.DeclaringType;

  public override Module Module => this.PropertyInfo.Module;

  public SpecificPropertyInfo(PropertyInfo propertyInfo)
  {
    this.Name = propertyInfo.Name;
    this.PropertyInfo = propertyInfo;
  }

  public override object? GetValue(object? obj) => this.PropertyInfo.GetValue(obj);

  public override void SetValue(object? obj, object? value)
  {
    this.PropertyInfo.SetValue(obj, value);
  }

  public override T? GetAttribute<T>(bool includeBacking = false)
  {
    SpecificFieldInfo field;
    return includeBacking && this.TryGetBackingField(out field) ? this.PropertyInfo.GetCustomAttribute<T>() ?? field.GetAttribute<T>(includeBacking) : this.PropertyInfo.GetCustomAttribute<T>(includeBacking);
  }

  public override IEnumerable<T> GetAttributes<T>(bool includeBacking = false)
  {
    SpecificPropertyInfo specificPropertyInfo = this;
    foreach (T customAttribute in specificPropertyInfo.PropertyInfo.GetCustomAttributes<T>())
      yield return customAttribute;
    SpecificFieldInfo field;
    if (includeBacking && specificPropertyInfo.TryGetBackingField(out field))
    {
      foreach (T attribute in field.GetAttributes<T>(includeBacking))
        yield return attribute;
    }
  }

  public override bool HasAttribute<T>(bool includeBacking = false)
  {
    SpecificFieldInfo field;
    if (!includeBacking || !this.TryGetBackingField(out field))
      return this.PropertyInfo.HasCustomAttribute<T>();
    return this.PropertyInfo.HasCustomAttribute<T>() || field.HasAttribute<T>(includeBacking);
  }

  public override bool TryGetAttribute<T>([NotNullWhen(true)] out T? attribute, bool includeBacking = false)
  {
    return this.PropertyInfo.TryGetCustomAttribute<T>(out attribute);
  }

  public override bool TryGetAttribute(Type type, [NotNullWhen(true)] out Attribute? attribute, bool includeBacking = false)
  {
    return this.PropertyInfo.TryGetCustomAttribute(type, out attribute);
  }

  public override bool IsBackingField() => false;

  public override bool HasBackingField()
  {
    Type declaringType = this.DeclaringType;
    return (object) declaringType != null && declaringType.HasBackingField(this.Name);
  }

  public override SpecificFieldInfo? GetBackingField()
  {
    Type declaringType = this.DeclaringType;
    return (object) declaringType == null ? (SpecificFieldInfo) null : declaringType.GetBackingField(this.Name);
  }

  public override bool TryGetBackingField([NotNullWhen(true)] out SpecificFieldInfo? field)
  {
    return (field = this.GetBackingField()) != null;
  }

  public bool IsMostOverridden(Type type)
  {
    if (this.DeclaringType == type)
      return true;
    MethodInfo baseDefinition1 = this.PropertyInfo.SetMethod?.GetBaseDefinition();
    MethodInfo baseDefinition2 = this.PropertyInfo.GetMethod?.GetBaseDefinition();
    Type type1 = type;
    List<PropertyInfo> list = type.GetAllProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == this.PropertyInfo.Name)).ToList<PropertyInfo>();
    for (; type1 != (Type) null; type1 = type1.BaseType)
    {
      foreach (PropertyInfo propertyInfo in list)
      {
        if (!(propertyInfo.DeclaringType != type1))
        {
          if (baseDefinition1 != (MethodInfo) null && baseDefinition1 == propertyInfo.SetMethod?.GetBaseDefinition())
            return propertyInfo == this.PropertyInfo;
          if (baseDefinition2 != (MethodInfo) null && baseDefinition2 == propertyInfo.GetMethod?.GetBaseDefinition())
            return propertyInfo == this.PropertyInfo;
        }
      }
    }
    return false;
  }

  public static implicit operator PropertyInfo(SpecificPropertyInfo f) => f.PropertyInfo;

  public static explicit operator SpecificPropertyInfo(PropertyInfo f)
  {
    return new SpecificPropertyInfo(f);
  }

  public override string? ToString() => this.PropertyInfo.ToString();
}
