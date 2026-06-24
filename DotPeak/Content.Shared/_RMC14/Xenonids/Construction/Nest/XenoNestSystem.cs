// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Nest.XenoNestSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Ghost;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Ghost;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Maps;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Nest;

public sealed class XenoNestSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedGhostSystem _ghost;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private OccluderSystem _occluder;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedXenoParasiteSystem _parasite;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedXenoWeedsSystem _xenoWeeds;
  private Robust.Shared.GameObjects.EntityQuery<OccluderComponent> _occluderQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoNestComponent> _xenoNestQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoNestSurfaceComponent> _xenoNestSurfaceQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoWeedableComponent> _xenoWeedableQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._occluderQuery = this.GetEntityQuery<OccluderComponent>();
    this._xenoNestQuery = this.GetEntityQuery<XenoNestComponent>();
    this._xenoNestSurfaceQuery = this.GetEntityQuery<XenoNestSurfaceComponent>();
    this._xenoWeedableQuery = this.GetEntityQuery<XenoWeedableComponent>();
    this.SubscribeLocalEvent<GhostAttemptHandleEvent>(new EntityEventHandler<GhostAttemptHandleEvent>(this.OnNestedGhostAttemptHandle));
    this.SubscribeLocalEvent<XenoComponent, GetUsedEntityEvent>(new EntityEventRefHandler<XenoComponent, GetUsedEntityEvent>(this.OnXenoGetUsedEntity));
    this.SubscribeLocalEvent<XenoNestSurfaceComponent, InteractHandEvent>(new EntityEventRefHandler<XenoNestSurfaceComponent, InteractHandEvent>(this.OnSurfaceInteractHand));
    this.SubscribeLocalEvent<XenoNestSurfaceComponent, DoAfterAttemptEvent<XenoNestDoAfterEvent>>(new EntityEventRefHandler<XenoNestSurfaceComponent, DoAfterAttemptEvent<XenoNestDoAfterEvent>>(this.OnSurfaceDoAfterAttempt));
    this.SubscribeLocalEvent<XenoNestSurfaceComponent, XenoNestDoAfterEvent>(new EntityEventRefHandler<XenoNestSurfaceComponent, XenoNestDoAfterEvent>(this.OnNestSurfaceDoAfter));
    this.SubscribeLocalEvent<XenoNestSurfaceComponent, CanDropTargetEvent>(new EntityEventRefHandler<XenoNestSurfaceComponent, CanDropTargetEvent>(this.OnSurfaceCanDropTarget));
    this.SubscribeLocalEvent<XenoNestSurfaceComponent, DragDropTargetEvent>(new EntityEventRefHandler<XenoNestSurfaceComponent, DragDropTargetEvent>(this.OnSurfaceDragDropTarget));
    this.SubscribeLocalEvent<XenoNestSurfaceComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoNestSurfaceComponent, EntityTerminatingEvent>(this.OnSurfaceTerminating));
    this.SubscribeLocalEvent<XenoNestComponent, ComponentRemove>(new EntityEventRefHandler<XenoNestComponent, ComponentRemove>(this.OnNestRemove));
    this.SubscribeLocalEvent<XenoNestComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoNestComponent, EntityTerminatingEvent>(this.OnNestTerminating));
    this.SubscribeLocalEvent<XenoNestableComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<XenoNestableComponent, BeforeRangedInteractEvent>(this.OnNestableBeforeRangedInteract));
    this.SubscribeLocalEvent<XenoNestableComponent, ShouldHandleVirtualItemInteractEvent>(new EntityEventRefHandler<XenoNestableComponent, ShouldHandleVirtualItemInteractEvent>(this.OnNestableShouldHandle));
    this.SubscribeLocalEvent<XenoNestedComponent, ComponentStartup>(new EntityEventRefHandler<XenoNestedComponent, ComponentStartup>(this.OnNestedAdd));
    this.SubscribeLocalEvent<XenoNestedComponent, ComponentRemove>(new EntityEventRefHandler<XenoNestedComponent, ComponentRemove>(this.OnNestedRemove));
    this.SubscribeLocalEvent<XenoNestedComponent, PreventCollideEvent>(new EntityEventRefHandler<XenoNestedComponent, PreventCollideEvent>(this.OnNestedPreventCollide));
    this.SubscribeLocalEvent<XenoNestedComponent, PullAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, PullAttemptEvent>(this.OnNestedPullAttempt));
    this.SubscribeLocalEvent<XenoNestedComponent, InteractionAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, InteractionAttemptEvent>(this.OnNestedInteractionAttempt));
    this.SubscribeLocalEvent<XenoNestedComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<XenoNestedComponent, UpdateCanMoveEvent>(this.OnNestedCancel<UpdateCanMoveEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, UseAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, UseAttemptEvent>(this.OnNestedCancel<UseAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, ThrowAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, ThrowAttemptEvent>(this.OnNestedCancel<ThrowAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, PickupAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, PickupAttemptEvent>(this.OnNestedCancel<PickupAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, AttackAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, AttackAttemptEvent>(this.OnNestedCancel<AttackAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, ChangeDirectionAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, ChangeDirectionAttemptEvent>(this.OnNestedCancel<ChangeDirectionAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, DownAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, DownAttemptEvent>(this.OnNestedCancel<DownAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, IsEquippingAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, IsEquippingAttemptEvent>(this.OnNestedCancel<IsEquippingAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, IsUnequippingAttemptEvent>(new EntityEventRefHandler<XenoNestedComponent, IsUnequippingAttemptEvent>(this.OnNestedCancel<IsUnequippingAttemptEvent>));
    this.SubscribeLocalEvent<XenoNestedComponent, GetInfectedIncubationMultiplierEvent>(new EntityEventRefHandler<XenoNestedComponent, GetInfectedIncubationMultiplierEvent>(this.OnInNestGetInfectedIncubationMultiplier));
  }

  private void OnXenoGetUsedEntity(Entity<XenoComponent> ent, ref GetUsedEntityEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? pulling = (EntityUid?) this.CompOrNull<PullerComponent>((EntityUid) ent)?.Pulling;
    if (!pulling.HasValue)
      return;
    EntityUid valueOrDefault = pulling.GetValueOrDefault();
    if (!this.HasComp<XenoNestableComponent>(valueOrDefault) || this.HasComp<XenoNestedComponent>(valueOrDefault))
      return;
    args.Used = new EntityUid?(valueOrDefault);
  }

  private void OnSurfaceInteractHand(
    Entity<XenoNestSurfaceComponent> ent,
    ref InteractHandEvent args)
  {
    EntityUid? pulling = (EntityUid?) this.CompOrNull<PullerComponent>(args.User)?.Pulling;
    if (!pulling.HasValue)
      return;
    EntityUid valueOrDefault = pulling.GetValueOrDefault();
    args.Handled = true;
    this.TryStartNesting(args.User, ent, valueOrDefault);
  }

  private void OnNestRemove(Entity<XenoNestComponent> ent, ref ComponentRemove args)
  {
    this.DetachNested(new EntityUid?((EntityUid) ent), ent.Comp.Nested);
  }

  private void OnNestTerminating(Entity<XenoNestComponent> ent, ref EntityTerminatingEvent args)
  {
    this.DetachNested(new EntityUid?((EntityUid) ent), ent.Comp.Nested);
  }

  private void OnNestableBeforeRangedInteract(
    Entity<XenoNestableComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    XenoNestSurfaceComponent comp;
    if (!args.CanReach || !this.TryComp<XenoNestSurfaceComponent>(args.Target, out comp))
      return;
    args.Handled = true;
    this.TryStartNesting(args.User, (Entity<XenoNestSurfaceComponent>) (args.Target.Value, comp), (EntityUid) ent);
  }

  private void OnNestableShouldHandle(
    Entity<XenoNestableComponent> ent,
    ref ShouldHandleVirtualItemInteractEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Event.User) || !this.HasComp<XenoNestSurfaceComponent>(args.Event.Target))
      return;
    args.Handle = true;
  }

  private void OnNestedAdd(Entity<XenoNestedComponent> ent, ref ComponentStartup args)
  {
    this._parasite.RefreshIncubationMultipliers((Entity<VictimInfectedComponent>) ent.Owner);
  }

  private void OnNestedRemove(Entity<XenoNestedComponent> ent, ref ComponentRemove args)
  {
    this.DetachNested(new EntityUid?(), new EntityUid?((EntityUid) ent));
    this._actionBlocker.UpdateCanMove((EntityUid) ent);
    this._parasite.RefreshIncubationMultipliers((Entity<VictimInfectedComponent>) ent.Owner);
    if (this.HasComp<KnockedDownComponent>((EntityUid) ent) || this._mobState.IsIncapacitated((EntityUid) ent))
      this._standing.Down((EntityUid) ent, changeCollision: true);
    NetUserId? nullable = ent.Comp.GhostedId;
    ICommonSession session;
    if (!nullable.HasValue || !this._player.TryGetSessionById(new NetUserId?(nullable.GetValueOrDefault()), out session))
      return;
    EntityUid? attachedEntity = session.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    if (!this.HasComp<GhostComponent>(valueOrDefault))
      return;
    SharedCMChatSystem rmcChat = this._rmcChat;
    EntityUid target = valueOrDefault;
    Color? colorOverride = new Color?();
    nullable = new NetUserId?();
    NetUserId? author = nullable;
    rmcChat.ChatMessageToOne("\n[font size=24][color=red]You have been freed from your nest and may go back to your body![/color][/font]\n", target, colorOverride: colorOverride, author: author);
    RMCGhostReturnComponent ghostReturnComponent = this.EnsureComp<RMCGhostReturnComponent>(valueOrDefault);
    ghostReturnComponent.Target = new EntityUid?((EntityUid) ent);
    this.Dirty(valueOrDefault, (IComponent) ghostReturnComponent);
    this._ghost.SetCanReturnToBody((Entity<GhostComponent>) (valueOrDefault, (GhostComponent) null), true);
  }

  private void OnSurfaceDoAfterAttempt(
    Entity<XenoNestSurfaceComponent> ent,
    ref DoAfterAttemptEvent<XenoNestDoAfterEvent> args)
  {
    EntityUid? target = args.DoAfter.Args.Target;
    if (target.HasValue)
    {
      EntityUid valueOrDefault = target.GetValueOrDefault();
      if (!this.TerminatingOrDeleted(valueOrDefault) && this.CanNestPopup(args.DoAfter.Args.User, new EntityUid?(valueOrDefault), ent, out Direction? _, allDirs: args.Event.AllDirs))
        return;
    }
    args.Cancel();
  }

  private void OnNestSurfaceDoAfter(
    Entity<XenoNestSurfaceComponent> ent,
    ref XenoNestDoAfterEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault1 = target.GetValueOrDefault();
    if (args.Cancelled)
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(29, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" stopped nesting ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault1), "victim", "ToPrettyString(victim)");
      logStringHandler.AppendLiteral(" to surface ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "surface", "ToPrettyString(ent)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCXenoNest, ref local);
    }
    else
    {
      Direction? direction;
      if (args.Handled || !this.CanNestPopup(args.User, new EntityUid?(valueOrDefault1), ent, out direction, allDirs: args.AllDirs))
        return;
      args.Handled = true;
      PullableComponent comp;
      if (this.TryComp<PullableComponent>(valueOrDefault1, out comp))
        this._pulling.TryStopPull(valueOrDefault1, comp);
      if (this._net.IsClient)
        return;
      EntityCoordinates coordinates = ent.Owner.ToCoordinates();
      Vector2 vector2;
      if (direction.HasValue)
      {
        switch ((int) direction.GetValueOrDefault())
        {
          case 0:
            vector2 = new Vector2(0.0f, -0.52f);
            goto label_16;
          case 2:
            vector2 = new Vector2(0.52f, 0.0f);
            goto label_16;
          case 4:
            vector2 = new Vector2(0.0f, 0.52f);
            goto label_16;
          case 6:
            vector2 = new Vector2(-0.52f, 0.0f);
            goto label_16;
        }
      }
      vector2 = Vector2.Zero;
label_16:
      Vector2 position = vector2;
      EntityUid entityUid = this.SpawnAttachedTo((string) ent.Comp.Nest, coordinates, rotation: DirectionExtensions.ToAngle(direction.Value));
      this._transform.SetCoordinates(entityUid, coordinates.Offset(position));
      this._hive.SetSameHive((Entity<HiveMemberComponent>) args.User, (Entity<HiveMemberComponent>) entityUid);
      ent.Comp.Nests[direction.Value] = entityUid;
      this.Dirty<XenoNestSurfaceComponent>(ent);
      XenoNestComponent xenoNestComponent = this.EnsureComp<XenoNestComponent>(entityUid);
      xenoNestComponent.Surface = new EntityUid?((EntityUid) ent);
      xenoNestComponent.Nested = new EntityUid?(valueOrDefault1);
      this.Dirty(entityUid, (IComponent) xenoNestComponent);
      XenoNestedComponent xenoNestedComponent = this.EnsureComp<XenoNestedComponent>(valueOrDefault1);
      xenoNestedComponent.Nest = entityUid;
      this.Dirty(valueOrDefault1, (IComponent) xenoNestedComponent);
      this._transform.SetCoordinates(valueOrDefault1, entityUid.ToCoordinates());
      this._transform.SetLocalRotation(valueOrDefault1, Angle.Zero);
      this._standing.Stand(valueOrDefault1, force: true);
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-nest-securing-self", ("target", (object) valueOrDefault1)), args.User, new EntityUid?(args.User));
      foreach (ICommonSession recipient in Filter.PvsExcept(args.User).Recipients)
      {
        EntityUid? attachedEntity = recipient.AttachedEntity;
        if (attachedEntity.HasValue)
        {
          EntityUid valueOrDefault2 = attachedEntity.GetValueOrDefault();
          if (valueOrDefault2 == valueOrDefault1)
            this._popup.PopupEntity(this.Loc.GetString("cm-xeno-nest-securing-target", ("user", (object) args.User)), args.User, valueOrDefault2, PopupType.MediumCaution);
          else
            this._popup.PopupEntity(this.Loc.GetString("cm-xeno-nest-securing-observer", ("user", (object) args.User), ("target", (object) valueOrDefault1)), args.User, valueOrDefault2);
        }
      }
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(20, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" nested ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault1), "victim", "ToPrettyString(victim)");
      logStringHandler.AppendLiteral(" to surface ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "surface", "ToPrettyString(ent)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCXenoNest, ref local);
    }
  }

  private void OnSurfaceCanDropTarget(
    Entity<XenoNestSurfaceComponent> ent,
    ref CanDropTargetEvent args)
  {
    if (args.Handled)
      return;
    args.CanDrop = this.CanBeNested(args.User, new EntityUid?(args.Dragged), (Entity<XenoNestSurfaceComponent>) ((EntityUid) ent, ent.Comp), out List<Direction> _, true);
    args.Handled = true;
  }

  private void OnSurfaceDragDropTarget(
    Entity<XenoNestSurfaceComponent> ent,
    ref DragDropTargetEvent args)
  {
    args.Handled = true;
    this.TryStartNesting(args.User, ent, args.Dragged);
  }

  private void OnSurfaceTerminating(
    Entity<XenoNestSurfaceComponent> ent,
    ref EntityTerminatingEvent args)
  {
    XenoWeedableComponent component;
    if (this.TerminatingOrDeleted(ent.Comp.Weedable) || !this._xenoWeedableQuery.TryComp(ent.Comp.Weedable, out component))
      return;
    EntityUid? entity = component.Entity;
    EntityUid entityUid = (EntityUid) ent;
    if ((entity.HasValue ? (entity.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return;
    component.Entity = new EntityUid?();
    this.Dirty(ent.Comp.Weedable.Value, (IComponent) component);
  }

  private void OnNestedPreventCollide(Entity<XenoNestedComponent> ent, ref PreventCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnNestedPullAttempt(Entity<XenoNestedComponent> ent, ref PullAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnNestedInteractionAttempt(
    Entity<XenoNestedComponent> ent,
    ref InteractionAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnNestedCancel<T>(Entity<XenoNestedComponent> ent, ref T args) where T : CancellableEntityEventArgs
  {
    args.Cancel();
  }

  private void OnInNestGetInfectedIncubationMultiplier(
    Entity<XenoNestedComponent> ent,
    ref GetInfectedIncubationMultiplierEvent args)
  {
    if (!ent.Comp.Running)
      return;
    args.Multiply(ent.Comp.IncubationMultiplier);
  }

  private void OnNestedGhostAttemptHandle(GhostAttemptHandleEvent args)
  {
    EntityUid? currentEntity = args.Mind.CurrentEntity;
    if (!currentEntity.HasValue)
      return;
    EntityUid valueOrDefault1 = currentEntity.GetValueOrDefault();
    XenoNestedComponent comp;
    if (!this.TryComp<XenoNestedComponent>(valueOrDefault1, out comp))
      return;
    NetUserId? userId = args.Mind.UserId;
    if (!userId.HasValue)
      return;
    NetUserId valueOrDefault2 = userId.GetValueOrDefault();
    comp.GhostedId = new NetUserId?(valueOrDefault2);
    this.Dirty(valueOrDefault1, (IComponent) comp);
  }

  public bool TryStartNesting(
    EntityUid user,
    Entity<XenoNestSurfaceComponent> surface,
    EntityUid victim,
    out DoAfterId? doAfterId,
    bool allDirs = false)
  {
    doAfterId = new DoAfterId?();
    if (!this.HasComp<XenoComponent>(user) || !this.HasComp<HandsComponent>(user) || !this.CanNestPopup(user, new EntityUid?(victim), surface, out Direction? _, allDirs: allDirs))
      return false;
    XenoNestDoAfterEvent @event = new XenoNestDoAfterEvent()
    {
      AllDirs = allDirs
    };
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, surface.Comp.DoAfter, (DoAfterEvent) @event, new EntityUid?((EntityUid) surface), new EntityUid?(victim))
    {
      BreakOnMove = true,
      AttemptFrequency = AttemptFrequency.EveryTick
    }, out doAfterId))
      return true;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-nest-pin-self", ("target", (object) victim)), user, new EntityUid?(user));
    foreach (ICommonSession recipient in Filter.PvsExcept(user).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        if (valueOrDefault == victim)
          this._popup.PopupEntity(this.Loc.GetString("cm-xeno-nest-pin-target", (nameof (user), (object) user)), user, valueOrDefault, PopupType.MediumCaution);
        else
          this._popup.PopupEntity(this.Loc.GetString("cm-xeno-nest-pin-observer", (nameof (user), (object) user), ("target", (object) victim)), user, valueOrDefault);
      }
    }
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(29, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" started nesting ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) victim), nameof (victim), "ToPrettyString(victim)");
    logStringHandler.AppendLiteral(" to surface ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) surface)), nameof (surface), "ToPrettyString(surface)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCXenoNest, ref local);
    return true;
  }

  public bool TryStartNesting(
    EntityUid user,
    Entity<XenoNestSurfaceComponent> surface,
    EntityUid victim)
  {
    return this.TryStartNesting(user, surface, victim, out DoAfterId? _);
  }

  private bool CanBeNested(
    EntityUid user,
    EntityUid? victim,
    Entity<XenoNestSurfaceComponent?> surface,
    out List<Direction> directions,
    bool silent = false,
    bool allDirs = false)
  {
    directions = new List<Direction>();
    if (victim.HasValue)
    {
      if (!this.HasComp<XenoNestableComponent>(victim))
      {
        if (!silent)
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-nest-failed", ("target", (object) victim)), (EntityUid) surface, new EntityUid?(user));
        return false;
      }
      if (this._mobState.IsDead(victim.Value))
      {
        if (!silent)
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-nest-failed-dead", ("target", (object) victim)), (EntityUid) surface, new EntityUid?(user));
        return false;
      }
    }
    Vector2 delta;
    if (!this._transform.GetMoverCoordinates(user).TryDelta((IEntityManager) this.EntityManager, this._transform, this._transform.GetMoverCoordinates((EntityUid) surface), out delta))
      return false;
    Direction dir = DirectionExtensions.GetDir(delta);
    Angle angle = Angle.FromWorldVec(delta);
    Direction cardinalDir = ((Angle) ref angle).GetCardinalDir();
    if (dir == -1)
      return false;
    directions.Add(cardinalDir);
    DirectionFlag directionFlag = DirectionExtensions.AsFlag(dir);
    if (!directions.Contains((Direction) 0) && (allDirs || ((Enum) (object) directionFlag).HasFlag((Enum) (object) (DirectionFlag) 1)))
      directions.Add((Direction) 0);
    if (!directions.Contains((Direction) 2) && (allDirs || ((Enum) (object) directionFlag).HasFlag((Enum) (object) (DirectionFlag) 2)))
      directions.Add((Direction) 2);
    if (!directions.Contains((Direction) 4) && (allDirs || ((Enum) (object) directionFlag).HasFlag((Enum) (object) (DirectionFlag) 4)))
      directions.Add((Direction) 4);
    if (!directions.Contains((Direction) 6) && (allDirs || ((Enum) (object) directionFlag).HasFlag((Enum) (object) (DirectionFlag) 8)))
      directions.Add((Direction) 6);
    if (this.Resolve<XenoNestSurfaceComponent>((EntityUid) surface, ref surface.Comp) && this.IsNestSurfaceFromHiveWeeds((Entity<XenoNestSurfaceComponent>) (surface.Owner, surface.Comp), cardinalDir, user))
      return true;
    if (!silent)
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-nest-failed-cant-there"), (EntityUid) surface, new EntityUid?(user));
    return false;
  }

  public bool CanNestPopup(
    EntityUid user,
    EntityUid? victim,
    Entity<XenoNestSurfaceComponent> surface,
    [NotNullWhen(true)] out Direction? direction,
    bool silent = false,
    bool allDirs = false)
  {
    direction = new Direction?();
    List<Direction> directions;
    if (!this.CanBeNested(user, victim, (Entity<XenoNestSurfaceComponent>) ((EntityUid) surface, surface.Comp), out directions, silent, allDirs))
      return false;
    if (victim.HasValue && !this._standing.IsDown(victim.Value))
    {
      if (!silent)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-nest-failed-target-resisting", ("target", (object) victim)), victim.Value, new EntityUid?(user), PopupType.MediumCaution);
      return false;
    }
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) surface);
    string message = (string) null;
    foreach (Direction key in directions)
    {
      TileRef? tileRef = this._turf.GetTileRef(moverCoordinates.Offset(DirectionExtensions.ToVec(key)));
      if (!tileRef.HasValue || this._turf.IsTileBlocked(tileRef.GetValueOrDefault(), CollisionGroup.Impassable))
      {
        if (message == null)
          message = this.Loc.GetString("cm-xeno-nest-failed-cant-there");
      }
      else if (surface.Comp.Nests.ContainsKey(key))
      {
        if (message == null)
          message = this.Loc.GetString("cm-xeno-nest-failed-cant-already-there");
      }
      else
      {
        message = (string) null;
        direction = new Direction?(key);
        break;
      }
    }
    if (direction.HasValue)
      return true;
    if (!silent)
      this._popup.PopupClient(message, (EntityUid) surface, new EntityUid?(user));
    return false;
  }

  private bool IsNestSurfaceFromHiveWeeds(
    Entity<XenoNestSurfaceComponent> nestSurface,
    Direction dir,
    EntityUid user)
  {
    (EntityUid entityUid, XenoNestSurfaceComponent comp1) = nestSurface;
    if (!comp1.Weedable.HasValue)
    {
      EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) entityUid);
      MapGridComponent comp2;
      if (!grid.HasValue || !this.TryComp<MapGridComponent>(grid, out comp2))
        return false;
      return this.HasComp<XenoNestIgnoreWeedsUserComponent>(user) || this._xenoWeeds.IsOnHiveWeeds((Entity<MapGridComponent>) (grid.Value, comp2), entityUid.ToCoordinates());
    }
    EntityCoordinates coordinates = this._transform.GetMoverCoordinates((EntityUid) nestSurface).Offset(DirectionExtensions.ToVec(dir));
    EntityUid? grid1 = this._transform.GetGrid(coordinates);
    if (grid1.HasValue)
    {
      EntityUid valueOrDefault = grid1.GetValueOrDefault();
      MapGridComponent comp3;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp3))
        return this.HasComp<XenoNestIgnoreWeedsUserComponent>(user) || this._xenoWeeds.IsOnHiveWeeds((Entity<MapGridComponent>) (valueOrDefault, comp3), coordinates);
    }
    return false;
  }

  private void DetachNested(EntityUid? nest, EntityUid? nestedNullable)
  {
    if (this._timing.ApplyingState || !nestedNullable.HasValue)
      return;
    EntityUid valueOrDefault = nestedNullable.GetValueOrDefault();
    TransformComponent comp1;
    if (this.TerminatingOrDeleted(valueOrDefault) || !this.TryComp(valueOrDefault, out comp1))
      return;
    XenoNestedComponent comp2;
    if (this.TryComp<XenoNestedComponent>(valueOrDefault, out comp2))
    {
      EntityUid entityUid2 = nest.GetValueOrDefault();
      if (!nest.HasValue)
      {
        entityUid2 = comp2.Nest;
        nest = new EntityUid?(entityUid2);
      }
      if (comp2.Detached)
        return;
      comp2.Detached = true;
      this.Dirty(valueOrDefault, (IComponent) comp2);
      XenoNestComponent comp3;
      XenoNestSurfaceComponent comp4;
      if (this.TryComp<XenoNestComponent>(nest, out comp3) && this.TryComp<XenoNestSurfaceComponent>(comp3.Surface, out comp4))
      {
        Direction key2;
        foreach ((key2, entityUid2) in comp4.Nests)
        {
          entityUid2 = entityUid2;
          EntityUid? nullable = nest;
          if ((nullable.HasValue ? (entityUid2 != nullable.GetValueOrDefault() ? 1 : 0) : 1) == 0)
          {
            comp4.Nests.Remove(key2);
            break;
          }
        }
        this.Dirty(comp3.Surface.Value, (IComponent) comp4);
      }
    }
    Vector2 localPosition = comp1.LocalPosition;
    SharedTransformSystem transform = this._transform;
    EntityUid uid = valueOrDefault;
    Vector2 vector2_1 = localPosition;
    Angle localRotation = comp1.LocalRotation;
    Vector2 vector2_2 = ((Angle) ref localRotation).ToWorldVec() / 2f;
    Vector2 vector2_3 = vector2_1 + vector2_2;
    transform.SetLocalPosition(uid, vector2_3);
    this._transform.AttachToGridOrMap(valueOrDefault, comp1);
    this.RemCompDeferred<XenoNestedComponent>(valueOrDefault);
    this.QueueDel(nest);
  }

  private bool TryGetNestedWallOccluder(
    Entity<XenoNestedComponent> nested,
    out Entity<OccluderComponent> occluder)
  {
    occluder = new Entity<OccluderComponent>();
    XenoNestComponent component1;
    if (!this._xenoNestQuery.TryComp(nested.Comp.Nest, out component1))
      return false;
    EntityUid? surface = component1.Surface;
    if (!surface.HasValue)
      return false;
    EntityUid valueOrDefault = surface.GetValueOrDefault();
    OccluderComponent component2;
    if (this._occluderQuery.TryComp(valueOrDefault, out component2))
    {
      occluder = (Entity<OccluderComponent>) (valueOrDefault, component2);
      return true;
    }
    XenoNestSurfaceComponent component3;
    if (!this._xenoNestSurfaceQuery.TryComp(valueOrDefault, out component3) || !this._occluderQuery.TryComp(component3.Weedable, out component2))
      return false;
    occluder = (Entity<OccluderComponent>) (component3.Weedable.Value, component2);
    return true;
  }

  public bool HasAdjacentNestFacing(EntityCoordinates coordinates)
  {
    ImmutableArray<Direction>.Enumerator enumerator = this._rmcMap.CardinalDirections.GetEnumerator();
label_5:
    while (enumerator.MoveNext())
    {
      Direction current = enumerator.Current;
      RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(coordinates, new Direction?(current), (DirectionFlag) 0);
      Direction opposite = DirectionExtensions.GetOpposite(current);
      EntityUid uid1;
      XenoNestSurfaceComponent component;
      EntityUid uid2;
      do
      {
        if (!entitiesEnumerator.MoveNext(out uid1))
          goto label_5;
      }
      while (!this._xenoNestSurfaceQuery.TryComp(uid1, out component) || !component.Nests.TryGetValue(opposite, out uid2) || this.TerminatingOrDeleted(uid2));
      return true;
    }
    return false;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsServer)
      return;
    EntityUid? localEntity = this._player.LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    XenoNestedComponent comp;
    Entity<OccluderComponent> occluder;
    if (!this.TryComp<XenoNestedComponent>(valueOrDefault, out comp) || !this.TryGetNestedWallOccluder((Entity<XenoNestedComponent>) (valueOrDefault, comp), out occluder))
      return;
    this._occluder.SetEnabled((EntityUid) occluder, false, (OccluderComponent) occluder);
  }
}
