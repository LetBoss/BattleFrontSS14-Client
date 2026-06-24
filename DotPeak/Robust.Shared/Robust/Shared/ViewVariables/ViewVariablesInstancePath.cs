// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesInstancePath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesInstancePath : ViewVariablesPath
{
  private readonly object? _object;

  public ViewVariablesInstancePath(object? obj) => this._object = obj;

  public override Type Type
  {
    get
    {
      Type type = this._object?.GetType();
      return (object) type != null ? type : typeof (void);
    }
  }

  public override object? Get() => this._object;

  public override void Set(object? value)
  {
  }

  public override object? Invoke(object?[]? parameters) => (object) null;
}
