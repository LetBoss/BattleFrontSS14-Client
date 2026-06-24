// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.PrototypeAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Robust.Shared.Prototypes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansDataDefinition]
[Virtual]
public class PrototypeAttribute : Attribute
{
  public readonly int LoadPriority;

  public string? Type { get; internal set; }

  public PrototypeAttribute(string? type = null, int loadPriority = 1)
  {
    this.Type = type;
    this.LoadPriority = loadPriority;
  }

  public PrototypeAttribute(int loadPriority)
  {
    this.Type = (string) null;
    this.LoadPriority = loadPriority;
  }
}
