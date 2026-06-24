// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerCharge.PowerChargeBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Power.PowerCharge;

public sealed class PowerChargeBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private PowerChargeWindow? _window;

  public void SetPowerSwitch(bool on)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SwitchChargingMachineMessage(on));
  }

  protected virtual void Open()
  {
    base.Open();
    PowerChargeComponent powerChargeComponent;
    if (!this.EntMan.TryGetComponent<PowerChargeComponent>(this.Owner, ref powerChargeComponent))
      return;
    this._window = BoundUserInterfaceExt.CreateWindow<PowerChargeWindow>((BoundUserInterface) this);
    this._window.UpdateWindow(this, Loc.GetString(LocId.op_Implicit(powerChargeComponent.WindowTitle)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is PowerChargeState state1))
      return;
    this._window?.UpdateState(state1);
  }
}
