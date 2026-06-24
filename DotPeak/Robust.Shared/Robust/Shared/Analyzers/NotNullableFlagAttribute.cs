// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Analyzers.NotNullableFlagAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
public sealed class NotNullableFlagAttribute : Attribute
{
  public readonly string TypeParameterName;

  public NotNullableFlagAttribute(string typeParameterName)
  {
    this.TypeParameterName = typeParameterName;
  }
}
