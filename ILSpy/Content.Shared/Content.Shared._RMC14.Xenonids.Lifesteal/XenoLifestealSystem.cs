using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Marines;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Lifesteal;

public sealed class XenoLifestealSystem : EntitySystem
{
	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCEmoteSystem _rmcEmote;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private INetManager _net;

	private readonly HashSet<Entity<MobStateComponent>> _targets = new HashSet<Entity<MobStateComponent>>();

	private EntityQuery<DamageableComponent> _damageableQuery;

	private EntityQuery<MarineComponent> _marineQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_damageableQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		_marineQuery = ((EntitySystem)this).GetEntityQuery<MarineComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoLifestealComponent, MeleeHitEvent>((EntityEventRefHandler<XenoLifestealComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
	}

	private void OnMeleeHit(Entity<XenoLifestealComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || !_xeno.CanHeal(xeno.Owner))
		{
			return;
		}
		bool found = false;
		foreach (EntityUid hit in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoLifestealComponent>.op_Implicit(xeno), hit))
			{
				found = true;
				break;
			}
		}
		if (!found)
		{
			return;
		}
		ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
		if (emote.HasValue)
		{
			ProtoId<EmotePrototype> emote2 = emote.GetValueOrDefault();
			_rmcEmote.TryEmoteWithChat(Entity<XenoLifestealComponent>.op_Implicit(xeno), emote2, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, xeno.Comp.EmoteCooldown);
		}
		DamageableComponent damageable = default(DamageableComponent);
		if (!_damageableQuery.TryComp(Entity<XenoLifestealComponent>.op_Implicit(xeno), ref damageable))
		{
			return;
		}
		FixedPoint2 total = damageable.TotalDamage;
		if (total == FixedPoint2.Zero)
		{
			return;
		}
		_targets.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(xeno.Owner.ToCoordinates(), xeno.Comp.TargetRange, _targets, (LookupFlags)110);
		FixedPoint2 lifesteal = xeno.Comp.BasePercentage;
		foreach (Entity<MobStateComponent> hit2 in _targets)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoLifestealComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(hit2)))
			{
				lifesteal += xeno.Comp.TargetIncreasePercentage;
				if (lifesteal >= xeno.Comp.MaxPercentage)
				{
					lifesteal = xeno.Comp.MaxPercentage;
					break;
				}
			}
		}
		FixedPoint2 amount = -FixedPoint2.Clamp(total * lifesteal, xeno.Comp.MinHeal, xeno.Comp.MaxHeal);
		DamageSpecifier heal = _rmcDamageable.DistributeTypes(Entity<DamageableComponent>.op_Implicit(xeno.Owner), amount);
		_damageable.TryChangeDamage(Entity<XenoLifestealComponent>.op_Implicit(xeno), heal, ignoreResistances: true, interruptsDoAfters: true, null, Entity<XenoLifestealComponent>.op_Implicit(xeno), Entity<XenoLifestealComponent>.op_Implicit(xeno));
		if (!(lifesteal >= xeno.Comp.MaxPercentage))
		{
			return;
		}
		Filter marines = Filter.PvsExcept(Entity<XenoLifestealComponent>.op_Implicit(xeno), 2f, (IEntityManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => !_marineQuery.HasComp(e)));
		string marineMsg = base.Loc.GetString("rmc-lifesteal-more-marine", (ValueTuple<string, object>)("xeno", xeno.Owner));
		_popup.PopupEntity(marineMsg, Entity<XenoLifestealComponent>.op_Implicit(xeno), marines, recordReplay: true, PopupType.SmallCaution);
		string selfMsg = base.Loc.GetString("rmc-lifesteal-more-self");
		_popup.PopupClient(selfMsg, Entity<XenoLifestealComponent>.op_Implicit(xeno), Entity<XenoLifestealComponent>.op_Implicit(xeno));
		_aura.GiveAura(Entity<XenoLifestealComponent>.op_Implicit(xeno), xeno.Comp.AuraColor, TimeSpan.FromSeconds(1L));
		if (_net.IsServer)
		{
			EntProtoId? maxEffect = xeno.Comp.MaxEffect;
			if (maxEffect.HasValue)
			{
				EntProtoId effect = maxEffect.GetValueOrDefault();
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(effect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
	}
}
