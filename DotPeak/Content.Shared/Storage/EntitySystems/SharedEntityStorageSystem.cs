// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.SharedEntityStorageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Body.Components;
using Content.Shared.Destructible;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Content.Shared.Wall;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedEntityStorageSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedJointSystem _joints;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  protected SharedTransformSystem TransformSystem;
  [Dependency]
  private WeldableSystem _weldable;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  public const string ContainerName = "entity_storage";

  protected void OnEntityUnpausedEvent(
    EntityUid uid,
    SharedEntityStorageComponent component,
    EntityUnpausedEvent args)
  {
    component.NextInternalOpenAttempt += args.PausedTime;
  }

  protected void OnGetState(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new EntityStorageComponentState(component.Open, component.Capacity, component.IsCollidableWhenOpen, component.OpenOnMove, component.EnteringRange, component.NextInternalOpenAttempt);
  }

  protected void OnHandleState(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is EntityStorageComponentState current))
      return;
    component.Open = current.Open;
    component.Capacity = current.Capacity;
    component.IsCollidableWhenOpen = current.IsCollidableWhenOpen;
    component.OpenOnMove = current.OpenOnMove;
    component.EnteringRange = current.EnteringRange;
    component.NextInternalOpenAttempt = current.NextInternalOpenAttempt;
  }

  protected virtual void OnComponentInit(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ComponentInit args)
  {
    component.Contents = this._container.EnsureContainer<Container>(uid, "entity_storage");
    component.Contents.ShowContents = component.ShowContents;
    component.Contents.OccludesLight = component.OccludesLight;
  }

  protected virtual void OnComponentStartup(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ComponentStartup args)
  {
    this._appearance.SetData(uid, (Enum) StorageVisuals.Open, (object) component.Open);
  }

  protected void OnInteract(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    args.Handled = true;
    this.ToggleOpen(args.User, uid, component);
  }

  public abstract bool ResolveStorage(EntityUid uid, [NotNullWhen(true)] ref SharedEntityStorageComponent? component);

  protected void OnLockToggleAttempt(
    EntityUid uid,
    SharedEntityStorageComponent target,
    ref LockToggleAttemptEvent args)
  {
    if (target.Open)
      args.Cancelled = true;
    if (!target.Contents.Contains(args.User))
      return;
    args.Cancelled = true;
  }

  protected void OnDestruction(
    EntityUid uid,
    SharedEntityStorageComponent component,
    DestructionEventArgs args)
  {
    component.Open = true;
    this.Dirty(uid, (IComponent) component);
    if (!component.DeleteContentsOnDestruction)
    {
      this.EmptyContents(uid, component);
    }
    else
    {
      foreach (EntityUid entityUid in new List<EntityUid>((IEnumerable<EntityUid>) component.Contents.ContainedEntities))
        this.Del(new EntityUid?(entityUid));
    }
  }

  protected void OnRelayMovement(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ref ContainerRelayMovementEntityEvent args)
  {
    if (!this.HasComp<HandsComponent>(args.Entity) || !this._actionBlocker.CanMove(args.Entity) || this._timing.CurTime < component.NextInternalOpenAttempt)
      return;
    component.NextInternalOpenAttempt = this._timing.CurTime + SharedEntityStorageComponent.InternalOpenAttemptDelay;
    this.Dirty(uid, (IComponent) component);
    if (!component.OpenOnMove)
      return;
    this.TryOpenStorage(args.Entity, uid);
  }

  protected void OnFoldAttempt(
    EntityUid uid,
    SharedEntityStorageComponent component,
    ref FoldAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    args.Cancelled = component.Open || component.Contents.ContainedEntities.Count != 0;
  }

  protected void AddToggleOpenVerb(
    EntityUid uid,
    SharedEntityStorageComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !this.CanOpen(args.User, args.Target, true, component))
      return;
    InteractionVerb interactionVerb = new InteractionVerb();
    if (component.Open)
    {
      interactionVerb.Text = this.Loc.GetString("verb-common-close");
      interactionVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
    }
    else
    {
      interactionVerb.Text = this.Loc.GetString("verb-common-open");
      interactionVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
    }
    interactionVerb.Act = (Action) (() => this.ToggleOpen(args.User, args.Target, component));
    args.Verbs.Add(interactionVerb);
  }

  public void ToggleOpen(EntityUid user, EntityUid target, SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(target, ref component))
      return;
    if (component.Open)
    {
      if (this.HasComp<XenoComponent>(user))
        return;
      this.TryCloseStorage(target, new EntityUid?(user));
    }
    else
      this.TryOpenStorage(user, target);
  }

  public void EmptyContents(EntityUid uid, SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(uid, ref component))
      return;
    TransformComponent xform = this.Transform(uid);
    foreach (EntityUid toRemove in component.Contents.ContainedEntities.ToArray<EntityUid>())
      this.Remove(toRemove, uid, component, xform);
  }

  public void OpenStorage(EntityUid uid, SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(uid, ref component) || component.Open)
      return;
    StorageBeforeOpenEvent args1 = new StorageBeforeOpenEvent();
    this.RaiseLocalEvent<StorageBeforeOpenEvent>(uid, ref args1);
    component.Open = true;
    this.Dirty(uid, (IComponent) component);
    this.EmptyContents(uid, component);
    this.ModifyComponents(uid, component);
    if (this._net.IsClient && this._timing.IsFirstTimePredicted)
      this._audio.PlayPvs(component.OpenSound, uid);
    this.ReleaseGas(uid, component);
    StorageAfterOpenEvent args2 = new StorageAfterOpenEvent();
    this.RaiseLocalEvent<StorageAfterOpenEvent>(uid, ref args2);
  }

  public void CloseStorage(EntityUid uid, SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(uid, ref component) || !component.Open || this.EntityManager.IsQueuedForDeletion(uid))
      return;
    component.Open = false;
    this.Dirty(uid, (IComponent) component);
    HashSet<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(new EntityCoordinates(uid, component.EnteringOffset), component.EnteringRange, LookupFlags.Approximate | LookupFlags.Dynamic | LookupFlags.Sundries);
    entitiesInRange.Remove(uid);
    StorageBeforeCloseEvent args1 = new StorageBeforeCloseEvent(entitiesInRange, new HashSet<EntityUid>());
    this.RaiseLocalEvent<StorageBeforeCloseEvent>(uid, ref args1);
    foreach (EntityUid content in args1.Contents)
    {
      if ((args1.BypassChecks.Contains(content) || this.CanInsert(content, uid, component)) && this.AddToContents(content, uid, component))
      {
        if (component.Contents.ContainedEntities.Count >= component.Capacity)
          break;
      }
    }
    this.TakeGas(uid, component);
    this.ModifyComponents(uid, component);
    if (this._net.IsClient && this._timing.IsFirstTimePredicted)
      this._audio.PlayPvs(component.CloseSound, uid);
    StorageAfterCloseEvent args2 = new StorageAfterCloseEvent();
    this.RaiseLocalEvent<StorageAfterCloseEvent>(uid, ref args2);
  }

  public bool Insert(
    EntityUid toInsert,
    EntityUid container,
    SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(container, ref component))
      return false;
    if (component.Open)
    {
      this.TransformSystem.DropNextTo((Entity<TransformComponent>) toInsert, (Entity<TransformComponent>) container);
      return true;
    }
    this._joints.RecursiveClearJoints(toInsert);
    if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) toInsert, (BaseContainer) component.Contents))
      return false;
    this.EnsureComp<InsideEntityStorageComponent>(toInsert).Storage = container;
    return true;
  }

  public bool Remove(
    EntityUid toRemove,
    EntityUid container,
    SharedEntityStorageComponent? component = null,
    TransformComponent? xform = null)
  {
    if (!this.Resolve(container, ref xform, false) || !this.ResolveStorage(container, ref component))
      return false;
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) toRemove, (BaseContainer) component.Contents);
    BaseContainer container1;
    if (this._container.IsEntityInContainer(container) && this._container.TryGetOuterContainer(container, this.Transform(container), out container1) && !this.HasComp<HandsComponent>(container1.Owner))
    {
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) toRemove, container1);
      return true;
    }
    this.RemComp<InsideEntityStorageComponent>(toRemove);
    Vector2 worldPosition = this.TransformSystem.GetWorldPosition(xform);
    Angle worldRotation = this.TransformSystem.GetWorldRotation(xform);
    Vector2 vector2 = ((Angle) ref worldRotation).RotateVec(ref component.EnteringOffset);
    Vector2 worldPos = worldPosition + vector2;
    this.TransformSystem.SetWorldPosition(toRemove, worldPos);
    return true;
  }

  public bool CanInsert(
    EntityUid toInsert,
    EntityUid container,
    SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(container, ref component))
      return false;
    if (component.Open)
      return true;
    if (component.Contents.ContainedEntities.Count >= component.Capacity)
      return false;
    Box2 aabbNoContainer = this._lookup.GetAABBNoContainer(toInsert, Vector2.Zero, Angle.op_Implicit(0.0f));
    if ((double) component.MaxSize < (double) ((Box2) ref aabbNoContainer).Size.X || (double) component.MaxSize < (double) ((Box2) ref aabbNoContainer).Size.Y)
      return false;
    InsertIntoEntityStorageAttemptEvent args1 = new InsertIntoEntityStorageAttemptEvent(toInsert, container);
    this.RaiseLocalEvent<InsertIntoEntityStorageAttemptEvent>(toInsert, ref args1);
    if (args1.Cancelled)
      return false;
    EntityStorageInsertedIntoAttemptEvent args2 = new EntityStorageInsertedIntoAttemptEvent(toInsert);
    this.RaiseLocalEvent<EntityStorageInsertedIntoAttemptEvent>(container, ref args2);
    if (args2.Cancelled)
      return false;
    if (component.Whitelist != null)
      return this._whitelistSystem.IsValid(component.Whitelist, toInsert);
    return this.HasComp<BodyComponent>(toInsert) || this.HasComp<ItemComponent>(toInsert);
  }

  public bool TryOpenStorage(EntityUid user, EntityUid target, bool silent = false)
  {
    if (!this.CanOpen(user, target, silent))
      return false;
    this.OpenStorage(target);
    return true;
  }

  public bool TryCloseStorage(EntityUid target, EntityUid? user = null)
  {
    if (!this.CanClose(target, user))
      return false;
    this.CloseStorage(target);
    return true;
  }

  public bool IsOpen(EntityUid target, SharedEntityStorageComponent? component = null)
  {
    return this.ResolveStorage(target, ref component) && component.Open;
  }

  public bool CanOpen(
    EntityUid user,
    EntityUid target,
    bool silent = false,
    SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(target, ref component) || !this.HasComp<HandsComponent>(user))
      return false;
    if (this._weldable.IsWelded(target))
    {
      if (!silent && !component.Contents.Contains(user))
        this.Popup.PopupClient(this.Loc.GetString("entity-storage-component-welded-shut-message"), target, new EntityUid?(user));
      return false;
    }
    if (component.EnteringOffset != new Vector2(0.0f, 0.0f) && !this.HasComp<WallMountComponent>(target))
    {
      EntityCoordinates other = new EntityCoordinates(target, component.EnteringOffset);
      if (!this._interaction.InRangeUnobstructed(target, other, 0.0f, component.EnteringOffsetCollisionFlags))
      {
        if (!silent && this._net.IsServer)
          this.Popup.PopupEntity(this.Loc.GetString("entity-storage-component-cannot-open-no-space"), target);
        return false;
      }
    }
    StorageOpenAttemptEvent args = new StorageOpenAttemptEvent(user, silent);
    this.RaiseLocalEvent<StorageOpenAttemptEvent>(target, ref args, true);
    return !args.Cancelled;
  }

  public bool CanClose(EntityUid target, EntityUid? user = null, bool silent = false)
  {
    StorageCloseAttemptEvent args = new StorageCloseAttemptEvent(user);
    this.RaiseLocalEvent<StorageCloseAttemptEvent>(target, ref args, silent);
    return !args.Cancelled;
  }

  public bool AddToContents(
    EntityUid toAdd,
    EntityUid container,
    SharedEntityStorageComponent? component = null)
  {
    return this.ResolveStorage(container, ref component) && !(toAdd == container) && this.Insert(toAdd, container, component);
  }

  private void ModifyComponents(EntityUid uid, SharedEntityStorageComponent? component = null)
  {
    if (!this.ResolveStorage(uid, ref component))
      return;
    FixturesComponent comp;
    if (!component.IsCollidableWhenOpen && this.TryComp<FixturesComponent>(uid, out comp) && comp.Fixtures.Count > 0)
    {
      KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
      if (component.Open)
      {
        component.RemovedMasks = keyValuePair.Value.CollisionLayer & component.MasksToRemove;
        this._physics.SetCollisionLayer(uid, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionLayer & ~component.MasksToRemove, comp);
      }
      else
      {
        this._physics.SetCollisionLayer(uid, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionLayer | component.RemovedMasks, comp);
        component.RemovedMasks = 0;
      }
    }
    this._appearance.SetData(uid, (Enum) StorageVisuals.Open, (object) component.Open);
    this._appearance.SetData(uid, (Enum) StorageVisuals.HasContents, (object) (component.Contents.ContainedEntities.Count > 0));
  }

  protected virtual void TakeGas(EntityUid uid, SharedEntityStorageComponent component)
  {
  }

  public virtual void ReleaseGas(EntityUid uid, SharedEntityStorageComponent component)
  {
  }
}
