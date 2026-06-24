// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.PointerInputCmdMessage
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
public sealed class PointerInputCmdMessage : EventInputCmdMessage
{
  public NetCoordinates Coordinates { get; }

  public NetEntity Uid { get; }

  public PointerInputCmdMessage(
    GameTick tick,
    ushort subTick,
    KeyFunctionId inputFunctionId,
    NetCoordinates coordinates)
    : this(tick, subTick, inputFunctionId, coordinates, NetEntity.Invalid)
  {
  }

  public PointerInputCmdMessage(
    GameTick tick,
    ushort subTick,
    KeyFunctionId inputFunctionId,
    NetCoordinates coordinates,
    NetEntity uid)
    : base(tick, subTick, inputFunctionId)
  {
    this.Coordinates = coordinates;
    this.Uid = uid;
  }
}
