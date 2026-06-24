using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Aid;

public sealed class XenoAidSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoEnergySystem _xenoEnergy;

	[Dependency]
	private XenoStrainSystem _xenoStrain;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoAidComponent, XenoAidActionEvent>((EntityEventRefHandler<XenoAidComponent, XenoAidActionEvent>)OnXenoAidAction, (Type[])null, (Type[])null);
	}

	private void OnXenoAidAction(Entity<XenoAidComponent> xeno, ref XenoAidActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			string msg = base.Loc.GetString("rmc-xeno-heal-sisters");
			_popup.PopupClient(msg, Entity<XenoAidComponent>.op_Implicit(xeno), Entity<XenoAidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			string msg2 = base.Loc.GetString("rmc-xeno-not-same-hive");
			_popup.PopupClient(msg2, target, Entity<XenoAidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (xeno.Owner == target)
		{
			string msg3 = base.Loc.GetString("rmc-xeno-aid-self");
			_popup.PopupClient(msg3, target, Entity<XenoAidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		if (_mobState.IsDead(target))
		{
			string msg4 = base.Loc.GetString("rmc-xeno-aid-on-fire");
			_popup.PopupClient(msg4, target, Entity<XenoAidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		switch (args.aidType)
		{
		case XenoAidMode.Healing:
			if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(target)))
			{
				break;
			}
			if (!_xeno.CanHeal(target))
			{
				string msg5 = base.Loc.GetString("rmc-xeno-aid-on-fire");
				_popup.PopupClient(msg5, target, Entity<XenoAidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			}
			else if (_xenoEnergy.TryRemoveEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), xeno.Comp.EnergyCost))
			{
				((HandledEntityEventArgs)args).Handled = true;
				FixedPoint2 heal = xeno.Comp.Heal;
				double bonusHeal = ((double?)((EntitySystem)this).CompOrNull<XenoEnergyComponent>(Entity<XenoAidComponent>.op_Implicit(xeno))?.Current * 0.5).GetValueOrDefault();
				_xenoEnergy.RemoveEnergy(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), (int)bonusHeal);
				if (_xenoStrain.AreSameStrain(Entity<XenoStrainComponent>.op_Implicit(xeno.Owner), Entity<XenoStrainComponent>.op_Implicit(target)))
				{
					heal /= 2f;
				}
				else
				{
					heal += (FixedPoint2)bonusHeal;
				}
				DamageSpecifier toHeal = -_rmcDamageable.DistributeTypesTotal(Entity<DamageableComponent>.op_Implicit(target), heal);
				_damageable.TryChangeDamage(target, toHeal);
				toHeal = -_rmcDamageable.DistributeTypesTotal(Entity<DamageableComponent>.op_Implicit(xeno.Owner), xeno.Comp.Heal * 0.5 + bonusHeal * 0.5);
				_damageable.TryChangeDamage(Entity<XenoAidComponent>.op_Implicit(xeno), toHeal);
				string selfMsg2 = base.Loc.GetString("rmc-xeno-heal-self", (ValueTuple<string, object>)("target", target));
				_popup.PopupClient(selfMsg2, target, Entity<XenoAidComponent>.op_Implicit(xeno));
				string targetMsg2 = base.Loc.GetString("rmc-xeno-heal-target", (ValueTuple<string, object>)("target", xeno));
				_popup.PopupEntity(targetMsg2, target, target);
				string othersMsg2 = base.Loc.GetString("rmc-xeno-heal-others", (ValueTuple<string, object>)("user", xeno), (ValueTuple<string, object>)("target", target));
				Filter filter2 = Filter.Pvs(target, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null).RemovePlayersByAttachedEntity((EntityUid[])(object)new EntityUid[2]
				{
					Entity<XenoAidComponent>.op_Implicit(xeno),
					target
				});
				_popup.PopupEntity(othersMsg2, target, filter2, recordReplay: true);
				EntProtoId? ailmentsEffects = xeno.Comp.HealEffect;
				if (ailmentsEffects.HasValue)
				{
					EntProtoId effect2 = ailmentsEffects.GetValueOrDefault();
					((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(effect2), target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
				}
				ActivateCooldown(Entity<XenoAidComponent>.op_Implicit(xeno));
			}
			break;
		case XenoAidMode.Ailments:
			if (_examine.InRangeUnOccluded(xeno.Owner, target, xeno.Comp.AilmentsRange) && _xenoEnergy.TryRemoveEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), xeno.Comp.EnergyCost))
			{
				ProtoId<StatusEffectPrototype>[] ailmentsRemove = xeno.Comp.AilmentsRemove;
				foreach (ProtoId<StatusEffectPrototype> status in ailmentsRemove)
				{
					_statusEffects.TryRemoveStatusEffect(target, ProtoId<StatusEffectPrototype>.op_Implicit(status));
				}
				base.EntityManager.RemoveComponents(target, xeno.Comp.ComponentsRemove);
				string selfMsg = base.Loc.GetString("rmc-xeno-heal-ailments-self", (ValueTuple<string, object>)("target", target));
				_popup.PopupClient(selfMsg, target, Entity<XenoAidComponent>.op_Implicit(xeno));
				string targetMsg = base.Loc.GetString("rmc-xeno-heal-ailments-target", (ValueTuple<string, object>)("target", target));
				_popup.PopupEntity(targetMsg, target, target);
				string othersMsg = base.Loc.GetString("rmc-xeno-heal-ailments-others", (ValueTuple<string, object>)("user", xeno), (ValueTuple<string, object>)("target", target));
				Filter filter = Filter.Pvs(target, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null).RemovePlayersByAttachedEntity((EntityUid[])(object)new EntityUid[2]
				{
					Entity<XenoAidComponent>.op_Implicit(xeno),
					target
				});
				_popup.PopupEntity(othersMsg, target, filter, recordReplay: true);
				EntProtoId? ailmentsEffects = xeno.Comp.AilmentsEffects;
				if (ailmentsEffects.HasValue)
				{
					EntProtoId effect = ailmentsEffects.GetValueOrDefault();
					((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(effect), target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
				}
				_jitter.DoJitter(target, xeno.Comp.AilmentsJitterDuration, refresh: true, 80f, 8f, forceValueChange: true);
				ActivateCooldown(Entity<XenoAidComponent>.op_Implicit(xeno));
			}
			break;
		}
	}

	private void ActivateCooldown(EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoAidActionEvent>(user))
		{
			_actions.StartUseDelay(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))));
		}
	}
}
