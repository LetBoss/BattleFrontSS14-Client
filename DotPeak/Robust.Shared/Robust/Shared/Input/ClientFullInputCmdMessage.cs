// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.ClientFullInputCmdMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;

#nullable disable
namespace Robust.Shared.Input;

public sealed class ClientFullInputCmdMessage : InputCmdMessage, IFullInputCmdMessage
{
  public BoundKeyState State { get; init; }

  public EntityCoordinates Coordinates { get; init; }

  public ScreenCoordinates ScreenCoordinates { get; init; }

  public EntityUid Uid { get; init; }

  public ClientFullInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId)
    : base(tick, subTick, inputFunctionId)
  {
  }

  public ClientFullInputCmdMessage(
    GameTick tick,
    ushort subTick,
    KeyFunctionId inputFunctionId,
    EntityCoordinates coordinates,
    ScreenCoordinates screenCoordinates,
    BoundKeyState state,
    EntityUid uid)
    : base(tick, subTick, inputFunctionId)
  {
    this.Coordinates = coordinates;
    this.ScreenCoordinates = screenCoordinates;
    this.State = state;
    this.Uid = uid;
  }
}
