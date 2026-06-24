// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.SharedContainerSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Containers;

public abstract class SharedContainerSystem : EntitySystem
{
  [Dependency]
  private readonly IDynamicTypeFactoryInternal _dynFactory;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  [Dependency]
  private readonly EntityLookupSystem _lookup;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly SharedJointSystem _joint;
  [Dependency]
  private readonly IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<ContainerManagerComponent> _managerQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapComponent> _mapQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> MetaQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> PhysicsQuery;
  protected Robust.Shared.GameObjects.EntityQuery<JointComponent> JointQuery;
  protected Robust.Shared.GameObjects.EntityQuery<TransformComponent> TransformQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EntParentChangedMessage>(new EntityEventRefHandler<EntParentChangedMessage>(this.OnParentChanged));
    this.SubscribeLocalEvent<ContainerManagerComponent, ComponentInit>(new EntityEventRefHandler<ContainerManagerComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<ContainerManagerComponent, ComponentStartup>(new ComponentEventHandler<ContainerManagerComponent, ComponentStartup>(this.OnStartupValidation));
    this.SubscribeLocalEvent<ContainerManagerComponent, ComponentGetState>(new ComponentEventRefHandler<ContainerManagerComponent, ComponentGetState>(this.OnContainerGetState));
    this.SubscribeLocalEvent<ContainerManagerComponent, ComponentRemove>(new ComponentEventHandler<ContainerManagerComponent, ComponentRemove>(this.OnContainerManagerRemove));
    this._managerQuery = this.GetEntityQuery<ContainerManagerComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._mapQuery = this.GetEntityQuery<MapComponent>();
    this.MetaQuery = this.GetEntityQuery<MetaDataComponent>();
    this.PhysicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.JointQuery = this.GetEntityQuery<JointComponent>();
    this.TransformQuery = this.GetEntityQuery<TransformComponent>();
  }

  private void OnInit(Entity<ContainerManagerComponent> ent, ref ComponentInit args)
  {
    foreach ((string str, BaseContainer baseContainer) in ent.Comp.Containers)
      baseContainer.Init(this, str, ent);
  }

  private void OnContainerGetState(
    EntityUid uid,
    ContainerManagerComponent component,
    ref ComponentGetState args)
  {
    Dictionary<string, ContainerManagerComponent.ContainerManagerComponentState.ContainerData> containers = new Dictionary<string, ContainerManagerComponent.ContainerManagerComponentState.ContainerData>(component.Containers.Count);
    foreach (BaseContainer baseContainer in component.Containers.Values)
    {
      NetEntity[] containedEntities = new NetEntity[baseContainer.ContainedEntities.Count];
      for (int index = 0; index < baseContainer.ContainedEntities.Count; ++index)
        containedEntities[index] = this.GetNetEntity(baseContainer.ContainedEntities[index]);
      ContainerManagerComponent.ContainerManagerComponentState.ContainerData containerData = new ContainerManagerComponent.ContainerManagerComponentState.ContainerData(baseContainer.GetType().Name, baseContainer.ShowContents, baseContainer.OccludesLight, containedEntities);
      containers.Add(baseContainer.ID, containerData);
    }
    args.State = (IComponentState) new ContainerManagerComponent.ContainerManagerComponentState(containers);
  }

  private void OnContainerManagerRemove(
    EntityUid uid,
    ContainerManagerComponent component,
    ComponentRemove args)
  {
    foreach (BaseContainer container in component.Containers.Values)
      this.ShutdownContainer(container);
    component.Containers.Clear();
  }

  public T MakeContainer<T>(EntityUid uid, string id, ContainerManagerComponent? containerManager = null) where T : BaseContainer
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager, false))
      containerManager = this.AddComp<ContainerManagerComponent>(uid);
    if (this.HasContainer(uid, id, containerManager))
      throw new ArgumentException($"Container with specified ID already exists: '{id}'");
    T instanceUnchecked = this._dynFactory.CreateInstanceUnchecked<T>(typeof (T), inject: false);
    instanceUnchecked.Init(this, id, (Entity<ContainerManagerComponent>) (uid, containerManager));
    containerManager.Containers[id] = (BaseContainer) instanceUnchecked;
    this.Dirty(uid, (IComponent) containerManager);
    return instanceUnchecked;
  }

  public virtual void ShutdownContainer(BaseContainer container)
  {
    container.InternalShutdown((IEntityManager) this.EntityManager, this, this._net.IsClient);
    container.Manager.Containers.Remove(container.ID);
    container.ExpectedEntities.Clear();
  }

  public T EnsureContainer<T>(
    EntityUid uid,
    string id,
    out bool alreadyExisted,
    ContainerManagerComponent? containerManager = null)
    where T : BaseContainer
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager, false))
      containerManager = this.AddComp<ContainerManagerComponent>(uid);
    BaseContainer container;
    if (this.TryGetContainer(uid, id, out container, containerManager))
    {
      alreadyExisted = true;
      return container is T obj ? obj : throw new InvalidOperationException($"The container exists but is of a different type: {container.GetType()}");
    }
    alreadyExisted = false;
    return this.MakeContainer<T>(uid, id, containerManager);
  }

  public T EnsureContainer<T>(EntityUid uid, string id, ContainerManagerComponent? containerManager = null) where T : BaseContainer
  {
    return this.EnsureContainer<T>(uid, id, out bool _, containerManager);
  }

  public BaseContainer GetContainer(
    EntityUid uid,
    string id,
    ContainerManagerComponent? containerManager = null)
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager))
      throw new ArgumentException("Entity does not have a ContainerManagerComponent!", nameof (uid));
    return containerManager.Containers[id];
  }

  public bool HasContainer(EntityUid uid, string id, ContainerManagerComponent? containerManager)
  {
    return this.Resolve<ContainerManagerComponent>(uid, ref containerManager, false) && containerManager.Containers.ContainsKey(id);
  }

  public bool TryGetContainer(
    EntityUid uid,
    string id,
    [NotNullWhen(true)] out BaseContainer? container,
    ContainerManagerComponent? containerManager = null)
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager, false))
    {
      container = (BaseContainer) null;
      return false;
    }
    return containerManager.Containers.TryGetValue(id, out container);
  }

  public bool TryGetContainingContainer(
    EntityUid uid,
    EntityUid containedUid,
    [NotNullWhen(true)] out BaseContainer? container,
    ContainerManagerComponent? containerManager = null)
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager, false))
    {
      container = (BaseContainer) null;
      return false;
    }
    foreach (BaseContainer baseContainer in containerManager.Containers.Values)
    {
      if (baseContainer.Contains(containedUid))
      {
        container = baseContainer;
        return true;
      }
    }
    container = (BaseContainer) null;
    return false;
  }

  public bool ContainsEntity(
    EntityUid uid,
    EntityUid containedUid,
    ContainerManagerComponent? containerManager = null)
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager, false))
      return false;
    foreach (BaseContainer baseContainer in containerManager.Containers.Values)
    {
      if (baseContainer.Contains(containedUid))
        return true;
    }
    return false;
  }

  public bool RemoveEntity(
    EntityUid uid,
    EntityUid toremove,
    ContainerManagerComponent? containerManager = null,
    TransformComponent? containedXform = null,
    MetaDataComponent? containedMeta = null,
    bool reparent = true,
    bool force = false,
    EntityCoordinates? destination = null,
    Angle? localRotation = null)
  {
    if (!this.Resolve<ContainerManagerComponent>(uid, ref containerManager) || !this.Resolve<MetaDataComponent, TransformComponent>(toremove, ref containedMeta, ref containedXform))
      return false;
    foreach (BaseContainer container in containerManager.Containers.Values)
    {
      if (container.Contains(toremove))
        return this.Remove((Entity<TransformComponent, MetaDataComponent>) (toremove, containedXform, containedMeta), container, reparent, force, destination, localRotation);
    }
    return true;
  }

  public ContainerManagerComponent.AllContainersEnumerable GetAllContainers(
    EntityUid uid,
    ContainerManagerComponent? containerManager = null)
  {
    return !this.Resolve<ContainerManagerComponent>(uid, ref containerManager) ? new ContainerManagerComponent.AllContainersEnumerable() : new ContainerManagerComponent.AllContainersEnumerable(containerManager);
  }

  public bool TryGetContainingContainer(
    Entity<TransformComponent?, MetaDataComponent?> ent,
    [NotNullWhen(true)] out BaseContainer? container)
  {
    container = (BaseContainer) null;
    return this.Resolve((EntityUid) ent, ref ent.Comp2, false) && (ent.Comp2.Flags & MetaDataFlags.InContainer) != MetaDataFlags.None && this.Resolve((EntityUid) ent, ref ent.Comp1, false) && this.TryGetContainingContainer(ent.Comp1.ParentUid, (EntityUid) ent, out container);
  }

  public bool IsEntityInContainer(EntityUid uid, MetaDataComponent? meta = null)
  {
    return this.Resolve(uid, ref meta, false) && (meta.Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer;
  }

  public bool IsEntityOrParentInContainer(
    EntityUid uid,
    MetaDataComponent? meta = null,
    TransformComponent? xform = null)
  {
    if (!this.MetaQuery.Resolve(uid, ref meta))
      return false;
    if ((meta.Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer)
      return true;
    return this.TransformQuery.Resolve(uid, ref xform) && xform.ParentUid.Valid && this.IsEntityOrParentInContainer(xform.ParentUid);
  }

  public bool TryFindComponentOnEntityContainerOrParent<T>(
    EntityUid uid,
    Robust.Shared.GameObjects.EntityQuery<T> entityQuery,
    [NotNullWhen(true)] ref T? foundComponent,
    MetaDataComponent? meta = null,
    TransformComponent? xform = null)
    where T : IComponent
  {
    if (!this.MetaQuery.Resolve(uid, ref meta) || (meta.Flags & MetaDataFlags.InContainer) != MetaDataFlags.InContainer || !this.TransformQuery.Resolve(uid, ref xform) || !xform.ParentUid.Valid)
      return false;
    return entityQuery.TryComp(xform.ParentUid, out foundComponent) || this.TryFindComponentOnEntityContainerOrParent<T>(xform.ParentUid, entityQuery, ref foundComponent);
  }

  public bool TryFindComponentsOnEntityContainerOrParent<T>(
    EntityUid uid,
    Robust.Shared.GameObjects.EntityQuery<T> entityQuery,
    List<T> foundComponents,
    MetaDataComponent? meta = null,
    TransformComponent? xform = null)
    where T : IComponent
  {
    if (!this.MetaQuery.Resolve(uid, ref meta))
      return foundComponents.Any<T>();
    if ((meta.Flags & MetaDataFlags.InContainer) != MetaDataFlags.InContainer)
      return foundComponents.Any<T>();
    if (!this.TransformQuery.Resolve(uid, ref xform))
      return foundComponents.Any<T>();
    if (!xform.ParentUid.Valid)
      return foundComponents.Any<T>();
    T comp;
    if (this.TryComp<T>(xform.ParentUid, out comp))
      foundComponents.Add(comp);
    return this.TryFindComponentsOnEntityContainerOrParent<T>(xform.ParentUid, entityQuery, foundComponents);
  }

  public bool IsInSameOrNoContainer(
    Entity<TransformComponent?, MetaDataComponent?> user,
    Entity<TransformComponent?, MetaDataComponent?> other)
  {
    BaseContainer container1;
    bool containingContainer1 = this.TryGetContainingContainer(user, out container1);
    BaseContainer container2;
    bool containingContainer2 = this.TryGetContainingContainer(other, out container2);
    if (!containingContainer1 && !containingContainer2)
      return true;
    return containingContainer1 == containingContainer2 && container1 == container2;
  }

  public bool IsInSameOrParentContainer(
    Entity<TransformComponent?, MetaDataComponent?> user,
    Entity<TransformComponent?, MetaDataComponent?> other)
  {
    return this.IsInSameOrParentContainer(user, other, out BaseContainer _, out BaseContainer _);
  }

  public bool IsInSameOrParentContainer(
    Entity<TransformComponent?, MetaDataComponent?> user,
    Entity<TransformComponent?, MetaDataComponent?> other,
    out BaseContainer? userContainer,
    out BaseContainer? otherContainer)
  {
    bool containingContainer1 = this.TryGetContainingContainer(user, out userContainer);
    bool containingContainer2 = this.TryGetContainingContainer(other, out otherContainer);
    if (!containingContainer1 && !containingContainer2)
      return true;
    EntityUid? owner = userContainer?.Owner;
    EntityUid entityUid1 = (EntityUid) other;
    if ((owner.HasValue ? (owner.GetValueOrDefault() == entityUid1 ? 1 : 0) : 0) == 0)
    {
      owner = otherContainer?.Owner;
      EntityUid entityUid2 = (EntityUid) user;
      if ((owner.HasValue ? (owner.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) == 0)
        return containingContainer1 == containingContainer2 && userContainer == otherContainer;
    }
    return true;
  }

  public bool IsInSameOrTransparentContainer(
    Entity<TransformComponent?, MetaDataComponent?> user,
    Entity<TransformComponent?, MetaDataComponent?> other,
    BaseContainer? userContainer = null,
    BaseContainer? otherContainer = null,
    bool userSeeInsideSelf = false)
  {
    if (userContainer == null)
      this.TryGetContainingContainer(user, out userContainer);
    if (otherContainer == null)
      this.TryGetContainingContainer(other, out otherContainer);
    if (userContainer == otherContainer)
      return true;
    EntityUid? owner1 = userContainer?.Owner;
    EntityUid entityUid1 = (EntityUid) other;
    if ((owner1.HasValue ? (owner1.GetValueOrDefault() == entityUid1 ? 1 : 0) : 0) != 0)
      return true;
    if (userSeeInsideSelf)
    {
      EntityUid? owner2 = otherContainer?.Owner;
      EntityUid entityUid2 = (EntityUid) user;
      if ((owner2.HasValue ? (owner2.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) != 0)
        return true;
    }
    if (userContainer != null && userContainer.ShowContents)
      return this.IsInSameOrTransparentContainer((Entity<TransformComponent, MetaDataComponent>) (userContainer.Owner, (TransformComponent) null, (MetaDataComponent) null), other, otherContainer: otherContainer);
    return otherContainer != null && otherContainer.ShowContents && this.IsInSameOrTransparentContainer(user, (Entity<TransformComponent, MetaDataComponent>) (otherContainer.Owner, (TransformComponent) null, (MetaDataComponent) null), userContainer, userSeeInsideSelf: userSeeInsideSelf);
  }

  public IEnumerable<BaseContainer> GetContainingContainers(Entity<TransformComponent?> ent)
  {
    SharedContainerSystem sharedContainerSystem = this;
    if (ent.Owner.IsValid() && sharedContainerSystem.Resolve((EntityUid) ent, ref ent.Comp))
    {
      EntityUid entityUid = ent.Owner;
      TransformComponent component1;
      for (EntityUid parent = ent.Comp.ParentUid; parent.IsValid(); parent = component1.ParentUid)
      {
        ContainerManagerComponent component2;
        BaseContainer container;
        if ((sharedContainerSystem.MetaQuery.GetComponent(entityUid).Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer && sharedContainerSystem._managerQuery.TryGetComponent(parent, out component2) && sharedContainerSystem.TryGetContainingContainer(parent, entityUid, out container, component2))
          yield return container;
        component1 = sharedContainerSystem.TransformQuery.GetComponent(parent);
        entityUid = parent;
      }
    }
  }

  public bool TryGetOuterContainer(
    EntityUid uid,
    TransformComponent xform,
    [NotNullWhen(true)] out BaseContainer? container)
  {
    return this.TryGetOuterContainer(uid, xform, out container, this.TransformQuery);
  }

  public bool TryGetOuterContainer(
    EntityUid uid,
    TransformComponent xform,
    [NotNullWhen(true)] out BaseContainer? container,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    container = (BaseContainer) null;
    if (!uid.IsValid())
      return false;
    EntityUid entityUid = uid;
    TransformComponent component1;
    for (EntityUid parentUid = xform.ParentUid; parentUid.IsValid(); parentUid = component1.ParentUid)
    {
      ContainerManagerComponent component2;
      BaseContainer container1;
      if ((this.MetaQuery.GetComponent(entityUid).Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer && this._managerQuery.TryGetComponent(parentUid, out component2) && this.TryGetContainingContainer(parentUid, entityUid, out container1, component2))
        container = container1;
      component1 = xformQuery.GetComponent(parentUid);
      entityUid = parentUid;
    }
    return container != null;
  }

  public bool TryRemoveFromContainer(
    Entity<TransformComponent?, MetaDataComponent?> entity,
    bool force,
    out bool wasInContainer)
  {
    BaseContainer container;
    if (this.TryGetContainingContainer(entity, out container))
    {
      wasInContainer = true;
      if (!force)
        return this.Remove(entity, container);
      this.Remove(entity, container, force: true);
      return true;
    }
    wasInContainer = false;
    return false;
  }

  public bool TryRemoveFromContainer(
    Entity<TransformComponent?, MetaDataComponent?> entity,
    bool force = false)
  {
    return this.TryRemoveFromContainer(entity, force, out bool _);
  }

  public List<EntityUid> EmptyContainer(
    BaseContainer container,
    bool force = false,
    EntityCoordinates? destination = null,
    bool reparent = true)
  {
    List<EntityUid> list = new List<EntityUid>((IEnumerable<EntityUid>) container.ContainedEntities);
    for (int index = list.Count - 1; index >= 0; --index)
    {
      if (!this.Remove((Entity<TransformComponent, MetaDataComponent>) list[index], container, reparent, force, destination))
        list.RemoveSwap<EntityUid>(index);
    }
    return list;
  }

  public void CleanContainer(BaseContainer container)
  {
    foreach (EntityUid entityUid in container.ContainedEntities.ToArray<EntityUid>())
    {
      if (!this.Deleted(entityUid))
      {
        this.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, container, force: true);
        this.PredictedDel((Entity<MetaDataComponent, TransformComponent>) entityUid);
      }
    }
  }

  public void AttachParentToContainerOrGrid(Entity<TransformComponent> transform)
  {
    BaseContainer container;
    if (transform.Comp.ParentUid.IsValid() && this.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (transform.Comp.ParentUid, this.Transform(transform.Comp.ParentUid)), out container) && this.TryInsertIntoContainer(transform, container))
      return;
    this._transform.AttachToGridOrMap((EntityUid) transform, transform.Comp);
  }

  private bool TryInsertIntoContainer(Entity<TransformComponent> transform, BaseContainer container)
  {
    if (this.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) (transform.Owner, transform.Comp, (MetaDataComponent) null, (PhysicsComponent) null), container))
      return true;
    TransformComponent transformComponent = this.Transform(container.Owner);
    BaseContainer container1;
    return transformComponent.ParentUid.IsValid() && this.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (container.Owner, transformComponent), out container1) && this.TryInsertIntoContainer(transform, container1);
  }

  internal bool TryGetManagerComp(EntityUid entity, [NotNullWhen(true)] out ContainerManagerComponent? manager)
  {
    if (this.TryComp<ContainerManagerComponent>(entity, out manager))
      return true;
    TransformComponent transformComponent = this.Transform(entity);
    return transformComponent.ParentUid.IsValid() && this.TryGetManagerComp(transformComponent.ParentUid, out manager);
  }

  protected virtual void OnParentChanged(ref EntParentChangedMessage message)
  {
    MetaDataComponent containedMeta = this.MetaData(message.Entity);
    ContainerManagerComponent comp;
    if ((containedMeta.Flags & MetaDataFlags.InContainer) == MetaDataFlags.None || !this.TryComp<ContainerManagerComponent>(message.OldParent, out comp))
      return;
    this.RemoveEntity(message.OldParent.Value, message.Entity, comp, message.Transform, containedMeta, false, true);
  }

  [Conditional("DEBUG")]
  [Access(new Type[] {typeof (BaseContainer)})]
  public void AssertInContainer(EntityUid uid, BaseContainer container)
  {
    if (this._timing.ApplyingState)
      return;
    int flags = (int) this.MetaData(uid).Flags;
  }

  public bool Insert(
    Entity<TransformComponent?, MetaDataComponent?, PhysicsComponent?> toInsert,
    BaseContainer container,
    TransformComponent? containerXform = null,
    bool force = false)
  {
    (EntityUid entityUid, TransformComponent comp1, MetaDataComponent comp2, PhysicsComponent component) = toInsert;
    if (!this.Resolve<TransformComponent, MetaDataComponent>(entityUid, ref comp1, ref comp2))
      return false;
    MetaDataComponent comp3;
    if (!this.TryComp(container.Owner, out comp3))
    {
      this.Log.Error($"Attempted to insert an entity {this.ToPrettyString(new EntityUid?((EntityUid) toInsert))} into a non-existent entity. Trace: {Environment.StackTrace}");
      this.QueueDel(new EntityUid?((EntityUid) toInsert));
      return false;
    }
    if (comp3.EntityLifeStage >= EntityLifeStage.Terminating)
    {
      this.Log.Error($"Attempted to insert an entity {this.ToPrettyString(new EntityUid?((EntityUid) toInsert))} into an entity that is terminating. Entity: {this.ToPrettyString((Entity<MetaDataComponent>) container.Owner)}. Trace: {Environment.StackTrace}");
      this.QueueDel(new EntityUid?((EntityUid) toInsert));
      return false;
    }
    if (!force && !this.CanInsert(entityUid, container, containerXform: containerXform))
      return false;
    if (comp2.EntityLifeStage >= EntityLifeStage.Terminating)
    {
      this.Log.Error($"Attempted to insert a terminating entity {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)} into a container {container.ID} in entity: {this.ToPrettyString((Entity<MetaDataComponent>) container.Owner)}. Trace: {Environment.StackTrace}");
      return false;
    }
    ContainerManagerComponent comp4;
    BaseContainer container1;
    if ((comp2.Flags & MetaDataFlags.InContainer) != MetaDataFlags.None && this.TryComp<ContainerManagerComponent>(comp1.ParentUid, out comp4) && this.TryGetContainingContainer(comp1.ParentUid, (EntityUid) toInsert, out container1, comp4) && !this.Remove((Entity<TransformComponent, MetaDataComponent>) (entityUid, comp1, comp2), container1, false))
      return false;
    comp2.Flags |= MetaDataFlags.InContainer;
    this._lookup.RemoveFromEntityTree((EntityUid) toInsert, comp1);
    BroadphaseData? broadphase = comp1.Broadphase;
    comp1.Broadphase = new BroadphaseData?(BroadphaseData.Invalid);
    this._transform.Unanchor((EntityUid) toInsert, comp1, false);
    this.PhysicsQuery.Resolve((EntityUid) toInsert, ref component, false);
    this.RecursivelyUpdatePhysics((Entity<TransformComponent, PhysicsComponent>) ((EntityUid) toInsert, comp1, component));
    EntityUid parentUid = comp1.ParentUid;
    this._transform.SetCoordinates((EntityUid) toInsert, comp1, new EntityCoordinates(container.Owner, Vector2.Zero), new Angle?(Angle.Zero));
    comp1.Broadphase = broadphase;
    container.InternalInsert((EntityUid) toInsert, (IEntityManager) this.EntityManager);
    this.RecursivelyUpdateJoints((Entity<TransformComponent>) ((EntityUid) toInsert, comp1));
    this.RaiseLocalEvent<EntInsertedIntoContainerMessage>(container.Owner, new EntInsertedIntoContainerMessage((EntityUid) toInsert, parentUid, container), true);
    this.RaiseLocalEvent<EntGotInsertedIntoContainerMessage>((EntityUid) toInsert, new EntGotInsertedIntoContainerMessage((EntityUid) toInsert, container), true);
    this.Dirty(container.Owner, (IComponent) container.Manager);
    return true;
  }

  public bool InsertOrDrop(
    Entity<TransformComponent?, MetaDataComponent?, PhysicsComponent?> toInsert,
    BaseContainer container,
    TransformComponent? containerXform = null)
  {
    if (!this.Resolve(toInsert.Owner, ref toInsert.Comp1) || !this.Resolve(container.Owner, ref containerXform))
      return false;
    if (this.Insert(toInsert, container, containerXform))
      return true;
    this._transform.DropNextTo((Entity<TransformComponent>) toInsert, (Entity<TransformComponent>) (container.Owner, containerXform));
    return false;
  }

  public bool CanInsert(
    EntityUid toInsert,
    BaseContainer container,
    bool assumeEmpty = false,
    TransformComponent? containerXform = null)
  {
    if (container.Owner == toInsert || !assumeEmpty && container.Contains(toInsert) || !container.CanInsert(toInsert, assumeEmpty, (IEntityManager) this.EntityManager) || this._mapQuery.HasComponent(toInsert) || this._gridQuery.HasComponent(toInsert) || this._transform.ContainsEntity(toInsert, (Entity<TransformComponent>) (container.Owner, containerXform)))
      return false;
    ContainerIsInsertingAttemptEvent args1 = new ContainerIsInsertingAttemptEvent(container, toInsert, assumeEmpty);
    this.RaiseLocalEvent<ContainerIsInsertingAttemptEvent>(container.Owner, args1, true);
    if (args1.Cancelled)
      return false;
    ContainerGettingInsertedAttemptEvent args2 = new ContainerGettingInsertedAttemptEvent(container, toInsert, assumeEmpty);
    this.RaiseLocalEvent<ContainerGettingInsertedAttemptEvent>(toInsert, args2, true);
    return !args2.Cancelled;
  }

  private void RecursivelyUpdatePhysics(
    Entity<TransformComponent, PhysicsComponent?> entity)
  {
    PhysicsComponent comp2 = entity.Comp2;
    if (comp2 != null)
    {
      this._physics.SetLinearVelocity((EntityUid) entity, Vector2.Zero, false, body: comp2);
      this._physics.SetAngularVelocity((EntityUid) entity, 0.0f, false, body: comp2);
      this._physics.SetCanCollide((EntityUid) entity, false, false, body: comp2);
    }
    foreach (EntityUid child in entity.Comp1._children)
    {
      TransformComponent component1 = this.TransformQuery.GetComponent(child);
      PhysicsComponent component2;
      this.PhysicsQuery.TryGetComponent(child, out component2);
      this.RecursivelyUpdatePhysics((Entity<TransformComponent, PhysicsComponent>) (child, component1, component2));
    }
  }

  internal void RecursivelyUpdateJoints(Entity<TransformComponent> entity)
  {
    if (this._timing.ApplyingState)
      return;
    JointComponent component1;
    if (this.JointQuery.TryGetComponent((EntityUid) entity, out component1))
      this._joint.RefreshRelay((EntityUid) entity, component1);
    foreach (EntityUid child in entity.Comp._children)
    {
      TransformComponent component2 = this.TransformQuery.GetComponent(child);
      this.RecursivelyUpdateJoints((Entity<TransformComponent>) (child, component2));
    }
  }

  public bool Remove(
    Entity<TransformComponent?, MetaDataComponent?> toRemove,
    BaseContainer container,
    bool reparent = true,
    bool force = false,
    EntityCoordinates? destination = null,
    Angle? localRotation = null)
  {
    (EntityUid entityUid, TransformComponent comp1, MetaDataComponent comp2) = toRemove;
    if (!this.Resolve<TransformComponent, MetaDataComponent>(entityUid, ref comp1, ref comp2) || !force && !this.CanRemove((EntityUid) toRemove, container) || force && !container.Contains((EntityUid) toRemove))
      return false;
    if (comp2.EntityLifeStage >= EntityLifeStage.Terminating && !force | reparent)
    {
      this.Log.Error($"Attempting to remove an entity from a container while it is terminating. Entity: {this.ToPrettyString((EntityUid) toRemove, comp2)}. Container: {this.ToPrettyString((Entity<MetaDataComponent>) container.Owner)}. Trace: {Environment.StackTrace}");
      return false;
    }
    comp2.Flags &= ~MetaDataFlags.InContainer;
    container.InternalRemove((EntityUid) toRemove, (IEntityManager) this.EntityManager);
    EntityUid parentUid = comp1.ParentUid;
    if (destination.HasValue)
      this._transform.SetCoordinates((EntityUid) toRemove, comp1, destination.Value, localRotation);
    else if (reparent)
    {
      this.AttachParentToContainerOrGrid((Entity<TransformComponent>) ((EntityUid) toRemove, comp1));
      if (localRotation.HasValue)
        this._transform.SetLocalRotation(entityUid, localRotation.Value, comp1);
    }
    if (comp1.ParentUid == parentUid && !comp1.Broadphase.HasValue)
      this._lookup.FindAndAddToEntityTree((EntityUid) toRemove, xform: comp1);
    this.RecursivelyUpdateJoints((Entity<TransformComponent>) ((EntityUid) toRemove, comp1));
    this.RaiseLocalEvent<EntRemovedFromContainerMessage>(container.Owner, new EntRemovedFromContainerMessage((EntityUid) toRemove, container), true);
    this.RaiseLocalEvent<EntGotRemovedFromContainerMessage>((EntityUid) toRemove, new EntGotRemovedFromContainerMessage((EntityUid) toRemove, container));
    this.Dirty(container.Owner, (IComponent) container.Manager);
    return true;
  }

  public bool CanRemove(EntityUid toRemove, BaseContainer container)
  {
    if (!container.Contains(toRemove))
      return false;
    ContainerIsRemovingAttemptEvent args1 = new ContainerIsRemovingAttemptEvent(container, toRemove);
    this.RaiseLocalEvent<ContainerIsRemovingAttemptEvent>(container.Owner, args1, true);
    if (args1.Cancelled)
      return false;
    ContainerGettingRemovedAttemptEvent args2 = new ContainerGettingRemovedAttemptEvent(container, toRemove);
    this.RaiseLocalEvent<ContainerGettingRemovedAttemptEvent>(toRemove, args2, true);
    return !args2.Cancelled;
  }

  private void OnStartupValidation(
    EntityUid uid,
    ContainerManagerComponent component,
    ComponentStartup args)
  {
    foreach (BaseContainer cont in component.Containers.Values)
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) cont.ContainedEntities)
      {
        MetaDataComponent component1;
        if (!this.MetaQuery.TryGetComponent(containedEntity, out component1))
        {
          this.ValidateMissingEntity(uid, cont, containedEntity);
        }
        else
        {
          TransformComponent component2 = this.TransformQuery.GetComponent(containedEntity);
          PhysicsComponent component3;
          this.PhysicsQuery.TryGetComponent(containedEntity, out component3);
          component1.Flags |= MetaDataFlags.InContainer;
          this._lookup.RemoveFromEntityTree(containedEntity, component2);
          this.RecursivelyUpdatePhysics((Entity<TransformComponent, PhysicsComponent>) (containedEntity, component2, component3));
          this.ValidateChildren(component2, this.TransformQuery, this.PhysicsQuery);
        }
      }
    }
  }

  protected abstract void ValidateMissingEntity(
    EntityUid uid,
    BaseContainer cont,
    EntityUid missing);

  private void ValidateChildren(
    TransformComponent xform,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery,
    Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> physicsQuery)
  {
    foreach (EntityUid child in xform._children)
    {
      TransformComponent component;
      if (xformQuery.TryGetComponent(child, out component))
        this.ValidateChildren(component, xformQuery, physicsQuery);
    }
  }
}
