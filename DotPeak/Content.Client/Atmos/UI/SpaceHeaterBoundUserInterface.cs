// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.SpaceHeaterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Piping.Portable.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class SpaceHeaterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private SpaceHeaterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<SpaceHeaterWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.ToggleStatusButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnToggleStatusButtonPressed());
    ((BaseButton) this._window.IncreaseTempRange).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnTemperatureRangeChanged((float) this._window.TemperatureChangeDelta));
    ((BaseButton) this._window.DecreaseTempRange).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnTemperatureRangeChanged((float) -this._window.TemperatureChangeDelta));
    this._window.ModeSelector.OnItemSelected += new Action<OptionButton.ItemSelectedEventArgs>(this.OnModeChanged);
    this._window.PowerLevelSelector.OnItemSelected += new Action<RadioOptionItemSelectedEventArgs<int>>(this.OnPowerLevelChange);
  }

  private void OnToggleStatusButtonPressed()
  {
    this._window?.SetActive(!this._window.Active);
    this.SendMessage((BoundUserInterfaceMessage) new SpaceHeaterToggleMessage());
  }

  private void OnTemperatureRangeChanged(float changeAmount)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SpaceHeaterChangeTemperatureMessage(changeAmount));
  }

  private void OnModeChanged(OptionButton.ItemSelectedEventArgs args)
  {
    this._window?.ModeSelector.SelectId(args.Id);
    this.SendMessage((BoundUserInterfaceMessage) new SpaceHeaterChangeModeMessage((SpaceHeaterMode) args.Id));
  }

  private void OnPowerLevelChange(RadioOptionItemSelectedEventArgs<int> args)
  {
    this._window?.PowerLevelSelector.Select(args.Id);
    this.SendMessage((BoundUserInterfaceMessage) new SpaceHeaterChangePowerLevelMessage((SpaceHeaterPowerLevel) args.Id));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is SpaceHeaterBoundUserInterfaceState userInterfaceState))
      return;
    this._window.SetActive(userInterfaceState.Enabled);
    this._window.ModeSelector.SelectId((int) userInterfaceState.Mode);
    this._window.PowerLevelSelector.Select((int) userInterfaceState.PowerLevel);
    this._window.MinTemp = userInterfaceState.MinTemperature;
    this._window.MaxTemp = userInterfaceState.MaxTemperature;
    this._window.SetTemperature(userInterfaceState.TargetTemperature);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
