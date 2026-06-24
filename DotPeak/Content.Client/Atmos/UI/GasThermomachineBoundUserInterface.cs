// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasThermomachineBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.EntitySystems;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Systems;
using Content.Shared.Power.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasThermomachineBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasThermomachineWindow? _window;
  [Robust.Shared.ViewVariables.ViewVariables]
  private float _minTemp;
  [Robust.Shared.ViewVariables.ViewVariables]
  private float _maxTemp;
  [Robust.Shared.ViewVariables.ViewVariables]
  private bool _isHeater = true;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasThermomachineWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.ToggleStatusButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnToggleStatusButtonPressed());
    this._window.TemperatureSpinbox.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (_ => this.OnTemperatureChanged(this._window.TemperatureSpinbox.Value));
    this._window.Entity = this.Owner;
    base.Update();
  }

  private void OnToggleStatusButtonPressed()
  {
    if (this._window == null)
      return;
    this._window.SetActive(!this._window.Active);
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasThermomachineToggleMessage());
  }

  private void OnTemperatureChanged(float value)
  {
    float temperature = Math.Max(!this._isHeater ? Math.Max(value, this._minTemp) : Math.Min(value, this._maxTemp), 2.7f);
    if (!MathHelper.CloseTo((double) temperature, (double) value, 0.09))
      this._window?.SetTemperature(temperature);
    else
      this.SendPredictedMessage((BoundUserInterfaceMessage) new GasThermomachineChangeTemperatureMessage(temperature));
  }

  public virtual void Update()
  {
    GasThermoMachineComponent comp;
    if (this._window == null || !this.EntMan.TryGetComponent<GasThermoMachineComponent>(this.Owner, ref comp))
      return;
    SharedGasThermoMachineSystem thermoMachineSystem = this.EntMan.System<SharedGasThermoMachineSystem>();
    this._minTemp = comp.MinTemperature;
    this._maxTemp = comp.MaxTemperature;
    this._isHeater = thermoMachineSystem.IsHeater(comp);
    this._window.SetTemperature(comp.TargetTemperature);
    PowerReceiverSystem powerReceiverSystem = this.EntMan.System<PowerReceiverSystem>();
    SharedApcPowerReceiverComponent receiverComponent = (SharedApcPowerReceiverComponent) null;
    EntityUid owner = this.Owner;
    ref SharedApcPowerReceiverComponent local = ref receiverComponent;
    powerReceiverSystem.ResolveApc(owner, ref local);
    if (receiverComponent != null)
      this._window.SetActive(!receiverComponent.PowerDisabled);
    this._window.Title = this._isHeater ? Loc.GetString("comp-gas-thermomachine-ui-title-heater") : Loc.GetString("comp-gas-thermomachine-ui-title-freezer");
  }
}
