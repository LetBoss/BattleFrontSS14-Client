// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Systems.SharedBodySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DragDrop;
using Content.Shared.FixedPoint;
using Content.Shared.Gibbing.Components;
using Content.Shared.Gibbing.Events;
using Content.Shared.Gibbing.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Body.Systems;

public abstract class SharedBodySystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private GibbingSystem _gibbingSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  private const float GibletLaunchImpulse = 8f;
  private const float GibletLaunchImpulseVariance = 3f;
  public const string PartSlotContainerIdPrefix = "body_part_slot_";
  public const string BodyRootContainerId = "body_root_part";
  public const string OrganSlotContainerIdPrefix = "body_organ_slot_";
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  protected IPrototypeManager Prototypes;
  [Dependency]
  protected DamageableSystem Damageable;
  [Dependency]
  protected MovementSpeedModifierSystem Movement;
  [Dependency]
  protected SharedContainerSystem Containers;
  [Dependency]
  protected SharedTransformSystem SharedTransform;
  [Dependency]
  protected StandingStateSystem Standing;
  private static readonly ProtoId<DamageTypePrototype> BloodlossDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Bloodloss");

  private void InitializeBody()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<BodyComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnBodyInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<BodyComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnBodyRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyComponent, ComponentInit>(new EntityEventRefHandler<BodyComponent, ComponentInit>((object) this, __methodptr(OnBodyInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyComponent, MapInitEvent>(new EntityEventRefHandler<BodyComponent, MapInitEvent>((object) this, __methodptr(OnBodyMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyComponent, CanDragEvent>(new EntityEventRefHandler<BodyComponent, CanDragEvent>((object) this, __methodptr(OnBodyCanDrag)), (Type[]) null, (Type[]) null);
  }

  private void OnBodyInserted(Entity<BodyComponent> ent, ref EntInsertedIntoContainerMessage args)
  {
    string id = ((ContainerModifiedMessage) args).Container.ID;
    if (id != "body_root_part")
      return;
    EntityUid entity = ((ContainerModifiedMessage) args).Entity;
    BodyPartComponent bodyPartComponent;
    if (this.TryComp<BodyPartComponent>(entity, ref bodyPartComponent))
    {
      this.AddPart(Entity<BodyComponent>.op_Implicit((Entity<BodyComponent>.op_Implicit(ent), Entity<BodyComponent>.op_Implicit(ent))), Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), id);
      this.RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), new EntityUid?(Entity<BodyComponent>.op_Implicit(ent)));
    }
    OrganComponent organComponent;
    if (!this.TryComp<OrganComponent>(entity, ref organComponent))
      return;
    this.AddOrgan(Entity<OrganComponent>.op_Implicit((entity, organComponent)), Entity<BodyComponent>.op_Implicit(ent), Entity<BodyComponent>.op_Implicit(ent));
  }

  private void OnBodyRemoved(Entity<BodyComponent> ent, ref EntRemovedFromContainerMessage args)
  {
    string id = ((ContainerModifiedMessage) args).Container.ID;
    if (id != "body_root_part")
      return;
    EntityUid entity = ((ContainerModifiedMessage) args).Entity;
    BodyPartComponent bodyPartComponent;
    if (this.TryComp<BodyPartComponent>(entity, ref bodyPartComponent))
    {
      this.RemovePart(Entity<BodyComponent>.op_Implicit((Entity<BodyComponent>.op_Implicit(ent), Entity<BodyComponent>.op_Implicit(ent))), Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), id);
      this.RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), new EntityUid?());
    }
    OrganComponent organComponent;
    if (!this.TryComp<OrganComponent>(entity, ref organComponent))
      return;
    this.RemoveOrgan(Entity<OrganComponent>.op_Implicit((entity, organComponent)), Entity<BodyComponent>.op_Implicit(ent));
  }

  private void OnBodyInit(Entity<BodyComponent> ent, ref ComponentInit args)
  {
    ent.Comp.RootContainer = this.Containers.EnsureContainer<ContainerSlot>(Entity<BodyComponent>.op_Implicit(ent), "body_root_part", (ContainerManagerComponent) null);
  }

  private void OnBodyMapInit(Entity<BodyComponent> ent, ref MapInitEvent args)
  {
    if (!ent.Comp.Prototype.HasValue)
      return;
    BodyPrototype prototype = this.Prototypes.Index<BodyPrototype>(ent.Comp.Prototype.Value);
    this.MapInitBody(Entity<BodyComponent>.op_Implicit(ent), prototype);
  }

  private void MapInitBody(EntityUid bodyEntity, BodyPrototype prototype)
  {
    BodyPrototypeSlot slot = prototype.Slots[prototype.Root];
    if (!slot.Part.HasValue)
      return;
    EntProtoId? part = slot.Part;
    EntityUid rootPartId = this.SpawnInContainerOrDrop(part.HasValue ? EntProtoId.op_Implicit(part.GetValueOrDefault()) : (string) null, bodyEntity, "body_root_part", (TransformComponent) null, (ContainerManagerComponent) null, (ComponentRegistry) null);
    BodyPartComponent bodyPartComponent = this.Comp<BodyPartComponent>(rootPartId);
    bodyPartComponent.Body = new EntityUid?(bodyEntity);
    this.Dirty(rootPartId, (IComponent) bodyPartComponent, (MetaDataComponent) null);
    this.SetupOrgans(Entity<BodyPartComponent>.op_Implicit((rootPartId, bodyPartComponent)), slot.Organs);
    this.MapInitParts(rootPartId, prototype);
  }

  private void OnBodyCanDrag(Entity<BodyComponent> ent, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void MapInitParts(EntityUid rootPartId, BodyPrototype prototype)
  {
    string root = prototype.Root;
    Queue<string> stringQueue = new Queue<string>();
    stringQueue.Enqueue(root);
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    dictionary1[root] = root;
    Dictionary<string, EntityUid> dictionary2 = new Dictionary<string, EntityUid>();
    dictionary2[root] = rootPartId;
    string result;
    while (stringQueue.TryDequeue(out result))
    {
      foreach (string connection in prototype.Slots[result].Connections)
      {
        if (dictionary1.TryAdd(connection, result))
        {
          BodyPrototypeSlot slot = prototype.Slots[connection];
          EntityUid partUid = dictionary2[result];
          BodyPartComponent part1 = this.Comp<BodyPartComponent>(partUid);
          EntProtoId? part2 = slot.Part;
          EntityUid entityUid = this.Spawn(part2.HasValue ? EntProtoId.op_Implicit(part2.GetValueOrDefault()) : (string) null, new EntityCoordinates(partUid, Vector2.Zero));
          dictionary2[connection] = entityUid;
          BodyPartComponent bodyPartComponent = this.Comp<BodyPartComponent>(entityUid);
          BodyPartSlot? partSlot = this.CreatePartSlot(partUid, connection, bodyPartComponent.PartType, part1);
          BaseContainer container = this.Containers.GetContainer(partUid, SharedBodySystem.GetPartSlotContainerId(connection), (ContainerManagerComponent) null);
          if (!partSlot.HasValue || !this.Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entityUid), container, (TransformComponent) null, false))
          {
            this.Log.Error($"Could not create slot for connection {connection} in body {prototype.ID}");
            this.QueueDel(new EntityUid?(entityUid));
          }
          else
          {
            this.SetupOrgans(Entity<BodyPartComponent>.op_Implicit((entityUid, bodyPartComponent)), slot.Organs);
            stringQueue.Enqueue(connection);
          }
        }
      }
    }
  }

  private void SetupOrgans(Entity<BodyPartComponent> ent, Dictionary<string, string> organs)
  {
    foreach ((string str1, string str2) in organs)
    {
      OrganSlot? organSlot = this.CreateOrganSlot(Entity<BodyPartComponent>.op_Implicit((Entity<BodyPartComponent>.op_Implicit(ent), Entity<BodyPartComponent>.op_Implicit(ent))), str1);
      this.SpawnInContainerOrDrop(str2, Entity<BodyPartComponent>.op_Implicit(ent), SharedBodySystem.GetOrganContainerId(str1), (TransformComponent) null, (ContainerManagerComponent) null, (ComponentRegistry) null);
      if (!organSlot.HasValue)
        this.Log.Error($"Could not create organ for slot {str1} in {this.ToPrettyString(new EntityUid?(Entity<BodyPartComponent>.op_Implicit(ent)), (MetaDataComponent) null)}");
    }
  }

  public IEnumerable<BaseContainer> GetBodyContainers(
    EntityUid id,
    BodyComponent? body = null,
    BodyPartComponent? rootPart = null)
  {
    SharedBodySystem sharedBodySystem1 = this;
    if (sharedBodySystem1.Resolve<BodyComponent>(id, ref body, false))
    {
      EntityUid? containedEntity = body.RootContainer.ContainedEntity;
      if (containedEntity.HasValue)
      {
        SharedBodySystem sharedBodySystem2 = sharedBodySystem1;
        containedEntity = body.RootContainer.ContainedEntity;
        EntityUid entityUid = containedEntity.Value;
        ref BodyPartComponent local = ref rootPart;
        if (sharedBodySystem2.Resolve<BodyPartComponent>(entityUid, ref local, true))
        {
          yield return (BaseContainer) body.RootContainer;
          foreach (BaseContainer partContainer in sharedBodySystem1.GetPartContainers(body.RootContainer.ContainedEntity.Value, rootPart))
            yield return partContainer;
        }
      }
    }
  }

  public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildren(
    EntityUid? id,
    BodyComponent? body = null,
    BodyPartComponent? rootPart = null)
  {
    SharedBodySystem sharedBodySystem1 = this;
    if (id.HasValue && sharedBodySystem1.Resolve<BodyComponent>(id.Value, ref body, false))
    {
      EntityUid? containedEntity = body.RootContainer.ContainedEntity;
      if (containedEntity.HasValue)
      {
        SharedBodySystem sharedBodySystem2 = sharedBodySystem1;
        containedEntity = body.RootContainer.ContainedEntity;
        EntityUid entityUid = containedEntity.Value;
        ref BodyPartComponent local = ref rootPart;
        if (sharedBodySystem2.Resolve<BodyPartComponent>(entityUid, ref local, true))
        {
          SharedBodySystem sharedBodySystem3 = sharedBodySystem1;
          containedEntity = body.RootContainer.ContainedEntity;
          EntityUid partId = containedEntity.Value;
          BodyPartComponent part = rootPart;
          foreach ((EntityUid, BodyPartComponent) bodyPartChild in sharedBodySystem3.GetBodyPartChildren(partId, part))
            yield return bodyPartChild;
        }
      }
    }
  }

  public IEnumerable<(EntityUid Id, OrganComponent Component)> GetBodyOrgans(
    EntityUid? bodyId,
    BodyComponent? body = null)
  {
    SharedBodySystem sharedBodySystem = this;
    if (bodyId.HasValue && sharedBodySystem.Resolve<BodyComponent>(bodyId.Value, ref body, false))
    {
      foreach ((EntityUid, BodyPartComponent) bodyChild in sharedBodySystem.GetBodyChildren(bodyId, body))
      {
        IEnumerator<(EntityUid, OrganComponent)> enumerator = sharedBodySystem.GetPartOrgans(bodyChild.Item1, bodyChild.Item2).GetEnumerator();
        while (enumerator.MoveNext())
          yield return enumerator.Current;
        enumerator = (IEnumerator<(EntityUid, OrganComponent)>) null;
      }
    }
  }

  public IEnumerable<BodyPartSlot> GetBodyAllSlots(EntityUid bodyId, BodyComponent? body = null)
  {
    SharedBodySystem sharedBodySystem1 = this;
    if (sharedBodySystem1.Resolve<BodyComponent>(bodyId, ref body, false))
    {
      EntityUid? containedEntity = body.RootContainer.ContainedEntity;
      if (containedEntity.HasValue)
      {
        SharedBodySystem sharedBodySystem2 = sharedBodySystem1;
        containedEntity = body.RootContainer.ContainedEntity;
        EntityUid partId = containedEntity.Value;
        foreach (BodyPartSlot allBodyPartSlot in sharedBodySystem2.GetAllBodyPartSlots(partId))
          yield return allBodyPartSlot;
      }
    }
  }

  public virtual HashSet<EntityUid> GibBody(
    EntityUid bodyId,
    bool gibOrgans = false,
    BodyComponent? body = null,
    bool launchGibs = true,
    Vector2? splatDirection = null,
    float splatModifier = 1f,
    Angle splatCone = default (Angle),
    SoundSpecifier? gibSoundOverride = null)
  {
    HashSet<EntityUid> droppedEntities = new HashSet<EntityUid>();
    if (!this.Resolve<BodyComponent>(bodyId, ref body, false))
      return droppedEntities;
    (EntityUid Entity, BodyPartComponent BodyPart)? rootPartOrNull = this.GetRootPartOrNull(bodyId, body);
    GibbableComponent gibbableComponent;
    if (rootPartOrNull.HasValue && this.TryComp<GibbableComponent>(rootPartOrNull.Value.Entity, ref gibbableComponent) && gibSoundOverride == null)
      gibSoundOverride = gibbableComponent.GibSound;
    (EntityUid Id, BodyPartComponent Component)[] array = this.GetBodyChildren(new EntityUid?(bodyId), body).ToArray<(EntityUid, BodyPartComponent)>();
    droppedEntities.EnsureCapacity(array.Length);
    foreach ((EntityUid Id, BodyPartComponent Component) tuple in array)
    {
      this._gibbingSystem.TryGibEntityWithRef(Entity<TransformComponent>.op_Implicit(bodyId), Entity<GibbableComponent>.op_Implicit(tuple.Id), GibType.Gib, GibContentsOption.Skip, ref droppedEntities, launchDirection: splatDirection, launchImpulse: 8f * splatModifier, launchImpulseVariance: 3f, launchCone: splatCone, playAudio: false);
      if (gibOrgans)
      {
        foreach ((EntityUid Id, OrganComponent Component) partOrgan in this.GetPartOrgans(tuple.Id, tuple.Component))
        {
          GibbingSystem gibbingSystem = this._gibbingSystem;
          Entity<TransformComponent> outerEntity = Entity<TransformComponent>.op_Implicit(bodyId);
          Entity<GibbableComponent> gibbable = Entity<GibbableComponent>.op_Implicit(partOrgan.Id);
          ref HashSet<EntityUid> local = ref droppedEntities;
          float num = 8f * splatModifier;
          Angle angle = splatCone;
          Vector2? launchDirection = new Vector2?();
          double launchImpulse = (double) num;
          Angle launchCone = angle;
          gibbingSystem.TryGibEntityWithRef(outerEntity, gibbable, GibType.Drop, GibContentsOption.Skip, ref local, launchDirection: launchDirection, launchImpulse: (float) launchImpulse, launchImpulseVariance: 3f, launchCone: launchCone, playAudio: false);
        }
      }
    }
    TransformComponent transformComponent = this.Transform(bodyId);
    InventoryComponent inventoryComponent;
    if (this.TryComp<InventoryComponent>(bodyId, ref inventoryComponent))
    {
      foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(bodyId)))
      {
        this.SharedTransform.DropNextTo(Entity<TransformComponent>.op_Implicit(orInventoryEntity), Entity<TransformComponent>.op_Implicit((bodyId, transformComponent)));
        droppedEntities.Add(orInventoryEntity);
      }
    }
    this._audioSystem.PlayPredicted(gibSoundOverride, transformComponent.Coordinates, new EntityUid?(), new AudioParams?());
    return droppedEntities;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeBody();
    this.InitializeParts();
  }

  protected static string? GetPartSlotContainerIdFromContainer(string containerSlotId)
  {
    int startIndex = containerSlotId.IndexOf("body_part_slot_", StringComparison.Ordinal);
    return startIndex < 0 ? (string) null : containerSlotId.Remove(startIndex, "body_part_slot_".Length);
  }

  public static string GetPartSlotContainerId(string slotId) => "body_part_slot_" + slotId;

  public static string GetOrganContainerId(string slotId) => "body_organ_slot_" + slotId;

  private void AddOrgan(
    Entity<OrganComponent> organEnt,
    EntityUid bodyUid,
    EntityUid parentPartUid)
  {
    organEnt.Comp.Body = new EntityUid?(bodyUid);
    OrganAddedEvent organAddedEvent = new OrganAddedEvent(parentPartUid);
    this.RaiseLocalEvent<OrganAddedEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref organAddedEvent, false);
    if (organEnt.Comp.Body.HasValue)
    {
      OrganAddedToBodyEvent addedToBodyEvent = new OrganAddedToBodyEvent(bodyUid, parentPartUid);
      this.RaiseLocalEvent<OrganAddedToBodyEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref addedToBodyEvent, false);
    }
    this.Dirty(Entity<OrganComponent>.op_Implicit(organEnt), (IComponent) organEnt.Comp, (MetaDataComponent) null);
  }

  private void RemoveOrgan(Entity<OrganComponent> organEnt, EntityUid parentPartUid)
  {
    OrganRemovedEvent organRemovedEvent = new OrganRemovedEvent(parentPartUid);
    this.RaiseLocalEvent<OrganRemovedEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref organRemovedEvent, false);
    EntityUid? body = organEnt.Comp.Body;
    if (body.HasValue)
    {
      EntityUid valueOrDefault = body.GetValueOrDefault();
      if (((EntityUid) ref valueOrDefault).Valid)
      {
        OrganRemovedFromBodyEvent removedFromBodyEvent = new OrganRemovedFromBodyEvent(valueOrDefault, parentPartUid);
        this.RaiseLocalEvent<OrganRemovedFromBodyEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref removedFromBodyEvent, false);
      }
    }
    organEnt.Comp.Body = new EntityUid?();
    this.Dirty(Entity<OrganComponent>.op_Implicit(organEnt), (IComponent) organEnt.Comp, (MetaDataComponent) null);
  }

  private OrganSlot? CreateOrganSlot(Entity<BodyPartComponent?> parentEnt, string slotId)
  {
    if (!this.Resolve<BodyPartComponent>(Entity<BodyPartComponent>.op_Implicit(parentEnt), ref parentEnt.Comp, false))
      return new OrganSlot?();
    this.Containers.EnsureContainer<ContainerSlot>(Entity<BodyPartComponent>.op_Implicit(parentEnt), SharedBodySystem.GetOrganContainerId(slotId), (ContainerManagerComponent) null);
    OrganSlot organSlot = new OrganSlot(slotId);
    parentEnt.Comp.Organs.Add(slotId, organSlot);
    return new OrganSlot?(organSlot);
  }

  public bool TryCreateOrganSlot(
    EntityUid? parent,
    string slotId,
    [NotNullWhen(true)] out OrganSlot? slot,
    BodyPartComponent? part = null)
  {
    slot = new OrganSlot?();
    if (!parent.HasValue || !this.Resolve<BodyPartComponent>(parent.Value, ref part, false))
      return false;
    this.Containers.EnsureContainer<ContainerSlot>(parent.Value, SharedBodySystem.GetOrganContainerId(slotId), (ContainerManagerComponent) null);
    slot = new OrganSlot?(new OrganSlot(slotId));
    return part.Organs.TryAdd(slotId, slot.Value);
  }

  public bool CanInsertOrgan(EntityUid partId, string slotId, BodyPartComponent? part = null)
  {
    return this.Resolve<BodyPartComponent>(partId, ref part, true) && part.Organs.ContainsKey(slotId);
  }

  public bool CanInsertOrgan(EntityUid partId, OrganSlot slot, BodyPartComponent? part = null)
  {
    return this.CanInsertOrgan(partId, slot.Id, part);
  }

  public bool InsertOrgan(
    EntityUid partId,
    EntityUid organId,
    string slotId,
    BodyPartComponent? part = null,
    OrganComponent? organ = null)
  {
    if (!this.Resolve<OrganComponent>(organId, ref organ, false) || !this.Resolve<BodyPartComponent>(partId, ref part, false) || !this.CanInsertOrgan(partId, slotId, part))
      return false;
    string organContainerId = SharedBodySystem.GetOrganContainerId(slotId);
    BaseContainer baseContainer;
    return this.Containers.TryGetContainer(partId, organContainerId, ref baseContainer, (ContainerManagerComponent) null) && this.Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(organId), baseContainer, (TransformComponent) null, false);
  }

  public bool RemoveOrgan(EntityUid organId, OrganComponent? organ = null)
  {
    BaseContainer baseContainer;
    return this.Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((organId, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer) && this.HasComp<BodyPartComponent>(baseContainer.Owner) && this.Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(organId), baseContainer, true, false, new EntityCoordinates?(), new Angle?());
  }

  public bool AddOrganToFirstValidSlot(
    EntityUid partId,
    EntityUid organId,
    BodyPartComponent? part = null,
    OrganComponent? organ = null)
  {
    if (!this.Resolve<BodyPartComponent>(partId, ref part, false) || !this.Resolve<OrganComponent>(organId, ref organ, false))
      return false;
    using (Dictionary<string, OrganSlot>.KeyCollection.Enumerator enumerator = part.Organs.Keys.GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        this.InsertOrgan(partId, organId, current, part, organ);
        return true;
      }
    }
    return false;
  }

  public List<Entity<T, OrganComponent>> GetBodyOrganEntityComps<T>(Entity<BodyComponent?> entity) where T : IComponent
  {
    if (!this.Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(entity), ref entity.Comp, true))
      return new List<Entity<T, OrganComponent>>();
    EntityQuery<T> entityQuery = this.GetEntityQuery<T>();
    List<Entity<T, OrganComponent>> organEntityComps = new List<Entity<T, OrganComponent>>(3);
    foreach ((EntityUid Id, OrganComponent Component) bodyOrgan in this.GetBodyOrgans(new EntityUid?(entity.Owner), entity.Comp))
    {
      T obj;
      if (entityQuery.TryGetComponent(bodyOrgan.Id, ref obj))
        organEntityComps.Add(Entity<T, OrganComponent>.op_Implicit((bodyOrgan.Id, obj, bodyOrgan.Component)));
    }
    return organEntityComps;
  }

  public bool TryGetBodyOrganEntityComps<T>(
    Entity<BodyComponent?> entity,
    [NotNullWhen(true)] out List<Entity<T, OrganComponent>>? comps)
    where T : IComponent
  {
    if (!this.Resolve<BodyComponent>(entity.Owner, ref entity.Comp, true))
    {
      comps = (List<Entity<T, OrganComponent>>) null;
      return false;
    }
    comps = this.GetBodyOrganEntityComps<T>(entity);
    if (comps.Count != 0)
      return true;
    comps = (List<Entity<T, OrganComponent>>) null;
    return false;
  }

  private void InitializeParts()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyPartComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<BodyPartComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnBodyPartInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BodyPartComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<BodyPartComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnBodyPartRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnBodyPartInserted(
    Entity<BodyPartComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    EntityUid entity = ((ContainerModifiedMessage) args).Entity;
    string id = ((ContainerModifiedMessage) args).Container.ID;
    if (!ent.Comp.Body.HasValue)
      return;
    BodyPartComponent bodyPartComponent;
    if (this.TryComp<BodyPartComponent>(entity, ref bodyPartComponent))
    {
      this.AddPart(Entity<BodyComponent>.op_Implicit(ent.Comp.Body.Value), Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), id);
      this.RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), new EntityUid?(ent.Comp.Body.Value));
    }
    OrganComponent organComponent;
    if (!this.TryComp<OrganComponent>(entity, ref organComponent))
      return;
    this.AddOrgan(Entity<OrganComponent>.op_Implicit((entity, organComponent)), ent.Comp.Body.Value, Entity<BodyPartComponent>.op_Implicit(ent));
  }

  private void OnBodyPartRemoved(
    Entity<BodyPartComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    EntityUid entity = ((ContainerModifiedMessage) args).Entity;
    string id = ((ContainerModifiedMessage) args).Container.ID;
    BodyPartComponent bodyPartComponent;
    if (this.TryComp<BodyPartComponent>(entity, ref bodyPartComponent))
    {
      EntityUid? nullable = bodyPartComponent.Body;
      if (nullable.HasValue)
      {
        this.RemovePart(Entity<BodyComponent>.op_Implicit(bodyPartComponent.Body.Value), Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent)), id);
        Entity<BodyPartComponent> ent1 = Entity<BodyPartComponent>.op_Implicit((entity, bodyPartComponent));
        nullable = new EntityUid?();
        EntityUid? bodyUid = nullable;
        this.RecursiveBodyUpdate(ent1, bodyUid);
      }
    }
    OrganComponent organComponent;
    if (!this.TryComp<OrganComponent>(entity, ref organComponent))
      return;
    this.RemoveOrgan(Entity<OrganComponent>.op_Implicit((entity, organComponent)), Entity<BodyPartComponent>.op_Implicit(ent));
  }

  private void RecursiveBodyUpdate(Entity<BodyPartComponent> ent, EntityUid? bodyUid)
  {
    ent.Comp.Body = bodyUid;
    this.Dirty(Entity<BodyPartComponent>.op_Implicit(ent), (IComponent) ent.Comp, (MetaDataComponent) null);
    foreach (string key in ent.Comp.Organs.Keys)
    {
      BaseContainer baseContainer;
      if (this.Containers.TryGetContainer(Entity<BodyPartComponent>.op_Implicit(ent), SharedBodySystem.GetOrganContainerId(key), ref baseContainer, (ContainerManagerComponent) null))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
        {
          OrganComponent organComponent;
          if (this.TryComp<OrganComponent>(containedEntity, ref organComponent))
          {
            this.Dirty(containedEntity, (IComponent) organComponent, (MetaDataComponent) null);
            EntityUid? body = organComponent.Body;
            if (body.HasValue)
            {
              EntityUid valueOrDefault = body.GetValueOrDefault();
              if (((EntityUid) ref valueOrDefault).Valid)
              {
                OrganRemovedFromBodyEvent removedFromBodyEvent = new OrganRemovedFromBodyEvent(valueOrDefault, Entity<BodyPartComponent>.op_Implicit(ent));
                this.RaiseLocalEvent<OrganRemovedFromBodyEvent>(containedEntity, ref removedFromBodyEvent, false);
              }
            }
            organComponent.Body = bodyUid;
            if (bodyUid.HasValue)
            {
              OrganAddedToBodyEvent addedToBodyEvent = new OrganAddedToBodyEvent(bodyUid.Value, Entity<BodyPartComponent>.op_Implicit(ent));
              this.RaiseLocalEvent<OrganAddedToBodyEvent>(containedEntity, ref addedToBodyEvent, false);
            }
          }
        }
      }
    }
    foreach (string key in ent.Comp.Children.Keys)
    {
      BaseContainer baseContainer;
      if (this.Containers.TryGetContainer(Entity<BodyPartComponent>.op_Implicit(ent), SharedBodySystem.GetPartSlotContainerId(key), ref baseContainer, (ContainerManagerComponent) null))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
        {
          BodyPartComponent bodyPartComponent;
          if (this.TryComp<BodyPartComponent>(containedEntity, ref bodyPartComponent))
            this.RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((containedEntity, bodyPartComponent)), bodyUid);
        }
      }
    }
  }

  protected virtual void AddPart(
    Entity<BodyComponent?> bodyEnt,
    Entity<BodyPartComponent> partEnt,
    string slotId)
  {
    this.Dirty(Entity<BodyPartComponent>.op_Implicit(partEnt), (IComponent) partEnt.Comp, (MetaDataComponent) null);
    partEnt.Comp.Body = new EntityUid?(Entity<BodyComponent>.op_Implicit(bodyEnt));
    BodyPartAddedEvent bodyPartAddedEvent = new BodyPartAddedEvent(slotId, partEnt);
    this.RaiseLocalEvent<BodyPartAddedEvent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyPartAddedEvent, false);
    this.AddLeg(partEnt, bodyEnt);
  }

  protected virtual void RemovePart(
    Entity<BodyComponent?> bodyEnt,
    Entity<BodyPartComponent> partEnt,
    string slotId)
  {
    this.Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false);
    this.Dirty(Entity<BodyPartComponent>.op_Implicit(partEnt), (IComponent) partEnt.Comp, (MetaDataComponent) null);
    partEnt.Comp.Body = new EntityUid?();
    BodyPartRemovedEvent partRemovedEvent = new BodyPartRemovedEvent(slotId, partEnt);
    this.RaiseLocalEvent<BodyPartRemovedEvent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref partRemovedEvent, false);
    this.RemoveLeg(partEnt, bodyEnt);
    this.PartRemoveDamage(bodyEnt, partEnt);
  }

  private void AddLeg(Entity<BodyPartComponent> legEnt, Entity<BodyComponent?> bodyEnt)
  {
    if (!this.Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false) || legEnt.Comp.PartType != BodyPartType.Leg)
      return;
    bodyEnt.Comp.LegEntities.Add(Entity<BodyPartComponent>.op_Implicit(legEnt));
    this.UpdateMovementSpeed(Entity<BodyComponent>.op_Implicit(bodyEnt));
    this.Dirty(Entity<BodyComponent>.op_Implicit(bodyEnt), (IComponent) bodyEnt.Comp, (MetaDataComponent) null);
  }

  private void RemoveLeg(Entity<BodyPartComponent> legEnt, Entity<BodyComponent?> bodyEnt)
  {
    if (!this.Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false) || legEnt.Comp.PartType != BodyPartType.Leg)
      return;
    bodyEnt.Comp.LegEntities.Remove(Entity<BodyPartComponent>.op_Implicit(legEnt));
    this.UpdateMovementSpeed(Entity<BodyComponent>.op_Implicit(bodyEnt));
    this.Dirty(Entity<BodyComponent>.op_Implicit(bodyEnt), (IComponent) bodyEnt.Comp, (MetaDataComponent) null);
    if (bodyEnt.Comp.LegEntities.Any<EntityUid>())
      return;
    this.Standing.Down(Entity<BodyComponent>.op_Implicit(bodyEnt));
  }

  private void PartRemoveDamage(Entity<BodyComponent?> bodyEnt, Entity<BodyPartComponent> partEnt)
  {
    if (!this.Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false) || this._timing.ApplyingState || !partEnt.Comp.IsVital || this.GetBodyChildrenOfType(Entity<BodyComponent>.op_Implicit(bodyEnt), partEnt.Comp.PartType, bodyEnt.Comp).Any<(EntityUid, BodyPartComponent)>())
      return;
    DamageSpecifier damage = new DamageSpecifier(this.Prototypes.Index<DamageTypePrototype>(SharedBodySystem.BloodlossDamageType), (FixedPoint2) 300);
    this.Damageable.TryChangeDamage(new EntityUid?(Entity<BodyComponent>.op_Implicit(bodyEnt)), damage);
  }

  public EntityUid? GetParentPartOrNull(EntityUid uid)
  {
    BaseContainer baseContainer;
    if (!this.Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
      return new EntityUid?();
    EntityUid owner = baseContainer.Owner;
    return !this.HasComp<BodyPartComponent>(owner) ? new EntityUid?() : new EntityUid?(owner);
  }

  public (EntityUid Parent, string Slot)? GetParentPartAndSlotOrNull(EntityUid uid)
  {
    BaseContainer baseContainer;
    if (!this.Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
      return new (EntityUid, string)?();
    string containerIdFromContainer = SharedBodySystem.GetPartSlotContainerIdFromContainer(baseContainer.ID);
    if (string.IsNullOrEmpty(containerIdFromContainer))
      return new (EntityUid, string)?();
    EntityUid owner = baseContainer.Owner;
    BodyPartComponent bodyPartComponent;
    return !this.TryComp<BodyPartComponent>(owner, ref bodyPartComponent) || !bodyPartComponent.Children.ContainsKey(containerIdFromContainer) ? new (EntityUid, string)?() : new (EntityUid, string)?((owner, containerIdFromContainer));
  }

  public bool TryGetParentBodyPart(
    EntityUid partUid,
    [NotNullWhen(true)] out EntityUid? parentUid,
    [NotNullWhen(true)] out BodyPartComponent? parentComponent)
  {
    parentUid = new EntityUid?();
    parentComponent = (BodyPartComponent) null;
    BaseContainer baseContainer;
    if (!this.Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((partUid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer) || !this.TryComp<BodyPartComponent>(baseContainer.Owner, ref parentComponent))
      return false;
    parentUid = new EntityUid?(baseContainer.Owner);
    return true;
  }

  private BodyPartSlot? CreatePartSlot(
    EntityUid partUid,
    string slotId,
    BodyPartType partType,
    BodyPartComponent? part = null)
  {
    if (!this.Resolve<BodyPartComponent>(partUid, ref part, false))
      return new BodyPartSlot?();
    this.Containers.EnsureContainer<ContainerSlot>(partUid, SharedBodySystem.GetPartSlotContainerId(slotId), (ContainerManagerComponent) null);
    BodyPartSlot bodyPartSlot = new BodyPartSlot(slotId, partType);
    part.Children.Add(slotId, bodyPartSlot);
    this.Dirty(partUid, (IComponent) part, (MetaDataComponent) null);
    return new BodyPartSlot?(bodyPartSlot);
  }

  public bool TryCreatePartSlot(
    EntityUid? partId,
    string slotId,
    BodyPartType partType,
    [NotNullWhen(true)] out BodyPartSlot? slot,
    BodyPartComponent? part = null)
  {
    slot = new BodyPartSlot?();
    if (!partId.HasValue || !this.Resolve<BodyPartComponent>(partId.Value, ref part, false))
      return false;
    this.Containers.EnsureContainer<ContainerSlot>(partId.Value, SharedBodySystem.GetPartSlotContainerId(slotId), (ContainerManagerComponent) null);
    slot = new BodyPartSlot?(new BodyPartSlot(slotId, partType));
    if (!part.Children.TryAdd(slotId, slot.Value))
      return false;
    this.Dirty(partId.Value, (IComponent) part, (MetaDataComponent) null);
    return true;
  }

  public bool TryCreatePartSlotAndAttach(
    EntityUid parentId,
    string slotId,
    EntityUid childId,
    BodyPartType partType,
    BodyPartComponent? parent = null,
    BodyPartComponent? child = null)
  {
    return this.TryCreatePartSlot(new EntityUid?(parentId), slotId, partType, out BodyPartSlot? _, parent) && this.AttachPart(parentId, slotId, childId, parent, child);
  }

  public bool IsPartRoot(
    EntityUid bodyId,
    EntityUid partId,
    BodyComponent? body = null,
    BodyPartComponent? part = null)
  {
    BaseContainer baseContainer;
    return this.Resolve<BodyPartComponent>(partId, ref part, true) && this.Resolve<BodyComponent>(bodyId, ref body, true) && this.Containers.TryGetContainingContainer(bodyId, partId, ref baseContainer, (ContainerManagerComponent) null) && baseContainer.ID == "body_root_part";
  }

  public bool CanAttachToRoot(
    EntityUid bodyId,
    EntityUid partId,
    BodyComponent? body = null,
    BodyPartComponent? part = null)
  {
    if (!this.Resolve<BodyComponent>(bodyId, ref body, true) || !this.Resolve<BodyPartComponent>(partId, ref part, true) || body.RootContainer.ContainedEntity.HasValue)
      return false;
    EntityUid entityUid = bodyId;
    EntityUid? body1 = part.Body;
    return !body1.HasValue || EntityUid.op_Inequality(entityUid, body1.GetValueOrDefault());
  }

  public (EntityUid Entity, BodyPartComponent BodyPart)? GetRootPartOrNull(
    EntityUid bodyId,
    BodyComponent? body = null)
  {
    return !this.Resolve<BodyComponent>(bodyId, ref body, true) || !body.RootContainer.ContainedEntity.HasValue ? new (EntityUid, BodyPartComponent)?() : new (EntityUid, BodyPartComponent)?((body.RootContainer.ContainedEntity.Value, this.Comp<BodyPartComponent>(body.RootContainer.ContainedEntity.Value)));
  }

  public bool CanAttachPart(
    EntityUid parentId,
    BodyPartSlot slot,
    EntityUid partId,
    BodyPartComponent? parentPart = null,
    BodyPartComponent? part = null)
  {
    return this.Resolve<BodyPartComponent>(partId, ref part, false) && this.Resolve<BodyPartComponent>(parentId, ref parentPart, false) && this.CanAttachPart(parentId, slot.Id, partId, parentPart, part);
  }

  public bool CanAttachPart(
    EntityUid parentId,
    string slotId,
    EntityUid partId,
    BodyPartComponent? parentPart = null,
    BodyPartComponent? part = null)
  {
    BodyPartSlot bodyPartSlot;
    BaseContainer baseContainer;
    return this.Resolve<BodyPartComponent>(partId, ref part, false) && this.Resolve<BodyPartComponent>(parentId, ref parentPart, false) && parentPart.Children.TryGetValue(slotId, out bodyPartSlot) && part.PartType == bodyPartSlot.Type && this.Containers.TryGetContainer(parentId, SharedBodySystem.GetPartSlotContainerId(slotId), ref baseContainer, (ContainerManagerComponent) null) && this.Containers.CanInsert(partId, baseContainer, false, (TransformComponent) null);
  }

  public bool AttachPartToRoot(
    EntityUid bodyId,
    EntityUid partId,
    BodyComponent? body = null,
    BodyPartComponent? part = null)
  {
    return this.Resolve<BodyComponent>(bodyId, ref body, true) && this.Resolve<BodyPartComponent>(partId, ref part, true) && this.CanAttachToRoot(bodyId, partId, body, part) && this.Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(partId), (BaseContainer) body.RootContainer, (TransformComponent) null, false);
  }

  public bool AttachPart(
    EntityUid parentPartId,
    string slotId,
    EntityUid partId,
    BodyPartComponent? parentPart = null,
    BodyPartComponent? part = null)
  {
    BodyPartSlot slot;
    return this.Resolve<BodyPartComponent>(parentPartId, ref parentPart, false) && parentPart.Children.TryGetValue(slotId, out slot) && this.AttachPart(parentPartId, slot, partId, parentPart, part);
  }

  public bool AttachPart(
    EntityUid parentPartId,
    BodyPartSlot slot,
    EntityUid partId,
    BodyPartComponent? parentPart = null,
    BodyPartComponent? part = null)
  {
    BaseContainer baseContainer;
    return this.Resolve<BodyPartComponent>(parentPartId, ref parentPart, false) && this.Resolve<BodyPartComponent>(partId, ref part, false) && this.CanAttachPart(parentPartId, slot.Id, partId, parentPart, part) && parentPart.Children.ContainsKey(slot.Id) && this.Containers.TryGetContainer(parentPartId, SharedBodySystem.GetPartSlotContainerId(slot.Id), ref baseContainer, (ContainerManagerComponent) null) && this.Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(partId), baseContainer, (TransformComponent) null, false);
  }

  public void UpdateMovementSpeed(
    EntityUid bodyId,
    BodyComponent? body = null,
    MovementSpeedModifierComponent? movement = null)
  {
    if (!this.Resolve<BodyComponent, MovementSpeedModifierComponent>(bodyId, ref body, ref movement, false) || body.RequiredLegs <= 0)
      return;
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    foreach (EntityUid legEntity in body.LegEntities)
    {
      MovementBodyPartComponent bodyPartComponent;
      if (this.TryComp<MovementBodyPartComponent>(legEntity, ref bodyPartComponent))
      {
        num1 += bodyPartComponent.WalkSpeed;
        num2 += bodyPartComponent.SprintSpeed;
        num3 += bodyPartComponent.Acceleration;
      }
    }
    float baseWalkSpeed = num1 / (float) body.RequiredLegs;
    float baseSprintSpeed = num2 / (float) body.RequiredLegs;
    float acceleration = num3 / (float) body.RequiredLegs;
    this.Movement.ChangeBaseSpeed(bodyId, baseWalkSpeed, baseSprintSpeed, acceleration, movement);
  }

  public IEnumerable<(EntityUid Id, OrganComponent Component)> GetPartOrgans(
    EntityUid partId,
    BodyPartComponent? part = null)
  {
    SharedBodySystem sharedBodySystem = this;
    if (sharedBodySystem.Resolve<BodyPartComponent>(partId, ref part, false))
    {
      foreach (string key in part.Organs.Keys)
      {
        string organContainerId = SharedBodySystem.GetOrganContainerId(key);
        BaseContainer baseContainer;
        if (sharedBodySystem.Containers.TryGetContainer(partId, organContainerId, ref baseContainer, (ContainerManagerComponent) null))
        {
          IEnumerator<EntityUid> enumerator = baseContainer.ContainedEntities.GetEnumerator();
          while (enumerator.MoveNext())
          {
            EntityUid current = enumerator.Current;
            OrganComponent organComponent;
            if (sharedBodySystem.TryComp<OrganComponent>(current, ref organComponent))
              yield return (current, organComponent);
          }
          enumerator = (IEnumerator<EntityUid>) null;
        }
      }
    }
  }

  public IEnumerable<BaseContainer> GetPartContainers(EntityUid id, BodyPartComponent? part = null)
  {
    SharedBodySystem sharedBodySystem = this;
    if (sharedBodySystem.Resolve<BodyPartComponent>(id, ref part, false) && part.Children.Count != 0)
    {
      foreach (string key in part.Children.Keys)
      {
        string partSlotContainerId = SharedBodySystem.GetPartSlotContainerId(key);
        BaseContainer container;
        if (sharedBodySystem.Containers.TryGetContainer(id, partSlotContainerId, ref container, (ContainerManagerComponent) null))
        {
          yield return container;
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
          {
            IEnumerator<BaseContainer> enumerator = sharedBodySystem.GetPartContainers(containedEntity).GetEnumerator();
            while (enumerator.MoveNext())
              yield return enumerator.Current;
            enumerator = (IEnumerator<BaseContainer>) null;
          }
          container = (BaseContainer) null;
        }
      }
    }
  }

  public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyPartChildren(
    EntityUid partId,
    BodyPartComponent? part = null)
  {
    SharedBodySystem sharedBodySystem = this;
    if (sharedBodySystem.Resolve<BodyPartComponent>(partId, ref part, false))
    {
      yield return (partId, part);
      foreach (string key in part.Children.Keys)
      {
        string partSlotContainerId = SharedBodySystem.GetPartSlotContainerId(key);
        BaseContainer baseContainer;
        if (sharedBodySystem.Containers.TryGetContainer(partId, partSlotContainerId, ref baseContainer, (ContainerManagerComponent) null))
        {
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
          {
            BodyPartComponent part1;
            if (sharedBodySystem.TryComp<BodyPartComponent>(containedEntity, ref part1))
            {
              IEnumerator<(EntityUid, BodyPartComponent)> enumerator = sharedBodySystem.GetBodyPartChildren(containedEntity, part1).GetEnumerator();
              while (enumerator.MoveNext())
                yield return enumerator.Current;
              enumerator = (IEnumerator<(EntityUid, BodyPartComponent)>) null;
            }
          }
        }
      }
    }
  }

  public IEnumerable<BodyPartSlot> GetAllBodyPartSlots(EntityUid partId, BodyPartComponent? part = null)
  {
    SharedBodySystem sharedBodySystem = this;
    if (sharedBodySystem.Resolve<BodyPartComponent>(partId, ref part, false))
    {
      foreach ((string str, BodyPartSlot allBodyPartSlot) in part.Children)
      {
        yield return allBodyPartSlot;
        string organContainerId = SharedBodySystem.GetOrganContainerId(str);
        BaseContainer baseContainer;
        if (sharedBodySystem.Containers.TryGetContainer(partId, organContainerId, ref baseContainer, (ContainerManagerComponent) null))
        {
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
          {
            BodyPartComponent part1;
            if (sharedBodySystem.TryComp<BodyPartComponent>(containedEntity, ref part1))
            {
              IEnumerator<BodyPartSlot> enumerator = sharedBodySystem.GetAllBodyPartSlots(containedEntity, part1).GetEnumerator();
              while (enumerator.MoveNext())
                yield return enumerator.Current;
              enumerator = (IEnumerator<BodyPartSlot>) null;
            }
          }
        }
        str = (string) null;
      }
    }
  }

  public bool BodyHasPartType(EntityUid bodyId, BodyPartType type, BodyComponent? body = null)
  {
    return this.GetBodyChildrenOfType(bodyId, type, body).Any<(EntityUid, BodyPartComponent)>();
  }

  public bool PartHasChild(
    EntityUid parentId,
    EntityUid childId,
    BodyPartComponent? parent,
    BodyPartComponent? child)
  {
    if (!this.Resolve<BodyPartComponent>(parentId, ref parent, false) || !this.Resolve<BodyPartComponent>(childId, ref child, false))
      return false;
    foreach ((EntityUid Id, BodyPartComponent Component) bodyPartChild in this.GetBodyPartChildren(parentId, parent))
    {
      if (EntityUid.op_Equality(bodyPartChild.Id, childId))
        return true;
    }
    return false;
  }

  public bool BodyHasChild(
    EntityUid bodyId,
    EntityUid partId,
    BodyComponent? body = null,
    BodyPartComponent? part = null)
  {
    BodyPartComponent parent;
    return this.Resolve<BodyComponent>(bodyId, ref body, false) && body.RootContainer.ContainedEntity.HasValue && this.Resolve<BodyPartComponent>(partId, ref part, false) && this.TryComp<BodyPartComponent>(body.RootContainer.ContainedEntity, ref parent) && this.PartHasChild(body.RootContainer.ContainedEntity.Value, partId, parent, part);
  }

  public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildrenOfType(
    EntityUid bodyId,
    BodyPartType type,
    BodyComponent? body = null)
  {
    foreach ((EntityUid, BodyPartComponent) bodyChild in this.GetBodyChildren(new EntityUid?(bodyId), body))
    {
      if (bodyChild.Item2.PartType == type)
        yield return bodyChild;
    }
  }

  public List<(T Comp, OrganComponent Organ)> GetBodyPartOrganComponents<T>(
    EntityUid uid,
    BodyPartComponent? part = null)
    where T : IComponent
  {
    if (!this.Resolve<BodyPartComponent>(uid, ref part, true))
      return new List<(T, OrganComponent)>();
    EntityQuery<T> entityQuery = this.GetEntityQuery<T>();
    List<(T, OrganComponent)> partOrganComponents = new List<(T, OrganComponent)>();
    foreach ((EntityUid Id, OrganComponent Component) partOrgan in this.GetPartOrgans(uid, part))
    {
      T obj;
      if (entityQuery.TryGetComponent(partOrgan.Id, ref obj))
        partOrganComponents.Add((obj, partOrgan.Component));
    }
    return partOrganComponents;
  }

  public bool TryGetBodyPartOrganComponents<T>(
    EntityUid uid,
    [NotNullWhen(true)] out List<(T Comp, OrganComponent Organ)>? comps,
    BodyPartComponent? part = null)
    where T : IComponent
  {
    if (!this.Resolve<BodyPartComponent>(uid, ref part, true))
    {
      comps = (List<(T, OrganComponent)>) null;
      return false;
    }
    comps = this.GetBodyPartOrganComponents<T>(uid, part);
    if (comps.Count != 0)
      return true;
    comps = (List<(T, OrganComponent)>) null;
    return false;
  }

  public IEnumerable<EntityUid> GetBodyPartAdjacentParts(EntityUid partId, BodyPartComponent? part = null)
  {
    SharedBodySystem sharedBodySystem = this;
    if (sharedBodySystem.Resolve<BodyPartComponent>(partId, ref part, false))
    {
      EntityUid? parentUid;
      if (sharedBodySystem.TryGetParentBodyPart(partId, out parentUid, out BodyPartComponent _))
        yield return parentUid.Value;
      foreach (string key in part.Children.Keys)
      {
        IEnumerator<EntityUid> enumerator = sharedBodySystem.Containers.GetContainer(partId, SharedBodySystem.GetPartSlotContainerId(key), (ContainerManagerComponent) null).ContainedEntities.GetEnumerator();
        while (enumerator.MoveNext())
          yield return enumerator.Current;
        enumerator = (IEnumerator<EntityUid>) null;
      }
    }
  }

  public IEnumerable<(EntityUid AdjacentId, T Component)> GetBodyPartAdjacentPartsComponents<T>(
    EntityUid partId,
    BodyPartComponent? part = null)
    where T : IComponent
  {
    SharedBodySystem sharedBodySystem = this;
    if (sharedBodySystem.Resolve<BodyPartComponent>(partId, ref part, false))
    {
      EntityQuery<T> query = sharedBodySystem.GetEntityQuery<T>();
      foreach (EntityUid partAdjacentPart in sharedBodySystem.GetBodyPartAdjacentParts(partId, part))
      {
        T obj;
        if (query.TryGetComponent(partAdjacentPart, ref obj))
          yield return (partAdjacentPart, obj);
      }
    }
  }

  public bool TryGetBodyPartAdjacentPartsComponents<T>(
    EntityUid partId,
    [NotNullWhen(true)] out List<(EntityUid AdjacentId, T Component)>? comps,
    BodyPartComponent? part = null)
    where T : IComponent
  {
    if (!this.Resolve<BodyPartComponent>(partId, ref part, false))
    {
      comps = (List<(EntityUid, T)>) null;
      return false;
    }
    EntityQuery<T> entityQuery = this.GetEntityQuery<T>();
    comps = new List<(EntityUid, T)>();
    foreach (EntityUid partAdjacentPart in this.GetBodyPartAdjacentParts(partId, part))
    {
      T obj;
      if (entityQuery.TryGetComponent(partAdjacentPart, ref obj))
        comps.Add((partAdjacentPart, obj));
    }
    if (comps.Count != 0)
      return true;
    comps = (List<(EntityUid, T)>) null;
    return false;
  }
}
