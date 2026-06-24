// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasFilterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasFilterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private const float MaxTransferRate = 200f;
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasFilterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    AtmosphereSystem atmosphereSystem = this.EntMan.System<AtmosphereSystem>();
    this._window = BoundUserInterfaceExt.CreateWindow<GasFilterWindow>((BoundUserInterface) this);
    this._window.PopulateGasList(atmosphereSystem.Gases);
    this._window.ToggleStatusButtonPressed += new Action(this.OnToggleStatusButtonPressed);
    this._window.FilterTransferRateChanged += new Action<string>(this.OnFilterTransferRatePressed);
    this._window.SelectGasPressed += new Action(this.OnSelectGasPressed);
  }

  private void OnToggleStatusButtonPressed()
  {
    if (this._window == null)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new GasFilterToggleStatusMessage(this._window.FilterStatus));
  }

  private void OnFilterTransferRatePressed(string value)
  {
    float result;
    this.SendMessage((BoundUserInterfaceMessage) new GasFilterChangeRateMessage(UserInputParser.TryFloat((ReadOnlySpan<char>) value, out result) ? result : 0.0f));
  }

  private void OnSelectGasPressed()
  {
    if (this._window == null)
      return;
    if (this._window.SelectedGas == null)
    {
      this.SendMessage((BoundUserInterfaceMessage) new GasFilterSelectGasMessage(new int?()));
    }
    else
    {
      int result;
      if (!int.TryParse(this._window.SelectedGas, out result))
        return;
      this.SendMessage((BoundUserInterfaceMessage) new GasFilterSelectGasMessage(new int?(result)));
    }
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is GasFilterBoundUserInterfaceState userInterfaceState))
      return;
    this._window.Title = userInterfaceState.FilterLabel;
    this._window.SetFilterStatus(userInterfaceState.Enabled);
    this._window.SetTransferRate(userInterfaceState.TransferRate);
    Gas? filteredGas = userInterfaceState.FilteredGas;
    if (filteredGas.HasValue)
    {
      AtmosphereSystem atmosphereSystem = this.EntMan.System<AtmosphereSystem>();
      filteredGas = userInterfaceState.FilteredGas;
      int gasId = (int) filteredGas.Value;
      GasPrototype gas = atmosphereSystem.GetGas((Gas) gasId);
      string name = Loc.GetString(gas.Name);
      this._window.SetGasFiltered(gas.ID, name);
    }
    else
      this._window.SetGasFiltered((string) null, Loc.GetString("comp-gas-filter-ui-filter-gas-none"));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
