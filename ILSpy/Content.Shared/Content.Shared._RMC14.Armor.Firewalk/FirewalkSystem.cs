using System;
using Content.Shared._RMC14.Aura;
using Content.Shared.Actions;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Armor.Firewalk;

public sealed class FirewalkSystem : EntitySystem
{
	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private INetManager _net;

	private EntityQuery<FirewalkArmorComponent> _firewalkArmorQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_firewalkArmorQuery = ((EntitySystem)this).GetEntityQuery<FirewalkArmorComponent>();
		((EntitySystem)this).SubscribeLocalEvent<FirewalkArmorComponent, GetItemActionsEvent>((EntityEventRefHandler<FirewalkArmorComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirewalkArmorComponent, FirewalkActivateActionEvent>((EntityEventRefHandler<FirewalkArmorComponent, FirewalkActivateActionEvent>)OnFirewalkAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FirewalkArmorComponent, GotUnequippedEvent>((EntityEventRefHandler<FirewalkArmorComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<FirewalkArmorComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		FirewalkArmorComponent comp = ent.Comp;
		if (!args.InHands && _inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<FirewalkArmorComponent>.op_Implicit(ent), null, null)), comp.Slots))
		{
			args.AddAction(ref comp.Action, EntProtoId.op_Implicit(comp.ActionId));
			((EntitySystem)this).Dirty<FirewalkArmorComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnFirewalkAction(Entity<FirewalkArmorComponent> ent, ref FirewalkActivateActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			if (!_whitelist.IsValid(ent.Comp.Whitelist, args.Performer))
			{
				string popup = base.Loc.GetString("cm-gun-unskilled", (ValueTuple<string, object>)("gun", ent.Owner));
				_popup.PopupClient(popup, args.Performer, args.Performer, PopupType.SmallCaution);
			}
			else
			{
				EnableFirewalk(ent, args.Performer);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnUnequipped(Entity<FirewalkArmorComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !_inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<FirewalkArmorComponent>.op_Implicit(ent), null, null)), ent.Comp.Slots))
		{
			EntityUid user = args.Equipee;
			DisableFirewalk(ent, user);
		}
	}

	public void EnableFirewalk(Entity<FirewalkArmorComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		ActiveFirewalkerComponent activeFireWalker = ((EntitySystem)this).EnsureComp<ActiveFirewalkerComponent>(user);
		activeFireWalker.Suit = ent.Owner;
		activeFireWalker.EndAt = _timing.CurTime + ent.Comp.FirewalkTime;
		((EntitySystem)this).Dirty(user, (IComponent)(object)activeFireWalker, (MetaDataComponent)null);
		base.EntityManager.AddComponents(user, ent.Comp.AddComponentsOnFirewalk, true);
		_aura.GiveAura(user, ent.Comp.AuraColor, ent.Comp.FirewalkTime);
		_popup.PopupClient(base.Loc.GetString("rmc-firewalk-activate"), user, user, PopupType.Medium);
	}

	public void DisableFirewalk(Entity<FirewalkArmorComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ActiveFirewalkerComponent>(user);
		((EntitySystem)this).RemCompDeferred<AuraComponent>(user);
		base.EntityManager.RemoveComponents(user, ent.Comp.AddComponentsOnFirewalk);
		if (_net.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-firewalk-end"), user, user, PopupType.Medium);
		}
	}

	public Entity<FirewalkArmorComponent>? FindFirewalkArmor(EntityUid player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(player));
		ContainerSlot slot;
		FirewalkArmorComponent comp = default(FirewalkArmorComponent);
		while (slots.MoveNext(out slot))
		{
			if (((EntitySystem)this).TryComp<FirewalkArmorComponent>(slot.ContainedEntity, ref comp))
			{
				return Entity<FirewalkArmorComponent>.op_Implicit((slot.ContainedEntity.Value, comp));
			}
		}
		return null;
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveFirewalkerComponent> activeQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveFirewalkerComponent>();
		EntityUid uid = default(EntityUid);
		ActiveFirewalkerComponent comp = default(ActiveFirewalkerComponent);
		FirewalkArmorComponent suitComp = default(FirewalkArmorComponent);
		while (activeQuery.MoveNext(ref uid, ref comp))
		{
			if (!(comp.EndAt <= time))
			{
				continue;
			}
			EntityUid? suit = comp.Suit;
			if (suit.HasValue)
			{
				EntityUid suit2 = suit.GetValueOrDefault();
				if (_firewalkArmorQuery.TryComp(suit2, ref suitComp))
				{
					DisableFirewalk(Entity<FirewalkArmorComponent>.op_Implicit((suit2, suitComp)), uid);
				}
			}
		}
	}
}
