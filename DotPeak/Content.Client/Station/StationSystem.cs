// Decompiled with JetBrains decompiler
// Type: Content.Client.Station.StationSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Station;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Station;

public sealed class StationSystem : SharedStationSystem
{
  private readonly List<(string Name, NetEntity Entity)> _stations = new List<(string, NetEntity)>();

  public IReadOnlyList<(string Name, NetEntity Entity)> Stations
  {
    get => (IReadOnlyList<(string, NetEntity)>) this._stations;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<StationsUpdatedEvent>(new EntityEventHandler<StationsUpdatedEvent>(this.StationsUpdated), (Type[]) null, (Type[]) null);
  }

  private void StationsUpdated(StationsUpdatedEvent ev)
  {
    this._stations.Clear();
    this._stations.AddRange((IEnumerable<(string, NetEntity)>) ev.Stations);
  }
}
