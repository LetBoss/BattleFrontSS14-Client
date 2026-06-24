// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.BoundKeyEventArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Map;
using System;
using System.Diagnostics;

#nullable disable
namespace Robust.Shared.Input;

[Virtual]
[DebuggerDisplay("{Function}, {State}, CF: {CanFocus}, H: {Handled}")]
public class BoundKeyEventArgs : EventArgs
{
  public readonly bool IsRepeat;

  public BoundKeyFunction Function { get; }

  public BoundKeyState State { get; }

  public ScreenCoordinates PointerLocation { get; }

  public bool CanFocus { get; internal set; }

  public bool Handled { get; private set; }

  public BoundKeyEventArgs(
    BoundKeyFunction function,
    BoundKeyState state,
    ScreenCoordinates pointerLocation,
    bool canFocus)
  {
    this.Function = function;
    this.State = state;
    this.PointerLocation = pointerLocation;
    this.CanFocus = canFocus;
  }

  public BoundKeyEventArgs(
    BoundKeyFunction function,
    BoundKeyState state,
    ScreenCoordinates pointerLocation,
    bool canFocus,
    bool isRepeat = false)
    : this(function, state, pointerLocation, canFocus)
  {
    this.IsRepeat = isRepeat;
  }

  public void Handle() => this.Handled = true;
}
