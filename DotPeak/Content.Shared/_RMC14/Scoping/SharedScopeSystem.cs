// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Scoping.SharedScopeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Emplacements;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Camera;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Scoping;

public abstract class SharedScopeSystem : EntitySystem
{
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedContentEyeSystem _contentEye;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedWeaponMountSystem _weaponMount;

  public override void Initialize()
  {
    this.InitializeUser();
    this.SubscribeLocalEvent<ScopeComponent, MapInitEvent>(new EntityEventRefHandler<ScopeComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ScopeComponent, ComponentRemove>(new EntityEventRefHandler<ScopeComponent, ComponentRemove>(this.OnShutdown));
    this.SubscribeLocalEvent<ScopeComponent, EntityTerminatingEvent>(new EntityEventRefHandler<ScopeComponent, EntityTerminatingEvent>(this.OnScopeEntityTerminating));
    this.SubscribeLocalEvent<ScopeComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<ScopeComponent, GotUnequippedHandEvent>(this.OnUnequip));
    this.SubscribeLocalEvent<ScopeComponent, HandDeselectedEvent>(new EntityEventRefHandler<ScopeComponent, HandDeselectedEvent>(this.OnDeselectHand));
    this.SubscribeLocalEvent<ScopeComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<ScopeComponent, ItemUnwieldedEvent>(this.OnUnwielded));
    this.SubscribeLocalEvent<ScopeComponent, GetItemActionsEvent>(new EntityEventRefHandler<ScopeComponent, GetItemActionsEvent>(this.OnGetActions));
    this.SubscribeLocalEvent<ScopeComponent, ToggleActionEvent>(new EntityEventRefHandler<ScopeComponent, ToggleActionEvent>(this.OnToggleAction));
    this.SubscribeLocalEvent<ScopeComponent, ScopeCycleZoomLevelEvent>(new EntityEventRefHandler<ScopeComponent, ScopeCycleZoomLevelEvent>(this.OnCycleZoomLevel));
    this.SubscribeLocalEvent<ScopeComponent, ActivateInWorldEvent>(new EntityEventRefHandler<ScopeComponent, ActivateInWorldEvent>(this.OnActivateInWorld));
    this.SubscribeLocalEvent<ScopeComponent, GunShotEvent>(new EntityEventRefHandler<ScopeComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<ScopeComponent, ScopeDoAfterEvent>(new EntityEventRefHandler<ScopeComponent, ScopeDoAfterEvent>(this.OnScopeDoAfter));
    this.SubscribeLocalEvent<GunScopingComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<GunScopingComponent, GotUnequippedHandEvent>(this.OnGunUnequip));
    this.SubscribeLocalEvent<GunScopingComponent, HandDeselectedEvent>(new EntityEventRefHandler<GunScopingComponent, HandDeselectedEvent>(this.OnGunDeselectHand));
    this.SubscribeLocalEvent<GunScopingComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<GunScopingComponent, ItemUnwieldedEvent>(this.OnGunUnwielded));
    this.SubscribeLocalEvent<GunScopingComponent, GunShotEvent>(new EntityEventRefHandler<GunScopingComponent, GunShotEvent>(this.OnGunGunShot));
  }

  private void OnMapInit(Entity<ScopeComponent> ent, ref MapInitEvent args)
  {
    this._actionContainer.EnsureAction(ent.Owner, ref ent.Comp.ScopingToggleActionEntity, (string) ent.Comp.ScopingToggleAction);
    if (ent.Comp.ZoomLevels.Count > 1)
      this._actionContainer.EnsureAction(ent.Owner, ref ent.Comp.CycleZoomLevelActionEntity, (string) ent.Comp.CycleZoomLevelAction);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  private void OnShutdown(Entity<ScopeComponent> ent, ref ComponentRemove args)
  {
    EntityUid? user = ent.Comp.User;
    if (!user.HasValue)
      return;
    EntityUid valueOrDefault = user.GetValueOrDefault();
    this.Unscope(ent);
    this._actionsSystem.RemoveProvidedActions(valueOrDefault, ent.Owner);
  }

  private void OnScopeEntityTerminating(Entity<ScopeComponent> ent, ref EntityTerminatingEvent args)
  {
    this.Unscope(ent);
  }

  private void OnUnequip(Entity<ScopeComponent> ent, ref GotUnequippedHandEvent args)
  {
    this.Unscope(ent);
  }

  private void OnDeselectHand(Entity<ScopeComponent> ent, ref HandDeselectedEvent args)
  {
    this.Unscope(ent);
  }

  private void OnUnwielded(Entity<ScopeComponent> ent, ref ItemUnwieldedEvent args)
  {
    if (!ent.Comp.RequireWielding)
      return;
    this.Unscope(ent);
  }

  private void OnGetActions(Entity<ScopeComponent> ent, ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.ScopingToggleActionEntity, (string) ent.Comp.ScopingToggleAction);
    if (ent.Comp.ZoomLevels.Count <= 1)
      return;
    args.AddAction(ref ent.Comp.CycleZoomLevelActionEntity, (string) ent.Comp.CycleZoomLevelAction);
  }

  private void OnToggleAction(Entity<ScopeComponent> ent, ref ToggleActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.ToggleScoping(ent, args.Performer);
  }

  private void OnCycleZoomLevel(Entity<ScopeComponent> scope, ref ScopeCycleZoomLevelEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (scope.Comp.CurrentZoomLevel >= scope.Comp.ZoomLevels.Count - 1)
      scope.Comp.CurrentZoomLevel = 0;
    else
      ++scope.Comp.CurrentZoomLevel;
    ScopeZoomLevel currentZoomLevel = this.GetCurrentZoomLevel(scope);
    if (currentZoomLevel.Name != null)
      this._popup.PopupClient(this.Loc.GetString("rcm-action-popup-scope-cycle-zoom", ("zoom", (object) currentZoomLevel.Name)), args.Performer, new EntityUid?(args.Performer));
    this.Dirty<ScopeComponent>(scope);
  }

  private void OnActivateInWorld(Entity<ScopeComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex || !ent.Comp.UseInHand)
      return;
    args.Handled = true;
    this.ToggleScoping(ent, args.User);
  }

  private void OnGunShot(Entity<ScopeComponent> ent, ref GunShotEvent args)
  {
    if (this.HasComp<WeaponControllerComponent>(args.User))
      return;
    Angle localRotation = this.Transform(args.User).LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    Direction? scopingDirection = ent.Comp.ScopingDirection;
    Direction direction = cardinalDir;
    if (scopingDirection.GetValueOrDefault() == direction & scopingDirection.HasValue)
      return;
    this.Unscope(ent);
  }

  private void OnScopeDoAfter(Entity<ScopeComponent> ent, ref ScopeDoAfterEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (args.Cancelled)
    {
      this.DeleteRelay(ent, new EntityUid?(args.User));
    }
    else
    {
      EntityUid user = args.User;
      if (!this.CanScopePopup(ent, user))
        this.DeleteRelay(ent, new EntityUid?(args.User));
      else
        this.Scope(ent, user, args.Direction);
    }
  }

  private void OnGunUnequip(Entity<GunScopingComponent> ent, ref GotUnequippedHandEvent args)
  {
    this.UnscopeGun(ent);
  }

  private void OnGunDeselectHand(Entity<GunScopingComponent> ent, ref HandDeselectedEvent args)
  {
    this.UnscopeGun(ent);
  }

  private void OnGunUnwielded(Entity<GunScopingComponent> ent, ref ItemUnwieldedEvent args)
  {
    this.UnscopeGun(ent);
  }

  private void OnGunGunShot(Entity<GunScopingComponent> ent, ref GunShotEvent args)
  {
    Angle localRotation = this.Transform(args.User).LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    ScopeComponent comp;
    if (!this.TryComp<ScopeComponent>(ent.Comp.Scope, out comp))
      return;
    Direction? scopingDirection = comp.ScopingDirection;
    Direction direction = cardinalDir;
    if (scopingDirection.GetValueOrDefault() == direction & scopingDirection.HasValue)
      return;
    this.UnscopeGun(ent);
  }

  private bool CanScopePopup(Entity<ScopeComponent> scope, EntityUid user)
  {
    EntityUid active = scope.Owner;
    if (scope.Comp.Attachment && !this.TryGetActiveEntity(scope, out active))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-action-popup-scoping-must-attach", (nameof (scope), (object) active)), user, new EntityUid?(user));
      return false;
    }
    EntityUid? nullable1;
    if (this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out nullable1))
    {
      if (!scope.Comp.Attachment)
      {
        EntityUid? nullable2 = nullable1;
        EntityUid owner = scope.Owner;
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != owner ? 1 : 0) : 1) == 0)
          goto label_7;
      }
      else
        goto label_7;
    }
    if (!scope.Comp.CanUseInsideContainer)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-action-popup-scoping-user-must-hold", (nameof (scope), (object) active)), user, new EntityUid?(user));
      return false;
    }
label_7:
    if (this._pulling.IsPulled(user))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-action-popup-scoping-user-must-not-pulled", (nameof (scope), (object) active)), user, new EntityUid?(user));
      return false;
    }
    if (this._container.IsEntityInContainer(user) && !scope.Comp.CanUseInsideContainer)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-action-popup-scoping-user-must-not-contained", (nameof (scope), (object) active)), user, new EntityUid?(user));
      return false;
    }
    WieldableComponent comp;
    if (scope.Comp.RequireWielding && this.TryComp<WieldableComponent>(active, out comp) && !comp.Wielded)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-action-popup-scoping-user-must-wield", (nameof (scope), (object) active)), user, new EntityUid?(user));
      return false;
    }
    if (this.GetCurrentZoomLevel(scope).AllowMovement || !this.IsMoveHeld(user))
      return true;
    this._popup.PopupClient(this.Loc.GetString("cm-action-popup-scoping-user-must-not-move", (nameof (scope), (object) active)), user, new EntityUid?(user));
    return false;
  }

  public virtual Direction? StartScoping(Entity<ScopeComponent> scope, EntityUid user)
  {
    if (!this.CanScopePopup(scope, user))
      return new Direction?();
    Angle worldRotation = this._transform.GetWorldRotation(user);
    Direction cardinalDir = ((Angle) ref worldRotation).GetCardinalDir();
    ScopeDoAfterEvent @event = new ScopeDoAfterEvent(cardinalDir);
    ScopeZoomLevel currentZoomLevel = this.GetCurrentZoomLevel(scope);
    return this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, currentZoomLevel.DoAfter, (DoAfterEvent) @event, new EntityUid?((EntityUid) scope), used: new EntityUid?((EntityUid) scope))
    {
      BreakOnMove = !currentZoomLevel.AllowMovement
    }) ? new Direction?(cardinalDir) : new Direction?();
  }

  private void Scope(Entity<ScopeComponent> scope, EntityUid user, Direction direction)
  {
    ScopingComponent comp;
    if (this.TryComp<ScopingComponent>(user, out comp))
      this.UserStopScoping((Entity<ScopingComponent>) (user, comp));
    ScopeZoomLevel currentZoomLevel = this.GetCurrentZoomLevel(scope);
    int num = this.HasComp<MountableWeaponComponent>(scope.Owner) ? 1 : 0;
    scope.Comp.User = new EntityUid?(user);
    scope.Comp.ScopingDirection = new Direction?(direction);
    this.Dirty<ScopeComponent>(scope);
    ScopingComponent scopingComponent1 = this.EnsureComp<ScopingComponent>(user);
    scopingComponent1.Scope = new EntityUid?((EntityUid) scope);
    scopingComponent1.AllowMovement = currentZoomLevel.AllowMovement;
    this.Dirty(user, (IComponent) scopingComponent1);
    EntityUid active;
    if (scope.Comp.Attachment && this.TryGetActiveEntity(scope, out active))
    {
      GunScopingComponent scopingComponent2 = this.EnsureComp<GunScopingComponent>(active);
      scopingComponent2.Scope = new EntityUid?((EntityUid) scope);
      this.Dirty(active, (IComponent) scopingComponent2);
    }
    Angle rotation;
    Vector2 vector2 = num == 0 || !this._weaponMount.TryGetMountSeatingRotation(scope.Owner, out rotation) ? this.GetScopeOffset(scope, direction) : ((Angle) ref rotation).ToWorldVec() * this.GetScopeOffsetMagnitude(scope);
    scopingComponent1.EyeOffset = vector2;
    if (scope.Comp.ScopePopup != null)
      this._popup.PopupClient(this.Loc.GetString(scope.Comp.ScopePopup, (nameof (scope), (object) scope.Owner)), user, new EntityUid?(user));
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? toggleActionEntity = scope.Comp.ScopingToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.SetToggled(action, true);
    this._contentEye.SetZoom(user, Vector2.One * currentZoomLevel.Zoom, true);
    this.UpdateOffset(user);
    this.OnScoped(scope, user, direction);
    ScopedEvent args = new ScopedEvent(user, scope);
    this.RaiseLocalEvent<ScopedEvent>(user, ref args);
  }

  public virtual bool Unscope(Entity<ScopeComponent> scope)
  {
    EntityUid? user = scope.Comp.User;
    if (!user.HasValue)
      return false;
    EntityUid valueOrDefault = user.GetValueOrDefault();
    this.RemCompDeferred<ScopingComponent>(valueOrDefault);
    EntityUid active;
    if (scope.Comp.Attachment && this.TryGetActiveEntity(scope, out active))
      this.RemCompDeferred<GunScopingComponent>(active);
    if (scope.Comp.Attachment && scope.Comp.User.HasValue)
    {
      AttachableToggleableInterruptEvent args = new AttachableToggleableInterruptEvent(scope.Comp.User.Value);
      this.RaiseLocalEvent<AttachableToggleableInterruptEvent>(scope.Owner, ref args);
    }
    scope.Comp.User = new EntityUid?();
    scope.Comp.ScopingDirection = new Direction?();
    this.Dirty<ScopeComponent>(scope);
    if (scope.Comp.UnScopePopup != null)
      this._popup.PopupClient(this.Loc.GetString(scope.Comp.UnScopePopup, (nameof (scope), (object) scope.Owner)), valueOrDefault, new EntityUid?(valueOrDefault));
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? toggleActionEntity = scope.Comp.ScopingToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.SetToggled(action, false);
    this._contentEye.ResetZoom(valueOrDefault);
    return true;
  }

  private void UnscopeGun(Entity<GunScopingComponent> gun)
  {
    ScopeComponent comp;
    if (!this.TryComp<ScopeComponent>(gun.Comp.Scope, out comp))
      return;
    this.Unscope((Entity<ScopeComponent>) (gun.Comp.Scope.Value, comp));
  }

  private void ToggleScoping(Entity<ScopeComponent> scope, EntityUid user)
  {
    if (this.HasComp<ScopingComponent>(user))
    {
      this.Unscope(scope);
      ScopingComponent comp;
      if (!this.TryComp<ScopingComponent>(user, out comp))
        return;
      this.UserStopScoping((Entity<ScopingComponent>) (user, comp));
    }
    else
      this.StartScoping(scope, user);
  }

  private bool TryGetActiveEntity(Entity<ScopeComponent> scope, out EntityUid active)
  {
    if (!scope.Comp.Attachment)
    {
      active = (EntityUid) scope;
      return true;
    }
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) scope, (TransformComponent) null), out container) || !this.HasComp<GunComponent>(container.Owner))
    {
      active = new EntityUid();
      return false;
    }
    active = container.Owner;
    return true;
  }

  protected Vector2 GetScopeOffset(Entity<ScopeComponent> scope, Direction direction)
  {
    return DirectionExtensions.ToVec(direction) * this.GetScopeOffsetMagnitude(scope);
  }

  private float GetScopeOffsetMagnitude(Entity<ScopeComponent> scope)
  {
    ScopeZoomLevel currentZoomLevel = this.GetCurrentZoomLevel(scope);
    return (float) (((double) currentZoomLevel.Offset * (double) currentZoomLevel.Zoom - 1.0) / 2.0);
  }

  protected virtual void DeleteRelay(Entity<ScopeComponent> scope, EntityUid? user)
  {
  }

  protected virtual void OnScoped(
    Entity<ScopeComponent> scope,
    EntityUid user,
    Direction direction)
  {
  }

  private ScopeZoomLevel GetCurrentZoomLevel(Entity<ScopeComponent> scope)
  {
    this.ValidateCurrentZoomLevel(scope);
    return scope.Comp.ZoomLevels[scope.Comp.CurrentZoomLevel];
  }

  private void ValidateCurrentZoomLevel(Entity<ScopeComponent> scope)
  {
    bool flag = false;
    if (scope.Comp.ZoomLevels == null || scope.Comp.ZoomLevels.Count <= 0)
    {
      scope.Comp.ZoomLevels = new List<ScopeZoomLevel>()
      {
        new ScopeZoomLevel((string) null, 1f, 15f, false, TimeSpan.FromSeconds(1L))
      };
      flag = true;
    }
    if (scope.Comp.CurrentZoomLevel >= scope.Comp.ZoomLevels.Count)
    {
      scope.Comp.CurrentZoomLevel = 0;
      flag = true;
    }
    if (!flag)
      return;
    this.Dirty<ScopeComponent>(scope);
  }

  private void UpdateOffset(EntityUid user)
  {
    GetEyeOffsetEvent args = new GetEyeOffsetEvent();
    this.RaiseLocalEvent<GetEyeOffsetEvent>(user, ref args);
    this._eye.SetOffset(user, args.Offset);
  }

  private bool IsMoveHeld(EntityUid user)
  {
    InputMoverComponent comp;
    return this.TryComp<InputMoverComponent>(user, out comp) && (comp.HeldMoveButtons & MoveButtons.AnyDirection) != 0;
  }

  private void InitializeUser()
  {
    this.SubscribeLocalEvent<ScopingComponent, ComponentRemove>(new EntityEventRefHandler<ScopingComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<ScopingComponent, MoveInputEvent>(new EntityEventRefHandler<ScopingComponent, MoveInputEvent>(this.OnMoveInput));
    this.SubscribeLocalEvent<ScopingComponent, PullStartedMessage>(new EntityEventRefHandler<ScopingComponent, PullStartedMessage>(this.OnPullStarted));
    this.SubscribeLocalEvent<ScopingComponent, EntParentChangedMessage>(new EntityEventRefHandler<ScopingComponent, EntParentChangedMessage>(this.OnParentChanged));
    this.SubscribeLocalEvent<ScopingComponent, ContainerGettingInsertedAttemptEvent>(new EntityEventRefHandler<ScopingComponent, ContainerGettingInsertedAttemptEvent>(this.OnInsertAttempt));
    this.SubscribeLocalEvent<ScopingComponent, EntityTerminatingEvent>(new EntityEventRefHandler<ScopingComponent, EntityTerminatingEvent>(this.OnEntityTerminating));
    this.SubscribeLocalEvent<ScopingComponent, GetEyeOffsetEvent>(new EntityEventRefHandler<ScopingComponent, GetEyeOffsetEvent>(this.OnGetEyeOffset));
    this.SubscribeLocalEvent<ScopingComponent, PlayerDetachedEvent>(new EntityEventRefHandler<ScopingComponent, PlayerDetachedEvent>(this.OnPlayerDetached));
    this.SubscribeLocalEvent<ScopingComponent, KnockedDownEvent>(new EntityEventRefHandler<ScopingComponent, KnockedDownEvent>(this.OnKnockedDown));
    this.SubscribeLocalEvent<ScopingComponent, StunnedEvent>(new EntityEventRefHandler<ScopingComponent, StunnedEvent>(this.OnStunned));
    this.SubscribeLocalEvent<ScopingComponent, MobStateChangedEvent>(new EntityEventRefHandler<ScopingComponent, MobStateChangedEvent>(this.OnMobStateChanged));
  }

  private void OnRemove(Entity<ScopingComponent> user, ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) user))
      return;
    this.UpdateOffset((EntityUid) user);
  }

  private void OnMoveInput(Entity<ScopingComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement || ent.Comp.AllowMovement)
      return;
    this.UserStopScoping(ent);
  }

  private void OnPullStarted(Entity<ScopingComponent> ent, ref PullStartedMessage args)
  {
    if (args.PulledUid != ent.Owner)
      return;
    this.UserStopScoping(ent);
  }

  private void OnParentChanged(Entity<ScopingComponent> ent, ref EntParentChangedMessage args)
  {
    if (this.HasComp<WeaponControllerComponent>((EntityUid) ent))
      return;
    this.UserStopScoping(ent);
  }

  private void OnInsertAttempt(
    Entity<ScopingComponent> ent,
    ref ContainerGettingInsertedAttemptEvent args)
  {
    this.UserStopScoping(ent);
  }

  private void OnEntityTerminating(Entity<ScopingComponent> ent, ref EntityTerminatingEvent args)
  {
    this.UserStopScoping(ent);
  }

  private void OnGetEyeOffset(Entity<ScopingComponent> ent, ref GetEyeOffsetEvent args)
  {
    EntityUid? scope = ent.Comp.Scope;
    if (scope.HasValue)
    {
      EntityUid valueOrDefault = scope.GetValueOrDefault();
      ScopeComponent comp;
      Angle rotation;
      if (this.TryComp<ScopeComponent>(valueOrDefault, out comp) && this.HasComp<MountableWeaponComponent>(valueOrDefault) && this._weaponMount.TryGetMountSeatingRotation(valueOrDefault, out rotation))
      {
        args.Offset += ((Angle) ref rotation).ToWorldVec() * this.GetScopeOffsetMagnitude((Entity<ScopeComponent>) (valueOrDefault, comp));
        return;
      }
    }
    args.Offset += ent.Comp.EyeOffset;
  }

  private void OnPlayerDetached(Entity<ScopingComponent> ent, ref PlayerDetachedEvent args)
  {
    this.UserStopScoping(ent);
  }

  private void OnKnockedDown(Entity<ScopingComponent> ent, ref KnockedDownEvent args)
  {
    this.UserStopScoping(ent);
  }

  private void OnStunned(Entity<ScopingComponent> ent, ref StunnedEvent args)
  {
    this.UserStopScoping(ent);
  }

  private void OnMobStateChanged(Entity<ScopingComponent> ent, ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Alive)
      return;
    this.UserStopScoping(ent);
  }

  private void UserStopScoping(Entity<ScopingComponent> ent)
  {
    EntityUid? scope = ent.Comp.Scope;
    this.RemCompDeferred<ScopingComponent>((EntityUid) ent);
    ScopeComponent comp;
    if (!this.TryComp<ScopeComponent>(scope, out comp))
      return;
    EntityUid? user = comp.User;
    EntityUid entityUid = (EntityUid) ent;
    if ((user.HasValue ? (user.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0)
      return;
    this.Unscope((Entity<ScopeComponent>) (scope.Value, comp));
  }
}
