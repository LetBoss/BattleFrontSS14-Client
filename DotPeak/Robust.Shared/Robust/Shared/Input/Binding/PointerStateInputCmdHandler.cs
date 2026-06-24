// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.PointerStateInputCmdHandler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Player;

#nullable enable
namespace Robust.Shared.Input.Binding;

public sealed class PointerStateInputCmdHandler : InputCmdHandler
{
  private PointerInputCmdDelegate _enabled;
  private PointerInputCmdDelegate _disabled;

  public override bool FireOutsidePrediction { get; }

  public PointerStateInputCmdHandler(
    PointerInputCmdDelegate enabled,
    PointerInputCmdDelegate disabled,
    bool outsidePrediction = false)
  {
    this._enabled = enabled;
    this._disabled = disabled;
    this.FireOutsidePrediction = outsidePrediction;
  }

  public override bool HandleCmdMessage(
    IEntityManager entManager,
    ICommonSession? session,
    IFullInputCmdMessage message)
  {
    switch (message)
    {
      case ClientFullInputCmdMessage fullInputCmdMessage1:
        switch (fullInputCmdMessage1.State)
        {
          case BoundKeyState.Up:
            PointerInputCmdDelegate disabled1 = this._disabled;
            return disabled1 != null && disabled1(session, fullInputCmdMessage1.Coordinates, fullInputCmdMessage1.Uid);
          case BoundKeyState.Down:
            PointerInputCmdDelegate enabled1 = this._enabled;
            return enabled1 != null && enabled1(session, fullInputCmdMessage1.Coordinates, fullInputCmdMessage1.Uid);
        }
        break;
      case FullInputCmdMessage fullInputCmdMessage2:
        switch (fullInputCmdMessage2.State)
        {
          case BoundKeyState.Up:
            PointerInputCmdDelegate disabled2 = this._disabled;
            return disabled2 != null && disabled2(session, entManager.GetCoordinates(fullInputCmdMessage2.Coordinates), entManager.GetEntity(fullInputCmdMessage2.Uid));
          case BoundKeyState.Down:
            PointerInputCmdDelegate enabled2 = this._enabled;
            return enabled2 != null && enabled2(session, entManager.GetCoordinates(fullInputCmdMessage2.Coordinates), entManager.GetEntity(fullInputCmdMessage2.Uid));
        }
        break;
    }
    return false;
  }
}
