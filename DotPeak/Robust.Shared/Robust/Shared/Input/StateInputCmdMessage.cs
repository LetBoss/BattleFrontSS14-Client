// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.StateInputCmdMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable disable
namespace Robust.Shared.Input;

[NetSerializable]
[Serializable]
public sealed class StateInputCmdMessage : InputCmdMessage
{
  public BoundKeyState State { get; }

  public StateInputCmdMessage(
    GameTick tick,
    ushort subTick,
    KeyFunctionId inputFunctionId,
    BoundKeyState state)
    : base(tick, subTick, inputFunctionId)
  {
    this.State = state;
  }
}
