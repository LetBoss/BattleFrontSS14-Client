// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.InputCmdHandler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Player;

#nullable enable
namespace Robust.Shared.Input.Binding;

public abstract class InputCmdHandler
{
  public virtual bool FireOutsidePrediction => false;

  public virtual void Enabled(ICommonSession? session)
  {
  }

  public virtual void Disabled(ICommonSession? session)
  {
  }

  public abstract bool HandleCmdMessage(
    IEntityManager entManager,
    ICommonSession? session,
    IFullInputCmdMessage message);

  public static InputCmdHandler FromDelegate(
    StateInputCmdDelegate? enabled = null,
    StateInputCmdDelegate? disabled = null,
    bool handle = true,
    bool outsidePrediction = true)
  {
    return (InputCmdHandler) new InputCmdHandler.StateInputCmdHandler()
    {
      EnabledDelegate = enabled,
      DisabledDelegate = disabled,
      Handle = handle,
      OutsidePrediction = outsidePrediction
    };
  }

  private sealed class StateInputCmdHandler : InputCmdHandler
  {
    public StateInputCmdDelegate? EnabledDelegate;
    public StateInputCmdDelegate? DisabledDelegate;
    public bool OutsidePrediction;

    public bool Handle { get; set; }

    public override bool FireOutsidePrediction => this.OutsidePrediction;

    public override void Enabled(ICommonSession? session)
    {
      StateInputCmdDelegate enabledDelegate = this.EnabledDelegate;
      if (enabledDelegate == null)
        return;
      enabledDelegate(session);
    }

    public override void Disabled(ICommonSession? session)
    {
      StateInputCmdDelegate disabledDelegate = this.DisabledDelegate;
      if (disabledDelegate == null)
        return;
      disabledDelegate(session);
    }

    public override bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      switch (message.State)
      {
        case BoundKeyState.Up:
          this.Disabled(session);
          return this.Handle;
        case BoundKeyState.Down:
          this.Enabled(session);
          return this.Handle;
        default:
          return false;
      }
    }
  }
}
