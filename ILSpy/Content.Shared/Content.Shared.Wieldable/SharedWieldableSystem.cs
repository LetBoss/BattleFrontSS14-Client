using System;
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
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

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
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WieldableComponent, UseInHandEvent>((ComponentEventHandler<WieldableComponent, UseInHandEvent>)OnUseInHand, new Type[2]
		{
			typeof(SharedGunSystem),
			typeof(BatteryWeaponFireModesSystem)
		}, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableComponent, ItemUnwieldedEvent>((ComponentEventHandler<WieldableComponent, ItemUnwieldedEvent>)OnItemUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableComponent, GotUnequippedHandEvent>((ComponentEventHandler<WieldableComponent, GotUnequippedHandEvent>)OnItemLeaveHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableComponent, VirtualItemDeletedEvent>((ComponentEventHandler<WieldableComponent, VirtualItemDeletedEvent>)OnVirtualItemDeleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<WieldableComponent, GetVerbsEvent<InteractionVerb>>)AddToggleWieldVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldableComponent, HandDeselectedEvent>((ComponentEventHandler<WieldableComponent, HandDeselectedEvent>)OnDeselectWieldable, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldingBlockerComponent, GotEquippedEvent>((EntityEventRefHandler<WieldingBlockerComponent, GotEquippedEvent>)OnBlockerEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldingBlockerComponent, GotEquippedHandEvent>((EntityEventRefHandler<WieldingBlockerComponent, GotEquippedHandEvent>)OnBlockerEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldingBlockerComponent, WieldAttemptEvent>((EntityEventRefHandler<WieldingBlockerComponent, WieldAttemptEvent>)OnBlockerAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldingBlockerComponent, InventoryRelayedEvent<WieldAttemptEvent>>((EntityEventRefHandler<WieldingBlockerComponent, InventoryRelayedEvent<WieldAttemptEvent>>)OnBlockerAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WieldingBlockerComponent, HeldRelayedEvent<WieldAttemptEvent>>((EntityEventRefHandler<WieldingBlockerComponent, HeldRelayedEvent<WieldAttemptEvent>>)OnBlockerAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeRequiresWieldComponent, AttemptMeleeEvent>((ComponentEventRefHandler<MeleeRequiresWieldComponent, AttemptMeleeEvent>)OnMeleeAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunRequiresWieldComponent, ExaminedEvent>((EntityEventRefHandler<GunRequiresWieldComponent, ExaminedEvent>)OnExamineRequires, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunRequiresWieldComponent, ShotAttemptedEvent>((ComponentEventRefHandler<GunRequiresWieldComponent, ShotAttemptedEvent>)OnShootAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunWieldBonusComponent, ItemWieldedEvent>((ComponentEventRefHandler<GunWieldBonusComponent, ItemWieldedEvent>)OnGunWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunWieldBonusComponent, ItemUnwieldedEvent>((ComponentEventHandler<GunWieldBonusComponent, ItemUnwieldedEvent>)OnGunUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunWieldBonusComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunWieldBonusComponent, GunRefreshModifiersEvent>)OnGunRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunWieldBonusComponent, ExaminedEvent>((ComponentEventRefHandler<GunWieldBonusComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifiedOnWieldComponent, ItemWieldedEvent>((ComponentEventHandler<SpeedModifiedOnWieldComponent, ItemWieldedEvent>)OnSpeedModifierWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifiedOnWieldComponent, ItemUnwieldedEvent>((ComponentEventHandler<SpeedModifiedOnWieldComponent, ItemUnwieldedEvent>)OnSpeedModifierUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifiedOnWieldComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>((ComponentEventRefHandler<SpeedModifiedOnWieldComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>)OnRefreshSpeedWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IncreaseDamageOnWieldComponent, GetMeleeDamageEvent>((ComponentEventRefHandler<IncreaseDamageOnWieldComponent, GetMeleeDamageEvent>)OnGetMeleeDamage, (Type[])null, (Type[])null);
	}

	private void OnMeleeAttempt(EntityUid uid, MeleeRequiresWieldComponent component, ref AttemptMeleeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wieldable = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(uid, ref wieldable) && !wieldable.Wielded)
		{
			args.Cancelled = true;
			args.Message = base.Loc.GetString("wieldable-component-requires", (ValueTuple<string, object>)("item", uid));
		}
	}

	private void OnShootAttempt(EntityUid uid, GunRequiresWieldComponent component, ref ShotAttemptedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wieldable = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(uid, ref wieldable) && !wieldable.Wielded)
		{
			args.Cancel();
			TimeSpan time = _timing.CurTime;
			if (time > component.LastPopup + component.PopupCooldown && !((EntitySystem)this).HasComp<MeleeWeaponComponent>(uid) && !((EntitySystem)this).HasComp<MeleeRequiresWieldComponent>(uid))
			{
				component.LastPopup = time;
				string message = base.Loc.GetString("wieldable-component-requires", (ValueTuple<string, object>)("item", uid));
				_popup.PopupClient(message, Entity<GunComponent>.op_Implicit(args.Used), args.User);
			}
		}
	}

	private void OnGunUnwielded(EntityUid uid, GunWieldBonusComponent component, ItemUnwieldedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(uid));
	}

	private void OnGunWielded(EntityUid uid, GunWieldBonusComponent component, ref ItemWieldedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(uid));
	}

	private void OnDeselectWieldable(EntityUid uid, WieldableComponent component, HandDeselectedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_hands.GetHandCount(Entity<HandsComponent>.op_Implicit(args.User)) <= 2)
		{
			TryUnwield(uid, component, args.User);
		}
	}

	private void OnGunRefreshModifiers(Entity<GunWieldBonusComponent> bonus, ref GunRefreshModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wield = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(Entity<GunWieldBonusComponent>.op_Implicit(bonus), ref wield) && wield.Wielded)
		{
			args.MinAngle += bonus.Comp.MinAngle;
			args.MaxAngle += bonus.Comp.MaxAngle;
			args.AngleDecay += bonus.Comp.AngleDecay;
			args.AngleIncrease += bonus.Comp.AngleIncrease;
		}
	}

	private void OnSpeedModifierWielded(EntityUid uid, SpeedModifiedOnWieldComponent component, ItemWieldedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
	}

	private void OnSpeedModifierUnwielded(EntityUid uid, SpeedModifiedOnWieldComponent component, ItemUnwieldedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
	}

	private void OnRefreshSpeedWielded(EntityUid uid, SpeedModifiedOnWieldComponent component, ref HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wield = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(uid, ref wield) && wield.Wielded)
		{
			args.Args.ModifySpeed(component.WalkModifier, component.SprintModifier);
		}
	}

	private void OnExamineRequires(Entity<GunRequiresWieldComponent> entity, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.WieldRequiresExamineMessage.HasValue)
		{
			ExaminedEvent obj = args;
			ILocalizationManager loc = base.Loc;
			LocId? wieldRequiresExamineMessage = entity.Comp.WieldRequiresExamineMessage;
			obj.PushText(loc.GetString(wieldRequiresExamineMessage.HasValue ? LocId.op_Implicit(wieldRequiresExamineMessage.GetValueOrDefault()) : null));
		}
	}

	private void OnExamine(EntityUid uid, GunWieldBonusComponent component, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<GunRequiresWieldComponent>(uid) && component.WieldBonusExamineMessage.HasValue)
		{
			ExaminedEvent obj = args;
			ILocalizationManager loc = base.Loc;
			LocId? wieldBonusExamineMessage = component.WieldBonusExamineMessage;
			obj.PushText(loc.GetString(wieldBonusExamineMessage.HasValue ? LocId.op_Implicit(wieldBonusExamineMessage.GetValueOrDefault()) : null));
		}
	}

	private void AddToggleWieldVerb(EntityUid uid, WieldableComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (args.Hands != null && args.CanAccess && args.CanInteract && _hands.IsHolding(Entity<HandsComponent>.op_Implicit((args.User, args.Hands)), uid, out string _))
		{
			InteractionVerb verb = new InteractionVerb
			{
				Text = (component.Wielded ? base.Loc.GetString("wieldable-verb-text-unwield") : base.Loc.GetString("wieldable-verb-text-wield")),
				Act = (component.Wielded ? ((Action)delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					TryUnwield(uid, component, args.User);
				}) : ((Action)delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					TryWield(uid, component, args.User);
				}))
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnUseInHand(EntityUid uid, WieldableComponent component, UseInHandEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			if (!component.Wielded)
			{
				TryWield(uid, component, args.User);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (component.UnwieldOnUse)
			{
				TryUnwield(uid, component, args.User);
				((HandledEntityEventArgs)args).Handled = true;
			}
			if (((EntitySystem)this).HasComp<UseDelayComponent>(uid) && !component.UseDelayOnWield)
			{
				args.ApplyDelay = false;
			}
		}
	}

	private void OnBlockerEquipped(Entity<WieldingBlockerComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BlockEquipped)
		{
			UnwieldAll(Entity<HandsComponent>.op_Implicit(args.Equipee), force: true);
		}
	}

	private void OnBlockerEquippedHand(Entity<WieldingBlockerComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BlockInHand)
		{
			UnwieldAll(Entity<HandsComponent>.op_Implicit(args.User), force: true);
		}
	}

	private void OnBlockerAttempt(Entity<WieldingBlockerComponent> ent, ref InventoryRelayedEvent<WieldAttemptEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BlockEquipped)
		{
			args.Args.Message = base.Loc.GetString("wieldable-component-blocked-wield", (ValueTuple<string, object>)("blocker", ent.Owner), (ValueTuple<string, object>)("item", args.Args.Wielded));
			args.Args.Cancelled = true;
		}
	}

	private void OnBlockerAttempt(Entity<WieldingBlockerComponent> ent, ref HeldRelayedEvent<WieldAttemptEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BlockInHand)
		{
			args.Args.Message = base.Loc.GetString("wieldable-component-blocked-wield", (ValueTuple<string, object>)("blocker", ent.Owner), (ValueTuple<string, object>)("item", args.Args.Wielded));
			args.Args.Cancelled = true;
		}
	}

	private void OnBlockerAttempt(Entity<WieldingBlockerComponent> ent, ref WieldAttemptEvent args)
	{
		args.Cancelled = true;
	}

	public bool CanWield(EntityUid uid, WieldableComponent component, EntityUid user, bool quiet = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(user, ref hands))
		{
			if (!quiet)
			{
				_popup.PopupClient(base.Loc.GetString("wieldable-component-no-hands"), user, user);
			}
			return false;
		}
		if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit((user, hands)), uid, out string _))
		{
			if (!quiet)
			{
				_popup.PopupClient(base.Loc.GetString("wieldable-component-not-in-hands", (ValueTuple<string, object>)("item", uid)), user, user);
			}
			return false;
		}
		if (_hands.CountFreeableHands(Entity<HandsComponent>.op_Implicit((user, hands)), uid) < component.FreeHandsRequired)
		{
			if (!quiet)
			{
				string message = base.Loc.GetString("wieldable-component-not-enough-free-hands", (ValueTuple<string, object>)("number", component.FreeHandsRequired), (ValueTuple<string, object>)("item", uid));
				_popup.PopupClient(message, user, user);
			}
			return false;
		}
		return true;
	}

	public bool TryWield(EntityUid used, WieldableComponent component, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if (!CanWield(used, component, user))
		{
			return false;
		}
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(used, ref useDelay) && component.UseDelayOnWield && !_delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((used, useDelay)), checkDelayed: true))
		{
			return false;
		}
		WieldAttemptEvent attemptEv = new WieldAttemptEvent(user, used);
		((EntitySystem)this).RaiseLocalEvent<WieldAttemptEvent>(user, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			if (attemptEv.Message != null)
			{
				_popup.PopupClient(attemptEv.Message, user, user);
			}
			return false;
		}
		ItemComponent item = default(ItemComponent);
		if (((EntitySystem)this).TryComp<ItemComponent>(used, ref item))
		{
			component.OldInhandPrefix = item.HeldPrefix;
			_item.SetHeldPrefix(used, component.WieldedInhandPrefix, force: false, item);
		}
		SetWielded(Entity<WieldableComponent>.op_Implicit((used, component)), wielded: true);
		if (component.WieldSound != null)
		{
			_audio.PlayPredicted(component.WieldSound, used, (EntityUid?)user, (AudioParams?)null);
		}
		ValueList<EntityUid> virtuals = default(ValueList<EntityUid>);
		for (int i = 0; i < component.FreeHandsRequired; i++)
		{
			if (_virtualItem.TrySpawnVirtualItemInHand(used, user, out var virtualItem, dropOthers: true))
			{
				virtuals.Add(virtualItem.Value);
				continue;
			}
			Enumerator<EntityUid> enumerator = virtuals.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					EntityUid existingVirtual = enumerator.Current;
					((EntitySystem)this).QueueDel((EntityUid?)existingVirtual);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return false;
		}
		string selfMessage = base.Loc.GetString("wieldable-component-successful-wield", (ValueTuple<string, object>)("item", used));
		string othersMessage = base.Loc.GetString("wieldable-component-successful-wield-other", (ValueTuple<string, object>)("user", Identity.Entity(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", used));
		_popup.PopupPredicted(selfMessage, othersMessage, user, user);
		ItemWieldedEvent ev = new ItemWieldedEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ItemWieldedEvent>(used, ref ev, false);
		return true;
	}

	public bool TryUnwield(EntityUid used, WieldableComponent component, EntityUid user, bool force = false)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Wielded)
		{
			return false;
		}
		if (!force)
		{
			UnwieldAttemptEvent attemptEv = new UnwieldAttemptEvent(user, used);
			((EntitySystem)this).RaiseLocalEvent<UnwieldAttemptEvent>(user, ref attemptEv, false);
			if (attemptEv.Cancelled)
			{
				if (attemptEv.Message != null)
				{
					_popup.PopupClient(attemptEv.Message, user, user);
				}
				return false;
			}
		}
		SetWielded(Entity<WieldableComponent>.op_Implicit((used, component)), wielded: false);
		ItemUnwieldedEvent ev = new ItemUnwieldedEvent(user, force);
		((EntitySystem)this).RaiseLocalEvent<ItemUnwieldedEvent>(used, ref ev, false);
		return true;
	}

	public void UnwieldAll(Entity<HandsComponent?> wielder, bool force = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wieldable = default(WieldableComponent);
		foreach (EntityUid held in _hands.EnumerateHeld(wielder))
		{
			if (((EntitySystem)this).TryComp<WieldableComponent>(held, ref wieldable))
			{
				TryUnwield(held, wieldable, Entity<HandsComponent>.op_Implicit(wielder), force);
			}
		}
	}

	private void SetWielded(Entity<WieldableComponent> ent, bool wielded)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Wielded = wielded;
		((EntitySystem)this).Dirty<WieldableComponent>(ent, (MetaDataComponent)null);
		_appearance.SetData(Entity<WieldableComponent>.op_Implicit(ent), (Enum)WieldableVisuals.Wielded, (object)wielded, (AppearanceComponent)null);
	}

	private void OnItemUnwielded(EntityUid uid, WieldableComponent component, ItemUnwieldedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		_item.SetHeldPrefix(uid, component.OldInhandPrefix);
		EntityUid user = args.User;
		_virtualItem.DeleteInHandsMatching(user, uid);
		if (!args.Force)
		{
			if (component.UnwieldSound != null)
			{
				_audio.PlayPredicted(component.UnwieldSound, uid, (EntityUid?)user, (AudioParams?)null);
			}
			string selfMessage = base.Loc.GetString("wieldable-component-failed-wield", (ValueTuple<string, object>)("item", uid));
			string othersMessage = base.Loc.GetString("wieldable-component-failed-wield-other", (ValueTuple<string, object>)("user", Identity.Entity(args.User, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", uid));
			_popup.PopupPredicted(selfMessage, othersMessage, user, user);
		}
	}

	private void OnItemLeaveHand(EntityUid uid, WieldableComponent component, GotUnequippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (uid == args.Unequipped)
		{
			TryUnwield(uid, component, args.User, force: true);
		}
	}

	private void OnVirtualItemDeleted(EntityUid uid, WieldableComponent component, VirtualItemDeletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (args.BlockingEntity == uid)
		{
			TryUnwield(uid, component, args.User, force: true);
		}
	}

	private void OnGetMeleeDamage(EntityUid uid, IncreaseDamageOnWieldComponent component, ref GetMeleeDamageEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wield = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(uid, ref wield) && wield.Wielded)
		{
			args.Damage += component.BonusDamage;
		}
	}
}
