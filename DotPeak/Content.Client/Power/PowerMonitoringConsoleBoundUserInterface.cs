// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Power;

public sealed class PowerMonitoringConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private PowerMonitoringWindow? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<PowerMonitoringWindow>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.SendPowerMonitoringConsoleMessageAction += new Action<NetEntity?, PowerMonitoringConsoleGroup>(this.SendPowerMonitoringConsoleMessage);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    PowerMonitoringConsoleBoundInterfaceState boundInterfaceState = (PowerMonitoringConsoleBoundInterfaceState) state;
    TransformComponent transformComponent;
    this.EntMan.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent);
    this._menu?.ShowEntites(boundInterfaceState.TotalSources, boundInterfaceState.TotalBatteryUsage, boundInterfaceState.TotalLoads, boundInterfaceState.AllEntries, boundInterfaceState.FocusSources, boundInterfaceState.FocusLoads, transformComponent?.Coordinates);
  }

  public void SendPowerMonitoringConsoleMessage(
    NetEntity? netEntity,
    PowerMonitoringConsoleGroup group)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PowerMonitoringConsoleMessage(netEntity, group));
  }
}
