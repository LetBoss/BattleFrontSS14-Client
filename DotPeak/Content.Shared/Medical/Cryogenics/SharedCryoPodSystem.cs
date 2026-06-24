// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.Cryogenics.SharedCryoPodSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Medical.Cryogenics;

public abstract class SharedCryoPodSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Robust.Shared.IoC.Dependency]
  private StandingStateSystem _standingStateSystem;
  [Robust.Shared.IoC.Dependency]
  private EmagSystem _emag;
  [Robust.Shared.IoC.Dependency]
  private MobStateSystem _mobStateSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popupSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedContainerSystem _containerSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedPointLightSystem _light;
  [Robust.Shared.IoC.Dependency]
  private ISharedAdminLogManager _adminLogger;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CryoPodComponent, CanDropTargetEvent>(new ComponentEventRefHandler<CryoPodComponent, CanDropTargetEvent>(this.OnCryoPodCanDropOn));
    this.InitializeInsideCryoPod();
  }

  private void OnCryoPodCanDropOn(
    EntityUid uid,
    CryoPodComponent component,
    ref CanDropTargetEvent args)
  {
    if (args.Handled)
      return;
    args.CanDrop = this.HasComp<BodyComponent>(args.Dragged);
    args.Handled = true;
  }

  protected void OnComponentInit(
    EntityUid uid,
    CryoPodComponent cryoPodComponent,
    ComponentInit args)
  {
    cryoPodComponent.BodyContainer = this._containerSystem.EnsureContainer<ContainerSlot>(uid, "scanner-body");
  }

  protected void UpdateAppearance(
    EntityUid uid,
    CryoPodComponent? cryoPod = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<CryoPodComponent>(uid, ref cryoPod))
      return;
    bool flag = this.HasComp<ActiveCryoPodComponent>(uid);
    SharedPointLightComponent component;
    if (this._light.TryGetLight(uid, out component))
      this._light.SetEnabled(uid, flag && cryoPod.BodyContainer.ContainedEntity.HasValue, component);
    if (!this.Resolve<AppearanceComponent>(uid, ref appearance))
      return;
    this._appearanceSystem.SetData(uid, (Enum) CryoPodComponent.CryoPodVisuals.ContainsEntity, (object) !cryoPod.BodyContainer.ContainedEntity.HasValue, appearance);
    this._appearanceSystem.SetData(uid, (Enum) CryoPodComponent.CryoPodVisuals.IsOn, (object) flag, appearance);
  }

  public bool InsertBody(EntityUid uid, EntityUid target, CryoPodComponent cryoPodComponent)
  {
    if (cryoPodComponent.BodyContainer.ContainedEntity.HasValue || !this.HasComp<MobStateComponent>(target))
      return false;
    TransformComponent transformComponent = this.Transform(target);
    this._containerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) (target, transformComponent), (BaseContainer) cryoPodComponent.BodyContainer);
    this.EnsureComp<InsideCryoPodComponent>(target);
    this._standingStateSystem.Stand(target, force: true);
    this.UpdateAppearance(uid, cryoPodComponent);
    return true;
  }

  public void TryEjectBody(EntityUid uid, EntityUid userId, CryoPodComponent? cryoPodComponent)
  {
    if (!this.Resolve<CryoPodComponent>(uid, ref cryoPodComponent))
      return;
    if (cryoPodComponent.Locked)
    {
      this._popupSystem.PopupEntity(this.Loc.GetString("cryo-pod-locked"), uid, userId);
    }
    else
    {
      EntityUid? nullable = this.EjectBody(uid, cryoPodComponent);
      if (!nullable.HasValue)
        return;
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(18, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) nullable.Value), "ToPrettyString(ejected.Value)");
      logStringHandler.AppendLiteral(" ejected from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" by ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) userId), "ToPrettyString(userId)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Medium, ref local);
    }
  }

  public virtual EntityUid? EjectBody(EntityUid uid, CryoPodComponent? cryoPodComponent)
  {
    if (!this.Resolve<CryoPodComponent>(uid, ref cryoPodComponent))
      return new EntityUid?();
    EntityUid? containedEntity = cryoPodComponent.BodyContainer.ContainedEntity;
    if (containedEntity.HasValue)
    {
      EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
      if (valueOrDefault.Valid)
      {
        this._containerSystem.Remove((Entity<TransformComponent, MetaDataComponent>) valueOrDefault, (BaseContainer) cryoPodComponent.BodyContainer);
        if (this.HasComp<KnockedDownComponent>(valueOrDefault) || this._mobStateSystem.IsIncapacitated(valueOrDefault))
          this._standingStateSystem.Down(valueOrDefault);
        else
          this._standingStateSystem.Stand(valueOrDefault);
        this.UpdateAppearance(uid, cryoPodComponent);
        return new EntityUid?(valueOrDefault);
      }
    }
    return new EntityUid?();
  }

  protected void AddAlternativeVerbs(
    EntityUid uid,
    CryoPodComponent cryoPodComponent,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !cryoPodComponent.BodyContainer.ContainedEntity.HasValue)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("cryo-pod-verb-noun-occupant");
    alternativeVerb.Category = VerbCategory.Eject;
    alternativeVerb.Priority = 1;
    alternativeVerb.Act = (Action) (() => this.TryEjectBody(uid, args.User, cryoPodComponent));
    verbs.Add(alternativeVerb);
  }

  protected void OnEmagged(
    EntityUid uid,
    CryoPodComponent? cryoPodComponent,
    ref GotEmaggedEvent args)
  {
    if (!this.Resolve<CryoPodComponent>(uid, ref cryoPodComponent) || !this._emag.CompareFlag(args.Type, EmagType.Interaction) || cryoPodComponent.PermaLocked && cryoPodComponent.Locked)
      return;
    cryoPodComponent.PermaLocked = true;
    cryoPodComponent.Locked = true;
    args.Handled = true;
  }

  protected void OnCryoPodPryFinished(
    EntityUid uid,
    CryoPodComponent cryoPodComponent,
    SharedCryoPodSystem.CryoPodPryFinished args)
  {
    if (args.Cancelled)
      return;
    EntityUid? nullable = this.EjectBody(uid, cryoPodComponent);
    if (!nullable.HasValue)
      return;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(18, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) nullable.Value), "ToPrettyString(ejected.Value)");
    logStringHandler.AppendLiteral(" pried out of ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" by ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "ToPrettyString(args.User)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Medium, ref local);
  }

  public virtual void InitializeInsideCryoPod()
  {
    this.SubscribeLocalEvent<InsideCryoPodComponent, DownAttemptEvent>(new ComponentEventHandler<InsideCryoPodComponent, DownAttemptEvent>(this.HandleDown));
    this.SubscribeLocalEvent<InsideCryoPodComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<InsideCryoPodComponent, EntGotRemovedFromContainerMessage>(this.OnEntGotRemovedFromContainer));
  }

  private void HandleDown(EntityUid uid, InsideCryoPodComponent component, DownAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnEntGotRemovedFromContainer(
    EntityUid uid,
    InsideCryoPodComponent component,
    EntGotRemovedFromContainerMessage args)
  {
    if (this.Terminating(uid))
      return;
    this.RemComp<InsideCryoPodComponent>(uid);
  }

  [NetSerializable]
  [Serializable]
  public sealed class CryoPodPryFinished : 
    SimpleDoAfterEvent,
    ISerializationGenerated<SharedCryoPodSystem.CryoPodPryFinished>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedCryoPodSystem.CryoPodPryFinished target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedCryoPodSystem.CryoPodPryFinished) target1;
      serialization.TryCustomCopy<SharedCryoPodSystem.CryoPodPryFinished>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedCryoPodSystem.CryoPodPryFinished target,
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
      SharedCryoPodSystem.CryoPodPryFinished target1 = (SharedCryoPodSystem.CryoPodPryFinished) target;
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
      SharedCryoPodSystem.CryoPodPryFinished target1 = (SharedCryoPodSystem.CryoPodPryFinished) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedCryoPodSystem.CryoPodPryFinished SimpleDoAfterEvent.Instantiate()
    {
      return new SharedCryoPodSystem.CryoPodPryFinished();
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CryoPodDragFinished : 
    SimpleDoAfterEvent,
    ISerializationGenerated<SharedCryoPodSystem.CryoPodDragFinished>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedCryoPodSystem.CryoPodDragFinished target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedCryoPodSystem.CryoPodDragFinished) target1;
      serialization.TryCustomCopy<SharedCryoPodSystem.CryoPodDragFinished>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedCryoPodSystem.CryoPodDragFinished target,
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
      SharedCryoPodSystem.CryoPodDragFinished target1 = (SharedCryoPodSystem.CryoPodDragFinished) target;
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
      SharedCryoPodSystem.CryoPodDragFinished target1 = (SharedCryoPodSystem.CryoPodDragFinished) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedCryoPodSystem.CryoPodDragFinished SimpleDoAfterEvent.Instantiate()
    {
      return new SharedCryoPodSystem.CryoPodDragFinished();
    }
  }
}
