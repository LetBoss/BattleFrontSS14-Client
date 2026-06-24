using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.Damage;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Construction.Upgrades;

public sealed class RMCUpgradeSystem : EntitySystem
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedXenoAcidSystem _xenoAcid;

	private readonly Dictionary<EntProtoId, RMCConstructionUpgradeComponent> _upgradePrototypes = new Dictionary<EntProtoId, RMCConstructionUpgradeComponent>();

	private EntityQuery<RMCConstructionUpgradeItemComponent> _upgradeItemQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_upgradeItemQuery = ((EntitySystem)this).GetEntityQuery<RMCConstructionUpgradeItemComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionUpgradeTargetComponent, InteractUsingEvent>((EntityEventRefHandler<RMCConstructionUpgradeTargetComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCConstructionUpgradeTargetComponent>(((EntitySystem)this).Subs, (object)RMCConstructionUpgradeUiKey.Key, (BuiEventSubscriber<RMCConstructionUpgradeTargetComponent>)delegate(Subscriber<RMCConstructionUpgradeTargetComponent> subs)
		{
			subs.Event<RMCConstructionUpgradeBuiMsg>((EntityEventRefHandler<RMCConstructionUpgradeTargetComponent, RMCConstructionUpgradeBuiMsg>)OnUpgradeBuiMsg);
		});
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		RefreshUpgradePrototypes();
	}

	private void OnInteractUsing(Entity<RMCConstructionUpgradeTargetComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid user = args.User;
		EntityUid used = args.Used;
		RMCConstructionUpgradeItemComponent upgradeItem = default(RMCConstructionUpgradeItemComponent);
		if (_upgradeItemQuery.TryComp(used, ref upgradeItem) && _whitelist.IsValid(upgradeItem.Whitelist, Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent)))
		{
			if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill, ent.Comp.SkillAmountRequired))
			{
				string failPopup = base.Loc.GetString("rmc-construction-failure", (ValueTuple<string, object>)("ent", ent));
				_popup.PopupClient(failPopup, Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (_xenoAcid.IsMelted(Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent)))
			{
				string failPopup2 = base.Loc.GetString("rmc-construction-melted");
				_popup.PopupClient(failPopup2, Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (ent.Comp.Upgrades != null)
			{
				_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCConstructionUpgradeUiKey.Key, (EntityUid?)user, false);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnUpgradeBuiMsg(Entity<RMCConstructionUpgradeTargetComponent> ent, ref RMCConstructionUpgradeBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCConstructionUpgradeUiKey.Key);
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (!_upgradePrototypes.TryGetValue(args.Upgrade, out RMCConstructionUpgradeComponent upgradeComp))
		{
			return;
		}
		EntityUid? upgradeItem = null;
		foreach (EntityUid hand in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(user)))
		{
			if (_upgradeItemQuery.HasComp(hand))
			{
				upgradeItem = hand;
				break;
			}
		}
		if (!upgradeItem.HasValue)
		{
			return;
		}
		if (upgradeComp.Material.HasValue)
		{
			StackComponent stack = default(StackComponent);
			if (((EntitySystem)this).TryComp<StackComponent>(upgradeItem, ref stack))
			{
				ProtoId<StackPrototype>? val = ProtoId<StackPrototype>.op_Implicit(stack.StackTypeId);
				ProtoId<StackPrototype>? material = upgradeComp.Material;
				if (val.HasValue == material.HasValue && (!val.HasValue || val.GetValueOrDefault() == material.GetValueOrDefault()) && !_stack.Use(upgradeItem.Value, upgradeComp.Amount, stack))
				{
					string failPopup = base.Loc.GetString(LocId.op_Implicit(upgradeComp.FailurePopup), (ValueTuple<string, object>)("ent", ent));
					_popup.PopupClient(failPopup, Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
					return;
				}
			}
		}
		else
		{
			((EntitySystem)this).QueueDel(upgradeItem);
		}
		if (!_net.IsClient)
		{
			MapCoordinates coordinates = _transform.GetMapCoordinates(Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent), (TransformComponent)null);
			Angle rotation = _transform.GetWorldRotation(Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent));
			DamageSpecifier transferredDamage = null;
			DamageableComponent damageComp = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<RMCConstructionUpgradeTargetComponent>.op_Implicit(ent), ref damageComp))
			{
				transferredDamage = damageComp.Damage;
			}
			EntityUid spawn = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(upgradeComp.UpgradedEntity), coordinates, (ComponentRegistry)null, DirectionExtensions.ToAngle(((Angle)(ref rotation)).GetCardinalDir()));
			_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(upgradeComp.UpgradedPopup)), spawn, user);
			DamageableComponent newDamageComp = default(DamageableComponent);
			if (transferredDamage != null && ((EntitySystem)this).TryComp<DamageableComponent>(spawn, ref newDamageComp))
			{
				_damageable.SetDamage(spawn, newDamageComp, transferredDamage);
			}
			RMCConstructionUpgradedEvent upgradeEv = new RMCConstructionUpgradedEvent(spawn, ent.Owner);
			((EntitySystem)this).RaiseLocalEvent<RMCConstructionUpgradedEvent>(ent.Owner, upgradeEv, false);
			((EntitySystem)this).RaiseLocalEvent<RMCConstructionUpgradedEvent>(spawn, upgradeEv, true);
			((EntitySystem)this).QueueDel((EntityUid?)ent.Owner);
		}
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<EntityPrototype>())
		{
			RefreshUpgradePrototypes();
		}
	}

	private void RefreshUpgradePrototypes()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_upgradePrototypes.Clear();
		RMCConstructionUpgradeComponent upgrade = default(RMCConstructionUpgradeComponent);
		foreach (EntityPrototype prototype in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (prototype.TryGetComponent<RMCConstructionUpgradeComponent>(ref upgrade, _compFactory))
			{
				_upgradePrototypes[EntProtoId.op_Implicit(prototype.ID)] = upgrade;
			}
		}
	}
}
