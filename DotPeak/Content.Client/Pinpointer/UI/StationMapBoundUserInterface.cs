// Decompiled with JetBrains decompiler
// Type: Content.Client.Pinpointer.UI.StationMapBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Pinpointer;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Pinpointer.UI;

public sealed class StationMapBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private StationMapWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    EntityUid? mapUid = new EntityUid?();
    TransformComponent transformComponent;
    if (this.EntMan.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent))
      mapUid = transformComponent.GridUid;
    this._window = BoundUserInterfaceExt.CreateWindow<StationMapWindow>((BoundUserInterface) this);
    this._window.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    string stationName = string.Empty;
    MetaDataComponent metaDataComponent;
    if (this.EntMan.TryGetComponent<MetaDataComponent>(mapUid, ref metaDataComponent))
      stationName = metaDataComponent.EntityName;
    StationMapComponent stationMapComponent;
    if (this.EntMan.TryGetComponent<StationMapComponent>(this.Owner, ref stationMapComponent) && stationMapComponent.ShowLocation)
      this._window.Set(stationName, mapUid, new EntityUid?(this.Owner));
    else
      this._window.Set(stationName, mapUid, new EntityUid?());
  }
}
