using System;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Coordinates;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Tantrum;

public sealed class XenoTantrumSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private XenoStrainSystem _strain;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private XenoEnergySystem _energy;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private MovementSpeedModifierSystem _speed;

	[Dependency]
	private CMArmorSystem _armor;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoTantrumComponent, XenoTantrumActionEvent>((EntityEventRefHandler<XenoTantrumComponent, XenoTantrumActionEvent>)OnXenoTantrumAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TantrumingComponent, ComponentStartup>((EntityEventRefHandler<TantrumingComponent, ComponentStartup>)OnTantrumingAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TantrumingComponent, ComponentShutdown>((EntityEventRefHandler<TantrumingComponent, ComponentShutdown>)OnTantrumingRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TantrumingComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<TantrumingComponent, RefreshMovementSpeedModifiersEvent>)OnTantrumingRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TantrumingComponent, CMGetArmorEvent>((EntityEventRefHandler<TantrumingComponent, CMGetArmorEvent>)OnTantrumingGetArmor, (Type[])null, (Type[])null);
	}

	private void OnTantrumingAdded(Entity<TantrumingComponent> xeno, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<TantrumSpeedBuffComponent>(Entity<TantrumingComponent>.op_Implicit(xeno)))
		{
			_speed.RefreshMovementSpeedModifiers(Entity<TantrumingComponent>.op_Implicit(xeno));
		}
	}

	private void OnTantrumingRemoved(Entity<TantrumingComponent> xeno, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<TantrumingComponent>.op_Implicit(xeno), (MetaDataComponent)null))
		{
			if (((EntitySystem)this).HasComp<TantrumSpeedBuffComponent>(Entity<TantrumingComponent>.op_Implicit(xeno)))
			{
				_speed.RefreshMovementSpeedModifiers(Entity<TantrumingComponent>.op_Implicit(xeno));
			}
			if (_timing.IsFirstTimePredicted)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-tantrum-end"), Entity<TantrumingComponent>.op_Implicit(xeno), Entity<TantrumingComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			}
		}
	}

	private void OnTantrumingRefreshSpeed(Entity<TantrumingComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		TantrumSpeedBuffComponent speedBuff = default(TantrumSpeedBuffComponent);
		if (((EntitySystem)this).TryComp<TantrumSpeedBuffComponent>(Entity<TantrumingComponent>.op_Implicit(xeno), ref speedBuff) && ((Component)xeno.Comp).Running)
		{
			float modifier = speedBuff.SpeedIncrease;
			args.ModifySpeed(modifier, modifier);
		}
	}

	private void OnTantrumingGetArmor(Entity<TantrumingComponent> xeno, ref CMGetArmorEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<TantrumSpeedBuffComponent>(Entity<TantrumingComponent>.op_Implicit(xeno)) && ((Component)xeno.Comp).Running)
		{
			args.XenoArmor += xeno.Comp.ArmorGain;
		}
	}

	private void OnXenoTantrumAction(Entity<XenoTantrumComponent> xeno, ref XenoTantrumActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_transform.InRange(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(args.Target), xeno.Comp.Range))
		{
			return;
		}
		if (((EntitySystem)this).HasComp<TantrumingComponent>(Entity<XenoTantrumComponent>.op_Implicit(xeno)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-raging-self"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (!((EntitySystem)this).HasComp<XenoComponent>(args.Target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-not-xeno"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (xeno.Owner == args.Target)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-self"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(args.Target)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-wrong-hive"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (_mob.IsDead(args.Target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-dead"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (_strain.AreSameStrain(Entity<XenoStrainComponent>.op_Implicit(xeno.Owner), Entity<XenoStrainComponent>.op_Implicit(args.Target)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-valkyrie"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (((EntitySystem)this).HasComp<TantrumingComponent>(args.Target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-fail-raging", (ValueTuple<string, object>)("target", args.Target)), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (_energy.TryRemoveEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), xeno.Comp.FuryCost) && _plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			TimeSpan time = _timing.CurTime;
			TantrumingComponent tantrumingComponent = ((EntitySystem)this).EnsureComp<TantrumingComponent>(Entity<XenoTantrumComponent>.op_Implicit(xeno));
			tantrumingComponent.ArmorGain = xeno.Comp.SelfArmorBoost;
			tantrumingComponent.ExpireAt = time + xeno.Comp.SelfArmorDuration;
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tantrum-self"), Entity<XenoTantrumComponent>.op_Implicit(xeno), Entity<XenoTantrumComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			_audio.PlayPredicted(xeno.Comp.BuffSound, Entity<XenoTantrumComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoTantrumComponent>.op_Implicit(xeno), (AudioParams?)null);
			_aura.GiveAura(Entity<XenoTantrumComponent>.op_Implicit(xeno), xeno.Comp.EnrageColor, xeno.Comp.SelfArmorDuration);
			_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(xeno.Owner));
			TimeSpan otherDuration = (((EntitySystem)this).HasComp<TantrumSpeedBuffComponent>(args.Target) ? xeno.Comp.OtherSpeedDuration : xeno.Comp.OtherArmorDuration);
			((EntitySystem)this).EnsureComp<TantrumingComponent>(args.Target).ExpireAt = time + otherDuration;
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-tantrum-other"), args.Target, args.Target, PopupType.MediumCaution);
			}
			_audio.PlayPredicted(xeno.Comp.BuffSound, args.Target, (EntityUid?)Entity<XenoTantrumComponent>.op_Implicit(xeno), (AudioParams?)null);
			_aura.GiveAura(args.Target, xeno.Comp.EnrageColor, otherDuration);
			_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(args.Target));
			_speed.RefreshMovementSpeedModifiers(args.Target);
			if (!_net.IsClient)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.EnrageEffect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.EnrageEffect), args.Target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			TimeSpan time = _timing.CurTime;
			EntityQueryEnumerator<TantrumingComponent> tantrumQuery = ((EntitySystem)this).EntityQueryEnumerator<TantrumingComponent>();
			EntityUid uid = default(EntityUid);
			TantrumingComponent tantrum = default(TantrumingComponent);
			while (tantrumQuery.MoveNext(ref uid, ref tantrum) && !(time < tantrum.ExpireAt))
			{
				((EntitySystem)this).RemCompDeferred<TantrumingComponent>(uid);
				_speed.RefreshMovementSpeedModifiers(uid);
				_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(uid));
			}
		}
	}
}
