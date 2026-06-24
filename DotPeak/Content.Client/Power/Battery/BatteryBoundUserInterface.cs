// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.Battery.BatteryBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface;
using Content.Shared.Power;
using Robust.Client.Timing;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Power.Battery;

public sealed class BatteryBoundUserInterface : BoundUserInterface, IBuiPreTickUpdate
{
  [Dependency]
  private IClientGameTiming _gameTiming;
  [Robust.Shared.ViewVariables.ViewVariables]
  private BatteryMenu? _menu;
  private BuiPredictionState? _pred;
  private InputCoalescer<float> _chargeRateCoalescer;
  private InputCoalescer<float> _dischargeRateCoalescer;

  public BatteryBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<BatteryBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this._pred = new BuiPredictionState((BoundUserInterface) this, this._gameTiming);
    this._menu = BoundUserInterfaceExt.CreateWindow<BatteryMenu>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.OnInBreaker += (Action<bool>) (val => this._pred.SendMessage((BoundUserInterfaceMessage) new BatterySetInputBreakerMessage(val)));
    this._menu.OnOutBreaker += (Action<bool>) (val => this._pred.SendMessage((BoundUserInterfaceMessage) new BatterySetOutputBreakerMessage(val)));
    this._menu.OnChargeRate += (Action<float>) (val => this._chargeRateCoalescer.Set(val));
    this._menu.OnDischargeRate += (Action<float>) (val => this._dischargeRateCoalescer.Set(val));
  }

  void IBuiPreTickUpdate.PreTickUpdate()
  {
    float rate1;
    if (this._chargeRateCoalescer.CheckIsModified(out rate1))
      this._pred.SendMessage((BoundUserInterfaceMessage) new BatterySetChargeRateMessage(rate1));
    float rate2;
    if (!this._dischargeRateCoalescer.CheckIsModified(out rate2))
      return;
    this._pred.SendMessage((BoundUserInterfaceMessage) new BatterySetDischargeRateMessage(rate2));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is BatteryBuiState msg))
      return;
    foreach (BoundUserInterfaceMessage interfaceMessage in this._pred.MessagesToReplay())
    {
      switch (interfaceMessage)
      {
        case BatterySetInputBreakerMessage inputBreakerMessage:
          msg.CanCharge = inputBreakerMessage.On;
          continue;
        case BatterySetOutputBreakerMessage outputBreakerMessage:
          msg.CanDischarge = outputBreakerMessage.On;
          continue;
        case BatterySetChargeRateMessage chargeRateMessage:
          msg.MaxChargeRate = chargeRateMessage.Rate;
          continue;
        case BatterySetDischargeRateMessage dischargeRateMessage:
          msg.MaxSupply = dischargeRateMessage.Rate;
          continue;
        default:
          continue;
      }
    }
    this._menu?.Update(msg);
  }
}
