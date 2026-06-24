// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.PointerInputCmdHandler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Robust.Shared.Input.Binding;

public sealed class PointerInputCmdHandler : InputCmdHandler
{
  private PointerInputCmdDelegate2 _callback;
  private bool _ignoreUp;

  public override bool FireOutsidePrediction { get; }

  public PointerInputCmdHandler(
    PointerInputCmdDelegate callback,
    bool ignoreUp = true,
    bool outsidePrediction = false)
    : this((PointerInputCmdDelegate2) ((in PointerInputCmdHandler.PointerInputCmdArgs args) => callback(args.Session, args.Coordinates, args.EntityUid)), ignoreUp, outsidePrediction)
  {
  }

  public PointerInputCmdHandler(
    PointerInputCmdDelegate2 callback,
    bool ignoreUp = true,
    bool outsidePrediction = false)
  {
    this._callback = callback;
    this._ignoreUp = ignoreUp;
    this.FireOutsidePrediction = outsidePrediction;
  }

  public override bool HandleCmdMessage(
    IEntityManager entManager,
    ICommonSession? session,
    IFullInputCmdMessage message)
  {
    if (this._ignoreUp && message.State != BoundKeyState.Down)
      return false;
    switch (message)
    {
      case ClientFullInputCmdMessage fullInputCmdMessage1:
        PointerInputCmdDelegate2 callback1 = this._callback;
        bool? nullable1 = callback1 != null ? new bool?(callback1(new PointerInputCmdHandler.PointerInputCmdArgs(session, fullInputCmdMessage1.Coordinates, fullInputCmdMessage1.ScreenCoordinates, fullInputCmdMessage1.Uid, message.State, message))) : new bool?();
        return nullable1.HasValue && nullable1.Value;
      case FullInputCmdMessage fullInputCmdMessage2:
        PointerInputCmdDelegate2 callback2 = this._callback;
        bool? nullable2 = callback2 != null ? new bool?(callback2(new PointerInputCmdHandler.PointerInputCmdArgs(session, entManager.GetCoordinates(fullInputCmdMessage2.Coordinates), fullInputCmdMessage2.ScreenCoordinates, entManager.GetEntity(fullInputCmdMessage2.Uid), fullInputCmdMessage2.State, message))) : new bool?();
        return nullable2.HasValue && nullable2.Value;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  public readonly struct PointerInputCmdArgs(
    ICommonSession? session,
    EntityCoordinates coordinates,
    ScreenCoordinates screenCoordinates,
    EntityUid entityUid,
    BoundKeyState state,
    IFullInputCmdMessage originalMessage)
  {
    public readonly ICommonSession? Session = session;
    public readonly EntityCoordinates Coordinates = coordinates;
    public readonly ScreenCoordinates ScreenCoordinates = screenCoordinates;
    public readonly EntityUid EntityUid = entityUid;
    public readonly BoundKeyState State = state;
    public readonly IFullInputCmdMessage OriginalMessage = originalMessage;
  }
}
