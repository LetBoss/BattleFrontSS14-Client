// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.CommandBind
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Input.Binding;

public sealed class CommandBind
{
  private readonly BoundKeyFunction _boundKeyFunction;
  private readonly IEnumerable<Type> _after;
  private readonly IEnumerable<Type> _before;
  private readonly InputCmdHandler _handler;

  public BoundKeyFunction BoundKeyFunction => this._boundKeyFunction;

  public IEnumerable<Type> After => this._after;

  public IEnumerable<Type> Before => this._before;

  public InputCmdHandler Handler => this._handler;

  public CommandBind(
    BoundKeyFunction boundKeyFunction,
    InputCmdHandler handler,
    IEnumerable<Type>? before = null,
    IEnumerable<Type>? after = null)
  {
    this._boundKeyFunction = boundKeyFunction;
    this._after = after ?? Enumerable.Empty<Type>();
    this._before = before ?? Enumerable.Empty<Type>();
    this._handler = handler;
  }
}
