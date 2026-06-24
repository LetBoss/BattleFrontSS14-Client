// Decompiled with JetBrains decompiler
// Type: Content.Shared.Species.GibActionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Species.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Species;

public sealed class GibActionSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private SharedActionsSystem _actionsSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedBodySystem _bodySystem;
  [Robust.Shared.IoC.Dependency]
  private IPrototypeManager _protoManager;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popupSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GibActionComponent, MobStateChangedEvent>(new ComponentEventHandler<GibActionComponent, MobStateChangedEvent>(this.OnMobStateChanged));
    this.SubscribeLocalEvent<GibActionComponent, GibActionSystem.GibActionEvent>(new ComponentEventHandler<GibActionComponent, GibActionSystem.GibActionEvent>(this.OnGibAction));
  }

  private void OnMobStateChanged(EntityUid uid, GibActionComponent comp, MobStateChangedEvent args)
  {
    MobStateComponent comp1;
    if (!this.TryComp<MobStateComponent>(uid, out comp1) || !this._protoManager.TryIndex<EntityPrototype>((string) comp.ActionPrototype, out EntityPrototype _))
      return;
    foreach (MobState allowedState in comp.AllowedStates)
    {
      if (allowedState == comp1.CurrentState)
      {
        this._actionsSystem.AddAction(uid, ref comp.ActionEntity, (string) comp.ActionPrototype);
        return;
      }
    }
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = (Entity<ActionsComponent>) uid;
    EntityUid? actionEntity = comp.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) actionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
  }

  private void OnGibAction(
    EntityUid uid,
    GibActionComponent comp,
    GibActionSystem.GibActionEvent args)
  {
    this._popupSystem.PopupClient(this.Loc.GetString(comp.PopupText, ("name", (object) uid)), uid, new EntityUid?(uid));
    this._bodySystem.GibBody(uid, true, splatCone: new Angle());
  }

  public sealed class GibActionEvent : 
    InstantActionEvent,
    ISerializationGenerated<GibActionSystem.GibActionEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref GibActionSystem.GibActionEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      InstantActionEvent target1 = (InstantActionEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (GibActionSystem.GibActionEvent) target1;
      serialization.TryCustomCopy<GibActionSystem.GibActionEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref GibActionSystem.GibActionEvent target,
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
      GibActionSystem.GibActionEvent target1 = (GibActionSystem.GibActionEvent) target;
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
      GibActionSystem.GibActionEvent target1 = (GibActionSystem.GibActionEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual GibActionSystem.GibActionEvent InstantActionEvent.Instantiate()
    {
      return new GibActionSystem.GibActionEvent();
    }
  }
}
