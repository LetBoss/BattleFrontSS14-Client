// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesComponentPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesComponentPath : ViewVariablesPath
{
  public readonly IComponent Component;
  public readonly EntityUid Owner;

  public override Type Type
  {
    get
    {
      Type type = this.Component?.GetType();
      return (object) type != null ? type : typeof (void);
    }
  }

  public ViewVariablesComponentPath(IComponent component, EntityUid owner)
  {
    this.Component = component;
    this.Owner = owner;
  }

  public override object? Get() => (object) this.Component;

  public override void Set(object? value)
  {
  }

  public override object? Invoke(object?[]? parameters) => (object) null;
}
