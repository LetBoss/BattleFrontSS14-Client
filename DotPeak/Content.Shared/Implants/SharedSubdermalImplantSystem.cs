// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.SharedSubdermalImplantSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Implants;

public abstract class SharedSubdermalImplantSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  public const string BaseStorageId = "storagebase";
  private static readonly ProtoId<TagPrototype> MicroBombTag = (ProtoId<TagPrototype>) "MicroBomb";
  private static readonly ProtoId<TagPrototype> MacroBombTag = (ProtoId<TagPrototype>) "MacroBomb";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SubdermalImplantComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<SubdermalImplantComponent, EntGotInsertedIntoContainerMessage>(this.OnInsert));
    this.SubscribeLocalEvent<SubdermalImplantComponent, ContainerGettingRemovedAttemptEvent>(new ComponentEventHandler<SubdermalImplantComponent, ContainerGettingRemovedAttemptEvent>(this.OnRemoveAttempt));
    this.SubscribeLocalEvent<SubdermalImplantComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<SubdermalImplantComponent, EntGotRemovedFromContainerMessage>(this.OnRemove));
    this.SubscribeLocalEvent<ImplantedComponent, MobStateChangedEvent>(new ComponentEventHandler<ImplantedComponent, MobStateChangedEvent>(this.RelayToImplantEvent<MobStateChangedEvent>));
    this.SubscribeLocalEvent<ImplantedComponent, AfterInteractUsingEvent>(new ComponentEventHandler<ImplantedComponent, AfterInteractUsingEvent>(this.RelayToImplantEvent<AfterInteractUsingEvent>));
    this.SubscribeLocalEvent<ImplantedComponent, SuicideEvent>(new ComponentEventHandler<ImplantedComponent, SuicideEvent>(this.RelayToImplantEvent<SuicideEvent>));
  }

  private void OnInsert(
    EntityUid uid,
    SubdermalImplantComponent component,
    EntGotInsertedIntoContainerMessage args)
  {
    if (!component.ImplantedEntity.HasValue)
      return;
    EntProtoId? implantAction = component.ImplantAction;
    if (!string.IsNullOrWhiteSpace(implantAction.HasValue ? (string) implantAction.GetValueOrDefault() : (string) null))
    {
      SharedActionsSystem actionsSystem = this._actionsSystem;
      EntityUid performer = component.ImplantedEntity.Value;
      ref EntityUid? local = ref component.Action;
      implantAction = component.ImplantAction;
      string valueOrDefault = implantAction.HasValue ? (string) implantAction.GetValueOrDefault() : (string) null;
      EntityUid container = uid;
      actionsSystem.AddAction(performer, ref local, valueOrDefault, container);
    }
    BaseContainer container1;
    if (this._container.TryGetContainer(component.ImplantedEntity.Value, "implant", out container1) && this._tag.HasTag(uid, SharedSubdermalImplantSystem.MacroBombTag))
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container1.ContainedEntities)
      {
        if (this._tag.HasTag(containedEntity, SharedSubdermalImplantSystem.MicroBombTag))
        {
          this._container.Remove((Entity<TransformComponent, MetaDataComponent>) containedEntity, container1);
          this.PredictedQueueDel(containedEntity);
        }
      }
    }
    ImplantImplantedEvent args1 = new ImplantImplantedEvent(uid, new EntityUid?(component.ImplantedEntity.Value));
    this.RaiseLocalEvent<ImplantImplantedEvent>(uid, ref args1);
  }

  private void OnRemoveAttempt(
    EntityUid uid,
    SubdermalImplantComponent component,
    ContainerGettingRemovedAttemptEvent args)
  {
    if (!component.Permanent || !component.ImplantedEntity.HasValue)
      return;
    args.Cancel();
  }

  private void OnRemove(
    EntityUid uid,
    SubdermalImplantComponent component,
    EntGotRemovedFromContainerMessage args)
  {
    if (!component.ImplantedEntity.HasValue || this.Terminating(component.ImplantedEntity.Value))
      return;
    if (component.ImplantAction.HasValue)
      this._actionsSystem.RemoveProvidedActions(component.ImplantedEntity.Value, uid);
    BaseContainer container;
    if (!this._container.TryGetContainer(uid, "storagebase", out container))
      return;
    foreach (EntityUid entityUid in container.ContainedEntities.ToArray<EntityUid>())
      this._transformSystem.DropNextTo((Entity<TransformComponent>) entityUid, (Entity<TransformComponent>) uid);
  }

  public void AddImplants(EntityUid uid, IEnumerable<string> implants)
  {
    foreach (string implant in implants)
      this.AddImplant(uid, implant);
  }

  public EntityUid? AddImplant(EntityUid uid, string implantId)
  {
    EntityCoordinates coordinates = this.Transform(uid).Coordinates;
    EntityUid entityUid = this.Spawn(implantId, coordinates);
    SubdermalImplantComponent comp;
    if (this.TryComp<SubdermalImplantComponent>(entityUid, out comp))
    {
      this.ForceImplant(uid, entityUid, comp);
      return new EntityUid?(entityUid);
    }
    this.Log.Warning($"Found invalid starting implant '{implantId}' on {uid} {this.ToPrettyString((Entity<MetaDataComponent>) uid):implanted}");
    this.Del(new EntityUid?(entityUid));
    return new EntityUid?();
  }

  public void ForceImplant(
    EntityUid target,
    EntityUid implant,
    SubdermalImplantComponent component)
  {
    Container implantContainer = this.EnsureComp<ImplantedComponent>(target).ImplantContainer;
    component.ImplantedEntity = new EntityUid?(target);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) implant, (BaseContainer) implantContainer);
  }

  public void ForceRemove(EntityUid target, EntityUid implant)
  {
    ImplantedComponent comp;
    if (!this.TryComp<ImplantedComponent>(target, out comp))
      return;
    Container implantContainer = comp.ImplantContainer;
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) implant, (BaseContainer) implantContainer);
    this.QueueDel(new EntityUid?(implant));
  }

  public void WipeImplants(EntityUid target)
  {
    ImplantedComponent comp;
    if (!this.TryComp<ImplantedComponent>(target, out comp))
      return;
    this._container.CleanContainer((BaseContainer) comp.ImplantContainer);
  }

  private void RelayToImplantEvent<T>(EntityUid uid, ImplantedComponent component, T args) where T : notnull
  {
    BaseContainer container;
    if (!this._container.TryGetContainer(uid, "implant", out container))
      return;
    ImplantRelayEvent<T> args1 = new ImplantRelayEvent<T>(args);
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      if (args is HandledEntityEventArgs handledEntityEventArgs && handledEntityEventArgs.Handled)
        break;
      this.RaiseLocalEvent<ImplantRelayEvent<T>>(containedEntity, args1);
    }
  }
}
