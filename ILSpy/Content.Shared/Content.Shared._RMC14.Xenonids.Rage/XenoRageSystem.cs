using System;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Rage;

public sealed class XenoRageSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedXenoHealSystem _xenoHeal;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private CMArmorSystem _armor;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoRageComponent, MeleeHitEvent>((EntityEventRefHandler<XenoRageComponent, MeleeHitEvent>)OnRageMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRageComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoRageComponent, RefreshMovementSpeedModifiersEvent>)OnRageRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRageComponent, GetMeleeAttackRateEvent>((EntityEventRefHandler<XenoRageComponent, GetMeleeAttackRateEvent>)OnRageGetMeleeAttackRate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRageComponent, CMGetArmorEvent>((EntityEventRefHandler<XenoRageComponent, CMGetArmorEvent>)OnRageGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRageComponent, ExaminedEvent>((EntityEventRefHandler<XenoRageComponent, ExaminedEvent>)OnRageExamine, (Type[])null, (Type[])null);
	}

	public void IncrementRage(Entity<XenoRageComponent?> xeno, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<XenoRageComponent>(Entity<XenoRageComponent>.op_Implicit(xeno), ref xeno.Comp, false) && !(xeno.Comp.RageCooldownExpireAt > _timing.CurTime) && !xeno.Comp.RageLocked)
		{
			xeno.Comp.Rage += amount;
			xeno.Comp.Rage = Math.Min(xeno.Comp.Rage, xeno.Comp.MaxRage);
			((EntitySystem)this).Dirty<XenoRageComponent>(xeno, (MetaDataComponent)null);
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoRageComponent>.op_Implicit(xeno));
			_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(xeno.Owner));
			if (xeno.Comp.Rage >= xeno.Comp.MaxRage)
			{
				RageLock(Entity<XenoRageComponent>.op_Implicit((xeno.Owner, xeno.Comp)));
			}
		}
	}

	public int GetRage(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		XenoRageComponent rage = default(XenoRageComponent);
		if (!((EntitySystem)this).TryComp<XenoRageComponent>(uid, ref rage))
		{
			return 0;
		}
		return rage.Rage;
	}

	public void RageLock(Entity<XenoRageComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.RageLocked = true;
		xeno.Comp.RageLockExpireAt = _timing.CurTime + xeno.Comp.RageLockDuration;
		((EntitySystem)this).Dirty<XenoRageComponent>(xeno, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoRageComponent>.op_Implicit(xeno));
		_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(xeno.Owner));
		_aura.GiveAura(Entity<XenoRageComponent>.op_Implicit(xeno), xeno.Comp.RageLockColor, xeno.Comp.RageLockDuration, 3f);
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-rage-lock"), Entity<XenoRageComponent>.op_Implicit(xeno), Entity<XenoRageComponent>.op_Implicit(xeno), PopupType.Medium);
	}

	public void RageUnlock(Entity<XenoRageComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.RageLocked = false;
		IncrementRage(Entity<XenoRageComponent>.op_Implicit(xeno.Owner), -xeno.Comp.Rage);
		xeno.Comp.RageCooldownExpireAt = _timing.CurTime + xeno.Comp.RageCooldownDuration;
		((EntitySystem)this).Dirty<XenoRageComponent>(xeno, (MetaDataComponent)null);
		string msg = base.Loc.GetString("rmc-xeno-rage-expire", (ValueTuple<string, object>)("cooldown", xeno.Comp.RageCooldownDuration.Seconds.ToString()));
		_popup.PopupEntity(msg, Entity<XenoRageComponent>.op_Implicit(xeno), Entity<XenoRageComponent>.op_Implicit(xeno), PopupType.MediumCaution);
		_aura.GiveAura(Entity<XenoRageComponent>.op_Implicit(xeno), xeno.Comp.RageLockWeakenColor, xeno.Comp.RageCooldownDuration);
	}

	private void OnRageMeleeHit(Entity<XenoRageComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		bool validTarget = false;
		foreach (EntityUid entity in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(xeno.Owner, entity))
			{
				validTarget = true;
				break;
			}
		}
		if (validTarget)
		{
			IncrementRage(Entity<XenoRageComponent>.op_Implicit(xeno.Owner), 1);
			FixedPoint2 healAmount = (0.05 * (double)xeno.Comp.Rage + 0.3) * xeno.Comp.HealAmount;
			_xenoHeal.CreateHealStacks(Entity<XenoRageComponent>.op_Implicit(xeno), healAmount, xeno.Comp.RageHealTime, 1, xeno.Comp.RageHealTime);
			xeno.Comp.LastHit = _timing.CurTime;
			((EntitySystem)this).Dirty<XenoRageComponent>(xeno, (MetaDataComponent)null);
		}
	}

	private void OnRageRefreshSpeed(Entity<XenoRageComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		float speed = 1f + (float)xeno.Comp.Rage * xeno.Comp.SpeedBuffPerRage;
		args.ModifySpeed(speed, speed);
	}

	private void OnRageGetMeleeAttackRate(Entity<XenoRageComponent> xeno, ref GetMeleeAttackRateEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		args.Rate += (float)xeno.Comp.Rage * xeno.Comp.AttackSpeedPerRage;
	}

	private void OnRageGetArmor(Entity<XenoRageComponent> xeno, ref CMGetArmorEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.XenoArmor += xeno.Comp.Rage * xeno.Comp.ArmorPerRage;
	}

	private void OnRageExamine(Entity<XenoRageComponent> xeno, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("XenoRageComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-rage-examine", new(string, object)[3]
			{
				("xeno", xeno),
				("amount", xeno.Comp.Rage),
				("max", xeno.Comp.MaxRage)
			}));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoRageComponent> rageQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoRageComponent>();
		EntityUid uid = default(EntityUid);
		XenoRageComponent rage = default(XenoRageComponent);
		while (rageQuery.MoveNext(ref uid, ref rage))
		{
			if (rage.LastHit + rage.RageDecayTime <= time && rage.Rage > 0 && !rage.RageLocked)
			{
				IncrementRage(Entity<XenoRageComponent>.op_Implicit((uid, rage)), -1);
				rage.LastHit = time;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)rage, (MetaDataComponent)null);
			}
			if (rage.RageLocked && rage.RageLockExpireAt <= time)
			{
				RageUnlock(Entity<XenoRageComponent>.op_Implicit((uid, rage)));
			}
		}
	}
}
