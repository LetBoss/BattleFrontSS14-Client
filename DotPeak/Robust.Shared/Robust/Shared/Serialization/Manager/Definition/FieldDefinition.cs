// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Definition.FieldDefinition
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Definition;

internal sealed class FieldDefinition
{
  public FieldDefinition(
    DataFieldBaseAttribute attr,
    object? defaultValue,
    AbstractFieldInfo fieldInfo,
    AbstractFieldInfo backingField,
    InheritanceBehavior inheritanceBehavior)
  {
    this.BackingField = backingField;
    this.Attribute = attr;
    this.DefaultValue = defaultValue;
    this.FieldInfo = fieldInfo;
    this.InheritanceBehavior = inheritanceBehavior;
  }

  public DataFieldBaseAttribute Attribute { get; }

  public object? DefaultValue { get; }

  public InheritanceBehavior InheritanceBehavior { get; }

  public AbstractFieldInfo BackingField { get; }

  public AbstractFieldInfo FieldInfo { get; }

  public Type FieldType => this.FieldInfo.FieldType;

  public object? GetValue(object? obj) => this.BackingField.GetValue(obj);

  public void SetValue(object? obj, object? value) => this.BackingField.SetValue(obj, value);

  public override string ToString() => $"{this.FieldInfo.Name}({this.Attribute})";
}
