// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandArgumentAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Toolshed;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CommandArgumentAttribute : Attribute
{
  public CommandArgumentAttribute(Type? customParser = null, bool unparseable = false)
  {
    this.Unparseable = unparseable;
    if (customParser == (Type) null)
      return;
    this.CustomParser = customParser;
  }

  public bool Unparseable { get; }

  public Type? CustomParser { get; }
}
