using System;
using Content.Shared._RMC14.Armor.ThermalCloak;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Armor.Ghillie;

public sealed class SharedGhillieSuitSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ThermalCloakSystem _thermalCloak;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GhillieSuitComponent, GetItemActionsEvent>((EntityEventRefHandler<GhillieSuitComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhillieSuitComponent, GhillieSuitPreparePositionActionEvent>((EntityEventRefHandler<GhillieSuitComponent, GhillieSuitPreparePositionActionEvent>)OnPreparePositionAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhillieSuitComponent, GhillieSuitDoAfterEvent>((EntityEventRefHandler<GhillieSuitComponent, GhillieSuitDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhillieSuitComponent, GotEquippedEvent>((EntityEventRefHandler<GhillieSuitComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GhillieSuitComponent, GotUnequippedEvent>((EntityEventRefHandler<GhillieSuitComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPassiveStealthComponent, VaporHitEvent>((EntityEventRefHandler<RMCPassiveStealthComponent, VaporHitEvent>)OnVaporHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPassiveStealthComponent, MoveInputEvent>((EntityEventRefHandler<RMCPassiveStealthComponent, MoveInputEvent>)OnMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, GunShotEvent>((EntityEventRefHandler<GunComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<GhillieSuitComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		GhillieSuitComponent comp = ent.Comp;
		if (!args.InHands && _inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<GhillieSuitComponent>.op_Implicit(ent), null, null)), SlotFlags.OUTERCLOTHING))
		{
			args.AddAction(ref comp.Action, EntProtoId.op_Implicit(comp.ActionId));
			((EntitySystem)this).Dirty<GhillieSuitComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPreparePositionAction(Entity<GhillieSuitComponent> ent, ref GhillieSuitPreparePositionActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Performer;
		GhillieSuitComponent comp = ent.Comp;
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (!_whitelist.IsValid(ent.Comp.Whitelist, args.Performer))
		{
			string popup = base.Loc.GetString("cm-gun-unskilled", (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup, args.Performer, args.Performer, PopupType.SmallCaution);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!comp.Enabled)
		{
			GhillieSuitDoAfterEvent ev = new GhillieSuitDoAfterEvent();
			DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, comp.UseDelay, ev, ent.Owner)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				CancelDuplicate = true,
				DuplicateCondition = DuplicateConditions.SameTool
			};
			if (_doAfter.TryStartDoAfter(doAfterEventArgs))
			{
				string activatedPopupSelf = base.Loc.GetString("rmc-ghillie-activate-self");
				string activatedPopupOthers = base.Loc.GetString("rmc-ghillie-activate-others", (ValueTuple<string, object>)("user", user));
				_popup.PopupPredicted(activatedPopupSelf, activatedPopupOthers, user, user, PopupType.Medium);
			}
		}
		else
		{
			ToggleInvisibility(ent, user, enabling: false);
		}
	}

	private void OnDoAfter(Entity<GhillieSuitComponent> ent, ref GhillieSuitDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleInvisibility(ent, user, enabling: true);
		}
	}

	private void OnEquipped(Entity<GhillieSuitComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && _inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<GhillieSuitComponent>.op_Implicit(ent), null, null)), SlotFlags.OUTERCLOTHING))
		{
			EntityTurnInvisibleComponent comp = ((EntitySystem)this).EnsureComp<EntityTurnInvisibleComponent>(args.Equipee);
			((EntitySystem)this).Dirty(args.Equipee, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnUnequipped(Entity<GhillieSuitComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !_inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<GhillieSuitComponent>.op_Implicit(ent), null, null)), SlotFlags.OUTERCLOTHING))
		{
			((EntitySystem)this).RemCompDeferred<EntityTurnInvisibleComponent>(args.Equipee);
			ToggleInvisibility(ent, args.Equipee, enabling: false);
		}
	}

	public void ToggleInvisibility(Entity<GhillieSuitComponent> ent, EntityUid user, bool enabling)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		GhillieSuitComponent comp = ent.Comp;
		EntityTurnInvisibleComponent turnInvisible = default(EntityTurnInvisibleComponent);
		if (!((EntitySystem)this).TryComp<EntityTurnInvisibleComponent>(user, ref turnInvisible))
		{
			return;
		}
		if (enabling && !((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(user))
		{
			turnInvisible.Enabled = true;
			comp.Enabled = true;
			((EntitySystem)this).Dirty<GhillieSuitComponent>(ent, (MetaDataComponent)null);
			RMCPassiveStealthComponent passiveInvisibility = ((EntitySystem)this).EnsureComp<RMCPassiveStealthComponent>(user);
			passiveInvisibility.MaxOpacity = comp.Opacity;
			passiveInvisibility.MinOpacity = comp.Opacity;
			passiveInvisibility.Delay = comp.InvisibilityDelay;
			passiveInvisibility.Enabled = true;
			passiveInvisibility.ToggleTime = _timing.CurTime;
			((EntitySystem)this).Dirty(user, (IComponent)(object)passiveInvisibility, (MetaDataComponent)null);
			EntityActiveInvisibleComponent activeInvisibility = ((EntitySystem)this).EnsureComp<EntityActiveInvisibleComponent>(user);
			activeInvisibility.Opacity = comp.Opacity;
			activeInvisibility.DisableMobCollision = true;
			((EntitySystem)this).Dirty(user, (IComponent)(object)activeInvisibility, (MetaDataComponent)null);
			turnInvisible.UncloakTime = _timing.CurTime;
			((EntitySystem)this).Dirty(user, (IComponent)(object)turnInvisible, (MetaDataComponent)null);
			if (ent.Comp.BlockFriendlyFire)
			{
				((EntitySystem)this).EnsureComp<EntityIFFComponent>(user);
			}
			((EntitySystem)this).RemCompDeferred<RMCNightVisionVisibleComponent>(user);
			_thermalCloak.SpawnCloakEffects(user, comp.CloakEffect);
		}
		EntityActiveInvisibleComponent invisible = default(EntityActiveInvisibleComponent);
		if (!enabling && ((EntitySystem)this).TryComp<EntityActiveInvisibleComponent>(user, ref invisible))
		{
			invisible.Opacity = 1f;
			((EntitySystem)this).Dirty(user, (IComponent)(object)invisible, (MetaDataComponent)null);
			comp.Enabled = false;
			((EntitySystem)this).Dirty<GhillieSuitComponent>(ent, (MetaDataComponent)null);
			turnInvisible.Enabled = false;
			turnInvisible.UncloakTime = _timing.CurTime;
			((EntitySystem)this).Dirty(user, (IComponent)(object)turnInvisible, (MetaDataComponent)null);
			string deactivatedPopupSelf = base.Loc.GetString("rmc-ghillie-fail-self");
			string deactivatedPopupOthers = base.Loc.GetString("rmc-ghillie-fail-others", (ValueTuple<string, object>)("user", user));
			_popup.PopupPredicted(deactivatedPopupSelf, deactivatedPopupOthers, user, user, PopupType.Medium);
			((EntitySystem)this).EnsureComp<RMCNightVisionVisibleComponent>(user);
			((EntitySystem)this).RemComp<RMCPassiveStealthComponent>(user);
			((EntitySystem)this).RemComp<EntityActiveInvisibleComponent>(user);
			if (ent.Comp.BlockFriendlyFire)
			{
				((EntitySystem)this).RemCompDeferred<EntityIFFComponent>(user);
			}
		}
	}

	public void TryToggleInvisibility(EntityUid uid, bool enabling)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Entity<GhillieSuitComponent>? suit = FindSuit(uid);
		if (suit.HasValue)
		{
			ToggleInvisibility(suit.Value, uid, enabling);
		}
	}

	public Entity<GhillieSuitComponent>? FindSuit(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(uid), SlotFlags.OUTERCLOTHING);
		ContainerSlot slot;
		GhillieSuitComponent comp = default(GhillieSuitComponent);
		while (slots.MoveNext(out slot))
		{
			if (((EntitySystem)this).TryComp<GhillieSuitComponent>(slot.ContainedEntity, ref comp))
			{
				return Entity<GhillieSuitComponent>.op_Implicit((slot.ContainedEntity.Value, comp));
			}
		}
		return null;
	}

	private void OnVaporHit(Entity<RMCPassiveStealthComponent> ent, ref VaporHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TryToggleInvisibility(ent.Owner, enabling: false);
	}

	private void OnMove(Entity<RMCPassiveStealthComponent> ent, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement)
		{
			TryToggleInvisibility(ent.Owner, enabling: false);
		}
	}

	private void OnGunShot(Entity<GunComponent> ent, ref GunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		Entity<GhillieSuitComponent>? suit = FindSuit(user);
		if (suit.HasValue)
		{
			GhillieSuitComponent suitComp = suit.Value.Comp;
			EntityActiveInvisibleComponent invis = default(EntityActiveInvisibleComponent);
			RMCPassiveStealthComponent passive = default(RMCPassiveStealthComponent);
			if (suitComp.Enabled && ((EntitySystem)this).TryComp<EntityActiveInvisibleComponent>(user, ref invis) && ((EntitySystem)this).TryComp<RMCPassiveStealthComponent>(user, ref passive))
			{
				invis.Opacity += suitComp.AddedOpacityOnShoot;
				((EntitySystem)this).Dirty(user, (IComponent)(object)invis, (MetaDataComponent)null);
				passive.ToggleTime = _timing.CurTime + suitComp.InvisibilityBreakDelay;
				passive.MaxOpacity = invis.Opacity;
				((EntitySystem)this).Dirty(user, (IComponent)(object)passive, (MetaDataComponent)null);
			}
		}
	}
}
