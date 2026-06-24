using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Shields;

public sealed class XenoShieldSystem : EntitySystem
{
	public enum ShieldType
	{
		Generic,
		Ravager,
		Hedgehog,
		Vanguard,
		Praetorian,
		Crusher,
		Warden,
		Gardener,
		ShieldPillar,
		King,
		CumulativeGeneric
	}

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	private static readonly ProtoId<DamageTypePrototype> ShieldSoundDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Piercing");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoShieldComponent, DamageModifyAfterResistEvent>((EntityEventRefHandler<XenoShieldComponent, DamageModifyAfterResistEvent>)OnDamage, (Type[])null, (Type[])null);
	}

	private void OnDamage(Entity<XenoShieldComponent> ent, ref DamageModifyAfterResistEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Active || !args.Damage.AnyPositive())
		{
			return;
		}
		ent.Comp.ShieldAmount -= args.Damage.GetTotal();
		if (ent.Comp.ShieldAmount <= 0)
		{
			FixedPoint2 usableShield = ent.Comp.ShieldAmount + args.Damage.GetTotal();
			ent.Comp.ShieldAmount = 0;
			foreach (KeyValuePair<string, FixedPoint2> type in args.Damage.DamageDict)
			{
				if (usableShield == 0)
				{
					break;
				}
				if (type.Value > 0)
				{
					double tempVal = Math.Min(type.Value.Double(), usableShield.Double());
					args.Damage.DamageDict[type.Key] -= (FixedPoint2)tempVal;
					usableShield -= (FixedPoint2)tempVal;
				}
			}
			_audio.PlayPredicted(ent.Comp.ShieldBreak, Entity<XenoShieldComponent>.op_Implicit(ent), (EntityUid?)null, (AudioParams?)null);
			RemoveShield(Entity<XenoShieldComponent>.op_Implicit(ent), ent.Comp.Shield);
		}
		else
		{
			if (((EntitySystem)this).HasComp<ProjectileComponent>(args.Tool) && args.Damage.DamageDict.ContainsKey(ProtoId<DamageTypePrototype>.op_Implicit(ShieldSoundDamageType)))
			{
				_audio.PlayPredicted(ent.Comp.ShieldImpact, Entity<XenoShieldComponent>.op_Implicit(ent), (EntityUid?)null, (AudioParams?)null);
			}
			args.Damage.ClampMax(0);
			_appearance.SetData(Entity<XenoShieldComponent>.op_Implicit(ent), (Enum)RMCShieldVisuals.Current, (object)ent.Comp.ShieldAmount, (AppearanceComponent)null);
		}
		((EntitySystem)this).Dirty(Entity<XenoShieldComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	public bool ApplyShield(EntityUid uid, ShieldType type, FixedPoint2 amount, TimeSpan? duration = null, double decay = 0.0, bool addShield = false, double maxShield = 200.0, string? visualState = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<UnshieldableComponent>(uid))
		{
			return false;
		}
		XenoShieldComponent shieldComp = ((EntitySystem)this).EnsureComp<XenoShieldComponent>(uid);
		if (shieldComp.Active && shieldComp.Shield == type)
		{
			if (addShield)
			{
				shieldComp.ShieldAmount = Math.Min((shieldComp.ShieldAmount + amount).Double(), maxShield);
			}
			else
			{
				shieldComp.ShieldAmount = Math.Max(shieldComp.ShieldAmount.Double(), amount.Double());
			}
			_appearance.SetData(uid, (Enum)RMCShieldVisuals.Current, (object)shieldComp.ShieldAmount, (AppearanceComponent)null);
			return true;
		}
		RemoveShield(uid, shieldComp.Shield);
		shieldComp.Shield = type;
		shieldComp.ShieldAmount = amount;
		shieldComp.Duration = duration;
		shieldComp.DecayPerSecond = decay;
		if (duration.HasValue)
		{
			shieldComp.ShieldDecayAt = _timing.CurTime + duration.Value;
		}
		shieldComp.Active = true;
		if (visualState != null)
		{
			_appearance.SetData(uid, (Enum)RMCShieldVisuals.Prefix, (object)visualState, (AppearanceComponent)null);
			_appearance.SetData(uid, (Enum)RMCShieldVisuals.Active, (object)true, (AppearanceComponent)null);
			_appearance.SetData(uid, (Enum)RMCShieldVisuals.Current, (object)shieldComp.ShieldAmount, (AppearanceComponent)null);
			_appearance.SetData(uid, (Enum)RMCShieldVisuals.Max, (object)maxShield, (AppearanceComponent)null);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)shieldComp, (MetaDataComponent)null);
		return true;
	}

	public void RemoveShield(EntityUid uid, ShieldType shieldType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shieldComp = default(XenoShieldComponent);
		if (((EntitySystem)this).TryComp<XenoShieldComponent>(uid, ref shieldComp) && shieldComp.Active && shieldComp.Shield == shieldType)
		{
			shieldComp.Active = false;
			_appearance.SetData(uid, (Enum)RMCShieldVisuals.Active, (object)false, (AppearanceComponent)null);
			shieldComp.ShieldAmount = 0;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)shieldComp, (MetaDataComponent)null);
			RemovedShieldEvent ev = new RemovedShieldEvent(shieldType);
			((EntitySystem)this).RaiseLocalEvent<RemovedShieldEvent>(uid, ref ev, false);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoShieldComponent> shieldQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoShieldComponent>();
		EntityUid uid = default(EntityUid);
		XenoShieldComponent shield = default(XenoShieldComponent);
		while (shieldQuery.MoveNext(ref uid, ref shield))
		{
			if (shield.Duration.HasValue && !(time < shield.ShieldDecayAt))
			{
				shield.ShieldAmount -= (FixedPoint2)(shield.DecayPerSecond * (double)frameTime);
				_appearance.SetData(uid, (Enum)RMCShieldVisuals.Current, (object)shield.ShieldAmount, (AppearanceComponent)null);
				if (shield.ShieldAmount <= 0)
				{
					RemoveShield(uid, shield.Shield);
				}
				else
				{
					((EntitySystem)this).Dirty(uid, (IComponent)(object)shield, (MetaDataComponent)null);
				}
			}
		}
	}
}
