// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.Filter
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Player;

public sealed class Filter
{
  private HashSet<ICommonSession> _recipients = new HashSet<ICommonSession>();

  private Filter()
  {
  }

  public bool CheckPrediction { get; private set; } = true;

  public bool SendReliable { get; private set; }

  public int Count => this._recipients.Count;

  public IEnumerable<ICommonSession> Recipients => (IEnumerable<ICommonSession>) this._recipients;

  public Filter AddPlayer(ICommonSession player)
  {
    this._recipients.Add(player);
    return this;
  }

  public Filter AddPlayersByPvs(
    EntityUid origin,
    float rangeMultiplier = 2f,
    IEntityManager? entityManager = null,
    ISharedPlayerManager? playerMan = null,
    IConfigurationManager? cfgMan = null)
  {
    IoCManager.Resolve<IEntityManager, ISharedPlayerManager, IConfigurationManager>(ref entityManager, ref playerMan, ref cfgMan);
    TransformComponent component = entityManager.GetComponent<TransformComponent>(origin);
    return this.AddPlayersByPvs(entityManager.System<SharedTransformSystem>().GetMapCoordinates(component), rangeMultiplier, entityManager, playerMan, cfgMan);
  }

  public Filter AddPlayersByPvs(
    EntityCoordinates origin,
    float rangeMultiplier = 2f,
    IEntityManager? entityMan = null,
    ISharedPlayerManager? playerMan = null)
  {
    IoCManager.Resolve<IEntityManager, ISharedPlayerManager>(ref entityMan, ref playerMan);
    return this.AddPlayersByPvs(entityMan.System<SharedTransformSystem>().ToMapCoordinates(origin), rangeMultiplier, entityMan, playerMan);
  }

  public Filter AddPlayersByPvs(
    MapCoordinates origin,
    float rangeMultiplier = 2f,
    IEntityManager? entManager = null,
    ISharedPlayerManager? playerMan = null,
    IConfigurationManager? cfgMan = null)
  {
    IoCManager.Resolve<ISharedPlayerManager, IConfigurationManager>(ref playerMan, ref cfgMan);
    if (!cfgMan.GetCVar<bool>(CVars.NetPVS))
      return this.AddAllPlayers();
    float range = cfgMan.GetCVar<float>(CVars.NetMaxUpdateRange) * rangeMultiplier;
    return this.AddInRange(origin, range, playerMan, entManager);
  }

  public Filter AddPlayers(IEnumerable<ICommonSession> players)
  {
    foreach (ICommonSession player in players)
      this.AddPlayer(player);
    return this;
  }

  public static IEnumerable<ICommonSession> GetAllPlayers(ISharedPlayerManager? playerManager = null)
  {
    IoCManager.Resolve<ISharedPlayerManager>(ref playerManager);
    return (IEnumerable<ICommonSession>) playerManager.NetworkedSessions;
  }

  public Filter AddAllPlayers(ISharedPlayerManager? playerMan = null)
  {
    IoCManager.Resolve<ISharedPlayerManager>(ref playerMan);
    this._recipients = new HashSet<ICommonSession>((IEnumerable<ICommonSession>) playerMan.NetworkedSessions);
    return this;
  }

  public Filter AddWhere(Predicate<ICommonSession> predicate, ISharedPlayerManager? playerMan = null)
  {
    IoCManager.Resolve<ISharedPlayerManager>(ref playerMan);
    foreach (ICommonSession networkedSession in playerMan.NetworkedSessions)
    {
      if (predicate(networkedSession))
        this.AddPlayer(networkedSession);
    }
    return this;
  }

  public Filter AddWhereAttachedEntity(Predicate<EntityUid> predicate)
  {
    return this.AddWhere((Predicate<ICommonSession>) (session =>
    {
      EntityUid? attachedEntity = session.AttachedEntity;
      return attachedEntity.HasValue && predicate(attachedEntity.GetValueOrDefault());
    }));
  }

  public Filter AddInGrid(EntityUid uid, IEntityManager? entMan = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entMan);
    EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
    return this.AddWhereAttachedEntity((Predicate<EntityUid>) (entity =>
    {
      EntityUid? gridUid = xformQuery.GetComponent(entity).GridUid;
      EntityUid entityUid = uid;
      return gridUid.HasValue && gridUid.GetValueOrDefault() == entityUid;
    }));
  }

  public Filter AddInMap(MapId mapId, IEntityManager? entMan = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entMan);
    EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
    return this.AddWhereAttachedEntity((Predicate<EntityUid>) (entity => xformQuery.GetComponent(entity).MapID == mapId));
  }

  public Filter AddInRange(
    MapCoordinates position,
    float range,
    ISharedPlayerManager? playerMan = null,
    IEntityManager? entMan = null)
  {
    IoCManager.Resolve<ISharedPlayerManager, IEntityManager>(ref playerMan, ref entMan);
    EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
    SharedTransformSystem xformSystem = entMan.System<SharedTransformSystem>();
    TransformComponent component;
    return this.AddWhere((Predicate<ICommonSession>) (session => session.AttachedEntity.HasValue && xformQuery.TryGetComponent(session.AttachedEntity.Value, out component) && component.MapID == position.MapId && (double) (xformSystem.GetWorldPosition(component) - position.Position).Length() < (double) range), playerMan);
  }

  public Filter RemoveByVisibility(uint flag, IEntityManager? entMan = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entMan);
    EyeComponent component;
    return this.RemoveWhere((Predicate<ICommonSession>) (session => !session.AttachedEntity.HasValue || !entMan.TryGetComponent<EyeComponent>(session.AttachedEntity, out component) || ((long) component.VisibilityMask & (long) flag) == 0L));
  }

  public Filter RemovePlayer(ICommonSession player)
  {
    this._recipients.Remove(player);
    return this;
  }

  public Filter RemovePlayers(IEnumerable<ICommonSession> players)
  {
    foreach (ICommonSession player in players)
      this._recipients.Remove(player);
    return this;
  }

  public Filter RemovePlayers(params ICommonSession[] players)
  {
    return this.RemovePlayers((IEnumerable<ICommonSession>) players);
  }

  public Filter RemovePlayerByAttachedEntity(EntityUid uid)
  {
    return this.RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == uid));
  }

  public Filter RemovePlayersByAttachedEntity(IEnumerable<EntityUid> uids)
  {
    return this.RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => uids.Contains<EntityUid>(e)));
  }

  public Filter RemovePlayersByAttachedEntity(params EntityUid[] uids)
  {
    return this.RemovePlayersByAttachedEntity((IEnumerable<EntityUid>) uids);
  }

  public Filter RemoveWhere(Predicate<ICommonSession> predicate)
  {
    this._recipients.RemoveWhere(predicate);
    return this;
  }

  public Filter RemoveWhereAttachedEntity(Predicate<EntityUid> predicate)
  {
    this._recipients.RemoveWhere((Predicate<ICommonSession>) (session =>
    {
      EntityUid? attachedEntity = session.AttachedEntity;
      return attachedEntity.HasValue && predicate(attachedEntity.GetValueOrDefault());
    }));
    return this;
  }

  public Filter RemoveInRange(MapCoordinates position, float range, IEntityManager? entMan = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entMan);
    EntityQuery<TransformComponent> xformQuery = entMan.GetEntityQuery<TransformComponent>();
    SharedTransformSystem xformSystem = entMan.System<SharedTransformSystem>();
    TransformComponent component;
    return this.RemoveWhere((Predicate<ICommonSession>) (session => session.AttachedEntity.HasValue && xformQuery.TryGetComponent(session.AttachedEntity.Value, out component) && component.MapID == position.MapId && (double) (xformSystem.GetWorldPosition(component) - position.Position).Length() < (double) range));
  }

  public Filter Merge(Filter other)
  {
    return this.AddPlayers((IEnumerable<ICommonSession>) other._recipients);
  }

  public Filter FromEntities(params EntityUid[] entities)
  {
    SharedFilterSystem entitySystem;
    return !EntitySystem.TryGet<SharedFilterSystem>(out entitySystem) ? this : entitySystem.FromEntities(this, entities);
  }

  public Filter Clone()
  {
    return new Filter()
    {
      _recipients = new HashSet<ICommonSession>((IEnumerable<ICommonSession>) this._recipients),
      SendReliable = this.SendReliable,
      CheckPrediction = this.CheckPrediction
    };
  }

  public Filter Unpredicted()
  {
    this.CheckPrediction = false;
    return this;
  }

  public Filter SendReliably()
  {
    this.SendReliable = true;
    return this;
  }

  public static Filter Empty() => new Filter();

  public static Filter SinglePlayer(ICommonSession player) => Filter.Empty().AddPlayer(player);

  public static Filter Broadcast() => Filter.Empty().AddAllPlayers();

  public static Filter BroadcastGrid(EntityUid grid) => Filter.Empty().AddInGrid(grid);

  public static Filter BroadcastMap(MapId map) => Filter.Empty().AddInMap(map);

  public static Filter Pvs(
    EntityUid origin,
    float rangeMultiplier = 2f,
    IEntityManager? entityManager = null,
    ISharedPlayerManager? playerManager = null,
    IConfigurationManager? cfgManager = null)
  {
    return Filter.Empty().AddPlayersByPvs(origin, rangeMultiplier, entityManager, playerManager, cfgManager);
  }

  public static Filter Pvs(
    EntityCoordinates origin,
    float rangeMultiplier = 2f,
    IEntityManager? entityMan = null,
    ISharedPlayerManager? playerMan = null)
  {
    return Filter.Empty().AddPlayersByPvs(origin, rangeMultiplier, entityMan, playerMan);
  }

  public static Filter Pvs(MapCoordinates origin, float rangeMultiplier = 2f)
  {
    return Filter.Empty().AddPlayersByPvs(origin, rangeMultiplier);
  }

  public static Filter PvsExcept(
    EntityUid origin,
    float rangeMultiplier = 2f,
    IEntityManager? entityManager = null)
  {
    return Filter.Pvs(origin, rangeMultiplier, entityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == origin));
  }

  public static Filter Entities(params EntityUid[] entities)
  {
    return Filter.Empty().FromEntities(entities);
  }

  public static Filter Local() => Filter.Empty();
}
