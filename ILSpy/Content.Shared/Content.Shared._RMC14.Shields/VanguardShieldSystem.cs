using System;
using Content.Shared._RMC14.Damage;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Shields;

public sealed class VanguardShieldSystem : EntitySystem
{
	[Dependency]
	private XenoShieldSystem _shield;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VanguardShieldComponent, MapInitEvent>((EntityEventRefHandler<VanguardShieldComponent, MapInitEvent>)OnVanguardShieldInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VanguardShieldComponent, DamageModifyAfterResistEvent>((EntityEventRefHandler<VanguardShieldComponent, DamageModifyAfterResistEvent>)OnVanguardShieldHit, new Type[1] { typeof(XenoShieldSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VanguardShieldComponent, GetExplosionResistanceEvent>((EntityEventRefHandler<VanguardShieldComponent, GetExplosionResistanceEvent>)OnVanguardShieldGetResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VanguardShieldComponent, RemovedShieldEvent>((EntityEventRefHandler<VanguardShieldComponent, RemovedShieldEvent>)OnVanguardShieldRemoved, (Type[])null, (Type[])null);
	}

	private void OnVanguardShieldInit(Entity<VanguardShieldComponent> xeno, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RegenShield(Entity<VanguardShieldComponent>.op_Implicit(xeno));
	}

	private void OnVanguardShieldHit(Entity<VanguardShieldComponent> xeno, ref DamageModifyAfterResistEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (!(args.Damage.GetTotal() <= 0) && ((EntitySystem)this).TryComp<XenoShieldComponent>(Entity<VanguardShieldComponent>.op_Implicit(xeno), ref shield) && shield.Shield == XenoShieldSystem.ShieldType.Vanguard)
		{
			if (_net.IsServer)
			{
				xeno.Comp.LastTimeHit = _timing.CurTime;
			}
			if (!xeno.Comp.WasHit && args.Damage.GetTotal() > xeno.Comp.DecayThreshold)
			{
				xeno.Comp.WasHit = true;
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-shield-vanguard-hit"), Entity<VanguardShieldComponent>.op_Implicit(xeno), Entity<VanguardShieldComponent>.op_Implicit(xeno), PopupType.MediumCaution);
				args.Damage.ClampMax(0);
			}
		}
	}

	private void OnVanguardShieldGetResistance(Entity<VanguardShieldComponent> xeno, ref GetExplosionResistanceEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (((EntitySystem)this).TryComp<XenoShieldComponent>(Entity<VanguardShieldComponent>.op_Implicit(xeno), ref shield) && shield.Shield == XenoShieldSystem.ShieldType.Vanguard && !(shield.ShieldAmount <= 0))
		{
			int explosionResist = xeno.Comp.ExplosionResistance;
			float resist = (float)Math.Pow(1.1, (double)explosionResist / 5.0);
			args.DamageCoefficient /= resist;
		}
	}

	private void OnVanguardShieldRemoved(Entity<VanguardShieldComponent> xeno, ref RemovedShieldEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (args.Type == XenoShieldSystem.ShieldType.Vanguard)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-shield-vanguard-break"), Entity<VanguardShieldComponent>.op_Implicit(xeno), Entity<VanguardShieldComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			xeno.Comp.NextDecay = _timing.CurTime;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<VanguardShieldComponent, XenoShieldComponent> vanguardQuery = ((EntitySystem)this).EntityQueryEnumerator<VanguardShieldComponent, XenoShieldComponent>();
		EntityUid uid = default(EntityUid);
		VanguardShieldComponent vanguardShield = default(VanguardShieldComponent);
		XenoShieldComponent shield = default(XenoShieldComponent);
		while (vanguardQuery.MoveNext(ref uid, ref vanguardShield, ref shield))
		{
			if (vanguardShield.LastRecharge <= vanguardShield.LastTimeHit && vanguardShield.LastTimeHit + vanguardShield.RechargeTime <= time)
			{
				RegenShield(uid);
			}
			if (shield.Active && vanguardShield.WasHit && !(vanguardShield.NextDecay > time))
			{
				vanguardShield.NextDecay = time + vanguardShield.DecayEvery;
				shield.ShieldAmount = Math.Max(0.0, (shield.ShieldAmount * vanguardShield.DecayMult - vanguardShield.DecaySub).Double());
				if (shield.ShieldAmount <= 0)
				{
					_shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Vanguard);
				}
				((EntitySystem)this).Dirty(uid, (IComponent)(object)shield, (MetaDataComponent)null);
			}
		}
	}

	public bool ShieldBuff(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (!((EntitySystem)this).TryComp<XenoShieldComponent>(ent, ref shield))
		{
			return false;
		}
		if (shield.Shield == XenoShieldSystem.ShieldType.Vanguard && shield.Active)
		{
			return true;
		}
		VanguardShieldComponent vanguard = default(VanguardShieldComponent);
		if (((EntitySystem)this).TryComp<VanguardShieldComponent>(ent, ref vanguard) && vanguard.NextDecay + vanguard.BuffExtraTime > _timing.CurTime)
		{
			return true;
		}
		return false;
	}

	public void RegenShield(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		VanguardShieldComponent vanguard = default(VanguardShieldComponent);
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (((EntitySystem)this).TryComp<VanguardShieldComponent>(ent, ref vanguard) && ((EntitySystem)this).TryComp<XenoShieldComponent>(ent, ref shield))
		{
			vanguard.LastRecharge = _timing.CurTime;
			vanguard.WasHit = false;
			if (!shield.Active && _net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-shield-vanguard-regen"), ent, ent, PopupType.Medium);
			}
			XenoShieldSystem shield2 = _shield;
			FixedPoint2 regenAmount = vanguard.RegenAmount;
			double maxShield = vanguard.RegenAmount.Double();
			shield2.ApplyShield(ent, XenoShieldSystem.ShieldType.Vanguard, regenAmount, null, 0.0, addShield: false, maxShield);
		}
	}
}
