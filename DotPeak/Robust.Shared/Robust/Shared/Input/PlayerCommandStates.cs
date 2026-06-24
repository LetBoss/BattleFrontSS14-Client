// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.PlayerCommandStates
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Input;

public sealed class PlayerCommandStates : IPlayerCommandStates
{
  private readonly Dictionary<BoundKeyFunction, BoundKeyState> _functionStates = new Dictionary<BoundKeyFunction, BoundKeyState>();

  public BoundKeyState this[BoundKeyFunction function]
  {
    get => this.GetState(function);
    set => this.SetState(function, value);
  }

  public BoundKeyState GetState(BoundKeyFunction function)
  {
    BoundKeyState boundKeyState;
    return !this._functionStates.TryGetValue(function, out boundKeyState) ? BoundKeyState.Up : boundKeyState;
  }

  public void SetState(BoundKeyFunction function, BoundKeyState state)
  {
    this._functionStates[function] = state;
  }
}
