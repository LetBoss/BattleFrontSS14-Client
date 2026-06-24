// Decompiled with JetBrains decompiler
// Type: Content.Shared.Station.SharedStationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Station.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared.Station;

public abstract class SharedStationSystem : EntitySystem
{
  [Dependency]
  private MetaDataSystem _meta;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  private Robust.Shared.GameObjects.EntityQuery<StationMemberComponent> _stationMemberQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this._stationMemberQuery = this.GetEntityQuery<StationMemberComponent>();
    this.SubscribeLocalEvent<StationTrackerComponent, MapInitEvent>(new EntityEventRefHandler<StationTrackerComponent, MapInitEvent>(this.OnTrackerMapInit));
    this.SubscribeLocalEvent<StationTrackerComponent, ComponentRemove>(new EntityEventRefHandler<StationTrackerComponent, ComponentRemove>(this.OnTrackerRemove));
    this.SubscribeLocalEvent<StationTrackerComponent, GridUidChangedEvent>(new EntityEventRefHandler<StationTrackerComponent, GridUidChangedEvent>(this.OnTrackerGridChanged));
    this.SubscribeLocalEvent<StationTrackerComponent, MetaFlagRemoveAttemptEvent>(new EntityEventRefHandler<StationTrackerComponent, MetaFlagRemoveAttemptEvent>(this.OnMetaFlagRemoveAttempt));
  }

  private void OnTrackerMapInit(Entity<StationTrackerComponent> ent, ref MapInitEvent args)
  {
    this._meta.AddFlag((EntityUid) ent, MetaDataFlags.ExtraTransformEvents);
    this.UpdateStationTracker((Entity<StationTrackerComponent, TransformComponent>) ent.AsNullable());
  }

  private void OnTrackerRemove(Entity<StationTrackerComponent> ent, ref ComponentRemove args)
  {
    this._meta.RemoveFlag((EntityUid) ent, MetaDataFlags.ExtraTransformEvents);
  }

  private void OnTrackerGridChanged(
    Entity<StationTrackerComponent> ent,
    ref GridUidChangedEvent args)
  {
    this.UpdateStationTracker((Entity<StationTrackerComponent, TransformComponent>) ((EntityUid) ent, ent.Comp, args.Transform));
  }

  private void OnMetaFlagRemoveAttempt(
    Entity<StationTrackerComponent> ent,
    ref MetaFlagRemoveAttemptEvent args)
  {
    if ((args.ToRemove & MetaDataFlags.ExtraTransformEvents) == MetaDataFlags.None || ent.Comp.LifeStage > ComponentLifeStage.Running)
      return;
    args.ToRemove &= ~MetaDataFlags.ExtraTransformEvents;
  }

  public void UpdateStationTracker(
    Entity<StationTrackerComponent?, TransformComponent?> ent)
  {
    if (!this.Resolve<StationTrackerComponent>((EntityUid) ent, ref ent.Comp1))
      return;
    TransformComponent comp2 = ent.Comp2;
    if (!this._xformQuery.Resolve((EntityUid) ent, ref comp2))
      return;
    EntityUid? nullable;
    if (!(comp2.MapID == MapId.Nullspace))
    {
      nullable = comp2.GridUid;
      if (nullable.HasValue)
      {
        ref Robust.Shared.GameObjects.EntityQuery<StationMemberComponent> local1 = ref this._stationMemberQuery;
        nullable = comp2.GridUid;
        EntityUid uid = nullable.Value;
        StationMemberComponent stationMemberComponent;
        ref StationMemberComponent local2 = ref stationMemberComponent;
        if (!local1.TryGetComponent(uid, out local2))
        {
          Entity<StationTrackerComponent> ent1 = (Entity<StationTrackerComponent>) ent;
          nullable = new EntityUid?();
          EntityUid? station = nullable;
          this.SetStation(ent1, station);
          return;
        }
        this.SetStation((Entity<StationTrackerComponent>) ent, new EntityUid?(stationMemberComponent.Station));
        return;
      }
    }
    Entity<StationTrackerComponent> ent2 = (Entity<StationTrackerComponent>) ent;
    nullable = new EntityUid?();
    EntityUid? station1 = nullable;
    this.SetStation(ent2, station1);
  }

  public void SetStation(Entity<StationTrackerComponent?> ent, EntityUid? station)
  {
    if (!this.Resolve<StationTrackerComponent>((EntityUid) ent, ref ent.Comp))
      return;
    EntityUid? station1 = ent.Comp.Station;
    EntityUid? nullable = station;
    if ((station1.HasValue == nullable.HasValue ? (station1.HasValue ? (station1.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    ent.Comp.Station = station;
    this.Dirty<StationTrackerComponent>(ent);
  }

  public EntityUid? GetCurrentStation(Entity<StationTrackerComponent?> ent)
  {
    return !this.Resolve<StationTrackerComponent>((EntityUid) ent, ref ent.Comp, false) ? new EntityUid?() : ent.Comp.Station;
  }
}
