// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.RespawnTowers.PubgRespawnTowerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.RespawnTowers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.RespawnTowers;

public sealed class PubgRespawnTowerSystem : EntitySystem
{
  public MapId MapId { get; private set; } = MapId.Nullspace;

  public IReadOnlyList<Vector2> TowerPositions { get; private set; } = (IReadOnlyList<Vector2>) Array.Empty<Vector2>();

  public IReadOnlyList<Vector2> ActiveTowerPositions { get; private set; } = (IReadOnlyList<Vector2>) Array.Empty<Vector2>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgRespawnTowerStateEvent>(new EntitySessionEventHandler<PubgRespawnTowerStateEvent>(this.OnTowerState), (Type[]) null, (Type[]) null);
  }

  private void OnTowerState(PubgRespawnTowerStateEvent ev, EntitySessionEventArgs args)
  {
    this.MapId = ev.MapId;
    this.TowerPositions = (IReadOnlyList<Vector2>) ev.TowerPositions;
    this.ActiveTowerPositions = (IReadOnlyList<Vector2>) ev.ActiveTowerPositions;
  }
}
