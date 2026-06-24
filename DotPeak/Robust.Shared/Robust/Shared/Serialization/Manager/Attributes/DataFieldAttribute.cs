// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Attributes.DataFieldAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[MeansImplicitAssignment]
[Virtual]
public class DataFieldAttribute : DataFieldBaseAttribute
{
  public readonly bool Required;

  public string? Tag { get; internal set; }

  public DataFieldAttribute(
    string? tag = null,
    bool readOnly = false,
    int priority = 1,
    bool required = false,
    bool serverOnly = false,
    Type? customTypeSerializer = null)
    : base(readOnly, priority, serverOnly, customTypeSerializer)
  {
    this.Tag = tag;
    this.Required = required;
  }

  public override string? ToString() => this.Tag;
}
