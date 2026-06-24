// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.ViewVariables;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ViewVariablesAttribute : Attribute
{
  public readonly VVAccess Access;

  public ViewVariablesAttribute()
  {
  }

  public ViewVariablesAttribute(VVAccess access) => this.Access = access;
}
