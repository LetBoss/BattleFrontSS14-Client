// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Prometheus;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.Exceptions;
using Robust.Shared.GameStates;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Profiling;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[Virtual]
public abstract class EntityManager : IEntityManager
{
  [Robust.Shared.IoC.Dependency]
  private readonly IComponentFactory _componentFactory;
  [Robust.Shared.IoC.Dependency]
  private readonly IRuntimeLog _runtimeLog;
  private const int TypeCapacity = 32 /*0x20*/;
  private const int ComponentCollectionCapacity = 1024 /*0x0400*/;
  private const int EntityCapacity = 1024 /*0x0400*/;
  private const int NetComponentCapacity = 8;
  private FrozenDictionary<Type, Dictionary<EntityUid, IComponent>> _entTraitDict = FrozenDictionary<Type, Dictionary<EntityUid, IComponent>>.Empty;
  private Dictionary<EntityUid, IComponent>[] _entTraitArray = Array.Empty<Dictionary<EntityUid, IComponent>>();
  private readonly HashSet<IComponent> _deleteSet = new HashSet<IComponent>(32 /*0x20*/);
  private UniqueIndexHkm<EntityUid, IComponent> _entCompIndex = new UniqueIndexHkm<EntityUid, IComponent>(1024 /*0x0400*/);
  [Robust.Shared.IoC.Dependency]
  protected readonly IPrototypeManager PrototypeManager;
  [Robust.Shared.IoC.Dependency]
  protected readonly ILogManager LogManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IEntitySystemManager _entitySystemManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IMapManager _mapManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IGameTiming _gameTiming;
  [Robust.Shared.IoC.Dependency]
  private readonly ISerializationManager _serManager;
  [Robust.Shared.IoC.Dependency]
  private readonly ProfManager _prof;
  [Robust.Shared.IoC.Dependency]
  private readonly INetManager _netMan;
  [Robust.Shared.IoC.Dependency]
  private readonly IReflectionManager _reflection;
  [Robust.Shared.IoC.Dependency]
  private readonly EntityConsoleHost _entityConsoleHost;
  protected SharedTransformSystem _xforms;
  private SharedContainerSystem _containers;
  public Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> MetaQuery;
  public Robust.Shared.GameObjects.EntityQuery<TransformComponent> TransformQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ActorComponent> _actorQuery;
  public static readonly MapInitEvent MapInitEventInstance = new MapInitEvent();
  protected readonly Queue<EntityUid> QueuedDeletions = new Queue<EntityUid>();
  protected readonly HashSet<EntityUid> QueuedDeletionsSet = new HashSet<EntityUid>();
  private EntityDiffContext _context = new EntityDiffContext();
  protected readonly HashSet<EntityUid> Entities = new HashSet<EntityUid>();
  internal EntityEventBus EventBusInternal;
  protected int NextEntityUid = (int) EntityUid.FirstUid;
  protected int NextNetworkId = (int) NetEntity.First;
  private string _xformName = string.Empty;
  private ComponentRegistration _metaReg;
  private ComponentRegistration _xformReg;
  private SharedMapSystem _mapSystem;
  private ISawmill _sawmill;
  internal ISawmill ResolveSawmill;
  private static readonly ComponentAdd CompAddInstance = new ComponentAdd();
  private static readonly ComponentInit CompInitInstance = new ComponentInit();
  private static readonly ComponentStartup CompStartupInstance = new ComponentStartup();
  private static readonly ComponentShutdown CompShutdownInstance = new ComponentShutdown();
  private static readonly ComponentRemove CompRemoveInstance = new ComponentRemove();
  protected readonly Dictionary<NetEntity, (EntityUid, MetaDataComponent)> NetEntityLookup = new Dictionary<NetEntity, (EntityUid, MetaDataComponent)>(1024 /*0x0400*/);

  public uint GetModifiedFields(IComponentDelta delta, GameTick fromTick)
  {
    uint modifiedFields = 0;
    for (int index = 0; index < delta.LastModifiedFields.Length; ++index)
    {
      if (!(delta.LastModifiedFields[index] < fromTick))
        modifiedFields |= (uint) (1 << index);
    }
    return modifiedFields;
  }

  public void DirtyField(
    EntityUid uid,
    IComponentDelta comp,
    string fieldName,
    MetaDataComponent? metadata = null)
  {
    int index;
    if (!this.ComponentFactory.GetRegistration((IComponent) comp).NetworkedFieldLookup.TryGetValue(fieldName, out index))
    {
      this._sawmill.Error($"Tried to dirty delta field {fieldName} on {this.ToPrettyString((Entity<MetaDataComponent>) uid)} that isn't implemented.");
    }
    else
    {
      GameTick curTick = this._gameTiming.CurTick;
      comp.LastFieldUpdate = curTick;
      comp.LastModifiedFields[index] = curTick;
      this.Dirty(uid, (IComponent) comp, metadata);
    }
  }

  public virtual void DirtyField<T>(
    EntityUid uid,
    T comp,
    [ValidateMember] string fieldName,
    MetaDataComponent? metadata = null)
    where T : IComponentDelta
  {
    int index;
    if (!this.ComponentFactory.GetRegistration(CompIdx.Index<T>()).NetworkedFieldLookup.TryGetValue(fieldName, out index))
    {
      this._sawmill.Error($"Tried to dirty delta field {fieldName} on {this.ToPrettyString((Entity<MetaDataComponent>) uid)} that isn't implemented.");
    }
    else
    {
      GameTick curTick = this._gameTiming.CurTick;
      comp.LastFieldUpdate = curTick;
      comp.LastModifiedFields[index] = curTick;
      this.Dirty(uid, (IComponent) comp, metadata);
    }
  }

  public virtual void DirtyFields<T>(
    EntityUid uid,
    T comp,
    MetaDataComponent? meta,
    params string[] fields)
    where T : IComponentDelta
  {
    ComponentRegistration registration = this.ComponentFactory.GetRegistration(CompIdx.Index<T>());
    GameTick curTick = this._gameTiming.CurTick;
    foreach (string field in fields)
    {
      int index;
      if (!registration.NetworkedFieldLookup.TryGetValue(field, out index))
        this._sawmill.Error($"Tried to dirty delta field {field} on {this.ToPrettyString((Entity<MetaDataComponent>) uid)} that isn't implemented.");
      else
        comp.LastModifiedFields[index] = curTick;
    }
    comp.LastFieldUpdate = curTick;
    this.Dirty(uid, (IComponent) comp, meta);
  }

  public IComponentFactory ComponentFactory => this._componentFactory;

  public event Action<AddedComponentEventArgs>? ComponentAdded;

  public event Action<RemovedComponentEventArgs>? ComponentRemoved;

  public void InitializeComponents()
  {
    if (this.Initialized)
      throw new InvalidOperationException("Already initialized.");
    this.FillComponentDict();
    this._componentFactory.ComponentsAdded += new Action<ComponentRegistration[]>(this.OnComponentsAdded);
  }

  public void ClearComponents()
  {
    this._entCompIndex.Clear();
    this._deleteSet.Clear();
    foreach (Dictionary<EntityUid, IComponent> dictionary in this._entTraitDict.Values)
      dictionary.Clear();
  }

  private void RegisterComponents(IEnumerable<ComponentRegistration> components)
  {
    Dictionary<Type, Dictionary<EntityUid, IComponent>> dictionary1 = this._entTraitDict.ToDictionary<Type, Dictionary<EntityUid, IComponent>>();
    foreach (ComponentRegistration component in components)
    {
      Dictionary<EntityUid, IComponent> dictionary2 = new Dictionary<EntityUid, IComponent>();
      dictionary1.Add(component.Type, dictionary2);
      CompIdx.AssignArray<Dictionary<EntityUid, IComponent>>(ref this._entTraitArray, component.Idx, dictionary2);
    }
    this._entTraitDict = dictionary1.ToFrozenDictionary<Type, Dictionary<EntityUid, IComponent>>();
  }

  private void OnComponentsAdded(ComponentRegistration[] components)
  {
    this.RegisterComponents((IEnumerable<ComponentRegistration>) components);
  }

  public int Count<T>() where T : IComponent => this._entTraitDict[typeof (T)].Count;

  public int Count(Type component) => this._entTraitDict[component].Count;

  [Obsolete("Use InitializeEntity")]
  public void InitializeComponents(EntityUid uid, MetaDataComponent? metadata = null)
  {
    if (metadata == null)
      metadata = this.MetaQuery.GetComponent(uid);
    this.SetLifeStage(metadata, EntityLifeStage.Initializing);
    Span<IComponent> asSpan = new FixedArray32<IComponent>().AsSpan;
    this.CopyComponentsInto(ref asSpan, uid);
    Span<IComponent> span = asSpan;
    for (int index = 0; index < span.Length; ++index)
    {
      IComponent component = span[index];
      if (component != null && component.LifeStage == ComponentLifeStage.Added)
        this.LifeInitialize<IComponent>(uid, component, this._componentFactory.GetIndex(component.GetType()));
    }
    this.SetLifeStage(metadata, EntityLifeStage.Initialized);
  }

  [Obsolete("Use StartEntity")]
  public void StartComponents(EntityUid uid)
  {
    Span<IComponent> asSpan = new FixedArray32<IComponent>().AsSpan;
    this.CopyComponentsInto(ref asSpan, uid);
    TransformComponent component1 = this.TransformQuery.GetComponent(uid);
    if (component1.LifeStage == ComponentLifeStage.Initialized)
      this.LifeStartup<TransformComponent>(uid, component1, CompIdx.Index<TransformComponent>());
    PhysicsComponent component2;
    if (this._physicsQuery.TryComp(uid, out component2) && component2.LifeStage == ComponentLifeStage.Initialized)
      this.LifeStartup<PhysicsComponent>(uid, component2, CompIdx.Index<PhysicsComponent>());
    Span<IComponent> span = asSpan;
    for (int index = 0; index < span.Length; ++index)
    {
      IComponent component3 = span[index];
      if (component3 != null && component3.LifeStage == ComponentLifeStage.Initialized)
        this.LifeStartup<IComponent>(uid, component3, this._componentFactory.GetIndex(component3.GetType()));
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AddComponents(EntityUid target, Robust.Shared.Prototypes.EntityPrototype prototype, bool removeExisting = true)
  {
    this.AddComponents(target, prototype.Components, removeExisting);
  }

  public void AddComponents(EntityUid target, ComponentRegistry registry, bool removeExisting = true)
  {
    if (registry.Count == 0)
      return;
    MetaDataComponent component1 = this.MetaQuery.GetComponent(target);
    foreach ((string str, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry>) registry)
    {
      ComponentRegistration registration = this._componentFactory.GetRegistration(str);
      if (removeExisting)
      {
        IComponent component2 = this._componentFactory.GetComponent(registration);
        this._serManager.CopyTo<IComponent>(componentRegistryEntry.Component, ref component2, notNullableOverride: true);
        this.AddComponentInternal<IComponent>(target, component2, registration, true, component1);
      }
      else if (!this.HasComponent(target, registration))
      {
        IComponent component3 = this._componentFactory.GetComponent(registration);
        this._serManager.CopyTo<IComponent>(componentRegistryEntry.Component, ref component3, notNullableOverride: true);
        this.AddComponentInternal<IComponent>(target, component3, registration, metadata: component1);
      }
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RemoveComponents(EntityUid target, Robust.Shared.Prototypes.EntityPrototype prototype)
  {
    this.RemoveComponents(target, prototype.Components);
  }

  public void RemoveComponents(EntityUid target, ComponentRegistry registry)
  {
    if (registry.Count == 0)
      return;
    MetaDataComponent component = this.MetaQuery.GetComponent(target);
    foreach (Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry in registry.Values)
      this.RemoveComponent(target, componentRegistryEntry.Component.GetType(), component);
  }

  public IComponent AddComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
  {
    IComponent component = this._componentFactory.GetComponent(netId);
    this.AddComponent<IComponent>(uid, component, false, meta);
    return component;
  }

  public T AddComponent<T>(EntityUid uid) where T : IComponent, new()
  {
    T component = this._componentFactory.GetComponent<T>();
    this.AddComponent<T>(uid, component, false, (MetaDataComponent) null);
    return component;
  }

  public void AddComponent(
    EntityUid uid,
    Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry entry,
    bool overwrite = false,
    MetaDataComponent? metadata = null)
  {
    IComponent component = this._componentFactory.GetComponent(entry);
    this.AddComponent<IComponent>(uid, component, overwrite, metadata);
  }

  public void AddComponent<T>(
    EntityUid uid,
    T component,
    bool overwrite = false,
    MetaDataComponent? metadata = null)
    where T : IComponent
  {
    if (!this.MetaQuery.Resolve(uid, ref metadata, false))
      throw new ArgumentException($"Entity {uid} is not valid.", nameof (uid));
    if ((object) component == null)
      throw new ArgumentNullException(nameof (component));
    if (component.Owner == new EntityUid())
      component.Owner = uid;
    else if (component.Owner != uid)
      throw new InvalidOperationException("Component is not owned by entity.");
    this.AddComponentInternal<T>(uid, component, overwrite, false, metadata);
  }

  private void AddComponentInternal<T>(
    EntityUid uid,
    T component,
    ComponentRegistration compReg,
    bool overwrite = false,
    MetaDataComponent? metadata = null)
    where T : IComponent
  {
    if (!this.MetaQuery.Resolve(uid, ref metadata, false))
      throw new ArgumentException($"Entity {uid} is not valid.", nameof (uid));
    component.Owner = uid;
    this.AddComponentInternal<T>(uid, component, compReg, overwrite, false, metadata);
  }

  private void AddComponentInternal<T>(
    EntityUid uid,
    T component,
    bool overwrite,
    bool skipInit,
    MetaDataComponent? metadata)
    where T : IComponent
  {
    if (!this.MetaQuery.ResolveInternal(uid, ref metadata, false))
      throw new ArgumentException($"Entity {uid} is not valid.", nameof (uid));
    ComponentRegistration registration = this._componentFactory.GetRegistration((IComponent) component);
    this.AddComponentInternal<T>(uid, component, registration, overwrite, skipInit, metadata);
  }

  private void AddComponentInternal<T>(
    EntityUid uid,
    T component,
    ComponentRegistration reg,
    bool overwrite,
    bool skipInit,
    MetaDataComponent metadata)
    where T : IComponent
  {
    CompIdx idx = reg.Idx;
    Dictionary<EntityUid, IComponent> entTrait = this._entTraitArray[idx.Value];
    bool exists;
    ref IComponent local1 = ref CollectionsMarshal.GetValueRefOrAddDefault<EntityUid, IComponent>(entTrait, uid, out exists);
    if (exists)
    {
      if (!overwrite && !local1.Deleted)
        throw new InvalidOperationException($"Component reference type {reg.Name} already occupied by {local1}");
      this.RemoveComponentImmediate(uid, local1, idx, false, metadata);
      entTrait.Add(uid, (IComponent) component);
    }
    else
      local1 = (IComponent) component;
    this._entCompIndex.Add(uid, (IComponent) component);
    ushort? netId;
    if (reg.NetID.HasValue && component.NetSyncEnabled)
    {
      netId = reg.NetID;
      ushort key = netId.Value;
      if (metadata == null)
        metadata = this.MetaQuery.GetComponentInternal(uid);
      metadata.NetComponents.Add(key, (IComponent) component);
    }
    if (component is IComponentDelta componentDelta)
    {
      GameTick curTick = this._gameTiming.CurTick;
      componentDelta.LastModifiedFields = new GameTick[reg.NetworkedFields.Length];
      Array.Fill<GameTick>(componentDelta.LastModifiedFields, curTick);
    }
    ref T local2 = ref component;
    if ((object) default (T) == null)
    {
      T obj = local2;
      local2 = ref obj;
    }
    netId = reg.NetID;
    int num = netId.HasValue ? 1 : 0;
    local2.Networked = num != 0;
    AddedComponentEventArgs e = new AddedComponentEventArgs(new ComponentEventArgs((IComponent) component, uid), reg);
    Action<AddedComponentEventArgs> componentAdded = this.ComponentAdded;
    if (componentAdded != null)
      componentAdded(e);
    this.EventBusInternal.OnComponentAdded(in e);
    this.LifeAddToEntity<T>(uid, component, reg.Idx);
    if (skipInit)
      return;
    if (metadata == null)
      metadata = this.MetaQuery.GetComponentInternal(uid);
    if (!metadata.EntityInitialized && !metadata.EntityInitializing)
      return;
    if (component.Networked)
      this.DirtyEntity(uid, metadata);
    this.LifeInitialize<T>(uid, component, reg.Idx);
    if (metadata.EntityInitialized)
      this.LifeStartup<T>(uid, component, reg.Idx);
    if (metadata.EntityLifeStage < EntityLifeStage.MapInitialized)
      return;
    this.EventBusInternal.RaiseComponentEvent<MapInitEvent>(uid, (IComponent) component, reg.Idx, EntityManager.MapInitEventInstance);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool RemoveComponent<T>(EntityUid uid, MetaDataComponent? meta = null) where T : IComponent
  {
    T component;
    if (!this.TryGetComponent<T>(uid, out component))
      return false;
    this.RemoveComponentImmediate(uid, (IComponent) component, CompIdx.Index<T>(), false, meta);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool RemoveComponent(EntityUid uid, Type type, MetaDataComponent? meta = null)
  {
    IComponent component;
    if (!this.TryGetComponent(uid, type, out component))
      return false;
    this.RemoveComponentImmediate(uid, component, this._componentFactory.GetIndex(type), false, meta);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool RemoveComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
  {
    IComponent component;
    if (!this.MetaQuery.Resolve(uid, ref meta) || !this.TryGetComponent(uid, netId, out component, meta))
      return false;
    CompIdx index = this._componentFactory.GetIndex(component.GetType());
    this.RemoveComponentImmediate(uid, component, index, false, meta);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RemoveComponent(EntityUid uid, IComponent component, MetaDataComponent? meta = null)
  {
    CompIdx index = this._componentFactory.GetIndex(component.GetType());
    this.RemoveComponentImmediate(uid, component, index, false, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool RemoveComponentDeferred<T>(EntityUid uid)
  {
    return this.RemoveComponentDeferred(uid, typeof (T));
  }

  public bool RemoveComponentDeferred(EntityUid uid, Type type)
  {
    IComponent component;
    if (!this.TryGetComponent(uid, type, out component))
      return false;
    this.RemoveComponentDeferred(component, uid, false);
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool RemoveComponentDeferred(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
  {
    IComponent component;
    if (!this.MetaQuery.Resolve(uid, ref meta) || !this.TryGetComponent(uid, netId, out component, meta))
      return false;
    this.RemoveComponentDeferred(component, uid, false);
    return true;
  }

  public void RemoveComponentDeferred(EntityUid owner, IComponent component)
  {
    this.RemoveComponentDeferred(component, owner, false);
  }

  public void RemoveComponentDeferred(EntityUid owner, Component component)
  {
    this.RemoveComponentDeferred((IComponent) component, owner, false);
  }

  private static IEnumerable<IComponent> InSafeOrder(
    IEnumerable<IComponent> comps,
    bool forCreation = false)
  {
    return !forCreation ? (IEnumerable<IComponent>) comps.OrderByDescending<IComponent, int>(new Func<IComponent, int>(Sequence)) : (IEnumerable<IComponent>) comps.OrderBy<IComponent, int>(new Func<IComponent, int>(Sequence));

    static int Sequence(IComponent x)
    {
      int num;
      switch (x)
      {
        case MetaDataComponent _:
          num = 0;
          break;
        case TransformComponent _:
          num = 1;
          break;
        case PhysicsComponent _:
          num = 2;
          break;
        default:
          num = int.MaxValue;
          break;
      }
      return num;
    }
  }

  public void RemoveComponents(EntityUid uid, MetaDataComponent? meta = null)
  {
    if (!this.MetaQuery.Resolve(uid, ref meta))
      return;
    foreach (IComponent component in EntityManager.InSafeOrder((IEnumerable<IComponent>) this._entCompIndex[uid]))
    {
      CompIdx index = this._componentFactory.GetIndex(component.GetType());
      this.RemoveComponentImmediate(uid, component, index, false, meta);
    }
  }

  public void DisposeComponents(EntityUid uid, MetaDataComponent? meta = null)
  {
    if (!this.MetaQuery.Resolve(uid, ref meta))
      return;
    foreach (IComponent component in EntityManager.InSafeOrder((IEnumerable<IComponent>) this._entCompIndex[uid]))
    {
      try
      {
        CompIdx index = this._componentFactory.GetIndex(component.GetType());
        this.RemoveComponentImmediate(uid, component, index, true, meta);
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Caught exception while trying to remove component {this._componentFactory.GetComponentName(component.GetType())} from entity '{this.ToPrettyString((Entity<MetaDataComponent>) uid)}'");
      }
    }
    this._entCompIndex.Remove(uid);
  }

  private void RemoveComponentDeferred(IComponent component, EntityUid uid, bool terminating)
  {
    if (component == null)
      throw new ArgumentNullException(nameof (component));
    if (component.Owner != uid)
      throw new InvalidOperationException("Component is not owned by entity.");
    if (component.Deleted)
      return;
    try
    {
      bool flag1 = !terminating;
      if (flag1)
      {
        bool flag2;
        switch (component)
        {
          case TransformComponent _:
          case MetaDataComponent _:
            flag2 = true;
            break;
          default:
            flag2 = false;
            break;
        }
        flag1 = flag2;
      }
      if (flag1 || !this._deleteSet.Add(component))
        return;
      switch (component.LifeStage)
      {
        case ComponentLifeStage.Initialized:
        case ComponentLifeStage.Starting:
        case ComponentLifeStage.Running:
          this.LifeShutdown<IComponent>(uid, component, this._componentFactory.GetIndex(component.GetType()));
          break;
        default:
          if (component.LifeStage != ComponentLifeStage.Added)
            break;
          component.LifeStage = ComponentLifeStage.Stopped;
          break;
      }
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Caught exception while queuing deferred component removal. Entity={this.ToPrettyString((Entity<MetaDataComponent>) component.Owner)}, type={component.GetType()}");
      this._runtimeLog.LogException(ex, nameof (RemoveComponentDeferred));
    }
  }

  private void RemoveComponentImmediate(
    EntityUid uid,
    IComponent component,
    CompIdx idx,
    bool terminating,
    MetaDataComponent? meta)
  {
    if (component.Deleted)
    {
      this._sawmill.Warning($"Deleting an already deleted component. Entity: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}, Component: {this._componentFactory.GetComponentName(component.GetType())}.");
    }
    else
    {
      try
      {
        bool flag1 = !terminating;
        if (flag1)
        {
          bool flag2;
          switch (component)
          {
            case TransformComponent _:
            case MetaDataComponent _:
              flag2 = true;
              break;
            default:
              flag2 = false;
              break;
          }
          flag1 = flag2;
        }
        if (flag1)
          return;
        if (component.Running)
          this.LifeShutdown<IComponent>(uid, component, idx);
        if (component.LifeStage != ComponentLifeStage.PreAdd)
          this.LifeRemoveFromEntity<IComponent>(uid, component, idx);
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Caught exception during immediate component removal. Entity={this.ToPrettyString((Entity<MetaDataComponent>) component.Owner)}, type={component.GetType()}");
        this._runtimeLog.LogException(ex, nameof (RemoveComponentImmediate));
      }
      this.DeleteComponent(uid, component, idx, terminating, meta);
    }
  }

  public void CullRemovedComponents()
  {
    foreach (IComponent component1 in EntityManager.InSafeOrder((IEnumerable<IComponent>) this._deleteSet))
    {
      if (!component1.Deleted)
      {
        EntityUid owner = component1.Owner;
        CompIdx index = this._componentFactory.GetIndex(component1.GetType());
        try
        {
          if (component1.Running)
          {
            this._sawmill.Warning($"Found a running component while culling deferred deletions, owner={this.ToPrettyString((Entity<MetaDataComponent>) owner)}, type={component1.GetType()}");
            this.LifeShutdown<IComponent>(owner, component1, index);
          }
          if (component1.LifeStage != ComponentLifeStage.PreAdd)
            this.LifeRemoveFromEntity<IComponent>(owner, component1, index);
        }
        catch (Exception ex)
        {
          this._sawmill.Error($"Caught exception  while processing deferred component removal. Entity={this.ToPrettyString((Entity<MetaDataComponent>) component1.Owner)}, type={component1.GetType()}");
          this._runtimeLog.LogException(ex, nameof (CullRemovedComponents));
        }
        MetaDataComponent component2 = this.MetaQuery.GetComponent(owner);
        this.DeleteComponent(owner, component1, index, false, component2);
      }
    }
    this._deleteSet.Clear();
  }

  private void DeleteComponent(
    EntityUid entityUid,
    IComponent component,
    CompIdx idx,
    bool terminating,
    MetaDataComponent? metadata)
  {
    if (!this.MetaQuery.ResolveInternal(entityUid, ref metadata))
      return;
    RemovedComponentEventArgs e = new RemovedComponentEventArgs(new ComponentEventArgs(component, entityUid), false, metadata, idx);
    Action<RemovedComponentEventArgs> componentRemoved = this.ComponentRemoved;
    if (componentRemoved != null)
      componentRemoved(e);
    this.EventBusInternal.OnComponentRemoved(in e);
    if (!terminating)
    {
      ComponentRegistration registration = this._componentFactory.GetRegistration(component);
      ushort? netId = registration.NetID;
      if (netId.HasValue)
      {
        Dictionary<ushort, IComponent> netComponents = metadata.NetComponents;
        netId = registration.NetID;
        int key = (int) netId.Value;
        if (!netComponents.Remove((ushort) key))
          this._sawmill.Error($"Entity {this.ToPrettyString(entityUid, metadata)} did not have {component.GetType().Name} in its networked component dictionary during component deletion.");
        if (component.NetSyncEnabled)
        {
          this.DirtyEntity(entityUid, metadata);
          metadata.LastComponentRemoved = this._gameTiming.CurTick;
        }
      }
    }
    this._entTraitArray[idx.Value].Remove(entityUid);
    this._entCompIndex.Remove(entityUid, component);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent<T>(EntityUid uid) where T : IComponent
  {
    IComponent component;
    return this._entTraitArray[CompIdx.ArrayIndex<T>()].TryGetValue(uid, out component) && !component.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent<T>([NotNullWhen(true)] EntityUid? uid) where T : IComponent
  {
    return uid.HasValue && this.HasComponent<T>(uid.Value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent(EntityUid uid, ComponentRegistration reg)
  {
    IComponent component;
    return this._entTraitArray[reg.Idx.Value].TryGetValue(uid, out component) && !component.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent(EntityUid uid, Type type)
  {
    IComponent component;
    return this._entTraitDict[type].TryGetValue(uid, out component) && !component.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent([NotNullWhen(true)] EntityUid? uid, Type type)
  {
    IComponent component;
    return uid.HasValue && this._entTraitDict[type].TryGetValue(uid.Value, out component) && !component.Deleted;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
  {
    return this.MetaQuery.Resolve(uid, ref meta) && meta.NetComponents.ContainsKey(netId);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasComponent([NotNullWhen(true)] EntityUid? uid, ushort netId, MetaDataComponent? meta = null)
  {
    return uid.HasValue && this.HasComponent(uid.Value, netId, meta);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T EnsureComponent<T>(EntityUid uid) where T : IComponent, new()
  {
    T component;
    if (this.TryGetComponent<T>(uid, out component))
    {
      if (component.LifeStage <= ComponentLifeStage.Running)
        return component;
      this.RemoveComponent(uid, (IComponent) component, (MetaDataComponent) null);
    }
    return this.AddComponent<T>(uid);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool EnsureComponent<T>(ref Entity<T?> entity) where T : IComponent, new()
  {
    if ((object) entity.Comp != null)
    {
      if (entity.Comp.LifeStage <= ComponentLifeStage.Running)
        return true;
      this.RemoveComponent((EntityUid) entity, (IComponent) entity.Comp, (MetaDataComponent) null);
    }
    entity.Comp = this.AddComponent<T>((EntityUid) entity);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool EnsureComponent<T>(EntityUid entity, out T component) where T : IComponent, new()
  {
    T component1;
    if (this.TryGetComponent<T>(entity, out component1))
    {
      if (component1.LifeStage <= ComponentLifeStage.Running)
      {
        component = component1;
        return true;
      }
      this.RemoveComponent(entity, (IComponent) component1, (MetaDataComponent) null);
    }
    component = this.AddComponent<T>(entity);
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T GetComponent<T>(EntityUid uid) where T : IComponent
  {
    IComponent component;
    if (this._entTraitArray[CompIdx.ArrayIndex<T>()].TryGetValue(uid, out component) && !component.Deleted)
      return (T) component;
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {typeof (T)}");
  }

  public IComponent GetComponent(EntityUid uid, CompIdx type)
  {
    IComponent component;
    if (this._entTraitArray[type.Value].TryGetValue(uid, out component) && !component.Deleted)
      return component;
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {this._componentFactory.IdxToType(type)}");
  }

  public IComponent GetComponent(EntityUid uid, Type type)
  {
    IComponent component;
    if (this._entTraitDict[type].TryGetValue(uid, out component) && !component.Deleted)
      return component;
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {type}");
  }

  public IComponent GetComponent(EntityUid uid, ushort netId, MetaDataComponent? meta = null)
  {
    return (meta ?? this.MetaQuery.GetComponentInternal(uid)).NetComponents[netId];
  }

  public IComponent GetComponentInternal(EntityUid uid, CompIdx type)
  {
    IComponent componentInternal;
    if (this._entTraitArray[type.Value].TryGetValue(uid, out componentInternal))
      return componentInternal;
    throw new KeyNotFoundException($"Entity {uid} does not have a component of type {type}");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool TryGetComponent<T>(EntityUid uid, [NotNullWhen(true)] out T? component) where T : IComponent?
  {
    IComponent component1;
    if (this._entTraitArray[CompIdx.ArrayIndex<T>()].TryGetValue(uid, out component1) && !component1.Deleted)
    {
      component = (T) component1;
      return true;
    }
    component = default (T);
    return false;
  }

  public bool TryGetComponent<T>([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out T? component) where T : IComponent?
  {
    if (!uid.HasValue)
    {
      component = default (T);
      return false;
    }
    IComponent component1;
    if (this.TryGetComponent(uid.Value, typeof (T), out component1) && !component1.Deleted)
    {
      component = (T) component1;
      return true;
    }
    component = default (T);
    return false;
  }

  public bool TryGetComponent(EntityUid uid, ComponentRegistration reg, [NotNullWhen(true)] out IComponent? component)
  {
    IComponent component1;
    if (this._entTraitArray[reg.Idx.Value].TryGetValue(uid, out component1) && !component1.Deleted)
    {
      component = component1;
      return true;
    }
    component = (IComponent) null;
    return false;
  }

  public bool TryGetComponent(EntityUid uid, Type type, [NotNullWhen(true)] out IComponent? component)
  {
    IComponent component1;
    if (this._entTraitDict[type].TryGetValue(uid, out component1) && !component1.Deleted)
    {
      component = component1;
      return true;
    }
    component = (IComponent) null;
    return false;
  }

  public bool TryGetComponent(EntityUid uid, CompIdx type, [NotNullWhen(true)] out IComponent? component)
  {
    IComponent component1;
    if (this._entTraitArray[type.Value].TryGetValue(uid, out component1) && !component1.Deleted)
    {
      component = component1;
      return true;
    }
    component = (IComponent) null;
    return false;
  }

  public bool TryGetComponent([NotNullWhen(true)] EntityUid? uid, Type type, [NotNullWhen(true)] out IComponent? component)
  {
    if (!uid.HasValue)
    {
      component = (IComponent) null;
      return false;
    }
    IComponent component1;
    if (this._entTraitDict[type].TryGetValue(uid.Value, out component1) && !component1.Deleted)
    {
      component = component1;
      return true;
    }
    component = (IComponent) null;
    return false;
  }

  public bool TryGetComponent(
    EntityUid uid,
    ushort netId,
    [MaybeNullWhen(false)] out IComponent component,
    MetaDataComponent? meta = null)
  {
    MetaDataComponent component1;
    IComponent component2;
    if (this.MetaQuery.TryGetComponentInternal(uid, out component1) && component1.NetComponents.TryGetValue(netId, out component2))
    {
      component = component2;
      return true;
    }
    component = (IComponent) null;
    return false;
  }

  public bool TryGetComponent(
    [NotNullWhen(true)] EntityUid? uid,
    ushort netId,
    [MaybeNullWhen(false)] out IComponent component,
    MetaDataComponent? meta = null)
  {
    if (uid.HasValue)
      return this.TryGetComponent(uid.Value, netId, out component, meta);
    component = (IComponent) null;
    return false;
  }

  public bool TryCopyComponent<T>(
    EntityUid source,
    EntityUid target,
    ref T? sourceComponent,
    [NotNullWhen(true)] out T? targetComp,
    MetaDataComponent? meta = null)
    where T : IComponent
  {
    if (!this.MetaQuery.Resolve(target, ref meta))
    {
      targetComp = default (T);
      return false;
    }
    if ((object) sourceComponent == null && !this.TryGetComponent<T>(source, out sourceComponent))
    {
      targetComp = default (T);
      return false;
    }
    targetComp = this.CopyComponentInternal<T>(source, target, sourceComponent, meta);
    return true;
  }

  public bool TryCopyComponents(
    EntityUid source,
    EntityUid target,
    MetaDataComponent? meta = null,
    params Type[] sourceComponents)
  {
    if (!this.MetaQuery.TryGetComponent(target, out meta))
      return false;
    bool flag = true;
    foreach (Type sourceComponent in sourceComponents)
    {
      IComponent component;
      if (!this.TryGetComponent(source, sourceComponent, out component))
        flag = false;
      else
        this.CopyComponent(source, target, component, meta);
    }
    return flag;
  }

  public IComponent CopyComponent(
    EntityUid source,
    EntityUid target,
    IComponent sourceComponent,
    MetaDataComponent? meta = null)
  {
    if (!this.MetaQuery.Resolve(target, ref meta))
      throw new InvalidOperationException();
    return this.CopyComponentInternal<IComponent>(source, target, sourceComponent, meta);
  }

  public T CopyComponent<T>(
    EntityUid source,
    EntityUid target,
    T sourceComponent,
    MetaDataComponent? meta = null)
    where T : IComponent
  {
    if (!this.MetaQuery.Resolve(target, ref meta))
      throw new InvalidOperationException();
    return this.CopyComponentInternal<T>(source, target, sourceComponent, meta);
  }

  public void CopyComponents(
    EntityUid source,
    EntityUid target,
    MetaDataComponent? meta = null,
    params IComponent[] sourceComponents)
  {
    if (!this.MetaQuery.Resolve(target, ref meta))
      return;
    foreach (IComponent sourceComponent in sourceComponents)
      this.CopyComponentInternal<IComponent>(source, target, sourceComponent, meta);
  }

  private T CopyComponentInternal<T>(
    EntityUid source,
    EntityUid target,
    T sourceComponent,
    MetaDataComponent meta)
    where T : IComponent
  {
    ComponentRegistration registration = this.ComponentFactory.GetRegistration(sourceComponent.GetType());
    T component = (T) this.ComponentFactory.GetComponent(registration);
    this._serManager.CopyTo<T>(sourceComponent, ref component, notNullableOverride: true);
    component.Owner = target;
    this.AddComponentInternal<T>(target, component, registration, true, false, meta);
    return component;
  }

  public Robust.Shared.GameObjects.EntityQuery<TComp1> GetEntityQuery<TComp1>() where TComp1 : IComponent
  {
    return new Robust.Shared.GameObjects.EntityQuery<TComp1>(this, this._entTraitArray[CompIdx.ArrayIndex<TComp1>()]);
  }

  public Robust.Shared.GameObjects.EntityQuery<IComponent> GetEntityQuery(Type type)
  {
    return new Robust.Shared.GameObjects.EntityQuery<IComponent>(this, this._entTraitDict[type]);
  }

  public IEnumerable<IComponent> GetComponents(EntityUid uid)
  {
    IComponent[] componentArray = this._entCompIndex[uid].ToArray<IComponent>();
    for (int index = 0; index < componentArray.Length; ++index)
    {
      IComponent component = componentArray[index];
      if (!component.Deleted)
        yield return component;
    }
    componentArray = (IComponent[]) null;
  }

  internal IReadOnlyCollection<IComponent> GetComponentsInternal(EntityUid uid)
  {
    return (IReadOnlyCollection<IComponent>) this._entCompIndex[uid];
  }

  public int ComponentCount(EntityUid uid) => this._entCompIndex[uid].Count;

  private void CopyComponentsInto(ref Span<IComponent?> comps, EntityUid uid)
  {
    HashSet<IComponent> componentSet = this._entCompIndex[uid];
    if (componentSet.Count > comps.Length)
      comps = (Span<IComponent>) new IComponent[componentSet.Count];
    int num = 0;
    foreach (IComponent component in componentSet)
      comps[num++] = component;
  }

  public IEnumerable<T> GetComponents<T>(EntityUid uid)
  {
    IComponent[] componentArray = this._entCompIndex[uid].ToArray<IComponent>();
    for (int index = 0; index < componentArray.Length; ++index)
    {
      IComponent component1 = componentArray[index];
      if (!component1.Deleted && component1 is T component2)
        yield return component2;
    }
    componentArray = (IComponent[]) null;
  }

  public NetComponentEnumerable GetNetComponents(EntityUid uid, MetaDataComponent? meta = null)
  {
    if (meta == null)
      meta = this.MetaQuery.GetComponentInternal(uid);
    return new NetComponentEnumerable(meta.NetComponents);
  }

  public NetComponentEnumerable? GetNetComponentsOrNull(EntityUid uid, MetaDataComponent? meta = null)
  {
    return !this.MetaQuery.Resolve(uid, ref meta) ? new NetComponentEnumerable?() : new NetComponentEnumerable?(new NetComponentEnumerable(meta.NetComponents));
  }

  public (EntityUid Uid, T Component)[] AllComponents<T>() where T : IComponent
  {
    Robust.Shared.GameObjects.AllEntityQueryEnumerator<T> entityQueryEnumerator = this.AllEntityQueryEnumerator<T>();
    (EntityUid, T)[] array = new (EntityUid, T)[this.Count<T>()];
    int newSize = 0;
    EntityUid uid;
    T comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      array[newSize] = (uid, comp1);
      ++newSize;
    }
    Array.Resize<(EntityUid, T)>(ref array, newSize);
    return array;
  }

  public Entity<T>[] AllEntities<T>() where T : IComponent
  {
    Robust.Shared.GameObjects.AllEntityQueryEnumerator<T> entityQueryEnumerator = this.AllEntityQueryEnumerator<T>();
    Entity<T>[] array = new Entity<T>[this.Count<T>()];
    int newSize = 0;
    EntityUid uid;
    T comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      array[newSize++] = (Entity<T>) (uid, comp1);
    Array.Resize<Entity<T>>(ref array, newSize);
    return array;
  }

  public Entity<IComponent>[] AllEntities(Type tComp)
  {
    Robust.Shared.GameObjects.AllEntityQueryEnumerator<IComponent> entityQueryEnumerator = this.AllEntityQueryEnumerator(tComp);
    Entity<IComponent>[] array = new Entity<IComponent>[this.Count(tComp)];
    int newSize = 0;
    EntityUid uid;
    IComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      array[newSize++] = (Entity<IComponent>) (uid, comp1);
    Array.Resize<Entity<IComponent>>(ref array, newSize);
    return array;
  }

  public EntityUid[] AllEntityUids<T>() where T : IComponent
  {
    Robust.Shared.GameObjects.AllEntityQueryEnumerator<T> entityQueryEnumerator = this.AllEntityQueryEnumerator<T>();
    EntityUid[] array = new EntityUid[this.Count<T>()];
    int newSize = 0;
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out T _))
      array[newSize++] = uid;
    Array.Resize<EntityUid>(ref array, newSize);
    return array;
  }

  public EntityUid[] AllEntityUids(Type tComp)
  {
    Robust.Shared.GameObjects.AllEntityQueryEnumerator<IComponent> entityQueryEnumerator = this.AllEntityQueryEnumerator(tComp);
    EntityUid[] array = new EntityUid[this.Count(tComp)];
    int newSize = 0;
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out IComponent _))
      array[newSize++] = uid;
    Array.Resize<EntityUid>(ref array, newSize);
    return array;
  }

  public List<(EntityUid Uid, T Component)> AllComponentsList<T>() where T : IComponent
  {
    Robust.Shared.GameObjects.AllEntityQueryEnumerator<T> entityQueryEnumerator = this.AllEntityQueryEnumerator<T>();
    List<(EntityUid, T)> valueTupleList = new List<(EntityUid, T)>(this.Count<T>());
    EntityUid uid;
    T comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      valueTupleList.Add((uid, comp1));
    return valueTupleList;
  }

  public Robust.Shared.GameObjects.ComponentQueryEnumerator ComponentQueryEnumerator(
    ComponentRegistry registry)
  {
    return registry.Count == 0 ? new Robust.Shared.GameObjects.ComponentQueryEnumerator(new Dictionary<EntityUid, IComponent>()) : new Robust.Shared.GameObjects.ComponentQueryEnumerator(this._entTraitArray[this._componentFactory.GetArrayIndex(registry.First<KeyValuePair<string, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry>>().Value.Component.GetType())]);
  }

  public CompRegistryEntityEnumerator CompRegistryQueryEnumerator(ComponentRegistry registry)
  {
    return registry.Count == 0 ? new CompRegistryEntityEnumerator((IEntityManager) this, new Dictionary<EntityUid, IComponent>(), registry) : new CompRegistryEntityEnumerator((IEntityManager) this, this._entTraitArray[this._componentFactory.GetArrayIndex(registry.First<KeyValuePair<string, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry>>().Value.Component.GetType())], registry);
  }

  public Robust.Shared.GameObjects.AllEntityQueryEnumerator<IComponent> AllEntityQueryEnumerator(
    Type comp)
  {
    return new Robust.Shared.GameObjects.AllEntityQueryEnumerator<IComponent>(this._entTraitArray[this._componentFactory.GetIndex(comp).Value]);
  }

  public Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1> AllEntityQueryEnumerator<TComp1>() where TComp1 : IComponent
  {
    return new Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1>(this._entTraitArray[CompIdx.ArrayIndex<TComp1>()]);
  }

  public Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2> AllEntityQueryEnumerator<TComp1, TComp2>()
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    return new Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2>(this._entTraitArray[CompIdx.ArrayIndex<TComp1>()], this._entTraitArray[CompIdx.ArrayIndex<TComp2>()]);
  }

  public Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2, TComp3> AllEntityQueryEnumerator<TComp1, TComp2, TComp3>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait1 = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> entTrait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    Dictionary<EntityUid, IComponent> entTrait3 = this._entTraitArray[CompIdx.ArrayIndex<TComp3>()];
    Dictionary<EntityUid, IComponent> traitDict2 = entTrait2;
    Dictionary<EntityUid, IComponent> traitDict3 = entTrait3;
    return new Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2, TComp3>(entTrait1, traitDict2, traitDict3);
  }

  public Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait1 = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> entTrait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    Dictionary<EntityUid, IComponent> entTrait3 = this._entTraitArray[CompIdx.ArrayIndex<TComp3>()];
    Dictionary<EntityUid, IComponent> entTrait4 = this._entTraitArray[CompIdx.ArrayIndex<TComp4>()];
    Dictionary<EntityUid, IComponent> traitDict2 = entTrait2;
    Dictionary<EntityUid, IComponent> traitDict3 = entTrait3;
    Dictionary<EntityUid, IComponent> traitDict4 = entTrait4;
    return new Robust.Shared.GameObjects.AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>(entTrait1, traitDict2, traitDict3, traitDict4);
  }

  public Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1> EntityQueryEnumerator<TComp1>() where TComp1 : IComponent
  {
    return new Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1>(this._entTraitArray[CompIdx.ArrayIndex<TComp1>()], this.MetaQuery);
  }

  public Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2> EntityQueryEnumerator<TComp1, TComp2>()
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    return new Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2>(this._entTraitArray[CompIdx.ArrayIndex<TComp1>()], this._entTraitArray[CompIdx.ArrayIndex<TComp2>()], this.MetaQuery);
  }

  public Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3> EntityQueryEnumerator<TComp1, TComp2, TComp3>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait1 = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> entTrait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    Dictionary<EntityUid, IComponent> entTrait3 = this._entTraitArray[CompIdx.ArrayIndex<TComp3>()];
    Dictionary<EntityUid, IComponent> traitDict2 = entTrait2;
    Dictionary<EntityUid, IComponent> traitDict3 = entTrait3;
    Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> metaQuery = this.MetaQuery;
    return new Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3>(entTrait1, traitDict2, traitDict3, metaQuery);
  }

  public Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4> EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>()
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait1 = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> entTrait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    Dictionary<EntityUid, IComponent> entTrait3 = this._entTraitArray[CompIdx.ArrayIndex<TComp3>()];
    Dictionary<EntityUid, IComponent> entTrait4 = this._entTraitArray[CompIdx.ArrayIndex<TComp4>()];
    Dictionary<EntityUid, IComponent> traitDict2 = entTrait2;
    Dictionary<EntityUid, IComponent> traitDict3 = entTrait3;
    Dictionary<EntityUid, IComponent> traitDict4 = entTrait4;
    Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> metaQuery = this.MetaQuery;
    return new Robust.Shared.GameObjects.EntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>(entTrait1, traitDict2, traitDict3, traitDict4, metaQuery);
  }

  public IEnumerable<T> EntityQuery<T>(bool includePaused = false) where T : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait = this._entTraitArray[CompIdx.ArrayIndex<T>()];
    if (includePaused)
    {
      foreach (IComponent component in entTrait.Values)
      {
        if (!component.Deleted)
          yield return (T) component;
      }
    }
    else
    {
      foreach ((EntityUid entityUid, IComponent component1) in entTrait)
      {
        MetaDataComponent component2;
        if (!component1.Deleted && this.MetaQuery.TryGetComponentInternal(entityUid, out component2) && !component2.EntityPaused)
          yield return (T) component1;
      }
    }
  }

  public IEnumerable<(TComp1, TComp2)> EntityQuery<TComp1, TComp2>(bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> trait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    if (includePaused)
    {
      foreach ((EntityUid key, IComponent component1) in entTrait)
      {
        IComponent component2;
        if (trait2.TryGetValue(key, out component2) && !component2.Deleted)
          yield return ((TComp1) component1, (TComp2) component2);
      }
    }
    else
    {
      Dictionary<EntityUid, IComponent> metaComps = this._entTraitArray[CompIdx.ArrayIndex<MetaDataComponent>()];
      foreach ((EntityUid key, IComponent component3) in entTrait)
      {
        IComponent component4;
        IComponent component5;
        if (trait2.TryGetValue(key, out component4) && !component4.Deleted && !component3.Deleted && metaComps.TryGetValue(key, out component5) && !((MetaDataComponent) component5).EntityPaused)
          yield return ((TComp1) component3, (TComp2) component4);
      }
      metaComps = (Dictionary<EntityUid, IComponent>) null;
    }
  }

  public IEnumerable<(TComp1, TComp2, TComp3)> EntityQuery<TComp1, TComp2, TComp3>(
    bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> trait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    Dictionary<EntityUid, IComponent> trait3 = this._entTraitArray[CompIdx.ArrayIndex<TComp3>()];
    if (includePaused)
    {
      foreach ((EntityUid key, IComponent component1) in entTrait)
      {
        IComponent component2;
        IComponent component3;
        if (trait2.TryGetValue(key, out component2) && !component2.Deleted && trait3.TryGetValue(key, out component3) && !component3.Deleted)
          yield return ((TComp1) component1, (TComp2) component2, (TComp3) component3);
      }
    }
    else
    {
      Dictionary<EntityUid, IComponent> metaComps = this._entTraitArray[CompIdx.ArrayIndex<MetaDataComponent>()];
      foreach ((EntityUid key, IComponent component4) in entTrait)
      {
        IComponent component5;
        IComponent component6;
        IComponent component7;
        if (trait2.TryGetValue(key, out component5) && !component5.Deleted && trait3.TryGetValue(key, out component6) && !component6.Deleted && !component4.Deleted && metaComps.TryGetValue(key, out component7) && !((MetaDataComponent) component7).EntityPaused)
          yield return ((TComp1) component4, (TComp2) component5, (TComp3) component6);
      }
      metaComps = (Dictionary<EntityUid, IComponent>) null;
    }
  }

  public IEnumerable<(TComp1, TComp2, TComp3, TComp4)> EntityQuery<TComp1, TComp2, TComp3, TComp4>(
    bool includePaused = false)
    where TComp1 : IComponent
    where TComp2 : IComponent
    where TComp3 : IComponent
    where TComp4 : IComponent
  {
    Dictionary<EntityUid, IComponent> entTrait = this._entTraitArray[CompIdx.ArrayIndex<TComp1>()];
    Dictionary<EntityUid, IComponent> trait2 = this._entTraitArray[CompIdx.ArrayIndex<TComp2>()];
    Dictionary<EntityUid, IComponent> trait3 = this._entTraitArray[CompIdx.ArrayIndex<TComp3>()];
    Dictionary<EntityUid, IComponent> trait4 = this._entTraitArray[CompIdx.ArrayIndex<TComp4>()];
    if (includePaused)
    {
      foreach ((EntityUid key, IComponent component1) in entTrait)
      {
        IComponent component2;
        IComponent component3;
        IComponent component4;
        if (trait2.TryGetValue(key, out component2) && !component2.Deleted && trait3.TryGetValue(key, out component3) && !component3.Deleted && trait4.TryGetValue(key, out component4) && !component4.Deleted)
          yield return ((TComp1) component1, (TComp2) component2, (TComp3) component3, (TComp4) component4);
      }
    }
    else
    {
      Dictionary<EntityUid, IComponent> metaComps = this._entTraitArray[CompIdx.ArrayIndex<MetaDataComponent>()];
      foreach ((EntityUid key, IComponent component5) in entTrait)
      {
        IComponent component6;
        IComponent component7;
        IComponent component8;
        IComponent component9;
        if (trait2.TryGetValue(key, out component6) && !component6.Deleted && trait3.TryGetValue(key, out component7) && !component7.Deleted && trait4.TryGetValue(key, out component8) && !component8.Deleted && !component5.Deleted && metaComps.TryGetValue(key, out component9) && !((MetaDataComponent) component9).EntityPaused)
          yield return ((TComp1) component5, (TComp2) component6, (TComp3) component7, (TComp4) component8);
      }
      metaComps = (Dictionary<EntityUid, IComponent>) null;
    }
  }

  public IEnumerable<(EntityUid Uid, IComponent Component)> GetAllComponents(
    Type type,
    bool includePaused = false)
  {
    Dictionary<EntityUid, IComponent> dictionary = this._entTraitDict[type];
    if (includePaused)
    {
      foreach ((EntityUid key, IComponent component) in dictionary)
      {
        if (!component.Deleted)
          yield return (key, component);
      }
    }
    else
    {
      foreach ((EntityUid entityUid, IComponent component1) in dictionary)
      {
        MetaDataComponent component2;
        if (!component1.Deleted && this.MetaQuery.TryGetComponent(entityUid, out component2) && !component2.EntityPaused)
          yield return (entityUid, component1);
      }
    }
  }

  public IComponentState? GetComponentState(
    IEventBus eventBus,
    IComponent component,
    ICommonSession? session,
    GameTick fromTick)
  {
    ComponentGetState args = new ComponentGetState(session, fromTick);
    eventBus.RaiseComponentEvent<ComponentGetState>(component.Owner, component, ref args);
    return args.State;
  }

  public bool CanGetComponentState(IEventBus eventBus, IComponent component, ICommonSession player)
  {
    return this.CanGetComponentState(component, player);
  }

  public bool CanGetComponentState(IComponent component, ICommonSession player)
  {
    ComponentGetStateAttemptEvent args = new ComponentGetStateAttemptEvent(player);
    this.EventBusInternal.RaiseComponentEvent<ComponentGetStateAttemptEvent>(component.Owner, component, ref args);
    return !args.Cancelled;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void FillComponentDict()
  {
    this._entTraitDict = FrozenDictionary<Type, Dictionary<EntityUid, IComponent>>.Empty;
    Array.Fill<Dictionary<EntityUid, IComponent>>(this._entTraitArray, (Dictionary<EntityUid, IComponent>) null);
    this.RegisterComponents(this._componentFactory.GetAllRegistrations());
  }

  public GameTick CurrentTick => this._gameTiming.CurTick;

  IComponentFactory IEntityManager.ComponentFactory => this.ComponentFactory;

  public IEntitySystemManager EntitySysManager => this._entitySystemManager;

  public abstract IEntityNetworkManager EntityNetManager { get; }

  public IEventBus EventBus => (IEventBus) this.EventBusInternal;

  public event Action<Entity<MetaDataComponent>>? EntityAdded;

  public event Action<Entity<MetaDataComponent>>? EntityInitialized;

  public event Action<Entity<MetaDataComponent>>? EntityDeleted;

  internal event EntityManager.TerminatingEventHandler? BeforeEntityTerminating;

  public event Action? BeforeEntityFlush;

  public event Action? AfterEntityFlush;

  public event Action<EntityUid>? EntityQueueDeleted;

  public event Action<Entity<MetaDataComponent>>? EntityDirtied;

  public bool Started { get; protected set; }

  public bool ShuttingDown { get; protected set; }

  public bool Initialized { get; protected set; }

  public virtual void Initialize()
  {
    if (this.Initialized)
      throw new InvalidOperationException("Initialize() called multiple times");
    this.EventBusInternal = new EntityEventBus(this, this._reflection);
    this.InitializeComponents();
    this._metaReg = this._componentFactory.GetRegistration(typeof (MetaDataComponent));
    this._xformReg = this._componentFactory.GetRegistration(typeof (TransformComponent));
    this._xformName = this._xformReg.Name;
    this._sawmill = this.LogManager.GetSawmill("entity");
    this.ResolveSawmill = this.LogManager.GetSawmill("resolve");
    this.Initialized = true;
  }

  public bool IsDefault(EntityUid uid, ICollection<string>? ignoredComps = null)
  {
    MetaDataComponent component1;
    if (!this.MetaQuery.TryGetComponent(uid, out component1) || component1.EntityPrototype == null)
      return false;
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = component1.EntityPrototype;
    if (component1.EntityName != entityPrototype.Name || component1.EntityDescription != entityPrototype.Description)
      return false;
    IReadOnlyDictionary<string, MappingDataNode> prototypeData = this.PrototypeManager.GetPrototypeData(entityPrototype);
    HashSet<IComponent> componentSet = this._entCompIndex[uid];
    if (prototypeData.Count + 2 != componentSet.Count)
      return false;
    foreach (IComponent component2 in componentSet)
    {
      if (component2.Deleted)
        return false;
      Type type = component2.GetType();
      if (!(type == typeof (TransformComponent)) && !(type == typeof (MetaDataComponent)))
      {
        string componentName = this._componentFactory.GetComponentName(type);
        if (ignoredComps == null || !ignoredComps.Contains(componentName))
        {
          MappingDataNode node;
          if (!prototypeData.TryGetValue(componentName, out node))
            return false;
          MappingDataNode mappingDataNode;
          try
          {
            mappingDataNode = this._serManager.WriteValueAs<MappingDataNode>(type, (object) component2, true, (ISerializationContext) this._context);
          }
          catch (Exception ex)
          {
            this._sawmill.Error($"Failed to serialize {componentName} component of entity prototype {entityPrototype.ID}. Exception: {ex.Message}");
            return false;
          }
          if (mappingDataNode.AnyExcept(node))
            return false;
        }
      }
    }
    return true;
  }

  public virtual void Startup()
  {
    if (!this.Initialized)
      throw new InvalidOperationException("Startup() called without Initialized");
    if (this.Started)
      throw new InvalidOperationException("Startup() called multiple times");
    this._entitySystemManager.Initialize();
    this.Started = true;
    this.EventBusInternal.LockSubscriptions();
    this._mapSystem = this.System<SharedMapSystem>();
    this._xforms = this.System<SharedTransformSystem>();
    this._containers = this.System<SharedContainerSystem>();
    this.MetaQuery = this.GetEntityQuery<MetaDataComponent>();
    this.TransformQuery = this.GetEntityQuery<TransformComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._actorQuery = this.GetEntityQuery<ActorComponent>();
    this._entityConsoleHost.Startup();
  }

  public virtual void Shutdown()
  {
    this.ShuttingDown = true;
    this.FlushEntities();
    this.EventBusInternal.ClearSubscriptions();
    this._entitySystemManager.Shutdown();
    this.ClearComponents();
    this.ShuttingDown = false;
    this.Started = false;
    this._entityConsoleHost.Shutdown();
  }

  public virtual void Cleanup()
  {
    this._componentFactory.ComponentsAdded -= new Action<ComponentRegistration[]>(this.OnComponentsAdded);
    this.ShuttingDown = true;
    this.FlushEntities();
    this._entitySystemManager.Clear();
    this.EventBusInternal.Dispose();
    this.EventBusInternal = (EntityEventBus) null;
    this.ClearComponents();
    this.ShuttingDown = false;
    this.Initialized = false;
    this.Started = false;
  }

  public virtual void TickUpdate(float frameTime, bool noPredictions, Histogram? histogram)
  {
    ITimer itimer1;
    if (histogram == null)
      itimer1 = (ITimer) null;
    else
      itimer1 = TimerExtensions.NewTimer((IObserver) ((Collector<Histogram.Child>) histogram).WithLabels(new string[1]
      {
        "EntitySystems"
      }));
    using (itimer1)
    {
      using (this._prof.Group("Systems"))
        this._entitySystemManager.TickUpdate(frameTime, noPredictions);
    }
    ITimer itimer2;
    if (histogram == null)
      itimer2 = (ITimer) null;
    else
      itimer2 = TimerExtensions.NewTimer((IObserver) ((Collector<Histogram.Child>) histogram).WithLabels(new string[1]
      {
        "EntityEventBus"
      }));
    using (itimer2)
    {
      using (this._prof.Group("Events"))
        this.EventBusInternal.ProcessEventQueue();
    }
    ITimer itimer3;
    if (histogram == null)
      itimer3 = (ITimer) null;
    else
      itimer3 = TimerExtensions.NewTimer((IObserver) ((Collector<Histogram.Child>) histogram).WithLabels(new string[1]
      {
        "QueuedDeletion"
      }));
    using (itimer3)
    {
      using (this._prof.Group("QueueDel"))
        this.ProcessQueueudDeletions();
    }
    ITimer itimer4;
    if (histogram == null)
      itimer4 = (ITimer) null;
    else
      itimer4 = TimerExtensions.NewTimer((IObserver) ((Collector<Histogram.Child>) histogram).WithLabels(new string[1]
      {
        "ComponentCull"
      }));
    using (itimer4)
    {
      using (this._prof.Group("ComponentCull"))
        this.CullRemovedComponents();
    }
  }

  internal virtual void ProcessQueueudDeletions()
  {
    EntityUid result;
    while (this.QueuedDeletions.TryDequeue(out result))
      this.DeleteEntity(new EntityUid?(result));
    this.QueuedDeletionsSet.Clear();
  }

  public virtual void FrameUpdate(float frameTime)
  {
    this._entitySystemManager.FrameUpdate(frameTime);
  }

  public EntityUid CreateEntityUninitialized(
    string? prototypeName,
    EntityUid euid,
    ComponentRegistry? overrides = null)
  {
    return this.CreateEntity(prototypeName, out MetaDataComponent _, (IEntityLoadContext) overrides);
  }

  public EntityUid CreateEntityUninitialized(string? prototypeName, ComponentRegistry? overrides = null)
  {
    return this.CreateEntity(prototypeName, out MetaDataComponent _, (IEntityLoadContext) overrides);
  }

  public EntityUid CreateEntityUninitialized(
    string? prototypeName,
    out MetaDataComponent meta,
    ComponentRegistry? overrides = null)
  {
    return this.CreateEntity(prototypeName, out meta, (IEntityLoadContext) overrides);
  }

  public virtual EntityUid CreateEntityUninitialized(
    string? prototypeName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    EntityUid entity = this.CreateEntity(prototypeName, out MetaDataComponent _, (IEntityLoadContext) overrides);
    TransformComponent component = this.TransformQuery.GetComponent(entity);
    this._xforms.SetCoordinates(entity, component, coordinates, new Angle?(rotation), false);
    return entity;
  }

  public virtual EntityUid CreateEntityUninitialized(
    string? prototypeName,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    EntityUid entity = this.CreateEntity(prototypeName, out MetaDataComponent _, (IEntityLoadContext) overrides);
    TransformComponent component1 = this.TransformQuery.GetComponent(entity);
    if (coordinates.MapId == MapId.Nullspace)
    {
      component1._parent = EntityUid.Invalid;
      component1.Anchored = false;
      return entity;
    }
    EntityUid map = this._mapSystem.GetMap(coordinates.MapId);
    TransformComponent component2;
    if (!this.TryGetComponent<TransformComponent>(map, out component2))
      throw new ArgumentException($"Attempted to spawn entity on an invalid map. Coordinates: {coordinates}");
    EntityUid uid;
    MapGridComponent grid;
    MetaDataComponent component3;
    if (this._mapManager.TryFindGridAt(coordinates, out uid, out grid) && this.MetaQuery.TryGetComponentInternal(uid, out component3) && component3.EntityLifeStage < EntityLifeStage.Terminating)
    {
      EntityCoordinates entityCoordinates = new EntityCoordinates(uid, this._mapSystem.WorldToLocal(uid, grid, coordinates.Position));
      Angle angle = Angle.op_Subtraction(rotation, this._xforms.GetWorldRotation(uid));
      this._xforms.SetCoordinates(entity, component1, entityCoordinates, new Angle?(angle), false);
    }
    else
    {
      EntityCoordinates entityCoordinates = new EntityCoordinates(map, coordinates.Position);
      this._xforms.SetCoordinates(entity, component1, entityCoordinates, new Angle?(rotation), newParent: component2);
    }
    return entity;
  }

  public int EntityCount => this.Entities.Count;

  public IEnumerable<EntityUid> GetEntities() => (IEnumerable<EntityUid>) this.Entities;

  public virtual void DirtyEntity(EntityUid uid, MetaDataComponent? metadata = null)
  {
    if (!this.MetaQuery.ResolveInternal(uid, ref metadata) || metadata.EntityLastModifiedTick == this._gameTiming.CurTick)
      return;
    metadata.EntityLastModifiedTick = this._gameTiming.CurTick;
    if (metadata.EntityLifeStage <= EntityLifeStage.Initializing)
      return;
    Action<Entity<MetaDataComponent>> entityDirtied = this.EntityDirtied;
    if (entityDirtied == null)
      return;
    entityDirtied((Entity<MetaDataComponent>) (uid, metadata));
  }

  [Obsolete("use override with an EntityUid or Entity<T>")]
  public void Dirty(IComponent component, MetaDataComponent? meta = null)
  {
    this.Dirty(component.Owner, component, meta);
  }

  public virtual void Dirty(EntityUid uid, IComponent component, MetaDataComponent? meta = null)
  {
    if (component.LifeStage >= ComponentLifeStage.Removing || !component.NetSyncEnabled || component.LastModifiedTick == this.CurrentTick)
      return;
    this.DirtyEntity(uid, meta);
    component.LastModifiedTick = this.CurrentTick;
  }

  public virtual void Dirty<T>(Entity<T> ent, MetaDataComponent? meta = null) where T : IComponent
  {
    if (ent.Comp.LifeStage >= ComponentLifeStage.Removing || !ent.Comp.NetSyncEnabled || ent.Comp.LastModifiedTick == this.CurrentTick)
      return;
    this.DirtyEntity((EntityUid) ent, meta);
    ref T local = ref ent.Comp;
    if ((object) default (T) == null)
    {
      T obj = local;
      local = ref obj;
    }
    GameTick currentTick = this.CurrentTick;
    local.LastModifiedTick = currentTick;
  }

  public virtual void Dirty<T1, T2>(Entity<T1, T2> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
  {
    this.DirtyEntity((EntityUid) ent, meta);
    ref T1 local1 = ref ent.Comp1;
    if ((object) default (T1) == null)
    {
      T1 obj = local1;
      local1 = ref obj;
    }
    GameTick currentTick1 = this.CurrentTick;
    local1.LastModifiedTick = currentTick1;
    ref T2 local2 = ref ent.Comp2;
    if ((object) default (T2) == null)
    {
      T2 obj = local2;
      local2 = ref obj;
    }
    GameTick currentTick2 = this.CurrentTick;
    local2.LastModifiedTick = currentTick2;
  }

  public virtual void Dirty<T1, T2, T3>(Entity<T1, T2, T3> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
    where T3 : IComponent
  {
    this.DirtyEntity((EntityUid) ent, meta);
    ref T1 local1 = ref ent.Comp1;
    if ((object) default (T1) == null)
    {
      T1 obj = local1;
      local1 = ref obj;
    }
    GameTick currentTick1 = this.CurrentTick;
    local1.LastModifiedTick = currentTick1;
    ref T2 local2 = ref ent.Comp2;
    if ((object) default (T2) == null)
    {
      T2 obj = local2;
      local2 = ref obj;
    }
    GameTick currentTick2 = this.CurrentTick;
    local2.LastModifiedTick = currentTick2;
    ref T3 local3 = ref ent.Comp3;
    if ((object) default (T3) == null)
    {
      T3 obj = local3;
      local3 = ref obj;
    }
    GameTick currentTick3 = this.CurrentTick;
    local3.LastModifiedTick = currentTick3;
  }

  public virtual void Dirty<T1, T2, T3, T4>(Entity<T1, T2, T3, T4> ent, MetaDataComponent? meta = null)
    where T1 : IComponent
    where T2 : IComponent
    where T3 : IComponent
    where T4 : IComponent
  {
    this.DirtyEntity((EntityUid) ent, meta);
    ref T1 local1 = ref ent.Comp1;
    if ((object) default (T1) == null)
    {
      T1 obj = local1;
      local1 = ref obj;
    }
    GameTick currentTick1 = this.CurrentTick;
    local1.LastModifiedTick = currentTick1;
    ref T2 local2 = ref ent.Comp2;
    if ((object) default (T2) == null)
    {
      T2 obj = local2;
      local2 = ref obj;
    }
    GameTick currentTick2 = this.CurrentTick;
    local2.LastModifiedTick = currentTick2;
    ref T3 local3 = ref ent.Comp3;
    if ((object) default (T3) == null)
    {
      T3 obj = local3;
      local3 = ref obj;
    }
    GameTick currentTick3 = this.CurrentTick;
    local3.LastModifiedTick = currentTick3;
    ref T4 local4 = ref ent.Comp4;
    if ((object) default (T4) == null)
    {
      T4 obj = local4;
      local4 = ref obj;
    }
    GameTick currentTick4 = this.CurrentTick;
    local4.LastModifiedTick = currentTick4;
  }

  public bool TryQueueDeleteEntity(EntityUid? uid)
  {
    if (!uid.HasValue || this.Deleted(uid.Value) || this.QueuedDeletionsSet.Contains(uid.Value))
      return false;
    this.QueueDeleteEntity(uid);
    return true;
  }

  public virtual void DeleteEntity(EntityUid? uid)
  {
    MetaDataComponent component;
    if (!uid.HasValue || !this.Started || !this.MetaQuery.TryGetComponent(uid.Value, out component))
      return;
    this.DeleteEntity(uid.Value, component, this.TransformQuery.GetComponent(uid.Value));
  }

  public void DeleteEntity(EntityUid e, MetaDataComponent meta, TransformComponent xform)
  {
    if (!this.Started || meta.EntityLifeStage >= EntityLifeStage.Deleted)
      return;
    if (meta.EntityLifeStage == EntityLifeStage.Terminating)
      this._sawmill.Error($"{$"Called Delete on an entity already being deleted. Entity: {this.ToPrettyString((Entity<MetaDataComponent>) e)}"}. Trace: {Environment.StackTrace}");
    this.RecursiveFlagEntityTermination(e, meta, xform);
    TransformComponent component = (TransformComponent) null;
    if (xform.ParentUid.IsValid())
    {
      if (xform.LifeStage < ComponentLifeStage.Initialized)
      {
        if (this.TransformQuery.TryComp(xform.ParentUid, out component))
          component._children.Remove(e);
        component = (TransformComponent) null;
        xform._parent = EntityUid.Invalid;
        xform._anchored = false;
      }
      else
        this.TransformQuery.Resolve(xform.ParentUid, ref component);
    }
    this.RecursiveDeleteEntity(e, meta, xform, component);
  }

  private void RecursiveFlagEntityTermination(
    EntityUid uid,
    MetaDataComponent metadata,
    TransformComponent xform)
  {
    this.SetLifeStage(metadata, EntityLifeStage.Terminating);
    try
    {
      EntityTerminatingEvent terminatingEvent = new EntityTerminatingEvent((Entity<MetaDataComponent>) (uid, metadata));
      EntityManager.TerminatingEventHandler entityTerminating = this.BeforeEntityTerminating;
      if (entityTerminating != null)
        entityTerminating(ref terminatingEvent);
      this.EventBusInternal.RaiseLocalEvent<EntityTerminatingEvent>(uid, ref terminatingEvent, true);
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Caught exception while raising event {"EntityTerminatingEvent"} on entity {this.ToPrettyString(uid, metadata)}\n{ex}");
    }
    foreach (EntityUid child in xform._children)
    {
      MetaDataComponent component;
      if (!this.MetaQuery.TryGetComponent(child, out component) || component.EntityDeleted)
      {
        this._sawmill.Error($"A deleted entity was still the transform child of another entity. Parent: {this.ToPrettyString(uid, metadata)}.");
        xform._children.Remove(child);
      }
      else
        this.RecursiveFlagEntityTermination(child, component, this.TransformQuery.GetComponent(child));
    }
  }

  private void RecursiveDeleteEntity(
    EntityUid uid,
    MetaDataComponent metadata,
    TransformComponent transform,
    TransformComponent? parentXform)
  {
    this._xforms.DetachEntity(uid, transform, metadata, parentXform, true);
    foreach (EntityUid child in transform._children)
    {
      try
      {
        MetaDataComponent component1 = this.MetaQuery.GetComponent(child);
        TransformComponent component2 = this.TransformQuery.GetComponent(child);
        this.RecursiveDeleteEntity(child, component1, component2, transform);
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Caught exception while trying to recursively delete child entity '{this.ToPrettyString((Entity<MetaDataComponent>) child)}' of '{this.ToPrettyString(uid, metadata)}'\n{ex}");
      }
    }
    if (transform._children.Count != 0)
      this._sawmill.Error($"Failed to delete all children of entity: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    foreach (IComponent component in EntityManager.InSafeOrder((IEnumerable<IComponent>) this._entCompIndex[uid]))
    {
      if (component.Running)
      {
        try
        {
          this.LifeShutdown<IComponent>(uid, component, this._componentFactory.GetIndex(component.GetType()));
        }
        catch (Exception ex)
        {
          this._sawmill.Error($"Caught exception while trying to call shutdown on component of entity '{this.ToPrettyString(uid, metadata)}'\n{ex}");
        }
      }
    }
    this.DisposeComponents(uid, metadata);
    this.SetLifeStage(metadata, EntityLifeStage.Deleted);
    try
    {
      Action<Entity<MetaDataComponent>> entityDeleted = this.EntityDeleted;
      if (entityDeleted != null)
        entityDeleted((Entity<MetaDataComponent>) (uid, metadata));
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Caught exception while invoking event {"EntityDeleted"} on '{this.ToPrettyString(uid, metadata)}'\n{ex}");
    }
    this.EventBusInternal.OnEntityDeleted(uid);
    this.Entities.Remove(uid);
    this.NetEntityLookup.Remove(metadata.NetEntity);
  }

  public virtual void QueueDeleteEntity(EntityUid? uid)
  {
    if (!uid.HasValue || !this.QueuedDeletionsSet.Add(uid.Value))
      return;
    this.QueuedDeletions.Enqueue(uid.Value);
    Action<EntityUid> entityQueueDeleted = this.EntityQueueDeleted;
    if (entityQueueDeleted == null)
      return;
    entityQueueDeleted(uid.Value);
  }

  public virtual bool IsQueuedForDeletion(EntityUid uid) => this.QueuedDeletionsSet.Contains(uid);

  public virtual void PredictedDeleteEntity(Entity<MetaDataComponent?, TransformComponent?> ent)
  {
    this.DeleteEntity(new EntityUid?(ent.Owner));
  }

  public void PredictedDeleteEntity(Entity<MetaDataComponent?, TransformComponent?>? ent)
  {
    if (!ent.HasValue)
      return;
    this.PredictedDeleteEntity(ent.Value);
  }

  public virtual void PredictedQueueDeleteEntity(Entity<MetaDataComponent?> ent)
  {
    this.QueueDeleteEntity(new EntityUid?((EntityUid) ent));
  }

  public void PredictedQueueDeleteEntity(Entity<MetaDataComponent?>? ent)
  {
    if (!ent.HasValue)
      return;
    this.PredictedQueueDeleteEntity(ent.Value);
  }

  [Obsolete("use variant without TransformComponent")]
  public void PredictedQueueDeleteEntity(Entity<MetaDataComponent?, TransformComponent?> ent)
  {
    this.PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(ent.Owner, ent.Comp1));
  }

  [Obsolete("use variant without TransformComponent")]
  public void PredictedQueueDeleteEntity(Entity<MetaDataComponent?, TransformComponent?>? ent)
  {
    if (!ent.HasValue)
      return;
    this.PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(ent.Value.Owner, ent.Value.Comp1));
  }

  public void PredictedQueueDeleteEntity(EntityUid uid)
  {
    this.PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(uid, (MetaDataComponent) null));
  }

  public void PredictedQueueDeleteEntity(EntityUid? uid)
  {
    if (!uid.HasValue)
      return;
    this.PredictedQueueDeleteEntity(new Entity<MetaDataComponent>(uid.Value, (MetaDataComponent) null));
  }

  public bool EntityExists(EntityUid uid) => this.MetaQuery.HasComponentInternal(uid);

  public bool EntityExists(EntityUid? uid) => uid.HasValue && this.EntityExists(uid.Value);

  public bool IsPaused(EntityUid? uid, MetaDataComponent? metadata = null)
  {
    return uid.HasValue && this.MetaQuery.Resolve(uid.Value, ref metadata) && metadata.EntityPaused;
  }

  public bool Deleted(EntityUid uid)
  {
    MetaDataComponent component;
    return !this.MetaQuery.TryGetComponentInternal(uid, out component) || component.EntityDeleted;
  }

  public bool Deleted([NotNullWhen(false)] EntityUid? uid)
  {
    MetaDataComponent component;
    return !uid.HasValue || !this.MetaQuery.TryGetComponentInternal(uid.Value, out component) || component.EntityDeleted;
  }

  public virtual void FlushEntities()
  {
    this._sawmill.Info($"Flushing entities. Entity count: {this.Entities.Count}");
    Action beforeEntityFlush = this.BeforeEntityFlush;
    if (beforeEntityFlush != null)
      beforeEntityFlush();
    this.FlushEntitiesInternal();
    if (this.Entities.Count != 0)
    {
      this._sawmill.Error($"Failed to flush all entities. Entity count: {this.Entities.Count}");
      if (this.Entities.Count < 512 /*0x0200*/)
      {
        foreach (EntityUid entity in this.Entities)
          this._sawmill.Error($"Entity exists after flush: {this.ToPrettyString((Entity<MetaDataComponent>) entity)}");
      }
    }
    this.FlushEntitiesInternal();
    if (this.Entities.Count != 0)
      throw new Exception($"Failed to flush all entities. Entity count: {this.Entities.Count}");
    Action afterEntityFlush = this.AfterEntityFlush;
    if (afterEntityFlush == null)
      return;
    afterEntityFlush();
  }

  private void FlushEntitiesInternal()
  {
    this.QueuedDeletions.Clear();
    this.QueuedDeletionsSet.Clear();
    foreach (EntityUid entityUid in this._entTraitDict[typeof (MapComponent)].Keys.ToArray<EntityUid>())
    {
      try
      {
        this.DeleteEntity(new EntityUid?(entityUid));
      }
      catch (Exception ex)
      {
        this._sawmill.Log(LogLevel.Error, ex, $"Caught exception while trying to delete map entity {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)}, this might corrupt the game state...");
      }
    }
    foreach ((EntityUid entityUid, IComponent component) in this._entTraitDict[typeof (MetaDataComponent)].ToArray<EntityUid, IComponent>())
    {
      MetaDataComponent metaDataComponent = (MetaDataComponent) component;
      if (metaDataComponent.EntityLifeStage < EntityLifeStage.Terminating)
      {
        try
        {
          this.DeleteEntity(entityUid, metaDataComponent, this.TransformQuery.GetComponent(entityUid));
        }
        catch (Exception ex)
        {
          this._sawmill.Log(LogLevel.Error, ex, $"Caught exception while trying to delete entity {this.ToPrettyString(entityUid, metaDataComponent)}, this might corrupt the game state...");
        }
      }
    }
  }

  protected internal EntityUid AllocEntity(
    Robust.Shared.Prototypes.EntityPrototype? prototype,
    out MetaDataComponent metadata)
  {
    EntityUid uid = this.AllocEntity(out metadata);
    metadata._entityPrototype = prototype;
    this.Dirty(uid, (IComponent) metadata, metadata);
    return uid;
  }

  internal EntityUid AllocEntity(Robust.Shared.Prototypes.EntityPrototype? prototype)
  {
    return this.AllocEntity(prototype, out MetaDataComponent _);
  }

  private EntityUid AllocEntity(out MetaDataComponent metadata)
  {
    EntityUid entityUid = this.GenerateEntityUid();
    ref MetaDataComponent local = ref metadata;
    MetaDataComponent metaDataComponent = new MetaDataComponent();
    metaDataComponent.Owner = entityUid;
    metaDataComponent.EntityLastModifiedTick = this._gameTiming.CurTick;
    local = metaDataComponent;
    NetEntity netEntity = this.GenerateNetEntity();
    this.SetNetEntity(entityUid, netEntity, metadata);
    Action<Entity<MetaDataComponent>> entityAdded = this.EntityAdded;
    if (entityAdded != null)
      entityAdded((Entity<MetaDataComponent>) (entityUid, metadata));
    this.EventBusInternal.OnEntityAdded(entityUid);
    this.Entities.Add(entityUid);
    this.AddComponentInternal<MetaDataComponent>(entityUid, metadata, this._metaReg, false, true, metadata);
    TransformComponent component = Unsafe.As<TransformComponent>((object) this._componentFactory.GetComponent(this._xformReg));
    component.Owner = entityUid;
    this.AddComponentInternal<TransformComponent>(entityUid, component, false, true, metadata);
    return entityUid;
  }

  internal virtual EntityUid CreateEntity(
    string? prototypeName,
    out MetaDataComponent metadata,
    IEntityLoadContext? context = null)
  {
    if (prototypeName == null)
      return this.AllocEntity(out metadata);
    Robust.Shared.Prototypes.EntityPrototype prototype;
    if (!this.PrototypeManager.TryIndex<Robust.Shared.Prototypes.EntityPrototype>(prototypeName, out prototype))
      throw new EntityCreationException("Attempted to spawn an entity with an invalid prototype: " + prototypeName);
    return this.CreateEntity(prototype, out metadata, context);
  }

  private protected EntityUid CreateEntity(
    Robust.Shared.Prototypes.EntityPrototype prototype,
    out MetaDataComponent metadata,
    IEntityLoadContext? context = null)
  {
    EntityUid entity = this.AllocEntity(prototype, out metadata);
    try
    {
      Robust.Shared.Prototypes.EntityPrototype.LoadEntity((Entity<MetaDataComponent>) (entity, metadata), this.ComponentFactory, (IEntityManager) this, this._serManager, context);
      return entity;
    }
    catch (Exception ex)
    {
      this.DeleteEntity(new EntityUid?(entity));
      throw new EntityCreationException("Exception inside CreateEntity with prototype " + prototype.ID, ex);
    }
  }

  public void InitializeAndStartEntity(EntityUid entity, MapId? mapId = null)
  {
    bool doMapInit = this._mapSystem.IsInitialized(mapId ?? this.TransformQuery.GetComponent(entity).MapID);
    this.InitializeAndStartEntity((Entity<MetaDataComponent>) entity, doMapInit);
  }

  public void InitializeAndStartEntity(Entity<MetaDataComponent?> entity, bool doMapInit)
  {
    if (!this.MetaQuery.Resolve(entity.Owner, ref entity.Comp))
      return;
    try
    {
      this.InitializeEntity(entity.Owner, entity.Comp);
      this.StartEntity(entity.Owner);
      if (!doMapInit)
        return;
      this.RunMapInit(entity.Owner, entity.Comp);
    }
    catch (Exception ex)
    {
      this.DeleteEntity(new EntityUid?((EntityUid) entity));
      throw new EntityCreationException("Exception inside InitializeAndStartEntity", ex);
    }
  }

  public void InitializeEntity(EntityUid entity, MetaDataComponent? meta = null)
  {
    if (meta == null)
      meta = this.GetComponent<MetaDataComponent>(entity);
    this.InitializeComponents(entity, meta);
    Action<Entity<MetaDataComponent>> entityInitialized = this.EntityInitialized;
    if (entityInitialized == null)
      return;
    entityInitialized((Entity<MetaDataComponent>) (entity, meta));
  }

  public void StartEntity(EntityUid entity) => this.StartComponents(entity);

  public void RunMapInit(EntityUid entity, MetaDataComponent meta)
  {
    if (meta.EntityLifeStage == EntityLifeStage.MapInitialized)
      return;
    this.SetLifeStage(meta, EntityLifeStage.MapInitialized);
    this.EventBusInternal.RaiseLocalEvent<MapInitEvent>(entity, EntityManager.MapInitEventInstance, false);
  }

  [return: NotNullIfNotNull("uid")]
  public EntityStringRepresentation? ToPrettyString(EntityUid? uid, MetaDataComponent? metadata = null)
  {
    return uid.HasValue ? new EntityStringRepresentation?(this.ToPrettyString(uid.Value, metadata)) : new EntityStringRepresentation?();
  }

  public EntityStringRepresentation ToPrettyString(EntityUid uid, MetaDataComponent? metadata)
  {
    return this.ToPrettyString((Entity<MetaDataComponent>) (uid, metadata));
  }

  public EntityStringRepresentation ToPrettyString(Entity<MetaDataComponent?> entity)
  {
    return entity.Comp == null && !this.MetaQuery.Resolve(entity.Owner, ref entity.Comp, false) ? new EntityStringRepresentation(entity.Owner, new NetEntity(), true) : new EntityStringRepresentation(entity.Owner, entity.Comp, this._actorQuery.CompOrNull((EntityUid) entity));
  }

  [return: NotNullIfNotNull("netEntity")]
  public EntityStringRepresentation? ToPrettyString(NetEntity? netEntity)
  {
    return netEntity.HasValue ? new EntityStringRepresentation?(this.ToPrettyString(netEntity.Value)) : new EntityStringRepresentation?();
  }

  public EntityStringRepresentation ToPrettyString(NetEntity netEntity)
  {
    EntityUid? entity;
    MetaDataComponent meta;
    return !this.TryGetEntityData(netEntity, out entity, out meta) ? new EntityStringRepresentation(EntityUid.Invalid, netEntity, true) : this.ToPrettyString(entity.Value, meta);
  }

  public virtual void RaisePredictiveEvent<T>(T msg) where T : EntityEventArgs
  {
  }

  public abstract void RaiseSharedEvent<T>(T message, EntityUid? user = null) where T : EntityEventArgs;

  public abstract void RaiseSharedEvent<T>(T message, ICommonSession? user = null) where T : EntityEventArgs;

  internal EntityUid GenerateEntityUid() => new EntityUid(this.NextEntityUid++);

  protected virtual NetEntity GenerateNetEntity() => new NetEntity(this.NextNetworkId++);

  [Conditional("DEBUG")]
  protected void ThreadCheck()
  {
  }

  internal void LifeAddToEntity<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
  {
    component.LifeStage = ComponentLifeStage.Adding;
    ref T local1 = ref component;
    T obj;
    if ((object) default (T) == null)
    {
      obj = local1;
      local1 = ref obj;
    }
    GameTick currentTick1 = this.CurrentTick;
    local1.CreationTick = currentTick1;
    ref T local2 = ref component;
    obj = default (T);
    if ((object) obj == null)
    {
      obj = local2;
      local2 = ref obj;
    }
    GameTick currentTick2 = this.CurrentTick;
    local2.LastModifiedTick = currentTick2;
    this.EventBusInternal.RaiseComponentEvent<ComponentAdd>(uid, (IComponent) component, idx, EntityManager.CompAddInstance);
    component.LifeStage = ComponentLifeStage.Added;
  }

  internal void LifeInitialize<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
  {
    component.LifeStage = ComponentLifeStage.Initializing;
    this.EventBusInternal.RaiseComponentEvent<ComponentInit>(uid, (IComponent) component, idx, EntityManager.CompInitInstance);
    component.LifeStage = ComponentLifeStage.Initialized;
  }

  internal void LifeStartup<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
  {
    component.LifeStage = ComponentLifeStage.Starting;
    this.EventBusInternal.RaiseComponentEvent<ComponentStartup>(uid, (IComponent) component, idx, EntityManager.CompStartupInstance);
    component.LifeStage = ComponentLifeStage.Running;
  }

  internal void LifeShutdown<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
  {
    if (component.LifeStage <= ComponentLifeStage.Initialized)
    {
      component.LifeStage = ComponentLifeStage.Stopped;
    }
    else
    {
      component.LifeStage = ComponentLifeStage.Stopping;
      this.EventBusInternal.RaiseComponentEvent<ComponentShutdown>(uid, (IComponent) component, idx, EntityManager.CompShutdownInstance);
      component.LifeStage = ComponentLifeStage.Stopped;
    }
  }

  internal void LifeRemoveFromEntity<T>(EntityUid uid, T component, CompIdx idx) where T : IComponent
  {
    component.LifeStage = ComponentLifeStage.Removing;
    this.EventBusInternal.RaiseComponentEvent<ComponentRemove>(uid, (IComponent) component, idx, EntityManager.CompRemoveInstance);
    component.LifeStage = ComponentLifeStage.Deleted;
  }

  internal virtual void SetLifeStage(MetaDataComponent meta, EntityLifeStage stage)
  {
    meta.EntityLifeStage = stage;
  }

  internal void ClearNetEntity(NetEntity netEntity) => this.NetEntityLookup.Remove(netEntity);

  internal void SetNetEntity(EntityUid uid, NetEntity netEntity, MetaDataComponent component)
  {
    this.NetEntityLookup[netEntity] = (uid, component);
    component.NetEntity = netEntity;
  }

  public virtual bool IsClientSide(EntityUid uid, MetaDataComponent? metadata = null) => false;

  public bool TryParseNetEntity(string arg, [NotNullWhen(true)] out EntityUid? entity)
  {
    NetEntity entity1;
    if (NetEntity.TryParse(arg.AsSpan(), out entity1) && this.TryGetEntity(entity1, out entity))
      return true;
    entity = new EntityUid?();
    return false;
  }

  public bool TryGetEntity(NetEntity nEntity, [NotNullWhen(true)] out EntityUid? entity)
  {
    (EntityUid, MetaDataComponent) tuple;
    if (this.NetEntityLookup.TryGetValue(nEntity, out tuple))
    {
      entity = new EntityUid?(tuple.Item1);
      return true;
    }
    entity = new EntityUid?();
    return false;
  }

  public bool TryGetEntityData(
    NetEntity nEntity,
    [NotNullWhen(true)] out EntityUid? entity,
    [NotNullWhen(true)] out MetaDataComponent? meta)
  {
    (EntityUid, MetaDataComponent) tuple;
    if (this.NetEntityLookup.TryGetValue(nEntity, out tuple))
    {
      entity = new EntityUid?(tuple.Item1);
      meta = tuple.Item2;
      return true;
    }
    entity = new EntityUid?();
    meta = (MetaDataComponent) null;
    return false;
  }

  public bool TryGetEntity(NetEntity? nEntity, [NotNullWhen(true)] out EntityUid? entity)
  {
    if (nEntity.HasValue)
      return this.TryGetEntity(nEntity.Value, out entity);
    entity = new EntityUid?();
    return false;
  }

  public bool TryGetNetEntity(EntityUid uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
  {
    if (uid == EntityUid.Invalid)
    {
      netEntity = new NetEntity?();
      return false;
    }
    if (this.MetaQuery.Resolve(uid, ref metadata, false))
    {
      netEntity = new NetEntity?(metadata.NetEntity);
      return true;
    }
    netEntity = new NetEntity?(NetEntity.Invalid);
    return false;
  }

  public bool TryGetNetEntity(EntityUid? uid, [NotNullWhen(true)] out NetEntity? netEntity, MetaDataComponent? metadata = null)
  {
    if (uid.HasValue)
      return this.TryGetNetEntity(uid.Value, out netEntity, metadata);
    netEntity = new NetEntity?();
    return false;
  }

  public virtual EntityUid EnsureEntity<T>(NetEntity nEntity, EntityUid callerEntity)
  {
    return this.GetEntity(nEntity);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid? EnsureEntity<T>(NetEntity? nEntity, EntityUid callerEntity)
  {
    return !nEntity.HasValue ? new EntityUid?() : new EntityUid?(this.EnsureEntity<T>(nEntity.Value, callerEntity));
  }

  public EntityUid GetEntity(NetEntity nEntity)
  {
    (EntityUid, MetaDataComponent) tuple;
    return nEntity == NetEntity.Invalid || !this.NetEntityLookup.TryGetValue(nEntity, out tuple) ? EntityUid.Invalid : tuple.Item1;
  }

  public (EntityUid, MetaDataComponent) GetEntityData(NetEntity nEntity)
  {
    return this.NetEntityLookup[nEntity];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid? GetEntity(NetEntity? nEntity)
  {
    return !nEntity.HasValue ? new EntityUid?() : new EntityUid?(this.GetEntity(nEntity.Value));
  }

  public NetEntity GetNetEntity(EntityUid uid, MetaDataComponent? metadata = null)
  {
    return uid == EntityUid.Invalid || !this.MetaQuery.Resolve(uid, ref metadata) ? NetEntity.Invalid : metadata.NetEntity;
  }

  public NetEntity? GetNetEntity(EntityUid? uid, MetaDataComponent? metadata = null)
  {
    return !uid.HasValue ? new NetEntity?() : new NetEntity?(this.GetNetEntity(uid.Value, metadata));
  }

  public NetCoordinates GetNetCoordinates(EntityCoordinates coordinates, MetaDataComponent? metadata = null)
  {
    return new NetCoordinates(this.GetNetEntity(coordinates.EntityId, metadata), coordinates.Position);
  }

  public NetCoordinates? GetNetCoordinates(
    EntityCoordinates? coordinates,
    MetaDataComponent? metadata = null)
  {
    return !coordinates.HasValue ? new NetCoordinates?() : new NetCoordinates?(new NetCoordinates(this.GetNetEntity(coordinates.Value.EntityId, metadata), coordinates.Value.Position));
  }

  public EntityCoordinates GetCoordinates(NetCoordinates coordinates)
  {
    return new EntityCoordinates(this.GetEntity(coordinates.NetEntity), coordinates.Position);
  }

  public EntityCoordinates? GetCoordinates(NetCoordinates? coordinates)
  {
    return !coordinates.HasValue ? new EntityCoordinates?() : new EntityCoordinates?(new EntityCoordinates(this.GetEntity(coordinates.Value.NetEntity), coordinates.Value.Position));
  }

  public virtual EntityCoordinates EnsureCoordinates<T>(
    NetCoordinates netCoordinates,
    EntityUid callerEntity)
  {
    return this.GetCoordinates(netCoordinates);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityCoordinates? EnsureCoordinates<T>(
    NetCoordinates? netCoordinates,
    EntityUid callerEntity)
  {
    return !netCoordinates.HasValue ? new EntityCoordinates?() : new EntityCoordinates?(this.EnsureCoordinates<T>(netCoordinates.Value, callerEntity));
  }

  public HashSet<EntityUid> GetEntitySet(HashSet<NetEntity> netEntities)
  {
    HashSet<EntityUid> entitySet = new HashSet<EntityUid>(netEntities.Count);
    foreach (NetEntity netEntity in netEntities)
      entitySet.Add(this.GetEntity(netEntity));
    return entitySet;
  }

  public List<EntityUid> GetEntityList(List<NetEntity> netEntities)
  {
    List<EntityUid> entityList = new List<EntityUid>(netEntities.Count);
    foreach (NetEntity netEntity in netEntities)
      entityList.Add(this.GetEntity(netEntity));
    return entityList;
  }

  public Dictionary<EntityUid, T> GetEntityDictionary<T>(Dictionary<NetEntity, T> netEntities)
  {
    Dictionary<EntityUid, T> entityDictionary = new Dictionary<EntityUid, T>(netEntities.Count);
    foreach (KeyValuePair<NetEntity, T> netEntity in netEntities)
      entityDictionary.Add(this.GetEntity(netEntity.Key), netEntity.Value);
    return entityDictionary;
  }

  public Dictionary<T, EntityUid> GetEntityDictionary<T>(Dictionary<T, NetEntity> netEntities) where T : notnull
  {
    Dictionary<T, EntityUid> entityDictionary = new Dictionary<T, EntityUid>(netEntities.Count);
    foreach (KeyValuePair<T, NetEntity> netEntity in netEntities)
      entityDictionary.Add(netEntity.Key, this.GetEntity(netEntity.Value));
    return entityDictionary;
  }

  public Dictionary<T, EntityUid?> GetEntityDictionary<T>(Dictionary<T, NetEntity?> netEntities) where T : notnull
  {
    Dictionary<T, EntityUid?> entityDictionary = new Dictionary<T, EntityUid?>(netEntities.Count);
    foreach (KeyValuePair<T, NetEntity?> netEntity in netEntities)
      entityDictionary.Add(netEntity.Key, this.GetEntity(netEntity.Value));
    return entityDictionary;
  }

  public Dictionary<EntityUid, EntityUid> GetEntityDictionary(
    Dictionary<NetEntity, NetEntity> netEntities)
  {
    Dictionary<EntityUid, EntityUid> entityDictionary = new Dictionary<EntityUid, EntityUid>(netEntities.Count);
    foreach (KeyValuePair<NetEntity, NetEntity> netEntity in netEntities)
      entityDictionary.Add(this.GetEntity(netEntity.Key), this.GetEntity(netEntity.Value));
    return entityDictionary;
  }

  public Dictionary<EntityUid, EntityUid?> GetEntityDictionary(
    Dictionary<NetEntity, NetEntity?> netEntities)
  {
    Dictionary<EntityUid, EntityUid?> entityDictionary = new Dictionary<EntityUid, EntityUid?>(netEntities.Count);
    foreach (KeyValuePair<NetEntity, NetEntity?> netEntity in netEntities)
      entityDictionary.Add(this.GetEntity(netEntity.Key), this.GetEntity(netEntity.Value));
    return entityDictionary;
  }

  public HashSet<EntityUid> EnsureEntitySet<T>(
    HashSet<NetEntity> netEntities,
    EntityUid callerEntity)
  {
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>(netEntities.Count);
    foreach (NetEntity netEntity in netEntities)
      entityUidSet.Add(this.EnsureEntity<T>(netEntity, callerEntity));
    return entityUidSet;
  }

  public void EnsureEntitySet<T>(
    HashSet<NetEntity> netEntities,
    EntityUid callerEntity,
    HashSet<EntityUid> entities)
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (NetEntity netEntity in netEntities)
      entities.Add(this.EnsureEntity<T>(netEntity, callerEntity));
  }

  public List<EntityUid> EnsureEntityList<T>(List<NetEntity> netEntities, EntityUid callerEntity)
  {
    List<EntityUid> entityUidList = new List<EntityUid>(netEntities.Count);
    foreach (NetEntity netEntity in netEntities)
      entityUidList.Add(this.EnsureEntity<T>(netEntity, callerEntity));
    return entityUidList;
  }

  public void EnsureEntityList<T>(
    List<NetEntity> netEntities,
    EntityUid callerEntity,
    List<EntityUid> entities)
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (NetEntity netEntity in netEntities)
      entities.Add(this.EnsureEntity<T>(netEntity, callerEntity));
  }

  public void EnsureEntityDictionary<TComp, TValue>(
    Dictionary<NetEntity, TValue> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, TValue> entities)
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (KeyValuePair<NetEntity, TValue> netEntity in netEntities)
      entities.TryAdd(this.EnsureEntity<TComp>(netEntity.Key, callerEntity), netEntity.Value);
  }

  public void EnsureEntityDictionaryNullableValue<TComp, TValue>(
    Dictionary<NetEntity, TValue?> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, TValue?> entities)
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (KeyValuePair<NetEntity, TValue> netEntity in netEntities)
      entities.TryAdd(this.EnsureEntity<TComp>(netEntity.Key, callerEntity), netEntity.Value);
  }

  public void EnsureEntityDictionary<TComp, TKey>(
    Dictionary<TKey, NetEntity> netEntities,
    EntityUid callerEntity,
    Dictionary<TKey, EntityUid> entities)
    where TKey : notnull
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (KeyValuePair<TKey, NetEntity> netEntity in netEntities)
      entities.TryAdd(netEntity.Key, this.EnsureEntity<TComp>(netEntity.Value, callerEntity));
  }

  public void EnsureEntityDictionary<TComp, TKey>(
    Dictionary<TKey, NetEntity?> netEntities,
    EntityUid callerEntity,
    Dictionary<TKey, EntityUid?> entities)
    where TKey : notnull
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (KeyValuePair<TKey, NetEntity?> netEntity in netEntities)
      entities.TryAdd(netEntity.Key, this.EnsureEntity<TComp>(netEntity.Value, callerEntity));
  }

  public void EnsureEntityDictionary<TComp>(
    Dictionary<NetEntity, NetEntity> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, EntityUid> entities)
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (KeyValuePair<NetEntity, NetEntity> netEntity in netEntities)
      entities.TryAdd(this.EnsureEntity<TComp>(netEntity.Key, callerEntity), this.EnsureEntity<TComp>(netEntity.Value, callerEntity));
  }

  public void EnsureEntityDictionary<TComp>(
    Dictionary<NetEntity, NetEntity?> netEntities,
    EntityUid callerEntity,
    Dictionary<EntityUid, EntityUid?> entities)
  {
    entities.Clear();
    entities.EnsureCapacity(netEntities.Count);
    foreach (KeyValuePair<NetEntity, NetEntity?> netEntity in netEntities)
      entities.TryAdd(this.EnsureEntity<TComp>(netEntity.Key, callerEntity), this.EnsureEntity<TComp>(netEntity.Value, callerEntity));
  }

  public List<EntityUid> GetEntityList(ICollection<NetEntity> netEntities)
  {
    List<EntityUid> entityList = new List<EntityUid>(netEntities.Count);
    foreach (NetEntity netEntity in (IEnumerable<NetEntity>) netEntities)
      entityList.Add(this.GetEntity(netEntity));
    return entityList;
  }

  public List<EntityUid?> GetEntityList(List<NetEntity?> netEntities)
  {
    List<EntityUid?> entityList = new List<EntityUid?>(netEntities.Count);
    foreach (NetEntity? netEntity in netEntities)
      entityList.Add(this.GetEntity(netEntity));
    return entityList;
  }

  public EntityUid[] GetEntityArray(NetEntity[] netEntities)
  {
    EntityUid[] entityArray = new EntityUid[netEntities.Length];
    for (int index = 0; index < netEntities.Length; ++index)
      entityArray[index] = this.GetEntity(netEntities[index]);
    return entityArray;
  }

  public EntityUid?[] GetEntityArray(NetEntity?[] netEntities)
  {
    EntityUid?[] entityArray = new EntityUid?[netEntities.Length];
    for (int index = 0; index < netEntities.Length; ++index)
      entityArray[index] = this.GetEntity(netEntities[index]);
    return entityArray;
  }

  public HashSet<NetEntity> GetNetEntitySet(HashSet<EntityUid> entities)
  {
    HashSet<NetEntity> netEntitySet = new HashSet<NetEntity>(entities.Count);
    foreach (EntityUid entity in entities)
    {
      MetaDataComponent component;
      this.MetaQuery.TryGetComponent(entity, out component);
      netEntitySet.Add(this.GetNetEntity(entity, component));
    }
    return netEntitySet;
  }

  public List<NetEntity> GetNetEntityList(List<EntityUid> entities)
  {
    List<NetEntity> netEntityList = new List<NetEntity>(entities.Count);
    foreach (EntityUid entity in entities)
      netEntityList.Add(this.GetNetEntity(entity, (MetaDataComponent) null));
    return netEntityList;
  }

  public List<NetEntity> GetNetEntityList(IReadOnlyList<EntityUid> entities)
  {
    List<NetEntity> netEntityList = new List<NetEntity>(entities.Count);
    foreach (EntityUid entity in (IEnumerable<EntityUid>) entities)
      netEntityList.Add(this.GetNetEntity(entity, (MetaDataComponent) null));
    return netEntityList;
  }

  public List<NetEntity> GetNetEntityList(ICollection<EntityUid> entities)
  {
    List<NetEntity> netEntityList = new List<NetEntity>(entities.Count);
    foreach (EntityUid entity in (IEnumerable<EntityUid>) entities)
      netEntityList.Add(this.GetNetEntity(entity, (MetaDataComponent) null));
    return netEntityList;
  }

  public List<NetEntity?> GetNetEntityList(List<EntityUid?> entities)
  {
    List<NetEntity?> netEntityList = new List<NetEntity?>(entities.Count);
    foreach (EntityUid? entity in entities)
      netEntityList.Add(this.GetNetEntity(entity, (MetaDataComponent) null));
    return netEntityList;
  }

  public NetEntity[] GetNetEntityArray(EntityUid[] entities)
  {
    NetEntity[] netEntityArray = new NetEntity[entities.Length];
    for (int index = 0; index < entities.Length; ++index)
      netEntityArray[index] = this.GetNetEntity(entities[index], (MetaDataComponent) null);
    return netEntityArray;
  }

  public NetEntity?[] GetNetEntityArray(EntityUid?[] entities)
  {
    NetEntity?[] netEntityArray = new NetEntity?[entities.Length];
    for (int index = 0; index < entities.Length; ++index)
      netEntityArray[index] = this.GetNetEntity(entities[index], (MetaDataComponent) null);
    return netEntityArray;
  }

  public Dictionary<NetEntity, T> GetNetEntityDictionary<T>(Dictionary<EntityUid, T> entities)
  {
    Dictionary<NetEntity, T> entityDictionary = new Dictionary<NetEntity, T>(entities.Count);
    foreach (KeyValuePair<EntityUid, T> entity in entities)
      entityDictionary.Add(this.GetNetEntity(entity.Key, (MetaDataComponent) null), entity.Value);
    return entityDictionary;
  }

  public Dictionary<T, NetEntity> GetNetEntityDictionary<T>(Dictionary<T, EntityUid> entities) where T : notnull
  {
    Dictionary<T, NetEntity> entityDictionary = new Dictionary<T, NetEntity>(entities.Count);
    foreach (KeyValuePair<T, EntityUid> entity in entities)
      entityDictionary.Add(entity.Key, this.GetNetEntity(entity.Value, (MetaDataComponent) null));
    return entityDictionary;
  }

  public Dictionary<T, NetEntity?> GetNetEntityDictionary<T>(Dictionary<T, EntityUid?> entities) where T : notnull
  {
    Dictionary<T, NetEntity?> entityDictionary = new Dictionary<T, NetEntity?>(entities.Count);
    foreach (KeyValuePair<T, EntityUid?> entity in entities)
      entityDictionary.Add(entity.Key, this.GetNetEntity(entity.Value, (MetaDataComponent) null));
    return entityDictionary;
  }

  public Dictionary<NetEntity, NetEntity> GetNetEntityDictionary(
    Dictionary<EntityUid, EntityUid> entities)
  {
    Dictionary<NetEntity, NetEntity> entityDictionary = new Dictionary<NetEntity, NetEntity>(entities.Count);
    foreach (KeyValuePair<EntityUid, EntityUid> entity in entities)
      entityDictionary.Add(this.GetNetEntity(entity.Key, (MetaDataComponent) null), this.GetNetEntity(entity.Value, (MetaDataComponent) null));
    return entityDictionary;
  }

  public Dictionary<NetEntity, NetEntity?> GetNetEntityDictionary(
    Dictionary<EntityUid, EntityUid?> entities)
  {
    Dictionary<NetEntity, NetEntity?> entityDictionary = new Dictionary<NetEntity, NetEntity?>(entities.Count);
    foreach (KeyValuePair<EntityUid, EntityUid?> entity in entities)
      entityDictionary.Add(this.GetNetEntity(entity.Key, (MetaDataComponent) null), this.GetNetEntity(entity.Value, (MetaDataComponent) null));
    return entityDictionary;
  }

  public HashSet<EntityCoordinates> GetEntitySet(HashSet<NetCoordinates> netEntities)
  {
    HashSet<EntityCoordinates> entitySet = new HashSet<EntityCoordinates>(netEntities.Count);
    foreach (NetCoordinates netEntity in netEntities)
      entitySet.Add(this.GetCoordinates(netEntity));
    return entitySet;
  }

  public List<EntityCoordinates> GetEntityList(List<NetCoordinates> netEntities)
  {
    List<EntityCoordinates> entityList = new List<EntityCoordinates>(netEntities.Count);
    foreach (NetCoordinates netEntity in netEntities)
      entityList.Add(this.GetCoordinates(netEntity));
    return entityList;
  }

  public List<EntityCoordinates> GetEntityList(ICollection<NetCoordinates> netEntities)
  {
    List<EntityCoordinates> entityList = new List<EntityCoordinates>(netEntities.Count);
    foreach (NetCoordinates netEntity in (IEnumerable<NetCoordinates>) netEntities)
      entityList.Add(this.GetCoordinates(netEntity));
    return entityList;
  }

  public List<EntityCoordinates?> GetEntityList(List<NetCoordinates?> netEntities)
  {
    List<EntityCoordinates?> entityList = new List<EntityCoordinates?>(netEntities.Count);
    foreach (NetCoordinates? netEntity in netEntities)
      entityList.Add(this.GetCoordinates(netEntity));
    return entityList;
  }

  public EntityCoordinates[] GetEntityArray(NetCoordinates[] netEntities)
  {
    EntityCoordinates[] entityArray = new EntityCoordinates[netEntities.Length];
    for (int index = 0; index < netEntities.Length; ++index)
      entityArray[index] = this.GetCoordinates(netEntities[index]);
    return entityArray;
  }

  public EntityCoordinates?[] GetEntityArray(NetCoordinates?[] netEntities)
  {
    EntityCoordinates?[] entityArray = new EntityCoordinates?[netEntities.Length];
    for (int index = 0; index < netEntities.Length; ++index)
      entityArray[index] = this.GetCoordinates(netEntities[index]);
    return entityArray;
  }

  public HashSet<NetCoordinates> GetNetCoordinatesSet(HashSet<EntityCoordinates> entities)
  {
    HashSet<NetCoordinates> netCoordinatesSet = new HashSet<NetCoordinates>(entities.Count);
    foreach (EntityCoordinates entity in entities)
      netCoordinatesSet.Add(this.GetNetCoordinates(entity, (MetaDataComponent) null));
    return netCoordinatesSet;
  }

  public List<NetCoordinates> GetNetCoordinatesList(List<EntityCoordinates> entities)
  {
    List<NetCoordinates> netCoordinatesList = new List<NetCoordinates>(entities.Count);
    foreach (EntityCoordinates entity in entities)
      netCoordinatesList.Add(this.GetNetCoordinates(entity, (MetaDataComponent) null));
    return netCoordinatesList;
  }

  public List<NetCoordinates> GetNetCoordinatesList(ICollection<EntityCoordinates> entities)
  {
    List<NetCoordinates> netCoordinatesList = new List<NetCoordinates>(entities.Count);
    foreach (EntityCoordinates entity in (IEnumerable<EntityCoordinates>) entities)
      netCoordinatesList.Add(this.GetNetCoordinates(entity, (MetaDataComponent) null));
    return netCoordinatesList;
  }

  public List<NetCoordinates?> GetNetCoordinatesList(List<EntityCoordinates?> entities)
  {
    List<NetCoordinates?> netCoordinatesList = new List<NetCoordinates?>(entities.Count);
    foreach (EntityCoordinates? entity in entities)
      netCoordinatesList.Add(this.GetNetCoordinates(entity, (MetaDataComponent) null));
    return netCoordinatesList;
  }

  public NetCoordinates[] GetNetCoordinatesArray(EntityCoordinates[] entities)
  {
    NetCoordinates[] coordinatesArray = new NetCoordinates[entities.Length];
    for (int index = 0; index < entities.Length; ++index)
      coordinatesArray[index] = this.GetNetCoordinates(entities[index], (MetaDataComponent) null);
    return coordinatesArray;
  }

  public NetCoordinates?[] GetNetCoordinatesArray(EntityCoordinates?[] entities)
  {
    NetCoordinates?[] coordinatesArray = new NetCoordinates?[entities.Length];
    for (int index = 0; index < entities.Length; ++index)
      coordinatesArray[index] = this.GetNetCoordinates(entities[index], (MetaDataComponent) null);
    return coordinatesArray;
  }

  public EntityUid SpawnEntity(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null)
  {
    return this.SpawnAttachedTo(protoName, coordinates, overrides, new Angle());
  }

  public EntityUid SpawnEntity(
    string? protoName,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null)
  {
    return this.Spawn(protoName, coordinates, overrides, new Angle());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid[] SpawnEntitiesAttachedTo(
    EntityCoordinates coordinates,
    params string?[] protoNames)
  {
    EntityUid[] entityUidArray = new EntityUid[protoNames.Length];
    for (int index = 0; index < protoNames.Length; ++index)
      entityUidArray[index] = this.SpawnAttachedTo(protoNames[index], coordinates, (ComponentRegistry) null, new Angle());
    return entityUidArray;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid[] SpawnEntities(MapCoordinates coordinates, params string?[] protoNames)
  {
    EntityUid[] entityUidArray = new EntityUid[protoNames.Length];
    for (int index = 0; index < protoNames.Length; ++index)
      entityUidArray[index] = this.Spawn(protoNames[index], coordinates, (ComponentRegistry) null, new Angle());
    return entityUidArray;
  }

  public EntityUid[] SpawnEntities(MapCoordinates coordinates, string? prototype, int count)
  {
    EntityUid[] entityUidArray = new EntityUid[count];
    for (int index = 0; index < count; ++index)
      entityUidArray[index] = this.Spawn(prototype, coordinates, (ComponentRegistry) null, new Angle());
    return entityUidArray;
  }

  public EntityUid[] SpawnEntitiesAttachedTo(
    EntityCoordinates coordinates,
    params EntProtoId[] protoNames)
  {
    EntityUid[] entityUidArray = new EntityUid[protoNames.Length];
    for (int index = 0; index < protoNames.Length; ++index)
      entityUidArray[index] = this.SpawnAttachedTo((string) protoNames[index], coordinates, (ComponentRegistry) null, new Angle());
    return entityUidArray;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid[] SpawnEntitiesAttachedTo(EntityCoordinates coordinates, List<string?> protoNames)
  {
    EntityUid[] entityUidArray = new EntityUid[protoNames.Count];
    for (int index = 0; index < protoNames.Count; ++index)
      entityUidArray[index] = this.SpawnAttachedTo(protoNames[index], coordinates, (ComponentRegistry) null, new Angle());
    return entityUidArray;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid[] SpawnEntities(MapCoordinates coordinates, List<string?> protoNames)
  {
    EntityUid[] entityUidArray = new EntityUid[protoNames.Count];
    for (int index = 0; index < protoNames.Count; ++index)
      entityUidArray[index] = this.Spawn(protoNames[index], coordinates, (ComponentRegistry) null, new Angle());
    return entityUidArray;
  }

  public EntityUid[] SpawnEntitiesAttachedTo(
    EntityCoordinates coordinates,
    IEnumerable<EntProtoId> protoNames)
  {
    ValueList<EntityUid> valueList = new ValueList<EntityUid>();
    foreach (EntProtoId protoName in protoNames)
    {
      EntityUid entityUid = this.SpawnAttachedTo((string) protoName, coordinates, (ComponentRegistry) null, new Angle());
      valueList.Add(entityUid);
    }
    return valueList.ToArray();
  }

  public virtual EntityUid SpawnAttachedTo(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    if (!coordinates.IsValid((IEntityManager) this))
      throw new InvalidOperationException($"Tried to spawn entity {protoName} on invalid coordinates {coordinates}.");
    EntityUid entityUninitialized = this.CreateEntityUninitialized(protoName, coordinates, overrides, rotation);
    this.InitializeAndStartEntity(entityUninitialized, new MapId?(this._xforms.GetMapId(coordinates)));
    return entityUninitialized;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid Spawn(string? protoName = null, ComponentRegistry? overrides = null, bool doMapInit = true)
  {
    EntityUid entityUninitialized = this.CreateEntityUninitialized(protoName, MapCoordinates.Nullspace, overrides, new Angle());
    this.InitializeAndStartEntity((Entity<MetaDataComponent>) entityUninitialized, doMapInit);
    return entityUninitialized;
  }

  public virtual EntityUid Spawn(
    string? protoName,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    EntityUid entityUninitialized = this.CreateEntityUninitialized(protoName, coordinates, overrides, rotation);
    this.InitializeAndStartEntity(entityUninitialized, new MapId?(coordinates.MapId));
    return entityUninitialized;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public EntityUid SpawnAtPosition(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null)
  {
    return this.Spawn(protoName, this._xforms.ToMapCoordinates(coordinates), overrides, new Angle());
  }

  public bool TrySpawnNextTo(
    string? protoName,
    EntityUid target,
    [NotNullWhen(true)] out EntityUid? uid,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    uid = new EntityUid?();
    if (!this.TransformQuery.Resolve(target, ref xform) || !xform.ParentUid.IsValid())
      return false;
    BaseContainer container;
    if (!this._containers.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) target, out container))
    {
      uid = new EntityUid?(this.SpawnNextToOrDrop(protoName, target, xform, overrides));
      return true;
    }
    bool doMapInit = this._mapSystem.IsInitialized(xform.MapUid);
    uid = new EntityUid?(this.Spawn(protoName, overrides, doMapInit));
    if (this._containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid.Value, container))
      return true;
    this.DeleteEntity(new EntityUid?(uid.Value));
    uid = new EntityUid?();
    return false;
  }

  public bool TrySpawnInContainer(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    [NotNullWhen(true)] out EntityUid? uid,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    uid = new EntityUid?();
    BaseContainer container;
    if (containerComp == null && !this.TryGetComponent<ContainerManagerComponent>(containerUid, out containerComp) || !containerComp.Containers.TryGetValue(containerId, out container))
      return false;
    bool doMapInit = this._mapSystem.IsInitialized(this.TransformQuery.GetComponent(containerUid).MapUid);
    uid = new EntityUid?(this.Spawn(protoName, overrides, doMapInit));
    if (this._containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid.Value, container))
      return true;
    this.DeleteEntity(new EntityUid?(uid.Value));
    uid = new EntityUid?();
    return false;
  }

  public EntityUid SpawnNextToOrDrop(
    string? protoName,
    EntityUid target,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    if (xform == null)
      xform = this.TransformQuery.GetComponent(target);
    if (!xform.ParentUid.IsValid())
      return this.Spawn(protoName, (ComponentRegistry) null, true);
    bool doMapInit = this._mapSystem.IsInitialized(xform.MapUid);
    EntityUid orDrop = this.Spawn(protoName, overrides, doMapInit);
    this._xforms.DropNextTo((Entity<TransformComponent>) orDrop, (Entity<TransformComponent>) target);
    return orDrop;
  }

  public EntityUid SpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    TransformComponent? xform = null,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    return this.SpawnInContainerOrDrop(protoName, containerUid, containerId, out bool _, xform, containerComp, overrides);
  }

  public EntityUid SpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    out bool inserted,
    TransformComponent? xform = null,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    inserted = true;
    if (xform == null)
      xform = this.TransformQuery.GetComponent(containerUid);
    bool doMapInit = this._mapSystem.IsInitialized(xform.MapUid);
    EntityUid toInsert = this.Spawn(protoName, overrides, doMapInit);
    BaseContainer container;
    if (containerComp == null && !this.TryGetComponent<ContainerManagerComponent>(containerUid, out containerComp) || !containerComp.Containers.TryGetValue(containerId, out container) || !this._containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) toInsert, container))
    {
      inserted = false;
      if (xform.ParentUid.IsValid())
        this._xforms.DropNextTo((Entity<TransformComponent>) toInsert, (Entity<TransformComponent>) (containerUid, xform));
    }
    return toInsert;
  }

  public virtual EntityUid PredictedSpawnAttachedTo(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    return this.SpawnAttachedTo(protoName, coordinates, overrides, rotation);
  }

  public virtual EntityUid PredictedSpawn(
    string? protoName = null,
    ComponentRegistry? overrides = null,
    bool doMapInit = true)
  {
    return this.Spawn(protoName, overrides, doMapInit);
  }

  public virtual EntityUid PredictedSpawn(
    string? protoName,
    MapCoordinates coordinates,
    ComponentRegistry? overrides = null,
    Angle rotation = default (Angle))
  {
    return this.Spawn(protoName, coordinates, overrides, rotation);
  }

  public virtual EntityUid PredictedSpawnAtPosition(
    string? protoName,
    EntityCoordinates coordinates,
    ComponentRegistry? overrides = null)
  {
    return this.SpawnAtPosition(protoName, coordinates, overrides);
  }

  public virtual bool PredictedTrySpawnNextTo(
    string? protoName,
    EntityUid target,
    [NotNullWhen(true)] out EntityUid? uid,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    return this.TrySpawnNextTo(protoName, target, out uid, xform, overrides);
  }

  public virtual bool PredictedTrySpawnInContainer(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    [NotNullWhen(true)] out EntityUid? uid,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    return this.TrySpawnInContainer(protoName, containerUid, containerId, out uid, containerComp, overrides);
  }

  public virtual EntityUid PredictedSpawnNextToOrDrop(
    string? protoName,
    EntityUid target,
    TransformComponent? xform = null,
    ComponentRegistry? overrides = null)
  {
    return this.SpawnNextToOrDrop(protoName, target, xform, overrides);
  }

  public virtual EntityUid PredictedSpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    TransformComponent? xform = null,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    return this.SpawnInContainerOrDrop(protoName, containerUid, containerId, xform, containerComp, overrides);
  }

  public virtual EntityUid PredictedSpawnInContainerOrDrop(
    string? protoName,
    EntityUid containerUid,
    string containerId,
    out bool inserted,
    TransformComponent? xform = null,
    ContainerManagerComponent? containerComp = null,
    ComponentRegistry? overrides = null)
  {
    return this.SpawnInContainerOrDrop(protoName, containerUid, containerId, out inserted, xform, containerComp, overrides);
  }

  public virtual void FlagPredicted(Entity<MetaDataComponent?> ent)
  {
  }

  public T System<T>() where T : IEntitySystem => this._entitySystemManager.GetEntitySystem<T>();

  public T? SystemOrNull<T>() where T : IEntitySystem
  {
    return this._entitySystemManager.GetEntitySystemOrNull<T>();
  }

  public bool TrySystem<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem
  {
    return this._entitySystemManager.TryGetEntitySystem<T>(out entitySystem);
  }

  public readonly struct CompInitializeHandle<T>(
    IEntityManager entityManager,
    EntityUid owner,
    T comp,
    CompIdx compType) : IDisposable
    where T : IComponent
  {
    private readonly IEntityManager _entMan = entityManager;
    private readonly EntityUid _owner = owner;
    public readonly CompIdx CompType = compType;
    public readonly T Comp = comp;

    public void Dispose()
    {
      MetaDataComponent component = this._entMan.GetComponent<MetaDataComponent>(this._owner);
      if (!component.EntityInitialized && !component.EntityInitializing)
        return;
      if (!this.Comp.Initialized)
        ((EntityManager) this._entMan).LifeInitialize<T>(this._owner, this.Comp, this.CompType);
      if (!component.EntityInitialized || this.Comp.Running)
        return;
      ((EntityManager) this._entMan).LifeStartup<T>(this._owner, this.Comp, this.CompType);
    }

    public static implicit operator T(EntityManager.CompInitializeHandle<T> handle) => handle.Comp;
  }

  public delegate void TerminatingEventHandler(ref EntityTerminatingEvent ev);
}
