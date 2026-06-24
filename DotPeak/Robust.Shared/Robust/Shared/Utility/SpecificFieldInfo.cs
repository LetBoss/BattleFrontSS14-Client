// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SpecificFieldInfo
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class SpecificFieldInfo : AbstractFieldInfo
{
  public readonly FieldInfo FieldInfo;

  public override string Name { get; }

  internal override MemberInfo MemberInfo => (MemberInfo) this.FieldInfo;

  public override Type FieldType => this.FieldInfo.FieldType;

  public override Type? DeclaringType => this.FieldInfo.DeclaringType;

  public override Module Module => this.FieldInfo.Module;

  public SpecificFieldInfo(FieldInfo fieldInfo)
  {
    this.Name = fieldInfo.Name;
    this.FieldInfo = fieldInfo;
  }

  public override object? GetValue(object? obj) => this.FieldInfo.GetValue(obj);

  public override void SetValue(object? obj, object? value) => this.FieldInfo.SetValue(obj, value);

  public override T? GetAttribute<T>(bool includeBacking = false)
  {
    return this.FieldInfo.GetCustomAttribute<T>();
  }

  public override IEnumerable<T> GetAttributes<T>(bool includeBacking = false)
  {
    return this.FieldInfo.GetCustomAttributes<T>();
  }

  public override bool HasAttribute<T>(bool includeBacking = false)
  {
    return this.FieldInfo.HasCustomAttribute<T>();
  }

  public override bool TryGetAttribute<T>([NotNullWhen(true)] out T? attribute, bool includeBacking = false)
  {
    return this.FieldInfo.TryGetCustomAttribute<T>(out attribute);
  }

  public override bool TryGetAttribute(Type type, [NotNullWhen(true)] out Attribute? attribute, bool includeBacking = false)
  {
    return this.FieldInfo.TryGetCustomAttribute(type, out attribute);
  }

  public override bool IsBackingField() => this.FieldInfo.IsBackingField();

  public override bool HasBackingField() => false;

  public override SpecificFieldInfo? GetBackingField() => (SpecificFieldInfo) null;

  public override bool TryGetBackingField([NotNullWhen(true)] out SpecificFieldInfo? field)
  {
    field = (SpecificFieldInfo) null;
    return false;
  }

  public static implicit operator FieldInfo(SpecificFieldInfo f) => f.FieldInfo;

  public static explicit operator SpecificFieldInfo(FieldInfo f) => new SpecificFieldInfo(f);

  public override string? ToString() => this.FieldInfo.ToString();
}
