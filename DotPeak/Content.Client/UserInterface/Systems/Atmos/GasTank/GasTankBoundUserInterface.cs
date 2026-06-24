// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Atmos.GasTank.GasTankBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Atmos.GasTank;

public sealed class GasTankBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasTankWindow? _window;

  public void SetOutputPressure(float value)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasTankSetPressureMessage()
    {
      Pressure = value
    });
  }

  public void ToggleInternals()
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new GasTankToggleInternalsMessage());
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasTankWindow>((BoundUserInterface) this);
    this._window.Entity = this.Owner;
    this._window.SetTitle(this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName);
    this._window.OnOutputPressure += new Action<float>(this.SetOutputPressure);
    this._window.OnToggleInternals += new Action(this.ToggleInternals);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    GasTankComponent gasTankComponent;
    if (this.EntMan.TryGetComponent<GasTankComponent>(this.Owner, ref gasTankComponent))
      this._window?.Update(this.EntMan.System<SharedGasTankSystem>().CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((this.Owner, gasTankComponent))), gasTankComponent.IsConnected, gasTankComponent.OutputPressure);
    if (!(state is GasTankBoundUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    this._window?.Close();
  }
}
