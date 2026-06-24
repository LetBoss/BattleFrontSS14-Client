// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgMinimapStateSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgMinimapStateSystem : EntitySystem
{
  public Vector2? ZoneCurrentCenter { get; private set; }

  public float ZoneCurrentRadius { get; private set; }

  public Vector2? ZoneNextCenter { get; private set; }

  public float ZoneNextRadius { get; private set; }

  public bool ZoneActive { get; private set; }

  public bool ZoneVisible { get; private set; }

  public MapId ZoneMapId { get; private set; } = MapId.Nullspace;

  public bool RedZoneActive { get; private set; }

  public Vector2? RedZoneCenter { get; private set; }

  public float RedZoneRadius { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgZoneStateEvent>(new EntitySessionEventHandler<PubgZoneStateEvent>(this.OnZoneStateUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RedZoneStateEvent>(new EntitySessionEventHandler<RedZoneStateEvent>(this.OnRedZoneStateUpdate), (Type[]) null, (Type[]) null);
  }

  private void OnZoneStateUpdate(PubgZoneStateEvent msg, EntitySessionEventArgs args)
  {
    EntityUid entity = this.GetEntity(msg.ZoneMapEntity);
    MapId mapId = MapId.Nullspace;
    MapComponent mapComponent;
    if (this.TryComp<MapComponent>(entity, ref mapComponent))
      mapId = mapComponent.MapId;
    this.ZoneCurrentCenter = new Vector2?(msg.CurrentCenter);
    this.ZoneCurrentRadius = msg.CurrentRadius;
    this.ZoneNextCenter = new Vector2?(msg.NextCenter);
    this.ZoneNextRadius = msg.NextRadius;
    this.ZoneActive = msg.Active;
    this.ZoneVisible = msg.Visible;
    this.ZoneMapId = mapId;
  }

  private void OnRedZoneStateUpdate(RedZoneStateEvent msg, EntitySessionEventArgs args)
  {
    this.RedZoneActive = msg.ZoneActive;
    if (!msg.ZoneActive)
    {
      this.RedZoneCenter = new Vector2?();
      this.RedZoneRadius = 0.0f;
    }
    else
    {
      this.RedZoneCenter = new Vector2?(msg.ZoneCenter);
      this.RedZoneRadius = msg.ZoneRadius;
    }
  }
}
