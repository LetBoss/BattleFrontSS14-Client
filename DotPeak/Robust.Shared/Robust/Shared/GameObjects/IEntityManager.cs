// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IEntityManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Prometheus;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IEntityManager
{
  event Action<AddedComponentEventArgs>? ComponentAdded;

  event Action<RemovedComponentEventArgs>? ComponentRemoved;

  [Obsolete("Use InitializeEntity")]
  void InitializeComponents(EntityUid uid, MetaDataComponent? meta = null);

  [Obsolete("Use StartEntity")]
  void StartComponents(EntityUid uid);

  int Count<T>() where T : IComponent;

  int Count(Type component);

  void AddComponents(EntityUid target, EntityPrototype prototype, bool removeExisting = true);

  void AddComponents(EntityUid target, ComponentRegistry registry, bool removeExisting = true);

  void RemoveComponents(EntityUid target, EntityPrototype prototype);

  void RemoveComponents(EntityUid target, ComponentRegistry registry);

  T AddComponent<T>(EntityUid uid) where T : IComponent, new();

  IComponent AddComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null);

  void AddComponent<T>(EntityUid uid, T component, bool overwrite = false, MetaDataComponent? metadata = null) where T : IComponent;

  bool RemoveComponent<T>(EntityUid uid, MetaDataComponent? meta = null) where T : IComponent;

  bool RemoveComponent(EntityUid uid, Type type, MetaDataComponent? meta = null);

  bool RemoveComponent(EntityUid uid, ushort netID, MetaDataComponent? meta = null);

  void RemoveComponent(EntityUid uid, IComponent component, MetaDataComponent? meta = null);

  bool RemoveComponentDeferred<T>(EntityUid uid);

  bool RemoveComponentDeferred(EntityUid uid, Type type);

  bool RemoveComponentDeferred(EntityUid uid, ushort netID, MetaDataComponent? meta = null);

  void RemoveComponentDeferred(EntityUid uid, IComponent component);

  void RemoveComponents(EntityUid uid, MetaDataComponent? meta = null);

  void DisposeComponents(EntityUid uid, MetaDataComponent? meta = null);

  bool HasComponent<T>(EntityUid uid) where T : IComponent;

  bool HasComponent<T>([NotNullWhen(true)] EntityUid? uid) where T : IComponent;

  bool HasComponent(EntityUid uid, ComponentRegistration reg);

  bool HasComponent(EntityUid uid, Type type);

  bool HasComponent([NotNullWhen(true)] EntityUid? uid, Type type);

  bool HasComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null);

  bool HasComponent([NotNullWhen(true)] EntityUid? uid, ushort netId, MetaDataComponent? meta = null);

  T EnsureComponent<T>(EntityUid uid) where T : IComponent, new();

  bool EnsureComponent<T>(EntityUid uid, out T component) where T : IComponent, new();

  bool EnsureComponent<T>(ref Entity<T?> entity) where T : IComponent, new();

  T GetComponent<T>(EntityUid uid) where T : IComponent;

  IComponent GetComponent(EntityUid uid, CompIdx type);

  IComponent GetComponent(EntityUid uid, Type type);

  IComponent GetComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null);

  IComponent GetComponentInternal(EntityUid uid, CompIdx type);

  bool TryGetComponent<T>(EntityUid uid, [NotNullWhen(true)] out T? component) where T : IComponent?;

  bool TryGetComponent<T>([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out T? component) where T : IComponent?;

  bool TryGetComponent(EntityUid uid, ComponentRegistration reg, [NotNullWhen(true)] out IComponent? component);

  bool TryGetComponent(EntityUid uid, Type type, [NotNullWhen(true)] out IComponent? component);

  bool TryGetComponent(EntityUid uid, CompIdx type, [NotNullWhen(true)] out IComponent? component);

  bool TryGetComponent([NotNullWhen(true)] EntityUid? uid, Type type, [NotNullWhen(true)] out IComponent? component);

  bool TryGetComponent(
    EntityUid uid,
    ushort netId,
    [NotNullWhen(true)] out IComponent? component,
    MetaDataComponent? meta = null);

  bool TryGetComponent(
    [NotNullWhen(true)] EntityUid? uid,
    ushort netId,
    [NotNullWhen(true)] out IComponent? component,
    MetaDataComponent? meta = null);

  bool TryCopyComponent<T>(
    EntityUid source,
    EntityUid target,
    ref T? sourceComponent,
    [NotNullWhen(true)] out T? targetComp,
    MetaDataComponent? meta = null)
    where T : IComponent;

  bool TryCopyComponents(
    EntityUid source,
    EntityUid target,
    MetaDataComponent? meta = null,
    params Type[] sourceComponents);

  IComponent CopyComponent(
    EntityUid source,
    EntityUid target,
    IComponent sourceComponent,
    MetaDataComponent? meta = null);

  T CopyComponent<T>(
    EntityUid source,
    EntityUid target,
    T sourceComponent,
    MetaDataComponent? meta = null)
    where T : IComponent;

  void CopyComponents(
    EntityUid source,
    EntityUid target,
    MetaDataComponent? meta = null,
    params IComponent[] sourceComponents);

  Robust.Shared.GameObjects.EntityQuery<TComp1> GetEntityQuery<TComp1>() where TComp1 : IComponent;

  Robust.Shared.GameObjects.EntityQuery<IComponent> GetEntityQuery(Type type);

  IEnumerable<IComponent> GetComponents(EntityUid uid);

  IEnumerable<T> GetComponents<T>(EntityUid uid);

  int ComponentCount(EntityUid uid);

  NetComponentEnumerable GetNetComponents(EntityUid uid, MetaDataComponent? meta = null);

  NetComponentEnumerable? GetNetComponentsOrNull(EntityUid uid, MetaDataComponent? meta = null);

  IComponentState? GetComponentState(
    IEventBus eventBus,
    IComponent component,
    ICommonSession? player,
    GameTick fromTick);

  bool CanGetComponentState(IEventBus eventBus, IComponent component, ICommonSession player);

  (EntityUid Uid, T Component)[] AllComponents<T>() where T : IComponent;

  Entity<T>[] AllEntities<T>() where T : IComponent;

  Entity<IComponent>[] AllEntities(Type tComp);

  EntityUid[] AllEntityUids<T>() where T : IComponent;

  EntityUid[] AllEntityUids(Type tComp);

  List<(EntityUid Uid, T Component)> AllComponentsList<T>() where T : IComponent;

  Robust.Shared.GameObjects.ComponentQueryEnumerator ComponentQueryEnumerator(
    ComponentRegistry registry);

  CompRegistryEntityEnumerator CompRegistryQueryEnumerator(ComponentRegistry registry);

  Robust.Shared.GameObjects.AllEntityQueryEnumerator<IComponent> AllEntityQueryEnumerator(Type comp);

  Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1> AllEntityQueryEnumerator<TComp1>() where TComp1 : IComponent;

  Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2> AllEntityQueryEnumerator<TComp1, TComp2>()
    where TComp1 : IComponent
    where TComp2 : IComponent;

  Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2, TComp3> AllEntityQueryEnumerator<TComp1, TComp2, TComp3>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent;

  Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent;

  Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1> EntityQueryEnumerator<TComp1>() where TComp1 : IComponent;

  Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2> EntityQueryEnumerator<TComp1, TComp2>()
    where TComp1 : IComponent
    where TComp2 : IComponent;

  Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3> EntityQueryEnumerator<TComp1, TComp2, TComp3>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent;

  Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent;

  IEnumerable<T> EntityQuery<T>(bool includePaused = false) where T : IComponent;

  IEnumerable<(TComp1, TComp2)> EntityQuery<TComp1, TComp2>(bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent;

  IEnumerable<(TComp1, TComp2, TComp3)> EntityQuery<TComp1, TComp2, TComp3>(bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent;

  IEnumerable<(TComp1, TComp2, TComp3, TComp4)> EntityQuery<TComp1, TComp2, TComp3, TComp4>(
    bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent;

  IEnumerable<(EntityUid Uid, IComponent Component)> GetAllComponents(Type type, bool includePaused = false);

  void CullRemovedComponents();

  GameTick CurrentTick { get; }

  void Initialize();

  void Startup();

  void Shutdown();

  void Cleanup();

  void FlushEntities();

  void TickUpdate(float frameTime, bool noPredictions, Histogram? histogram = null);

  void FrameUpdate(float frameTime);

  IComponentFactory ComponentFactory { get; }

  IEntitySystemManager EntitySysManager { get; }

  IEntityNetworkManager EntityNetManager { get; }

  IEventBus EventBus { get; }

  event Action<Entity<MetaDataComponent>>? EntityAdded;

  event Action<Entity<MetaDataComponent>>? EntityInitialized;

  event Action<Entity<MetaDataComponent>>? EntityDeleted;

  event Action<Entity<MetaDataComponent>>? EntityDirtied;

  event Action? BeforeEntityFlush;

  event Action? AfterEntityFlush;

  [Obsolete("Use one of the other CreateEntityUninitialized overloads. euid no longer does anything.")]
  EntityUid CreateEntityUninitialized(
    string? prototypeName,
    EntityUid euid,
    ComponentRegistry? overrides = null);

  EntityUid CreateEntityUninitialized(string? prototypeName, ComponentRegistry? overrides = null);

  EntityUid CreateEntityUninitialized(
    string? prototypeName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle));

  EntityUid CreateEntityUninitialized(
    string? prototypeName,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle));

  void InitializeAndStartEntity(EntityUid entity, MapId? mapId = null);

  void InitializeAndStartEntity(Entity<MetaDataComponent?> entity, bool doMapInit);

  void InitializeEntity(EntityUid entity, MetaDataComponent? meta = null);

  void StartEntity(EntityUid entity);

  int EntityCount { get; }

  IEnumerable<EntityUid> GetEntities();

  void DirtyEntity(EntityUid uid, MetaDataComponent? metadata = null);

  [Obsolete("use override with an EntityUid")]
  void Dirty(IComponent component, MetaDataComponent? metadata = null);

  void Dirty(EntityUid uid, IComponent component, MetaDataComponent? meta = null);

  void Dirty<T>(Entity<T> ent, MetaDataComponent? meta = null) where T : IComponent;

  void Dirty<T1, T2>(Entity<T1, T2> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent;

  void Dirty<T1, T2, T3>(Entity<T1, T2, T3> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
    where T3 : IComponent;

  void Dirty<T1, T2, T3, T4>(Entity<T1, T2, T3, T4> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
    where T3 : IComponent
    where T4 : IComponent;

  bool TryQueueDeleteEntity(EntityUid? uid);

  void QueueDeleteEntity(EntityUid? uid);

  bool IsQueuedForDeletion(EntityUid uid);

  void PredictedDeleteEntity(Entity<MetaDataComponent?, TransformComponent?> ent);

  void PredictedDeleteEntity(Entity<MetaDataComponent?, TransformComponent?>? ent);

  void PredictedQueueDeleteEntity(Entity<MetaDataComponent?> ent);

  void PredictedQueueDeleteEntity(Entity<MetaDataComponent?>? ent);

  [Obsolete("use variant without TransformComponent")]
  void PredictedQueueDeleteEntity(Entity<MetaDataComponent?, TransformComponent?> ent);

  [Obsolete("use variant without TransformComponent")]
  void PredictedQueueDeleteEntity(Entity<MetaDataComponent?, TransformComponent?>? ent);

  void PredictedQueueDeleteEntity(EntityUid ent);

  void PredictedQueueDeleteEntity(EntityUid? ent);

  void DeleteEntity(EntityUid? uid);

  void DeleteEntity(EntityUid uid, MetaDataComponent meta, TransformComponent xform);

  bool EntityExists(EntityUid uid);

  bool EntityExists([NotNullWhen(true)] EntityUid? uid);

  bool IsPaused([NotNullWhen(true)] EntityUid? uid, MetaDataComponent? metadata = null);

  bool Deleted(EntityUid uid);

  bool Deleted([NotNullWhen(false)] EntityUid? uid);

  void RunMapInit(EntityUid entity, MetaDataComponent meta);

  EntityStringRepresentation ToPrettyString(EntityUid uid, MetaDataComponent? metadata);

  EntityStringRepresentation ToPrettyString(Entity<MetaDataComponent?> uid);

  EntityStringRepresentation ToPrettyString(NetEntity netEntity);

  [return: NotNullIfNotNull("uid")]
  EntityStringRepresentation? ToPrettyString(EntityUid? uid, MetaDataComponent? metadata = null);

  [return: NotNullIfNotNull("netEntity")]
  EntityStringRepresentation? ToPrettyString(NetEntity? netEntity);

  void RaisePredictiveEvent<T>(T msg) where T : EntityEventArgs;

  void DirtyField(
    EntityUid uid,
    IComponentDelta delta,
    string fieldName,
    MetaDataComponent? metadata = null);

  void DirtyField<T>(EntityUid uid, T component, string fieldName, MetaDataComponent? metadata = null) where T : IComponentDelta;

  bool TryParseNetEntity(string arg, [NotNullWhen(true)] out EntityUid? entity);

  bool TryGetEntity(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity);

  bool TryGetEntity(NetEntity? nEntity, [NotNullWhen(true)] out EntityUid? entity);

  bool TryGetEntityData(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity, [NotNullWhen(true)] out MetaDataComponent? meta);

  bool TryGetNetEntity(EntityUid uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null);

  bool TryGetNetEntity(EntityUid? uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null);

  bool IsClientSide(EntityUid uid, MetaDataComponent? metadata = null);

  EntityUid EnsureEntity<T>(NetEntity nEntity, EntityUid callerEntity);

  EntityUid? EnsureEntity<T>(NetEntity? nEntity, EntityUid callerEntity);

  EntityUid GetEntity(NetEntity nEntity);

  EntityUid? GetEntity(NetEntity? nEntity);

  NetEntity GetNetEntity(EntityUid uid, MetaDataComponent? metadata = null);

  NetEntity? GetNetEntity(EntityUid? uid, MetaDataComponent? metadata = null);

  HashSet<EntityUid> GetEntitySet(HashSet<NetEntity> netEntities);

  List<EntityUid> GetEntityList(List<NetEntity> netEntities);

  List<EntityUid> GetEntityList(ICollection<NetEntity> netEntities);

  List<EntityUid?> GetEntityList(List<NetEntity?> netEntities);

  EntityUid[] GetEntityArray(NetEntity[] netEntities);

  EntityUid?[] GetEntityArray(NetEntity?[] netEntities);

  Dictionary<EntityUid, T> GetEntityDictionary<T>(Dictionary<NetEntity, T> netEntities);

  Dictionary<T, EntityUid> GetEntityDictionary<T>(Dictionary<T, NetEntity> netEntities) where T : notnull;

  HashSet<NetEntity> GetNetEntitySet(HashSet<EntityUid> entities);

  List<NetEntity> GetNetEntityList(List<EntityUid> entities);

  List<NetEntity> GetNetEntityList(IReadOnlyList<EntityUid> entities);

  List<NetEntity> GetNetEntityList(ICollection<EntityUid> entities);

  List<NetEntity?> GetNetEntityList(List<EntityUid?> entities);

  NetEntity[] GetNetEntityArray(EntityUid[] entities);

  NetEntity?[] GetNetEntityArray(EntityUid?[] entities);

  Dictionary<NetEntity, T> GetNetEntityDictionary<T>(Dictionary<EntityUid, T> entities);

  Dictionary<T, NetEntity> GetNetEntityDictionary<T>(Dictionary<T, EntityUid> entities) where T : notnull;

  Dictionary<T, NetEntity?> GetNetEntityDictionary<T>(Dictionary<T, EntityUid?> entities) where T : notnull;

  Dictionary<NetEntity, NetEntity> GetNetEntityDictionary(Dictionary<EntityUid, EntityUid> entities);

  Dictionary<NetEntity, NetEntity?> GetNetEntityDictionary(
    Dictionary<EntityUid, EntityUid?> entities);

  NetCoordinates GetNetCoordinates(EntityCoordinates coordinates, MetaDataComponent? metadata = null);

  NetCoordinates? GetNetCoordinates(EntityCoordinates? coordinates, MetaDataComponent? metadata = null);

  EntityCoordinates GetCoordinates(NetCoordinates coordinates);

  EntityCoordinates? GetCoordinates(NetCoordinates? coordinates);

  EntityCoordinates EnsureCoordinates<T>(NetCoordinates netCoordinates, EntityUid callerEntity);

  EntityCoordinates? EnsureCoordinates<T>(NetCoordinates? netCoordinates, EntityUid callerEntity);

  HashSet<EntityCoordinates> GetEntitySet(HashSet<NetCoordinates> netEntities);

  List<EntityCoordinates> GetEntityList(List<NetCoordinates> netEntities);

  HashSet<EntityUid> EnsureEntitySet<T>(HashSet<NetEntity> netEntities, EntityUid callerEntity);

  List<EntityUid> EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity);

  void EnsureEntityList<T>(
    List<NetEntity> netEntities,
    EntityUid callerEntity,
    List<EntityUid> entities);

  void EnsureEntityDictionary<TComp, TValue>(
    Dictionary<NetEntity, TValue> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, TValue> entities);

  void EnsureEntityDictionaryNullableValue<TComp, TValue>(
    Dictionary<NetEntity, TValue?> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, TValue?> entities);

  void EnsureEntityDictionary<TComp, TKey>(
    Dictionary<TKey, NetEntity> netEntities,
    EntityUid callerEntity,
    Dictionary<TKey, EntityUid> entities)
    where TKey : notnull;

  void EnsureEntityDictionary<TComp, TKey>(
    Dictionary<TKey, NetEntity?> netEntities,
    EntityUid callerEntity,
    Dictionary<TKey, EntityUid?> entities)
    where TKey : notnull;

  void EnsureEntityDictionary<TComp>(
    Dictionary<NetEntity, NetEntity> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, EntityUid> entities);

  void EnsureEntityDictionary<TComp>(
    Dictionary<NetEntity, NetEntity?> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, EntityUid?> entities);

  List<EntityCoordinates> GetEntityList(ICollection<NetCoordinates> netEntities);

  List<EntityCoordinates?> GetEntityList(List<NetCoordinates?> netEntities);

  EntityCoordinates[] GetEntityArray(NetCoordinates[] netEntities);

  EntityCoordinates?[] GetEntityArray(NetCoordinates?[] netEntities);

  HashSet<NetCoordinates> GetNetCoordinatesSet(HashSet<EntityCoordinates> entities);

  List<NetCoordinates> GetNetCoordinatesList(List<EntityCoordinates> entities);

  List<NetCoordinates> GetNetCoordinatesList(ICollection<EntityCoordinates> entities);

  List<NetCoordinates?> GetNetCoordinatesList(List<EntityCoordinates?> entities);

  NetCoordinates[] GetNetCoordinatesArray(EntityCoordinates[] entities);

  NetCoordinates?[] GetNetCoordinatesArray(EntityCoordinates?[] entities);

  (EntityUid, MetaDataComponent) GetEntityData(NetEntity nEntity);

  EntityUid[] SpawnEntities(EntityCoordinates coordinates, List<string?> protoNames)
  {
    return this.SpawnEntitiesAttachedTo(coordinates, protoNames);
  }

  EntityUid SpawnEntity(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null);

  EntityUid SpawnEntity(string? protoName, MapCoordinates coordinates, ComponentRegistry? overrides = null);

  EntityUid[] SpawnEntities(MapCoordinates coordinates, params string?[] protoNames);

  EntityUid[] SpawnEntities(MapCoordinates coordinates, string? prototype, int count);

  EntityUid[] SpawnEntities(MapCoordinates coordinates, List<string?> protoNames);

  EntityUid[] SpawnEntitiesAttachedTo(
    EntityCoordinates coordinates,
    IEnumerable<EntProtoId> protoNames);

  EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, params EntProtoId[] protoNames);

  EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, List<string?> protoNames);

  EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, params string?[] protoNames);

  EntityUid Spawn(string? protoName = null, ComponentRegistry? overrides = null, bool doMapInit = true);

  EntityUid Spawn(
    string? protoName,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle));

  EntityUid SpawnAttachedTo(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle));

  EntityUid SpawnAtPosition(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null);

  bool TrySpawnInContainer(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    [NotNullWhen(true)] out EntityUid? uid,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null);

  EntityUid SpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    TransformComponent? xform = null,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null);

  EntityUid SpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    out bool inserted,
    TransformComponent? xform = null,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null);

  bool TrySpawnNextTo(
    string? protoName,
    EntityUid target,
    [NotNullWhen(true)] out EntityUid? uid,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null);

  EntityUid SpawnNextToOrDrop(
    string? protoName,
    EntityUid target,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null);

  T System<T>() where T : IEntitySystem;

  T? SystemOrNull<T>() where T : IEntitySystem;

  bool TrySystem<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem;
}
