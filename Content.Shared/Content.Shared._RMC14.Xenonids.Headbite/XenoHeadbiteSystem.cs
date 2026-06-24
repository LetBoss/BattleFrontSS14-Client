using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Headbite;

public sealed class XenoHeadbiteSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MobThresholdSystem _mobThresholds;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private SharedXenoHealSystem _xenoHeal;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private StatusEffectsSystem _status;

	private static readonly ProtoId<DamageTypePrototype> LethalDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Asphyxiation");

	private static readonly ProtoId<StatusEffectPrototype> Unconsciousness = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoHeadbiteComponent, XenoHeadbiteActionEvent>((EntityEventRefHandler<XenoHeadbiteComponent, XenoHeadbiteActionEvent>)OnXenoHeadbiteAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHeadbiteComponent, XenoHeadbiteDoAfterEvent>((EntityEventRefHandler<XenoHeadbiteComponent, XenoHeadbiteDoAfterEvent>)OnXenoHeadbiteDoAfter, (Type[])null, (Type[])null);
	}

	private void OnXenoHeadbiteAction(Entity<XenoHeadbiteComponent> xeno, ref XenoHeadbiteActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		if (CanHeadbite(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), target))
		{
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoHeadbiteComponent>.op_Implicit(xeno), xeno.Comp.HeadbiteDelay, new XenoHeadbiteDoAfterEvent(), Entity<XenoHeadbiteComponent>.op_Implicit(xeno), target)
			{
				BreakOnMove = true,
				BreakOnDamage = false,
				ForceVisible = true,
				CancelDuplicate = true,
				DuplicateCondition = DuplicateConditions.SameEvent
			};
			string selfMsg = base.Loc.GetString("rmc-xeno-headbite-self", (ValueTuple<string, object>)("xeno", xeno.Owner), (ValueTuple<string, object>)("target", target));
			_popup.PopupClient(selfMsg, Entity<XenoHeadbiteComponent>.op_Implicit(xeno), Entity<XenoHeadbiteComponent>.op_Implicit(xeno), PopupType.Medium);
			string othersMsg = base.Loc.GetString("rmc-xeno-headbite-others", (ValueTuple<string, object>)("xeno", xeno.Owner), (ValueTuple<string, object>)("target", target));
			_popup.PopupEntity(othersMsg, Entity<XenoHeadbiteComponent>.op_Implicit(xeno), Filter.PvsExcept(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnXenoHeadbiteDoAfter(Entity<XenoHeadbiteComponent> xeno, ref XenoHeadbiteDoAfterEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (!CanHeadbite(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), target2))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.HealEffect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.HeadbiteEffect), target2.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			_emote.TryEmoteWithChat(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), xeno.Comp.Emote, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, xeno.Comp.EmoteCooldown);
			_audio.PlayPvs(xeno.Comp.HitSound, Entity<XenoHeadbiteComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		_xenoHeal.CreateHealStacks(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), xeno.Comp.HealAmount, xeno.Comp.HealDelay, 1, xeno.Comp.HealDelay);
		_jitter.DoJitter(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), xeno.Comp.JitterTime, refresh: true, 80f, 8f, forceValueChange: true);
		if (_damage.TryChangeDamage(target2, xeno.Comp.Damage)?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(target2, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { target2 }, filter);
		}
		DamageableComponent damageable = default(DamageableComponent);
		if (_mobThresholds.TryGetDeadThreshold(target2, out var mobThreshold) && ((EntitySystem)this).TryComp<DamageableComponent>(target2, ref damageable))
		{
			FixedPoint2 lethalAmountOfDamage = mobThreshold.Value - damageable.TotalDamage;
			DamageSpecifier damage = new DamageSpecifier(_prototypeManager.Index<DamageTypePrototype>(LethalDamageType), lethalAmountOfDamage);
			_damage.TryChangeDamage(target2, damage, ignoreResistances: true, interruptsDoAfters: true, null, Entity<XenoHeadbiteComponent>.op_Implicit(xeno));
		}
		string selfMsg = base.Loc.GetString("rmc-xeno-headbite-hit-self", (ValueTuple<string, object>)("xeno", xeno.Owner), (ValueTuple<string, object>)("target", target2));
		_popup.PopupClient(selfMsg, Entity<XenoHeadbiteComponent>.op_Implicit(xeno), Entity<XenoHeadbiteComponent>.op_Implicit(xeno), PopupType.Medium);
		string othersMsg = base.Loc.GetString("rmc-xeno-headbite-hit-others", (ValueTuple<string, object>)("xeno", xeno.Owner), (ValueTuple<string, object>)("target", target2));
		_popup.PopupEntity(othersMsg, Entity<XenoHeadbiteComponent>.op_Implicit(xeno), Filter.PvsExcept(Entity<XenoHeadbiteComponent>.op_Implicit(xeno), 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
	}

	private bool CanHeadbite(EntityUid xeno, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsCritical(target) && !_status.HasStatusEffect(target, "Unconscious"))
		{
			string failMsg = base.Loc.GetString("rmc-xeno-headbite-warning");
			_popup.PopupClient(failMsg, xeno, xeno, PopupType.SmallCaution);
			return false;
		}
		VictimInfectedComponent victim = default(VictimInfectedComponent);
		if (((EntitySystem)this).HasComp<XenoComponent>(xeno) && ((EntitySystem)this).TryComp<VictimInfectedComponent>(target, ref victim) && _hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(xeno), victim.Hive))
		{
			string failMsg2 = base.Loc.GetString("rmc-xeno-headbite-warning-larva");
			_popup.PopupClient(failMsg2, xeno, xeno, PopupType.SmallCaution);
			return false;
		}
		return true;
	}
}
