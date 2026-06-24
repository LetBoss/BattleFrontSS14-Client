// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Attributes.DataFieldBaseAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Attributes;

public abstract class DataFieldBaseAttribute : Attribute
{
  public readonly int Priority;
  public readonly Type? CustomTypeSerializer;
  public readonly bool ReadOnly;
  public readonly bool ServerOnly;

  protected DataFieldBaseAttribute(
    bool readOnly = false,
    int priority = 1,
    bool serverOnly = false,
    Type? customTypeSerializer = null)
  {
    this.ReadOnly = readOnly;
    this.Priority = priority;
    this.ServerOnly = serverOnly;
    this.CustomTypeSerializer = customTypeSerializer;
  }
}
