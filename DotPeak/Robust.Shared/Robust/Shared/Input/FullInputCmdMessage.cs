// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.FullInputCmdMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable disable
namespace Robust.Shared.Input;

[NetSerializable]
[Serializable]
public sealed class FullInputCmdMessage : InputCmdMessage, IFullInputCmdMessage
{
  public BoundKeyState State { get; }

  public NetCoordinates Coordinates { get; }

  public ScreenCoordinates ScreenCoordinates { get; }

  public NetEntity Uid { get; init; }

  public FullInputCmdMessage(
    GameTick tick,
    ushort subTick,
    int inputSequence,
    KeyFunctionId inputFunctionId,
    BoundKeyState state,
    NetCoordinates coordinates,
    ScreenCoordinates screenCoordinates)
    : this(tick, subTick, inputFunctionId, state, coordinates, screenCoordinates, NetEntity.Invalid)
  {
  }

  public FullInputCmdMessage(
    GameTick tick,
    ushort subTick,
    KeyFunctionId inputFunctionId,
    BoundKeyState state,
    NetCoordinates coordinates,
    ScreenCoordinates screenCoordinates,
    NetEntity uid)
    : base(tick, subTick, inputFunctionId)
  {
    this.State = state;
    this.Coordinates = coordinates;
    this.ScreenCoordinates = screenCoordinates;
    this.Uid = uid;
  }
}
