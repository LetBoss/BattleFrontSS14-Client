// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.SharedActionsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Movement;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mind;
using Content.Shared.Rejuvenate;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Actions;

public abstract class SharedActionsSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming GameTiming;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private RotateToFaceSystem _rotateToFace;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCLagCompensationSystem _rmcLagCompensation;
  private EntityQuery<ActionComponent> _actionQuery;
  private EntityQuery<ActionsComponent> _actionsQuery;
  private EntityQuery<MindComponent> _mindQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._actionQuery = this.GetEntityQuery<ActionComponent>();
    this._actionsQuery = this.GetEntityQuery<ActionsComponent>();
    this._mindQuery = this.GetEntityQuery<MindComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionComponent, MapInitEvent>(new EntityEventRefHandler<ActionComponent, MapInitEvent>((object) this, __methodptr(OnActionMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionComponent, ComponentShutdown>(new EntityEventRefHandler<ActionComponent, ComponentShutdown>((object) this, __methodptr(OnActionShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, ActionComponentChangeEvent>(new EntityEventRefHandler<ActionsComponent, ActionComponentChangeEvent>((object) this, __methodptr(OnActionCompChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, RelayedActionComponentChangeEvent>(new EntityEventRefHandler<ActionsComponent, RelayedActionComponentChangeEvent>((object) this, __methodptr(OnRelayActionCompChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, DidEquipEvent>(new EntityEventRefHandler<ActionsComponent, DidEquipEvent>((object) this, __methodptr(OnDidEquip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, DidEquipHandEvent>(new EntityEventRefHandler<ActionsComponent, DidEquipHandEvent>((object) this, __methodptr(OnHandEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, DidUnequipEvent>(new ComponentEventHandler<ActionsComponent, DidUnequipEvent>((object) this, __methodptr(OnDidUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, DidUnequipHandEvent>(new ComponentEventHandler<ActionsComponent, DidUnequipHandEvent>((object) this, __methodptr(OnHandUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, RejuvenateEvent>(new EntityEventRefHandler<ActionsComponent, RejuvenateEvent>((object) this, __methodptr(OnRejuventate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, ComponentShutdown>(new EntityEventRefHandler<ActionsComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, ComponentGetState>(new EntityEventRefHandler<ActionsComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionComponent, ActionValidateEvent>(new EntityEventRefHandler<ActionComponent, ActionValidateEvent>((object) this, __methodptr(OnValidate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InstantActionComponent, ActionValidateEvent>(new EntityEventRefHandler<InstantActionComponent, ActionValidateEvent>((object) this, __methodptr(OnInstantValidate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTargetActionComponent, ActionValidateEvent>(new EntityEventRefHandler<EntityTargetActionComponent, ActionValidateEvent>((object) this, __methodptr(OnEntityValidate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WorldTargetActionComponent, ActionValidateEvent>(new EntityEventRefHandler<WorldTargetActionComponent, ActionValidateEvent>((object) this, __methodptr(OnWorldValidate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InstantActionComponent, ActionGetEventEvent>(new EntityEventRefHandler<InstantActionComponent, ActionGetEventEvent>((object) this, __methodptr(OnInstantGetEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTargetActionComponent, ActionGetEventEvent>(new EntityEventRefHandler<EntityTargetActionComponent, ActionGetEventEvent>((object) this, __methodptr(OnEntityGetEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WorldTargetActionComponent, ActionGetEventEvent>(new EntityEventRefHandler<WorldTargetActionComponent, ActionGetEventEvent>((object) this, __methodptr(OnWorldGetEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InstantActionComponent, ActionSetEventEvent>(new EntityEventRefHandler<InstantActionComponent, ActionSetEventEvent>((object) this, __methodptr(OnInstantSetEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTargetActionComponent, ActionSetEventEvent>(new EntityEventRefHandler<EntityTargetActionComponent, ActionSetEventEvent>((object) this, __methodptr(OnEntitySetEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WorldTargetActionComponent, ActionSetEventEvent>(new EntityEventRefHandler<WorldTargetActionComponent, ActionSetEventEvent>((object) this, __methodptr(OnWorldSetEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTargetActionComponent, ActionSetTargetEvent>(new EntityEventRefHandler<EntityTargetActionComponent, ActionSetTargetEvent>((object) this, __methodptr(OnEntitySetTarget)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WorldTargetActionComponent, ActionSetTargetEvent>(new EntityEventRefHandler<WorldTargetActionComponent, ActionSetTargetEvent>((object) this, __methodptr(OnWorldSetTarget)), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<RequestPerformActionEvent>(new EntitySessionEventHandler<RequestPerformActionEvent>(this.OnActionRequest), (Type[]) null, (Type[]) null);
  }

  private void OnActionMapInit(Entity<ActionComponent> ent, ref MapInitEvent args)
  {
    ActionComponent comp = ent.Comp;
    comp.OriginalIconColor = comp.IconColor;
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "OriginalIconColor", (MetaDataComponent) null);
  }

  private void OnActionShutdown(Entity<ActionComponent> ent, ref ComponentShutdown args)
  {
    EntityUid? attachedEntity = ent.Comp.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    if (this.TerminatingOrDeleted(valueOrDefault, (MetaDataComponent) null))
      return;
    this.RemoveAction(Entity<ActionsComponent>.op_Implicit(valueOrDefault), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent)))));
  }

  private void OnShutdown(Entity<ActionsComponent> ent, ref ComponentShutdown args)
  {
    foreach (EntityUid action in ent.Comp.Actions)
      this.RemoveAction(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(ent), Entity<ActionsComponent>.op_Implicit(ent))), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action)));
  }

  private void OnGetState(Entity<ActionsComponent> ent, ref ComponentGetState args)
  {
    ((ComponentGetState) ref args).State = (IComponentState) new ActionsComponentState(this.GetNetEntitySet(ent.Comp.Actions));
  }

  public Entity<ActionComponent>? GetAction(Entity<ActionComponent?>? action, bool logError = true)
  {
    if (action.HasValue)
    {
      Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
      if (!this.Deleted(Entity<ActionComponent>.op_Implicit(valueOrDefault), (MetaDataComponent) null))
        return !this._actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(valueOrDefault), ref valueOrDefault.Comp, logError) ? new Entity<ActionComponent>?() : new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp)));
    }
    return new Entity<ActionComponent>?();
  }

  public void SetCooldown(Entity<ActionComponent?>? action, TimeSpan start, TimeSpan end)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    valueOrDefault.Comp.Cooldown = new ActionCooldown?(new ActionCooldown()
    {
      Start = start,
      End = end
    });
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "Cooldown", (MetaDataComponent) null);
  }

  public void RemoveCooldown(Entity<ActionComponent?>? action)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    valueOrDefault.Comp.Cooldown = new ActionCooldown?();
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "Cooldown", (MetaDataComponent) null);
  }

  public void SetCooldown(Entity<ActionComponent?>? action, TimeSpan cooldown)
  {
    TimeSpan curTime = this.GameTiming.CurTime;
    this.SetCooldown(action, curTime, curTime + cooldown);
  }

  public void ClearCooldown(Entity<ActionComponent?>? action)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault1 = action1.GetValueOrDefault();
    ActionCooldown? cooldown = valueOrDefault1.Comp.Cooldown;
    if (!cooldown.HasValue)
      return;
    ActionCooldown valueOrDefault2 = cooldown.GetValueOrDefault();
    valueOrDefault1.Comp.Cooldown = new ActionCooldown?(new ActionCooldown()
    {
      Start = valueOrDefault2.Start,
      End = this.GameTiming.CurTime
    });
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault1), valueOrDefault1.Comp, "Cooldown", (MetaDataComponent) null);
  }

  public void SetIfBiggerCooldown(Entity<ActionComponent?>? action, TimeSpan cooldown)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    if (cooldown < TimeSpan.Zero)
      return;
    TimeSpan curTime = this.GameTiming.CurTime;
    TimeSpan end = curTime + cooldown;
    ref ActionCooldown? local = ref valueOrDefault.Comp.Cooldown;
    if ((local.HasValue ? (local.GetValueOrDefault().End > end ? 1 : 0) : 0) != 0)
      return;
    this.SetCooldown(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))), curTime, end);
  }

  public void StartUseDelay(Entity<ActionComponent?>? action)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault1 = action1.GetValueOrDefault();
    TimeSpan? useDelay = valueOrDefault1.Comp.UseDelay;
    if (!useDelay.HasValue)
      return;
    TimeSpan valueOrDefault2 = useDelay.GetValueOrDefault();
    this.SetCooldown(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault1), Entity<ActionComponent>.op_Implicit(valueOrDefault1)))), valueOrDefault2);
  }

  public void SetUseDelay(Entity<ActionComponent?>? action, TimeSpan? delay)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    TimeSpan? useDelay = valueOrDefault.Comp.UseDelay;
    TimeSpan? nullable = delay;
    if ((useDelay.HasValue == nullable.HasValue ? (useDelay.HasValue ? (useDelay.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    valueOrDefault.Comp.UseDelay = delay;
    this.UpdateAction(valueOrDefault);
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "UseDelay", (MetaDataComponent) null);
  }

  public void ReduceUseDelay(Entity<ActionComponent?>? action, TimeSpan? lowerDelay)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    if (valueOrDefault.Comp.UseDelay.HasValue && lowerDelay.HasValue)
    {
      ActionComponent comp = valueOrDefault.Comp;
      TimeSpan? useDelay = comp.UseDelay;
      TimeSpan? nullable = lowerDelay;
      comp.UseDelay = useDelay.HasValue & nullable.HasValue ? new TimeSpan?(useDelay.GetValueOrDefault() - nullable.GetValueOrDefault()) : new TimeSpan?();
    }
    TimeSpan? useDelay1 = valueOrDefault.Comp.UseDelay;
    TimeSpan zero = TimeSpan.Zero;
    if ((useDelay1.HasValue ? (useDelay1.GetValueOrDefault() < zero ? 1 : 0) : 0) != 0)
      valueOrDefault.Comp.UseDelay = new TimeSpan?();
    this.UpdateAction(valueOrDefault);
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "UseDelay", (MetaDataComponent) null);
  }

  private void OnRejuventate(Entity<ActionsComponent> ent, ref RejuvenateEvent args)
  {
    foreach (EntityUid action in ent.Comp.Actions)
      this.ClearCooldown(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action)));
  }

  public virtual void UpdateAction(Entity<ActionComponent> ent)
  {
  }

  public void SetToggled(Entity<ActionComponent?>? action, bool toggled)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    if (valueOrDefault.Comp.Toggled == toggled)
      return;
    valueOrDefault.Comp.Toggled = toggled;
    this.UpdateAction(valueOrDefault);
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "Toggled", (MetaDataComponent) null);
  }

  public void SetEnabled(Entity<ActionComponent?>? action, bool enabled)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    if (valueOrDefault.Comp.Enabled == enabled)
      return;
    valueOrDefault.Comp.Enabled = enabled;
    this.UpdateAction(valueOrDefault);
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "Enabled", (MetaDataComponent) null);
  }

  private void OnActionRequest(RequestPerformActionEvent ev, EntitySessionEventArgs args)
  {
    this._rmcLagCompensation.SetLastRealTick(((EntitySessionEventArgs) ref args).SenderSession.UserId, ev.LastRealTick);
    EntityUid? attachedEntity = ((EntitySessionEventArgs) ref args).SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault1 = attachedEntity.GetValueOrDefault();
    ActionsComponent actionsComponent;
    if (!this._actionsQuery.TryComp(valueOrDefault1, ref actionsComponent))
      return;
    EntityUid entity = this.GetEntity(ev.Action);
    MetaDataComponent metaDataComponent;
    if (!this.TryComp(entity, ref metaDataComponent))
      return;
    string str = this.Name(entity, metaDataComponent);
    if (!actionsComponent.Actions.Contains(entity))
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(56, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault1)), "user", "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" attempted to perform an action that they do not have: ");
      logStringHandler.AppendFormatted(str);
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, ref local);
    }
    else
    {
      Entity<ActionComponent>? action = this.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(entity)));
      if (!action.HasValue)
        return;
      Entity<ActionComponent> valueOrDefault2 = action.GetValueOrDefault();
      if (!valueOrDefault2.Comp.Enabled)
        return;
      TimeSpan curTime = this.GameTiming.CurTime;
      if (this.IsCooldownActive(Entity<ActionComponent>.op_Implicit(valueOrDefault2), new TimeSpan?(curTime)))
        return;
      ActionAttemptEvent actionAttemptEvent = new ActionAttemptEvent(valueOrDefault1);
      this.RaiseLocalEvent<ActionAttemptEvent>(Entity<ActionComponent>.op_Implicit(valueOrDefault2), ref actionAttemptEvent, false);
      if (actionAttemptEvent.Cancelled)
        return;
      EntityUid entityUid = valueOrDefault2.Comp.Container ?? valueOrDefault1;
      ActionValidateEvent actionValidateEvent = new ActionValidateEvent()
      {
        Input = ev,
        User = valueOrDefault1,
        Provider = entityUid
      };
      this.RaiseLocalEvent<ActionValidateEvent>(Entity<ActionComponent>.op_Implicit(valueOrDefault2), ref actionValidateEvent, false);
      if (actionValidateEvent.Invalid || !this._rmcActions.CanUseActionPopup(valueOrDefault1, entity, this.GetEntity(ev.EntityTarget)))
        return;
      this.PerformAction(Entity<ActionsComponent>.op_Implicit((valueOrDefault1, actionsComponent)), valueOrDefault2);
    }
  }

  private void OnValidate(Entity<ActionComponent> ent, ref ActionValidateEvent args)
  {
    if ((!ent.Comp.CheckConsciousness || this._actionBlocker.CanConsciouslyPerformAction(args.User)) && (!ent.Comp.CheckCanInteract || this._actionBlocker.CanInteract(args.User, new EntityUid?())))
      return;
    args.Invalid = true;
  }

  private void OnInstantValidate(Entity<InstantActionComponent> ent, ref ActionValidateEvent args)
  {
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(40, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" is performing the ");
    logStringHandler.AppendFormatted(this.Name(Entity<InstantActionComponent>.op_Implicit(ent), (MetaDataComponent) null), format: "action");
    logStringHandler.AppendLiteral(" action provided by ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Provider)), "provider", "ToPrettyString(args.Provider)");
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, ref local);
  }

  private void OnEntityValidate(
    Entity<EntityTargetActionComponent> ent,
    ref ActionValidateEvent args)
  {
    EntityTargetActionEvent targetActionEvent = ent.Comp.Event;
    if (targetActionEvent == null)
      return;
    NetEntity? entityTarget = args.Input.EntityTarget;
    if (entityTarget.HasValue)
    {
      NetEntity valueOrDefault = entityTarget.GetValueOrDefault();
      EntityUid user = args.User;
      EntityUid entity = this.GetEntity(valueOrDefault);
      Vector2 worldPosition = this._transform.GetWorldPosition(entity);
      if (ent.Comp.RotateOnUse)
        this._rotateToFace.TryFaceCoordinates(user, worldPosition);
      if (!this.ValidateEntityTarget(user, entity, ent))
      {
        args.Invalid = true;
      }
      else
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(55, 4);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" is performing the ");
        logStringHandler.AppendFormatted(this.Name(Entity<EntityTargetActionComponent>.op_Implicit(ent), (MetaDataComponent) null), format: "action");
        logStringHandler.AppendLiteral(" action (provided by ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Provider)), "provider", "ToPrettyString(args.Provider)");
        logStringHandler.AppendLiteral(") targeted at ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity)), "target", "ToPrettyString(target)");
        logStringHandler.AppendLiteral(".");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Action, ref local);
        targetActionEvent.Target = entity;
      }
    }
    else
      args.Invalid = true;
  }

  private void OnWorldValidate(Entity<WorldTargetActionComponent> ent, ref ActionValidateEvent args)
  {
    NetCoordinates? coordinatesTarget = args.Input.EntityCoordinatesTarget;
    if (coordinatesTarget.HasValue)
    {
      NetCoordinates valueOrDefault = coordinatesTarget.GetValueOrDefault();
      EntityUid user = args.User;
      EntityCoordinates coordinates = this.GetCoordinates(valueOrDefault);
      if (ent.Comp.RotateOnUse)
        this._rotateToFace.TryFaceCoordinates(user, this._transform.ToMapCoordinates(coordinates, true).Position);
      if (!this.ValidateWorldTarget(user, coordinates, ent))
        return;
      EntityUid? entity = this.GetEntity(args.Input.EntityTarget);
      EntityTargetActionComponent targetActionComponent;
      if (entity.HasValue && (!this.TryComp<EntityTargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent), ref targetActionComponent) || !this.ValidateEntityTarget(user, entity.Value, Entity<EntityTargetActionComponent>.op_Implicit((Entity<WorldTargetActionComponent>.op_Implicit(ent), targetActionComponent)))))
      {
        args.Invalid = true;
      }
      else
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(57, 5);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" is performing the ");
        logStringHandler.AppendFormatted(this.Name(Entity<WorldTargetActionComponent>.op_Implicit(ent), (MetaDataComponent) null), format: "action");
        logStringHandler.AppendLiteral(" action (provided by ");
        logStringHandler.AppendFormatted<EntityUid>(args.Provider, "args.Provider");
        logStringHandler.AppendLiteral(") targeting ");
        logStringHandler.AppendFormatted<EntityUid?>(entity, "targetEntity");
        logStringHandler.AppendLiteral(" at ");
        logStringHandler.AppendFormatted<EntityCoordinates>(coordinates, "target", "target");
        logStringHandler.AppendLiteral(".");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Action, ref local);
        WorldTargetActionEvent targetActionEvent = ent.Comp.Event;
        if (targetActionEvent == null)
          return;
        targetActionEvent.Target = coordinates;
        targetActionEvent.Entity = entity;
      }
    }
    else
      args.Invalid = true;
  }

  public bool ValidateEntityTarget(
    EntityUid user,
    EntityUid target,
    Entity<EntityTargetActionComponent> ent)
  {
    EntityUid entityUid1;
    EntityTargetActionComponent targetActionComponent1;
    ent.Deconstruct(ref entityUid1, ref targetActionComponent1);
    EntityUid entityUid2 = entityUid1;
    EntityTargetActionComponent targetActionComponent2 = targetActionComponent1;
    if (!((EntityUid) ref target).IsValid() || this.Deleted(target, (MetaDataComponent) null))
    {
      this.RaisePredictiveEvent<RMCMissedTargetActionEvent>(new RMCMissedTargetActionEvent(this.GetNetEntity(Entity<EntityTargetActionComponent>.op_Implicit(ent), (MetaDataComponent) null)));
      return false;
    }
    if (this._whitelist.IsWhitelistFail(targetActionComponent2.Whitelist, target) || this._whitelist.IsBlacklistPass(targetActionComponent2.Blacklist, target) || this._actionQuery.Comp(entityUid2).CheckCanInteract && !this._actionBlocker.CanInteract(user, new EntityUid?(target)) && ent.Comp.TargetCheckCanInteract)
      return false;
    if (EntityUid.op_Equality(user, target))
      return targetActionComponent2.CanTargetSelf;
    TargetActionComponent targetActionComponent3 = this.Comp<TargetActionComponent>(entityUid2);
    return !targetActionComponent3.CheckCanAccess || this._interaction.InRangeAndAccessible(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), targetActionComponent3.Range, lagCompensated: true) || this._interaction.CanAccessViaStorage(user, target);
  }

  public bool ValidateWorldTarget(
    EntityUid user,
    EntityCoordinates target,
    Entity<WorldTargetActionComponent> ent)
  {
    TargetActionComponent targetActionComponent = this.Comp<TargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent));
    return this.ValidateBaseTarget(user, target, Entity<TargetActionComponent>.op_Implicit((Entity<WorldTargetActionComponent>.op_Implicit(ent), targetActionComponent)));
  }

  private bool ValidateBaseTarget(
    EntityUid user,
    EntityCoordinates coords,
    Entity<TargetActionComponent> ent)
  {
    TargetActionComponent comp = ent.Comp;
    if (comp.CheckCanAccess)
      return this._interaction.InRangeUnobstructed(user, coords, comp.Range);
    TransformComponent transformComponent = this.Transform(user);
    if (MapId.op_Inequality(transformComponent.MapID, this._transform.GetMapId(coords)))
      return false;
    return (double) comp.Range <= 0.0 || this._transform.InRange(coords, transformComponent.Coordinates, comp.Range);
  }

  private void OnInstantGetEvent(Entity<InstantActionComponent> ent, ref ActionGetEventEvent args)
  {
    InstantActionEvent instantActionEvent = ent.Comp.Event;
    if (instantActionEvent == null)
      return;
    args.Event = (BaseActionEvent) instantActionEvent;
  }

  private void OnEntityGetEvent(
    Entity<EntityTargetActionComponent> ent,
    ref ActionGetEventEvent args)
  {
    EntityTargetActionEvent targetActionEvent = ent.Comp.Event;
    if (targetActionEvent == null)
      return;
    args.Event = (BaseActionEvent) targetActionEvent;
  }

  private void OnWorldGetEvent(Entity<WorldTargetActionComponent> ent, ref ActionGetEventEvent args)
  {
    WorldTargetActionEvent targetActionEvent = ent.Comp.Event;
    if (targetActionEvent == null)
      return;
    args.Event = (BaseActionEvent) targetActionEvent;
  }

  private void OnInstantSetEvent(Entity<InstantActionComponent> ent, ref ActionSetEventEvent args)
  {
    if (!(args.Event is InstantActionEvent instantActionEvent))
      return;
    ent.Comp.Event = instantActionEvent;
    args.Handled = true;
  }

  private void OnEntitySetEvent(
    Entity<EntityTargetActionComponent> ent,
    ref ActionSetEventEvent args)
  {
    if (!(args.Event is EntityTargetActionEvent targetActionEvent))
      return;
    ent.Comp.Event = targetActionEvent;
    args.Handled = true;
  }

  private void OnWorldSetEvent(Entity<WorldTargetActionComponent> ent, ref ActionSetEventEvent args)
  {
    if (!(args.Event is WorldTargetActionEvent targetActionEvent))
      return;
    ent.Comp.Event = targetActionEvent;
    args.Handled = true;
  }

  private void OnEntitySetTarget(
    Entity<EntityTargetActionComponent> ent,
    ref ActionSetTargetEvent args)
  {
    EntityTargetActionEvent targetActionEvent = ent.Comp.Event;
    if (targetActionEvent == null)
      return;
    targetActionEvent.Target = args.Target;
    args.Handled = true;
  }

  private void OnWorldSetTarget(
    Entity<WorldTargetActionComponent> ent,
    ref ActionSetTargetEvent args)
  {
    WorldTargetActionEvent targetActionEvent = ent.Comp.Event;
    if (targetActionEvent == null)
      return;
    targetActionEvent.Target = this.Transform(args.Target).Coordinates;
    targetActionEvent.Entity = this.HasComp<EntityTargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent)) ? new EntityUid?(args.Target) : new EntityUid?();
    args.Handled = true;
  }

  public void PerformAction(
    Entity<ActionsComponent?> performer,
    Entity<ActionComponent> action,
    BaseActionEvent? actionEvent = null,
    bool predicted = true)
  {
    int num = action.Comp.Toggled ? 1 : 0;
    if (action.Comp.AttachedEntity.HasValue)
    {
      EntityUid? attachedEntity = action.Comp.AttachedEntity;
      EntityUid entityUid = Entity<ActionsComponent>.op_Implicit(performer);
      if ((attachedEntity.HasValue ? (EntityUid.op_Inequality(attachedEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
      {
        this.Log.Error($"{this.ToPrettyString(new EntityUid?(Entity<ActionsComponent>.op_Implicit(performer)), (MetaDataComponent) null)} is attempting to perform an action {this.ToPrettyString(new EntityUid?(Entity<ActionComponent>.op_Implicit(action)), (MetaDataComponent) null)} that is attached to another entity {this.ToPrettyString(action.Comp.AttachedEntity, (MetaDataComponent) null)}");
        return;
      }
    }
    if (actionEvent == null)
      actionEvent = this.GetEvent(Entity<ActionComponent>.op_Implicit(action));
    if (actionEvent == null)
      return;
    BaseActionEvent baseActionEvent = actionEvent;
    baseActionEvent.Performer = Entity<ActionsComponent>.op_Implicit(performer);
    baseActionEvent.Handled = false;
    EntityUid entityUid1 = performer.Owner;
    baseActionEvent.Performer = Entity<ActionsComponent>.op_Implicit(performer);
    baseActionEvent.Action = action;
    EntityUid? nullable1;
    if (!action.Comp.RaiseOnUser)
    {
      nullable1 = action.Comp.Container;
      if (nullable1.HasValue)
      {
        EntityUid valueOrDefault = nullable1.GetValueOrDefault();
        if (!this._mindQuery.HasComp(valueOrDefault))
          entityUid1 = valueOrDefault;
      }
    }
    if (action.Comp.RaiseOnAction)
      entityUid1 = Entity<ActionComponent>.op_Implicit(action);
    this.RaiseLocalEvent(entityUid1, (object) baseActionEvent, true);
    if (!baseActionEvent.Handled)
      return;
    if (baseActionEvent != null && baseActionEvent.Toggle)
      this.SetToggled(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action)))), !action.Comp.Toggled);
    SharedAudioSystem audio = this._audio;
    SoundSpecifier sound = action.Comp.Sound;
    EntityUid entityUid2 = Entity<ActionsComponent>.op_Implicit(performer);
    EntityUid? nullable2;
    if (!predicted)
    {
      nullable1 = new EntityUid?();
      nullable2 = nullable1;
    }
    else
      nullable2 = new EntityUid?(Entity<ActionsComponent>.op_Implicit(performer));
    AudioParams? nullable3 = new AudioParams?();
    audio.PlayPredicted(sound, entityUid2, nullable2, nullable3);
    this.RemoveCooldown(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action)))));
    this.StartUseDelay(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action)))));
    this.UpdateAction(action);
    ActionPerformedEvent actionPerformedEvent = new ActionPerformedEvent(Entity<ActionsComponent>.op_Implicit(performer));
    this.RaiseLocalEvent<ActionPerformedEvent>(Entity<ActionComponent>.op_Implicit(action), ref actionPerformedEvent, false);
  }

  public EntityUid? AddAction(
    EntityUid performer,
    [ForbidLiteral] string? actionPrototypeId,
    EntityUid container = default (EntityUid),
    ActionsComponent? component = null)
  {
    EntityUid? actionId = new EntityUid?();
    this.AddAction(performer, ref actionId, out ActionComponent _, actionPrototypeId, container, component);
    return actionId;
  }

  public bool AddAction(
    EntityUid performer,
    [NotNullWhen(true)] ref EntityUid? actionId,
    [ForbidLiteral] string? actionPrototypeId,
    EntityUid container = default (EntityUid),
    ActionsComponent? component = null)
  {
    return this.AddAction(performer, ref actionId, out ActionComponent _, actionPrototypeId, container, component);
  }

  public bool AddAction(
    EntityUid performer,
    [NotNullWhen(true)] ref EntityUid? actionId,
    [NotNullWhen(true)] out ActionComponent? action,
    [ForbidLiteral] string? actionPrototypeId,
    EntityUid container = default (EntityUid),
    ActionsComponent? component = null)
  {
    if (!((EntityUid) ref container).IsValid())
      container = performer;
    return this._actionContainer.EnsureAction(container, ref actionId, out action, actionPrototypeId) && this.AddActionDirect(Entity<ActionsComponent>.op_Implicit((performer, component)), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((actionId.Value, action))));
  }

  public bool AddAction(
    Entity<ActionsComponent?> performer,
    Entity<ActionComponent?> action,
    Entity<ActionsContainerComponent?> container)
  {
    Entity<ActionComponent>? action1 = this.GetAction(new Entity<ActionComponent>?(action));
    if (!action1.HasValue)
      return false;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    EntityUid? container1 = valueOrDefault.Comp.Container;
    EntityUid owner = container.Owner;
    if ((container1.HasValue ? (EntityUid.op_Inequality(container1.GetValueOrDefault(), owner) ? 1 : 0) : 1) == 0 && this.Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true) && ((BaseContainer) container.Comp.Container).Contains(Entity<ActionComponent>.op_Implicit(valueOrDefault)))
      return this.AddActionDirect(performer, new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
    this.Log.Error($"Attempted to add an action with an invalid container: {this.ToPrettyString(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault)), (MetaDataComponent) null)}");
    return false;
  }

  public bool AddActionDirect(Entity<ActionsComponent?> performer, Entity<ActionComponent?>? action)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return false;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    EntityUid? attachedEntity = valueOrDefault.Comp.AttachedEntity;
    if (attachedEntity.HasValue)
      this.RemoveAction(Entity<ActionsComponent>.op_Implicit(attachedEntity.GetValueOrDefault()), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
    if (valueOrDefault.Comp.StartDelay && valueOrDefault.Comp.UseDelay.HasValue)
      this.SetCooldown(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))), valueOrDefault.Comp.UseDelay.Value);
    ref ActionsComponent local = ref performer.Comp;
    if (local == null)
      local = this.EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
    valueOrDefault.Comp.AttachedEntity = new EntityUid?(Entity<ActionsComponent>.op_Implicit(performer));
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "AttachedEntity", (MetaDataComponent) null);
    performer.Comp.Actions.Add(Entity<ActionComponent>.op_Implicit(valueOrDefault));
    this.Dirty(Entity<ActionsComponent>.op_Implicit(performer), (IComponent) performer.Comp, (MetaDataComponent) null);
    this.ActionAdded(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(performer), performer.Comp)), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp)));
    return true;
  }

  protected virtual void ActionAdded(
    Entity<ActionsComponent> performer,
    Entity<ActionComponent> action)
  {
  }

  public void GrantActions(
    Entity<ActionsComponent?> performer,
    IEnumerable<EntityUid> actions,
    Entity<ActionsContainerComponent?> container)
  {
    if (!this.Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true))
      return;
    ref ActionsComponent local = ref performer.Comp;
    if (local == null)
      local = this.EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
    foreach (EntityUid action in actions)
      this.AddAction(performer, Entity<ActionComponent>.op_Implicit(action), container);
  }

  public void GrantContainedActions(
    Entity<ActionsComponent?> performer,
    Entity<ActionsContainerComponent?> container)
  {
    if (!this.Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true))
      return;
    ref ActionsComponent local = ref performer.Comp;
    if (local == null)
      local = this.EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ((BaseContainer) container.Comp.Container).ContainedEntities)
    {
      Entity<ActionComponent>? action = this.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(containedEntity)));
      if (action.HasValue)
      {
        Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
        this.AddActionDirect(performer, new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
      }
    }
  }

  public void GrantContainedAction(
    Entity<ActionsComponent?> performer,
    Entity<ActionsContainerComponent?> container,
    EntityUid actionId)
  {
    if (!this.Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true))
      return;
    ref ActionsComponent local = ref performer.Comp;
    if (local == null)
      local = this.EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
    this.AddActionDirect(performer, new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionId)));
  }

  public IEnumerable<Entity<ActionComponent>> GetActions(
    EntityUid holderId,
    ActionsComponent? actions = null)
  {
    SharedActionsSystem sharedActionsSystem = this;
    if (sharedActionsSystem.Resolve<ActionsComponent>(holderId, ref actions, false))
    {
      foreach (EntityUid action1 in actions.Actions)
      {
        Entity<ActionComponent>? action2 = sharedActionsSystem.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action1)));
        if (action2.HasValue)
          yield return action2.GetValueOrDefault();
      }
    }
  }

  public void RemoveProvidedActions(
    EntityUid performer,
    EntityUid container,
    ActionsComponent? comp = null)
  {
    if (!this.Resolve<ActionsComponent>(performer, ref comp, false))
      return;
    foreach (EntityUid entityUid1 in comp.Actions.ToArray<EntityUid>())
    {
      Entity<ActionComponent>? action = this.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(entityUid1)));
      if (!action.HasValue)
        break;
      Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
      EntityUid? container1 = valueOrDefault.Comp.Container;
      EntityUid entityUid2 = container;
      if ((container1.HasValue ? (EntityUid.op_Equality(container1.GetValueOrDefault(), entityUid2) ? 1 : 0) : 0) != 0)
        this.RemoveAction(Entity<ActionsComponent>.op_Implicit((performer, comp)), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
    }
  }

  public void RemoveProvidedAction(
    EntityUid performer,
    EntityUid container,
    EntityUid actionId,
    ActionsComponent? comp = null)
  {
    if (!this._actionsQuery.Resolve(performer, ref comp, false))
      return;
    Entity<ActionComponent>? action = this.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionId)));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
    EntityUid? container1 = valueOrDefault.Comp.Container;
    EntityUid entityUid = container;
    if ((container1.HasValue ? (EntityUid.op_Equality(container1.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.RemoveAction(Entity<ActionsComponent>.op_Implicit((performer, comp)), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
  }

  public void RemoveAction(Entity<ActionComponent?>? action)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault1 = action1.GetValueOrDefault();
    EntityUid? attachedEntity = valueOrDefault1.Comp.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault2 = attachedEntity.GetValueOrDefault();
    ActionsComponent actionsComponent;
    if (!this._actionsQuery.TryComp(valueOrDefault2, ref actionsComponent))
      return;
    this.RemoveAction(Entity<ActionsComponent>.op_Implicit((valueOrDefault2, actionsComponent)), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault1), Entity<ActionComponent>.op_Implicit(valueOrDefault1)))));
  }

  public void RemoveAction(Entity<ActionsComponent?> performer, Entity<ActionComponent?>? action)
  {
    Entity<ActionComponent>? action1 = this.GetAction(action);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    EntityUid? attachedEntity = valueOrDefault.Comp.AttachedEntity;
    EntityUid owner = performer.Owner;
    if ((attachedEntity.HasValue ? (EntityUid.op_Inequality(attachedEntity.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0)
    {
      if (this.GameTiming.ApplyingState)
        return;
      this.Log.Error($"Attempted to remove an action {this.ToPrettyString(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault)), (MetaDataComponent) null)} from an entity that it was never attached to: {this.ToPrettyString(new EntityUid?(Entity<ActionsComponent>.op_Implicit(performer)), (MetaDataComponent) null)}. Trace: {Environment.StackTrace}");
    }
    else if (!this._actionsQuery.Resolve(Entity<ActionsComponent>.op_Implicit(performer), ref performer.Comp, false))
    {
      valueOrDefault.Comp.AttachedEntity = new EntityUid?();
    }
    else
    {
      performer.Comp.Actions.Remove(valueOrDefault.Owner);
      this.Dirty(Entity<ActionsComponent>.op_Implicit(performer), (IComponent) performer.Comp, (MetaDataComponent) null);
      valueOrDefault.Comp.AttachedEntity = new EntityUid?();
      this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "AttachedEntity", (MetaDataComponent) null);
      this.ActionRemoved(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(performer), performer.Comp)), valueOrDefault);
      if (!valueOrDefault.Comp.Temporary)
        return;
      this.QueueDel(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault)));
    }
  }

  protected virtual void ActionRemoved(
    Entity<ActionsComponent> performer,
    Entity<ActionComponent> action)
  {
  }

  public bool ValidAction(Entity<ActionComponent> ent, bool canReach = true)
  {
    EntityUid entityUid;
    ActionComponent actionComponent1;
    ent.Deconstruct(ref entityUid, ref actionComponent1);
    ActionComponent actionComponent2 = actionComponent1;
    if (!actionComponent2.Enabled)
      return false;
    TimeSpan curTime = this.GameTiming.CurTime;
    if (actionComponent2.Cooldown.HasValue && actionComponent2.Cooldown.Value.End > curTime)
      return false;
    if (canReach)
      return true;
    TargetActionComponent targetActionComponent = this.Comp<TargetActionComponent>(Entity<ActionComponent>.op_Implicit(ent));
    return targetActionComponent != null && !targetActionComponent.CheckCanAccess;
  }

  private void OnRelayActionCompChange(
    Entity<ActionsComponent> ent,
    ref RelayedActionComponentChangeEvent args)
  {
    if (args.Handled)
      return;
    AttemptRelayActionComponentChangeEvent componentChangeEvent = new AttemptRelayActionComponentChangeEvent();
    this.RaiseLocalEvent<AttemptRelayActionComponentChangeEvent>(ent.Owner, ref componentChangeEvent, false);
    EntityUid entityUid = componentChangeEvent.Target ?? ent.Owner;
    args.Handled = true;
    args.Toggle = true;
    if (!args.Action.Comp.Toggled)
      this.EntityManager.AddComponents(entityUid, args.Components, true);
    else
      this.EntityManager.RemoveComponents(entityUid, args.Components);
  }

  private void OnActionCompChange(Entity<ActionsComponent> ent, ref ActionComponentChangeEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    args.Toggle = true;
    EntityUid owner = ent.Owner;
    if (!args.Action.Comp.Toggled)
      this.EntityManager.AddComponents(owner, args.Components, true);
    else
      this.EntityManager.RemoveComponents(owner, args.Components);
  }

  private void OnDidEquip(Entity<ActionsComponent> ent, ref DidEquipEvent args)
  {
    if (this.GameTiming.ApplyingState)
      return;
    GetItemActionsEvent itemActionsEvent = new GetItemActionsEvent(this._actionContainer, args.Equipee, args.Equipment, new SlotFlags?(args.SlotFlags));
    this.RaiseLocalEvent<GetItemActionsEvent>(args.Equipment, itemActionsEvent, false);
    if (itemActionsEvent.Actions.Count == 0)
      return;
    this.GrantActions(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(ent), Entity<ActionsComponent>.op_Implicit(ent))), (IEnumerable<EntityUid>) itemActionsEvent.Actions, Entity<ActionsContainerComponent>.op_Implicit(args.Equipment));
  }

  private void OnHandEquipped(Entity<ActionsComponent> ent, ref DidEquipHandEvent args)
  {
    if (this.GameTiming.ApplyingState)
      return;
    GetItemActionsEvent itemActionsEvent = new GetItemActionsEvent(this._actionContainer, args.User, args.Equipped);
    this.RaiseLocalEvent<GetItemActionsEvent>(args.Equipped, itemActionsEvent, false);
    if (itemActionsEvent.Actions.Count == 0)
      return;
    this.GrantActions(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(ent), Entity<ActionsComponent>.op_Implicit(ent))), (IEnumerable<EntityUid>) itemActionsEvent.Actions, Entity<ActionsContainerComponent>.op_Implicit(args.Equipped));
  }

  private void OnDidUnequip(EntityUid uid, ActionsComponent component, DidUnequipEvent args)
  {
    if (this.GameTiming.ApplyingState)
      return;
    this.RemoveProvidedActions(uid, args.Equipment, component);
  }

  private void OnHandUnequipped(
    EntityUid uid,
    ActionsComponent component,
    DidUnequipHandEvent args)
  {
    if (this.GameTiming.ApplyingState)
      return;
    this.RemoveProvidedActions(uid, args.Unequipped, component);
  }

  public void SetEntityIcon(Entity<ActionComponent?> ent, EntityUid? icon)
  {
    if (!this._actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    EntityUid? entityIcon = ent.Comp.EntityIcon;
    EntityUid? nullable = icon;
    if ((entityIcon.HasValue == nullable.HasValue ? (entityIcon.HasValue ? (EntityUid.op_Equality(entityIcon.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
      return;
    ent.Comp.EntityIcon = icon;
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "EntIcon", (MetaDataComponent) null);
  }

  public void SetItemIconStyle(Entity<ActionComponent?> ent, ItemActionIconStyle style)
  {
    if (!this._actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) || ent.Comp.ItemIconStyle == style)
      return;
    ent.Comp.ItemIconStyle = style;
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "ItemIconStyle", (MetaDataComponent) null);
  }

  public void SetIcon(Entity<ActionComponent?> ent, SpriteSpecifier? icon)
  {
    if (!this._actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) || ent.Comp.Icon == icon)
      return;
    ent.Comp.Icon = icon;
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Icon", (MetaDataComponent) null);
  }

  public void SetIconOn(Entity<ActionComponent?> ent, SpriteSpecifier? iconOn)
  {
    if (!this._actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) || ent.Comp.IconOn == iconOn)
      return;
    ent.Comp.IconOn = iconOn;
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "IconOn", (MetaDataComponent) null);
  }

  public void SetIconColor(Entity<ActionComponent?> ent, Color color)
  {
    if (!this._actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) || Color.op_Equality(ent.Comp.IconColor, color))
      return;
    ent.Comp.IconColor = color;
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "IconColor", (MetaDataComponent) null);
  }

  public void SetEvent(EntityUid uid, BaseActionEvent ev)
  {
    ActionSetEventEvent actionSetEventEvent = new ActionSetEventEvent(ev);
    this.RaiseLocalEvent<ActionSetEventEvent>(uid, ref actionSetEventEvent, false);
    if (actionSetEventEvent.Handled)
      return;
    this.Log.Error($"Tried to set event of {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)):action} but nothing handled it!");
  }

  public BaseActionEvent? GetEvent(EntityUid uid)
  {
    ActionGetEventEvent actionGetEventEvent = new ActionGetEventEvent();
    this.RaiseLocalEvent<ActionGetEventEvent>(uid, ref actionGetEventEvent, false);
    return actionGetEventEvent.Event;
  }

  public bool SetEventTarget(EntityUid uid, EntityUid target)
  {
    ActionSetTargetEvent actionSetTargetEvent = new ActionSetTargetEvent(target);
    this.RaiseLocalEvent<ActionSetTargetEvent>(uid, ref actionSetTargetEvent, false);
    return actionSetTargetEvent.Handled;
  }

  public bool IsCooldownActive(ActionComponent action, TimeSpan? curTime = null)
  {
    if (!action.Cooldown.HasValue)
      return false;
    TimeSpan end = action.Cooldown.Value.End;
    TimeSpan? nullable = curTime;
    return nullable.HasValue && end > nullable.GetValueOrDefault();
  }

  public void SetTemporary(Entity<ActionComponent?> ent, bool temporary)
  {
    if (!this.Resolve<ActionComponent>(ent.Owner, ref ent.Comp, false))
      return;
    ent.Comp.Temporary = temporary;
    this.Dirty<ActionComponent>(ent, (MetaDataComponent) null);
  }
}
