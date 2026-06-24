// Decompiled with JetBrains decompiler
// Type: Content.Shared.Blocking.BlockingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Blocking;

public sealed class BlockingSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private FixtureSystem _fixtureSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedAudioSystem _audio;

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeUser();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, GotEquippedHandEvent>(new ComponentEventHandler<BlockingComponent, GotEquippedHandEvent>((object) this, __methodptr(OnEquip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, GotUnequippedHandEvent>(new ComponentEventHandler<BlockingComponent, GotUnequippedHandEvent>((object) this, __methodptr(OnUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, DroppedEvent>(new ComponentEventHandler<BlockingComponent, DroppedEvent>((object) this, __methodptr(OnDrop)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, GetItemActionsEvent>(new ComponentEventHandler<BlockingComponent, GetItemActionsEvent>((object) this, __methodptr(OnGetActions)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, ToggleActionEvent>(new ComponentEventHandler<BlockingComponent, ToggleActionEvent>((object) this, __methodptr(OnToggleAction)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, ComponentShutdown>(new ComponentEventHandler<BlockingComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<BlockingComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnVerbExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, MapInitEvent>(new ComponentEventHandler<BlockingComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(EntityUid uid, BlockingComponent component, MapInitEvent args)
  {
    this._actionContainer.EnsureAction(uid, ref component.BlockingToggleActionEntity, EntProtoId.op_Implicit(component.BlockingToggleAction));
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  private void OnEquip(EntityUid uid, BlockingComponent component, GotEquippedHandEvent args)
  {
    component.User = new EntityUid?(args.User);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    PhysicsComponent physicsComponent;
    if (!this.TryComp<PhysicsComponent>(args.User, ref physicsComponent) || physicsComponent.BodyType == 4 || this.HasComp<BlockingUserComponent>(args.User))
      return;
    BlockingUserComponent blockingUserComponent = this.EnsureComp<BlockingUserComponent>(args.User);
    blockingUserComponent.BlockingItem = new EntityUid?(uid);
    blockingUserComponent.OriginalBodyType = physicsComponent.BodyType;
  }

  private void OnUnequip(EntityUid uid, BlockingComponent component, GotUnequippedHandEvent args)
  {
    this.StopBlockingHelper(uid, component, args.User);
  }

  private void OnDrop(EntityUid uid, BlockingComponent component, DroppedEvent args)
  {
    this.StopBlockingHelper(uid, component, args.User);
  }

  private void OnGetActions(EntityUid uid, BlockingComponent component, GetItemActionsEvent args)
  {
    args.AddAction(ref component.BlockingToggleActionEntity, EntProtoId.op_Implicit(component.BlockingToggleAction));
  }

  private void OnToggleAction(EntityUid uid, BlockingComponent component, ToggleActionEvent args)
  {
    if (args.Handled)
      return;
    EntityQuery<BlockingComponent> entityQuery = this.GetEntityQuery<BlockingComponent>();
    HandsComponent handsComponent;
    if (!this.GetEntityQuery<HandsComponent>().TryGetComponent(args.Performer, ref handsComponent))
      return;
    foreach (EntityUid entityUid in this._handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit((args.Performer, handsComponent))).ToArray<EntityUid>())
    {
      BlockingComponent blockingComponent;
      if (!EntityUid.op_Equality(entityUid, uid) && entityQuery.TryGetComponent(entityUid, ref blockingComponent) && blockingComponent.IsBlocking)
      {
        this.CantBlockError(args.Performer);
        return;
      }
    }
    if (component.IsBlocking)
      this.StopBlocking(uid, component, args.Performer);
    else
      this.StartBlocking(uid, component, args.Performer);
    args.Handled = true;
  }

  private void OnShutdown(EntityUid uid, BlockingComponent component, ComponentShutdown args)
  {
    if (!component.User.HasValue)
      return;
    this._actionsSystem.RemoveProvidedActions(component.User.Value, uid);
    this.StopBlockingHelper(uid, component, component.User.Value);
  }

  public bool StartBlocking(EntityUid item, BlockingComponent component, EntityUid user)
  {
    if (component.IsBlocking)
      return false;
    TransformComponent transformComponent = this.Transform(user);
    string str = this.Name(item, (MetaDataComponent) null);
    EntityUid entityUid1 = Identity.Entity(user, (IEntityManager) this.EntityManager);
    string recipientMessage = this.Loc.GetString("action-popup-blocking-user", ("shield", (object) str));
    string othersMessage = this.Loc.GetString("action-popup-blocking-other", ("blockerName", (object) entityUid1), ("shield", (object) str));
    EntityUid? gridUid = transformComponent.GridUid;
    EntityUid parentUid = transformComponent.ParentUid;
    if ((gridUid.HasValue ? (EntityUid.op_Inequality(gridUid.GetValueOrDefault(), parentUid) ? 1 : 0) : 1) != 0)
    {
      this.CantBlockError(user);
      return false;
    }
    if (!this._handsSystem.IsHolding(Entity<HandsComponent>.op_Implicit(user), new EntityUid?(item), out string _))
    {
      this.CantBlockError(user);
      return false;
    }
    TileRef? tileRef = this._turf.GetTileRef(transformComponent.Coordinates);
    if (tileRef.HasValue)
    {
      IEnumerable<EntityUid> entitiesIntersecting = this._lookup.GetLocalEntitiesIntersecting(tileRef.Value, 0.0f, (LookupFlags) 110);
      EntityQuery<MobStateComponent> entityQuery = this.GetEntityQuery<MobStateComponent>();
      foreach (EntityUid entityUid2 in entitiesIntersecting)
      {
        if (EntityUid.op_Inequality(entityUid2, user) && entityQuery.HasComponent(entityUid2))
        {
          this.TooCloseError(user);
          return false;
        }
      }
    }
    this._transformSystem.AnchorEntity(user, transformComponent);
    if (!transformComponent.Anchored)
    {
      this.CantBlockError(user);
      return false;
    }
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? toggleActionEntity = component.BlockingToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.SetToggled(action, true);
    this._popupSystem.PopupPredicted(recipientMessage, othersMessage, user, new EntityUid?(user));
    PhysicsComponent physicsComponent;
    if (this.TryComp<PhysicsComponent>(user, ref physicsComponent))
      this._fixtureSystem.TryCreateFixture(user, component.Shape, "blocking-active", 1f, true, 223, 0, 0.4f, 0.0f, true, (FixturesComponent) null, physicsComponent, (TransformComponent) null);
    component.IsBlocking = true;
    this.Dirty(item, (IComponent) component, (MetaDataComponent) null);
    return true;
  }

  private void CantBlockError(EntityUid user)
  {
    this._popupSystem.PopupClient(this.Loc.GetString("action-popup-blocking-user-cant-block"), user, new EntityUid?(user));
  }

  private void TooCloseError(EntityUid user)
  {
    this._popupSystem.PopupClient(this.Loc.GetString("action-popup-blocking-user-too-close"), user, new EntityUid?(user));
  }

  public bool StopBlocking(EntityUid item, BlockingComponent component, EntityUid user)
  {
    if (!component.IsBlocking)
      return false;
    TransformComponent transformComponent = this.Transform(user);
    string str = this.Name(item, (MetaDataComponent) null);
    EntityUid entityUid = Identity.Entity(user, (IEntityManager) this.EntityManager);
    string recipientMessage = this.Loc.GetString("action-popup-blocking-disabling-user", ("shield", (object) str));
    string othersMessage = this.Loc.GetString("action-popup-blocking-disabling-other", ("blockerName", (object) entityUid), ("shield", (object) str));
    BlockingUserComponent blockingUserComponent;
    PhysicsComponent physicsComponent;
    if (this.TryComp<BlockingUserComponent>(user, ref blockingUserComponent) && this.TryComp<PhysicsComponent>(user, ref physicsComponent))
    {
      if (transformComponent.Anchored)
        this._transformSystem.Unanchor(user, transformComponent, true);
      SharedActionsSystem actionsSystem = this._actionsSystem;
      EntityUid? toggleActionEntity = component.BlockingToggleActionEntity;
      Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
      actionsSystem.SetToggled(action, false);
      this._fixtureSystem.DestroyFixture(user, "blocking-active", true, physicsComponent, (FixturesComponent) null, (TransformComponent) null);
      this._physics.SetBodyType(user, blockingUserComponent.OriginalBodyType, (FixturesComponent) null, physicsComponent, (TransformComponent) null);
      this._popupSystem.PopupPredicted(recipientMessage, othersMessage, user, new EntityUid?(user));
    }
    component.IsBlocking = false;
    this.Dirty(item, (IComponent) component, (MetaDataComponent) null);
    return true;
  }

  private void StopBlockingHelper(EntityUid uid, BlockingComponent component, EntityUid user)
  {
    if (component.IsBlocking)
      this.StopBlocking(uid, component, user);
    EntityQuery<BlockingUserComponent> entityQuery = this.GetEntityQuery<BlockingUserComponent>();
    HandsComponent handsComponent;
    if (!this.GetEntityQuery<HandsComponent>().TryGetComponent(user, ref handsComponent))
      return;
    foreach (EntityUid entityUid in this._handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit((user, handsComponent))).ToArray<EntityUid>())
    {
      BlockingUserComponent blockingUserComponent;
      if (this.HasComp<BlockingComponent>(entityUid) && entityQuery.TryGetComponent(user, ref blockingUserComponent))
      {
        blockingUserComponent.BlockingItem = new EntityUid?(entityUid);
        return;
      }
    }
    this.RemComp<BlockingUserComponent>(user);
    component.User = new EntityUid?();
  }

  private void OnVerbExamine(
    EntityUid uid,
    BlockingComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    float num = component.IsBlocking ? component.ActiveBlockFraction : component.PassiveBlockFraction;
    DamageModifierSet modifiers = component.IsBlocking ? component.ActiveBlockDamageModifier : component.PassiveBlockDamageModifer;
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddMarkupOrThrow(this.Loc.GetString("blocking-fraction", ("value", (object) MathF.Round(num * 100f, 1))));
    this.AppendCoefficients(modifiers, formattedMessage);
    this._examine.AddDetailedExamineVerb(args, (Component) component, formattedMessage, this.Loc.GetString("blocking-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png", this.Loc.GetString("blocking-examinable-verb-message"));
  }

  private void AppendCoefficients(DamageModifierSet modifiers, FormattedMessage msg)
  {
    foreach (KeyValuePair<string, float> coefficient in modifiers.Coefficients)
    {
      msg.PushNewline();
      msg.AddMarkupOrThrow(Loc.GetString("blocking-coefficient-value", new (string, object)[2]
      {
        ("type", (object) coefficient.Key),
        ("value", (object) MathF.Round(coefficient.Value * 100f, 1))
      }));
    }
    foreach (KeyValuePair<string, float> keyValuePair in modifiers.FlatReduction)
    {
      msg.PushNewline();
      msg.AddMarkupOrThrow(Loc.GetString("blocking-reduction-value", new (string, object)[2]
      {
        ("type", (object) keyValuePair.Key),
        ("value", (object) keyValuePair.Value)
      }));
    }
  }

  private void InitializeUser()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingUserComponent, DamageModifyEvent>(new ComponentEventHandler<BlockingUserComponent, DamageModifyEvent>((object) this, __methodptr(OnUserDamageModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingComponent, DamageModifyEvent>(new ComponentEventHandler<BlockingComponent, DamageModifyEvent>((object) this, __methodptr(OnDamageModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingUserComponent, EntParentChangedMessage>(new ComponentEventRefHandler<BlockingUserComponent, EntParentChangedMessage>((object) this, __methodptr(OnParentChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingUserComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<BlockingUserComponent, ContainerGettingInsertedAttemptEvent>((object) this, __methodptr(OnInsertAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingUserComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<BlockingUserComponent, AnchorStateChangedEvent>((object) this, __methodptr(OnAnchorChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BlockingUserComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<BlockingUserComponent, EntityTerminatingEvent>((object) this, __methodptr(OnEntityTerminating)), (Type[]) null, (Type[]) null);
  }

  private void OnParentChanged(
    EntityUid uid,
    BlockingUserComponent component,
    ref EntParentChangedMessage args)
  {
    this.UserStopBlocking(uid, component);
  }

  private void OnInsertAttempt(
    EntityUid uid,
    BlockingUserComponent component,
    ContainerGettingInsertedAttemptEvent args)
  {
    this.UserStopBlocking(uid, component);
  }

  private void OnAnchorChanged(
    EntityUid uid,
    BlockingUserComponent component,
    ref AnchorStateChangedEvent args)
  {
    if (((AnchorStateChangedEvent) ref args).Anchored)
      return;
    this.UserStopBlocking(uid, component);
  }

  private void OnUserDamageModified(
    EntityUid uid,
    BlockingUserComponent component,
    DamageModifyEvent args)
  {
    BlockingComponent blockingComponent;
    DamageableComponent damageableComponent;
    if (!this.TryComp<BlockingComponent>(component.BlockingItem, ref blockingComponent) || args.Damage.GetTotal() <= 0 || !this.TryComp<DamageableComponent>(component.BlockingItem, ref damageableComponent))
      return;
    float num = Math.Clamp(blockingComponent.IsBlocking ? blockingComponent.ActiveBlockFraction : blockingComponent.PassiveBlockFraction, 0.0f, 1f);
    this._damageable.TryChangeDamage(component.BlockingItem, num * args.OriginalDamage);
    DamageModifierSet modifierSet = new DamageModifierSet();
    foreach (string key in damageableComponent.Damage.DamageDict.Keys)
      modifierSet.Coefficients.TryAdd(key, 1f - num);
    args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, modifierSet);
    if (!blockingComponent.IsBlocking || args.Damage.Equals(args.OriginalDamage))
      return;
    this._audio.PlayPvs(blockingComponent.BlockSound, uid, new AudioParams?());
  }

  private void OnDamageModified(EntityUid uid, BlockingComponent component, DamageModifyEvent args)
  {
    DamageModifierSet modifierSet = component.IsBlocking ? component.ActiveBlockDamageModifier : component.PassiveBlockDamageModifer;
    if (modifierSet == null)
      return;
    args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, modifierSet);
  }

  private void OnEntityTerminating(
    EntityUid uid,
    BlockingUserComponent component,
    ref EntityTerminatingEvent args)
  {
    BlockingComponent component1;
    if (!this.TryComp<BlockingComponent>(component.BlockingItem, ref component1))
      return;
    this.StopBlockingHelper(component.BlockingItem.Value, component1, uid);
  }

  private void UserStopBlocking(EntityUid uid, BlockingUserComponent component)
  {
    BlockingComponent component1;
    if (!this.TryComp<BlockingComponent>(component.BlockingItem, ref component1) || !component1.IsBlocking)
      return;
    this.StopBlocking(component.BlockingItem.Value, component1, uid);
  }
}
