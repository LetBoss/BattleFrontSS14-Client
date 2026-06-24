// Decompiled with JetBrains decompiler
// Type: Content.Shared.Species.ReformSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Species.Components;
using Content.Shared.Stunnable;
using Content.Shared.Zombies;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Species;

public sealed class ReformSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private SharedActionsSystem _actionsSystem;
  [Robust.Shared.IoC.Dependency]
  private INetManager _netMan;
  [Robust.Shared.IoC.Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popupSystem;
  [Robust.Shared.IoC.Dependency]
  private IPrototypeManager _protoManager;
  [Robust.Shared.IoC.Dependency]
  private SharedStunSystem _stunSystem;
  [Robust.Shared.IoC.Dependency]
  private IGameTiming _gameTiming;
  [Robust.Shared.IoC.Dependency]
  private SharedMindSystem _mindSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ReformComponent, MapInitEvent>(new ComponentEventHandler<ReformComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ReformComponent, ComponentShutdown>(new ComponentEventHandler<ReformComponent, ComponentShutdown>(this.OnCompRemove));
    this.SubscribeLocalEvent<ReformComponent, ReformSystem.ReformEvent>(new ComponentEventHandler<ReformComponent, ReformSystem.ReformEvent>(this.OnReform));
    this.SubscribeLocalEvent<ReformComponent, ReformSystem.ReformDoAfterEvent>(new ComponentEventHandler<ReformComponent, ReformSystem.ReformDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<ReformComponent, EntityZombifiedEvent>(new ComponentEventRefHandler<ReformComponent, EntityZombifiedEvent>(this.OnZombified));
  }

  private void OnMapInit(EntityUid uid, ReformComponent comp, MapInitEvent args)
  {
    if (comp.ActionPrototype != new EntProtoId() && !this._protoManager.TryIndex<EntityPrototype>((string) comp.ActionPrototype, out EntityPrototype _))
      return;
    ActionComponent action;
    this._actionsSystem.AddAction(uid, ref comp.ActionEntity, out action, (string) comp.ActionPrototype);
    if (!comp.StartDelayed || action == null || !action.UseDelay.HasValue)
      return;
    TimeSpan curTime = this._gameTiming.CurTime;
    TimeSpan end = this._gameTiming.CurTime + action.UseDelay.Value;
    this._actionsSystem.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) comp.ActionEntity.Value), curTime, end);
  }

  private void OnCompRemove(EntityUid uid, ReformComponent comp, ComponentShutdown args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = (Entity<ActionsComponent>) uid;
    EntityUid? actionEntity = comp.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) actionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
  }

  private void OnReform(EntityUid uid, ReformComponent comp, ReformSystem.ReformEvent args)
  {
    if (comp.ShouldStun)
      this._stunSystem.TryStun(uid, TimeSpan.FromSeconds((double) comp.ReformTime), true);
    this._popupSystem.PopupClient(this.Loc.GetString(comp.PopupText, ("name", (object) uid)), uid, new EntityUid?(uid));
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, uid, comp.ReformTime, (DoAfterEvent) new ReformSystem.ReformDoAfterEvent(), new EntityUid?(uid))
    {
      BreakOnMove = true,
      BlockDuplicate = true,
      BreakOnDamage = true,
      CancelDuplicate = true,
      RequireCanInteract = false
    });
    args.Handled = true;
  }

  private void OnDoAfter(EntityUid uid, ReformComponent comp, ReformSystem.ReformDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || comp.Deleted || this._netMan.IsClient)
      return;
    EntityUid entityUid = this.Spawn((string) comp.ReformPrototype, this.Transform(uid).Coordinates);
    EntityUid mindId;
    MindComponent mind;
    if (this._mindSystem.TryGetMind(uid, out mindId, out mind))
      this._mindSystem.TransferTo(mindId, new EntityUid?(entityUid), mind: mind);
    this.QueueDel(new EntityUid?(uid));
  }

  private void OnZombified(EntityUid uid, ReformComponent comp, ref EntityZombifiedEvent args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = (Entity<ActionsComponent>) uid;
    EntityUid? actionEntity = comp.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) actionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
  }

  public sealed class ReformEvent : 
    InstantActionEvent,
    ISerializationGenerated<ReformSystem.ReformEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ReformSystem.ReformEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      InstantActionEvent target1 = (InstantActionEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (ReformSystem.ReformEvent) target1;
      serialization.TryCustomCopy<ReformSystem.ReformEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ReformSystem.ReformEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref InstantActionEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ReformSystem.ReformEvent target1 = (ReformSystem.ReformEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (InstantActionEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ReformSystem.ReformEvent target1 = (ReformSystem.ReformEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual ReformSystem.ReformEvent InstantActionEvent.Instantiate()
    {
      return new ReformSystem.ReformEvent();
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ReformDoAfterEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<ReformSystem.ReformDoAfterEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ReformSystem.ReformDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (ReformSystem.ReformDoAfterEvent) target1;
      serialization.TryCustomCopy<ReformSystem.ReformDoAfterEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ReformSystem.ReformDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ReformSystem.ReformDoAfterEvent target1 = (ReformSystem.ReformDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ReformSystem.ReformDoAfterEvent target1 = (ReformSystem.ReformDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual ReformSystem.ReformDoAfterEvent SimpleDoAfterEvent.Instantiate()
    {
      return new ReformSystem.ReformDoAfterEvent();
    }
  }
}
