// Decompiled with JetBrains decompiler
// Type: Content.Client.Medical.CrewMonitoring.CrewMonitoringBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Medical.CrewMonitoring;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Medical.CrewMonitoring;

public sealed class CrewMonitoringBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private CrewMonitoringWindow? _menu;

  protected virtual void Open()
  {
    base.Open();
    EntityUid? mapUid = new EntityUid?();
    string stationName = string.Empty;
    TransformComponent transformComponent;
    if (this.EntMan.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent))
    {
      mapUid = transformComponent.GridUid;
      MetaDataComponent metaDataComponent;
      if (this.EntMan.TryGetComponent<MetaDataComponent>(mapUid, ref metaDataComponent))
        stationName = metaDataComponent.EntityName;
    }
    this._menu = BoundUserInterfaceExt.CreateWindow<CrewMonitoringWindow>((BoundUserInterface) this);
    this._menu.Set(stationName, mapUid);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is CrewMonitoringState crewMonitoringState))
      return;
    TransformComponent transformComponent;
    this.EntMan.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent);
    this._menu?.ShowSensors(crewMonitoringState.Sensors, this.Owner, transformComponent?.Coordinates);
  }
}
