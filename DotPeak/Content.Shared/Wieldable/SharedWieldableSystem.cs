// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wieldable.SharedWieldableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Wieldable;

public abstract class SharedWieldableSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedVirtualItemSystem _virtualItem;
  [Dependency]
  private UseDelaySystem _delay;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<WieldableComponent, UseInHandEvent>(new ComponentEventHandler<WieldableComponent, UseInHandEvent>(this.OnUseInHand), new Type[2]
    {
      typeof (SharedGunSystem),
      typeof (BatteryWeaponFireModesSystem)
    });
    this.SubscribeLocalEvent<WieldableComponent, ItemUnwieldedEvent>(new ComponentEventHandler<WieldableComponent, ItemUnwieldedEvent>(this.OnItemUnwielded));
    this.SubscribeLocalEvent<WieldableComponent, GotUnequippedHandEvent>(new ComponentEventHandler<WieldableComponent, GotUnequippedHandEvent>(this.OnItemLeaveHand));
    this.SubscribeLocalEvent<WieldableComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<WieldableComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted));
    this.SubscribeLocalEvent<WieldableComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<WieldableComponent, GetVerbsEvent<InteractionVerb>>(this.AddToggleWieldVerb));
    this.SubscribeLocalEvent<WieldableComponent, HandDeselectedEvent>(new ComponentEventHandler<WieldableComponent, HandDeselectedEvent>(this.OnDeselectWieldable));
    this.SubscribeLocalEvent<WieldingBlockerComponent, GotEquippedEvent>(new EntityEventRefHandler<WieldingBlockerComponent, GotEquippedEvent>(this.OnBlockerEquipped));
    this.SubscribeLocalEvent<WieldingBlockerComponent, GotEquippedHandEvent>(new EntityEventRefHandler<WieldingBlockerComponent, GotEquippedHandEvent>(this.OnBlockerEquippedHand));
    this.SubscribeLocalEvent<WieldingBlockerComponent, WieldAttemptEvent>(new EntityEventRefHandler<WieldingBlockerComponent, WieldAttemptEvent>(this.OnBlockerAttempt));
    this.SubscribeLocalEvent<WieldingBlockerComponent, InventoryRelayedEvent<WieldAttemptEvent>>(new EntityEventRefHandler<WieldingBlockerComponent, InventoryRelayedEvent<WieldAttemptEvent>>(this.OnBlockerAttempt));
    this.SubscribeLocalEvent<WieldingBlockerComponent, HeldRelayedEvent<WieldAttemptEvent>>(new EntityEventRefHandler<WieldingBlockerComponent, HeldRelayedEvent<WieldAttemptEvent>>(this.OnBlockerAttempt));
    this.SubscribeLocalEvent<MeleeRequiresWieldComponent, AttemptMeleeEvent>(new ComponentEventRefHandler<MeleeRequiresWieldComponent, AttemptMeleeEvent>(this.OnMeleeAttempt));
    this.SubscribeLocalEvent<GunRequiresWieldComponent, ExaminedEvent>(new EntityEventRefHandler<GunRequiresWieldComponent, ExaminedEvent>(this.OnExamineRequires));
    this.SubscribeLocalEvent<GunRequiresWieldComponent, ShotAttemptedEvent>(new ComponentEventRefHandler<GunRequiresWieldComponent, ShotAttemptedEvent>(this.OnShootAttempt));
    this.SubscribeLocalEvent<GunWieldBonusComponent, ItemWieldedEvent>(new ComponentEventRefHandler<GunWieldBonusComponent, ItemWieldedEvent>(this.OnGunWielded));
    this.SubscribeLocalEvent<GunWieldBonusComponent, ItemUnwieldedEvent>(new ComponentEventHandler<GunWieldBonusComponent, ItemUnwieldedEvent>(this.OnGunUnwielded));
    this.SubscribeLocalEvent<GunWieldBonusComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunWieldBonusComponent, GunRefreshModifiersEvent>(this.OnGunRefreshModifiers));
    this.SubscribeLocalEvent<GunWieldBonusComponent, ExaminedEvent>(new ComponentEventRefHandler<GunWieldBonusComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<SpeedModifiedOnWieldComponent, ItemWieldedEvent>(new ComponentEventHandler<SpeedModifiedOnWieldComponent, ItemWieldedEvent>(this.OnSpeedModifierWielded));
    this.SubscribeLocalEvent<SpeedModifiedOnWieldComponent, ItemUnwieldedEvent>(new ComponentEventHandler<SpeedModifiedOnWieldComponent, ItemUnwieldedEvent>(this.OnSpeedModifierUnwielded));
    this.SubscribeLocalEvent<SpeedModifiedOnWieldComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(new ComponentEventRefHandler<SpeedModifiedOnWieldComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(this.OnRefreshSpeedWielded));
    this.SubscribeLocalEvent<IncreaseDamageOnWieldComponent, GetMeleeDamageEvent>(new ComponentEventRefHandler<IncreaseDamageOnWieldComponent, GetMeleeDamageEvent>(this.OnGetMeleeDamage));
  }

  private void OnMeleeAttempt(
    EntityUid uid,
    MeleeRequiresWieldComponent component,
    ref AttemptMeleeEvent args)
  {
    WieldableComponent comp;
    if (!this.TryComp<WieldableComponent>(uid, out comp) || comp.Wielded)
      return;
    args.Cancelled = true;
    args.Message = this.Loc.GetString("wieldable-component-requires", ("item", (object) uid));
  }

  private void OnShootAttempt(
    EntityUid uid,
    GunRequiresWieldComponent component,
    ref ShotAttemptedEvent args)
  {
    WieldableComponent comp;
    if (!this.TryComp<WieldableComponent>(uid, out comp) || comp.Wielded)
      return;
    args.Cancel();
    TimeSpan curTime = this._timing.CurTime;
    if (!(curTime > component.LastPopup + component.PopupCooldown) || this.HasComp<MeleeWeaponComponent>(uid) || this.HasComp<MeleeRequiresWieldComponent>(uid))
      return;
    component.LastPopup = curTime;
    this._popup.PopupClient(this.Loc.GetString("wieldable-component-requires", ("item", (object) uid)), (EntityUid) args.Used, new EntityUid?(args.User));
  }

  private void OnGunUnwielded(
    EntityUid uid,
    GunWieldBonusComponent component,
    ItemUnwieldedEvent args)
  {
    this._gun.RefreshModifiers((Entity<GunComponent>) uid);
  }

  private void OnGunWielded(
    EntityUid uid,
    GunWieldBonusComponent component,
    ref ItemWieldedEvent args)
  {
    this._gun.RefreshModifiers((Entity<GunComponent>) uid);
  }

  private void OnDeselectWieldable(
    EntityUid uid,
    WieldableComponent component,
    HandDeselectedEvent args)
  {
    if (this._hands.GetHandCount((Entity<HandsComponent>) args.User) > 2)
      return;
    this.TryUnwield(uid, component, args.User);
  }

  private void OnGunRefreshModifiers(
    Entity<GunWieldBonusComponent> bonus,
    ref GunRefreshModifiersEvent args)
  {
    WieldableComponent comp;
    if (!this.TryComp<WieldableComponent>((EntityUid) bonus, out comp) || !comp.Wielded)
      return;
    ref GunRefreshModifiersEvent local1 = ref args;
    local1.MinAngle = Angle.op_Addition(local1.MinAngle, bonus.Comp.MinAngle);
    ref GunRefreshModifiersEvent local2 = ref args;
    local2.MaxAngle = Angle.op_Addition(local2.MaxAngle, bonus.Comp.MaxAngle);
    ref GunRefreshModifiersEvent local3 = ref args;
    local3.AngleDecay = Angle.op_Addition(local3.AngleDecay, bonus.Comp.AngleDecay);
    ref GunRefreshModifiersEvent local4 = ref args;
    local4.AngleIncrease = Angle.op_Addition(local4.AngleIncrease, bonus.Comp.AngleIncrease);
  }

  private void OnSpeedModifierWielded(
    EntityUid uid,
    SpeedModifiedOnWieldComponent component,
    ItemWieldedEvent args)
  {
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
  }

  private void OnSpeedModifierUnwielded(
    EntityUid uid,
    SpeedModifiedOnWieldComponent component,
    ItemUnwieldedEvent args)
  {
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
  }

  private void OnRefreshSpeedWielded(
    EntityUid uid,
    SpeedModifiedOnWieldComponent component,
    ref HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
  {
    WieldableComponent comp;
    if (!this.TryComp<WieldableComponent>(uid, out comp) || !comp.Wielded)
      return;
    args.Args.ModifySpeed(component.WalkModifier, component.SprintModifier);
  }

  private void OnExamineRequires(Entity<GunRequiresWieldComponent> entity, ref ExaminedEvent args)
  {
    if (!entity.Comp.WieldRequiresExamineMessage.HasValue)
      return;
    ExaminedEvent examinedEvent = args;
    ILocalizationManager loc = this.Loc;
    LocId? requiresExamineMessage = entity.Comp.WieldRequiresExamineMessage;
    string valueOrDefault = requiresExamineMessage.HasValue ? (string) requiresExamineMessage.GetValueOrDefault() : (string) null;
    string text = loc.GetString(valueOrDefault);
    examinedEvent.PushText(text);
  }

  private void OnExamine(EntityUid uid, GunWieldBonusComponent component, ref ExaminedEvent args)
  {
    if (this.HasComp<GunRequiresWieldComponent>(uid) || !component.WieldBonusExamineMessage.HasValue)
      return;
    ExaminedEvent examinedEvent = args;
    ILocalizationManager loc = this.Loc;
    LocId? bonusExamineMessage = component.WieldBonusExamineMessage;
    string valueOrDefault = bonusExamineMessage.HasValue ? (string) bonusExamineMessage.GetValueOrDefault() : (string) null;
    string text = loc.GetString(valueOrDefault);
    examinedEvent.PushText(text);
  }

  private void AddToggleWieldVerb(
    EntityUid uid,
    WieldableComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract || !this._hands.IsHolding((Entity<HandsComponent>) (args.User, args.Hands), new EntityUid?(uid), out string _))
      return;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Text = component.Wielded ? this.Loc.GetString("wieldable-verb-text-unwield") : this.Loc.GetString("wieldable-verb-text-wield");
    interactionVerb1.Act = component.Wielded ? (Action) (() => this.TryUnwield(uid, component, args.User)) : (Action) (() => this.TryWield(uid, component, args.User));
    InteractionVerb interactionVerb2 = interactionVerb1;
    args.Verbs.Add(interactionVerb2);
  }

  private void OnUseInHand(EntityUid uid, WieldableComponent component, UseInHandEvent args)
  {
    if (args.Handled)
      return;
    if (!component.Wielded)
    {
      this.TryWield(uid, component, args.User);
      args.Handled = true;
    }
    else if (component.UnwieldOnUse)
    {
      this.TryUnwield(uid, component, args.User);
      args.Handled = true;
    }
    if (!this.HasComp<UseDelayComponent>(uid) || component.UseDelayOnWield)
      return;
    args.ApplyDelay = false;
  }

  private void OnBlockerEquipped(Entity<WieldingBlockerComponent> ent, ref GotEquippedEvent args)
  {
    if (!ent.Comp.BlockEquipped)
      return;
    this.UnwieldAll((Entity<HandsComponent>) args.Equipee, true);
  }

  private void OnBlockerEquippedHand(
    Entity<WieldingBlockerComponent> ent,
    ref GotEquippedHandEvent args)
  {
    if (!ent.Comp.BlockInHand)
      return;
    this.UnwieldAll((Entity<HandsComponent>) args.User, true);
  }

  private void OnBlockerAttempt(
    Entity<WieldingBlockerComponent> ent,
    ref InventoryRelayedEvent<WieldAttemptEvent> args)
  {
    if (!ent.Comp.BlockEquipped)
      return;
    args.Args.Message = this.Loc.GetString("wieldable-component-blocked-wield", ("blocker", (object) ent.Owner), ("item", (object) args.Args.Wielded));
    args.Args.Cancelled = true;
  }

  private void OnBlockerAttempt(
    Entity<WieldingBlockerComponent> ent,
    ref HeldRelayedEvent<WieldAttemptEvent> args)
  {
    if (!ent.Comp.BlockInHand)
      return;
    args.Args.Message = this.Loc.GetString("wieldable-component-blocked-wield", ("blocker", (object) ent.Owner), ("item", (object) args.Args.Wielded));
    args.Args.Cancelled = true;
  }

  private void OnBlockerAttempt(Entity<WieldingBlockerComponent> ent, ref WieldAttemptEvent args)
  {
    args.Cancelled = true;
  }

  public bool CanWield(EntityUid uid, WieldableComponent component, EntityUid user, bool quiet = false)
  {
    HandsComponent comp;
    if (!this.TryComp<HandsComponent>(user, out comp))
    {
      if (!quiet)
        this._popup.PopupClient(this.Loc.GetString("wieldable-component-no-hands"), user, new EntityUid?(user));
      return false;
    }
    if (!this._hands.IsHolding((Entity<HandsComponent>) (user, comp), new EntityUid?(uid), out string _))
    {
      if (!quiet)
        this._popup.PopupClient(this.Loc.GetString("wieldable-component-not-in-hands", ("item", (object) uid)), user, new EntityUid?(user));
      return false;
    }
    if (this._hands.CountFreeableHands((Entity<HandsComponent>) (user, comp), uid) >= component.FreeHandsRequired)
      return true;
    if (!quiet)
      this._popup.PopupClient(this.Loc.GetString("wieldable-component-not-enough-free-hands", ("number", (object) component.FreeHandsRequired), ("item", (object) uid)), user, new EntityUid?(user));
    return false;
  }

  public bool TryWield(EntityUid used, WieldableComponent component, EntityUid user)
  {
    UseDelayComponent comp1;
    if (!this.CanWield(used, component, user) || this.TryComp<UseDelayComponent>(used, out comp1) && component.UseDelayOnWield && !this._delay.TryResetDelay((Entity<UseDelayComponent>) (used, comp1), true))
      return false;
    WieldAttemptEvent args1 = new WieldAttemptEvent(user, used);
    this.RaiseLocalEvent<WieldAttemptEvent>(user, ref args1);
    if (args1.Cancelled)
    {
      if (args1.Message != null)
        this._popup.PopupClient(args1.Message, user, new EntityUid?(user));
      return false;
    }
    ItemComponent comp2;
    if (this.TryComp<ItemComponent>(used, out comp2))
    {
      component.OldInhandPrefix = comp2.HeldPrefix;
      this._item.SetHeldPrefix(used, component.WieldedInhandPrefix, component: comp2);
    }
    this.SetWielded((Entity<WieldableComponent>) (used, component), true);
    if (component.WieldSound != null)
      this._audio.PlayPredicted(component.WieldSound, used, new EntityUid?(user));
    ValueList<EntityUid> valueList = new ValueList<EntityUid>();
    for (int index = 0; index < component.FreeHandsRequired; ++index)
    {
      EntityUid? virtualItem;
      if (this._virtualItem.TrySpawnVirtualItemInHand(used, user, out virtualItem, true))
      {
        valueList.Add(virtualItem.Value);
      }
      else
      {
        foreach (EntityUid entityUid in valueList)
          this.QueueDel(new EntityUid?(entityUid));
        return false;
      }
    }
    this._popup.PopupPredicted(this.Loc.GetString("wieldable-component-successful-wield", ("item", (object) used)), this.Loc.GetString("wieldable-component-successful-wield-other", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager)), ("item", (object) used)), user, new EntityUid?(user));
    ItemWieldedEvent args2 = new ItemWieldedEvent(user);
    this.RaiseLocalEvent<ItemWieldedEvent>(used, ref args2);
    return true;
  }

  public bool TryUnwield(EntityUid used, WieldableComponent component, EntityUid user, bool force = false)
  {
    if (!component.Wielded)
      return false;
    if (!force)
    {
      UnwieldAttemptEvent args = new UnwieldAttemptEvent(user, used);
      this.RaiseLocalEvent<UnwieldAttemptEvent>(user, ref args);
      if (args.Cancelled)
      {
        if (args.Message != null)
          this._popup.PopupClient(args.Message, user, new EntityUid?(user));
        return false;
      }
    }
    this.SetWielded((Entity<WieldableComponent>) (used, component), false);
    ItemUnwieldedEvent args1 = new ItemUnwieldedEvent(user, force);
    this.RaiseLocalEvent<ItemUnwieldedEvent>(used, ref args1);
    return true;
  }

  public void UnwieldAll(Entity<HandsComponent?> wielder, bool force = false)
  {
    foreach (EntityUid entityUid in this._hands.EnumerateHeld(wielder))
    {
      WieldableComponent comp;
      if (this.TryComp<WieldableComponent>(entityUid, out comp))
        this.TryUnwield(entityUid, comp, (EntityUid) wielder, force);
    }
  }

  private void SetWielded(Entity<WieldableComponent> ent, bool wielded)
  {
    ent.Comp.Wielded = wielded;
    this.Dirty<WieldableComponent>(ent);
    this._appearance.SetData((EntityUid) ent, (Enum) WieldableVisuals.Wielded, (object) wielded);
  }

  private void OnItemUnwielded(
    EntityUid uid,
    WieldableComponent component,
    ItemUnwieldedEvent args)
  {
    this._item.SetHeldPrefix(uid, component.OldInhandPrefix);
    EntityUid user = args.User;
    this._virtualItem.DeleteInHandsMatching(user, uid);
    if (args.Force)
      return;
    if (component.UnwieldSound != null)
      this._audio.PlayPredicted(component.UnwieldSound, uid, new EntityUid?(user));
    this._popup.PopupPredicted(this.Loc.GetString("wieldable-component-failed-wield", ("item", (object) uid)), this.Loc.GetString("wieldable-component-failed-wield-other", ("user", (object) Identity.Entity(args.User, (IEntityManager) this.EntityManager)), ("item", (object) uid)), user, new EntityUid?(user));
  }

  private void OnItemLeaveHand(
    EntityUid uid,
    WieldableComponent component,
    GotUnequippedHandEvent args)
  {
    if (!(uid == args.Unequipped))
      return;
    this.TryUnwield(uid, component, args.User, true);
  }

  private void OnVirtualItemDeleted(
    EntityUid uid,
    WieldableComponent component,
    VirtualItemDeletedEvent args)
  {
    if (!(args.BlockingEntity == uid))
      return;
    this.TryUnwield(uid, component, args.User, true);
  }

  private void OnGetMeleeDamage(
    EntityUid uid,
    IncreaseDamageOnWieldComponent component,
    ref GetMeleeDamageEvent args)
  {
    WieldableComponent comp;
    if (!this.TryComp<WieldableComponent>(uid, out comp) || !comp.Wielded)
      return;
    args.Damage += component.BonusDamage;
  }
}
