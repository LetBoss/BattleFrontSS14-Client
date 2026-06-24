using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.Flash.Components;
using Content.Shared.Inventory;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Sticky.Components;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Explosion;

public abstract class SharedRMCExplosionSystem : EntitySystem
{
	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private RMCDazedSystem _dazed;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private SharedDeafnessSystem _deafness;

	[Dependency]
	private INetManager _net;

	private static readonly ProtoId<DamageTypePrototype> StructuralDamage = ProtoId<DamageTypePrototype>.op_Implicit("Structural");

	private static readonly ProtoId<StatusEffectPrototype> FlashedKey = ProtoId<StatusEffectPrototype>.op_Implicit("Flashed");

	private static readonly ProtoId<StatusEffectPrototype> BlindKey = ProtoId<StatusEffectPrototype>.op_Implicit("Blinded");

	private readonly HashSet<Entity<RMCWallExplosionDeletableComponent>> _walls = new HashSet<Entity<RMCWallExplosionDeletableComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CMExplosionEffectComponent, CMExplosiveTriggeredEvent>((EntityEventRefHandler<CMExplosionEffectComponent, CMExplosiveTriggeredEvent>)OnExplosionEffectTriggered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCExplosiveDeleteComponent, CMExplosiveTriggeredEvent>((EntityEventRefHandler<RMCExplosiveDeleteComponent, CMExplosiveTriggeredEvent>)OnDeleteWallsTriggered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExplosionRandomResistanceComponent, GetExplosionResistanceEvent>((EntityEventRefHandler<ExplosionRandomResistanceComponent, GetExplosionResistanceEvent>)OnExplosionRandomResistanceGet, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunOnExplosionReceivedComponent, ExplosionReceivedEvent>((EntityEventRefHandler<StunOnExplosionReceivedComponent, ExplosionReceivedEvent>)OnStunOnExplosionReceivedBeforeExplode, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DestroyedByExplosionTypeComponent, ExplosionReceivedEvent>((EntityEventRefHandler<DestroyedByExplosionTypeComponent, ExplosionReceivedEvent>)OnDestroyedByExplosionReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobGibbedByExplosionTypeComponent, ExplosionReceivedEvent>((EntityEventRefHandler<MobGibbedByExplosionTypeComponent, ExplosionReceivedEvent>)OnMobGibbedByExplosionReceived, (Type[])null, (Type[])null);
	}

	private void OnExplosionEffectTriggered(Entity<CMExplosionEffectComponent> ent, ref CMExplosiveTriggeredEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DoEffect(ent);
	}

	private void OnDeleteWallsTriggered(Entity<RMCExplosiveDeleteComponent> ent, ref CMExplosiveTriggeredEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		_walls.Clear();
		_entityLookup.GetEntitiesInRange<RMCWallExplosionDeletableComponent>(ent.Owner.ToCoordinates(), (float)ent.Comp.Range, _walls, (LookupFlags)110);
		foreach (Entity<RMCWallExplosionDeletableComponent> wall in _walls)
		{
			((EntitySystem)this).QueueDel((EntityUid?)Entity<RMCWallExplosionDeletableComponent>.op_Implicit(wall));
		}
		if (ent.Comp.Whitelist != null && ((EntitySystem)this).HasComp<StickyComponent>(Entity<RMCExplosiveDeleteComponent>.op_Implicit(ent)))
		{
			EntityUid parent = ((EntitySystem)this).Transform(Entity<RMCExplosiveDeleteComponent>.op_Implicit(ent)).ParentUid;
			if (((EntityUid)(ref parent)).Valid && _entityWhitelist.IsWhitelistPass(ent.Comp.Whitelist, parent))
			{
				((EntitySystem)this).QueueDel((EntityUid?)parent);
			}
		}
	}

	private void OnExplosionRandomResistanceGet(Entity<ExplosionRandomResistanceComponent> ent, ref GetExplosionResistanceEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float resistance = _random.NextFloat(ent.Comp.Min.Float(), ent.Comp.Max.Float());
		args.DamageCoefficient *= resistance;
	}

	public void ChangeExplosionStunResistance(EntityUid ent, StunOnExplosionReceivedComponent? comp, bool isStunnable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StunOnExplosionReceivedComponent>(ent, ref comp, false))
		{
			comp.Weak = isStunnable;
		}
	}

	private void OnStunOnExplosionReceivedBeforeExplode(Entity<StunOnExplosionReceivedComponent> ent, ref ExplosionReceivedEvent args)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 damage = args.Damage.GetTotal();
		double factor = Math.Round(damage.Double() * 0.05) / 2.0;
		factor = Math.Min(20.0, factor);
		if (_standing.IsDown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent)))
		{
			factor *= 0.5;
		}
		_sizeStun.TryGetSize(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), out var size);
		Vector2 dir = _transform.GetWorldPosition(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent)) - args.Epicenter.Position;
		if (size == RMCSizes.Humanoid)
		{
			CMGetArmorEvent ev = new CMGetArmorEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
			((EntitySystem)this).RaiseLocalEvent<CMGetArmorEvent>(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), ref ev, false);
			double bombArmorMult = (double)(100 - ev.ExplosionArmor) * 0.01;
			double severity = factor * 5.0;
			_statusEffects.TryAddStatusEffect<FlashedComponent>(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), ProtoId<StatusEffectPrototype>.op_Implicit(FlashedKey), ent.Comp.BlindTime * bombArmorMult, true, (StatusEffectsComponent?)null, false);
			_deafness.TryDeafen(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(severity * 0.5), refresh: true);
			float knockBackDistance = (float)Math.Clamp(severity / 5.0 / (double)dir.Length(), 0.5, Math.Max(severity / 10.0, 0.5));
			if (!((EntitySystem)this).HasComp<XenoNestedComponent>(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent)))
			{
				_sizeStun.KnockBack(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), args.Epicenter, knockBackDistance, knockBackDistance, (float)severity);
			}
			double num = severity * 0.1;
			double knockoutValue = damage.Double() * 0.1;
			TimeSpan knockdownTime = TimeSpan.FromSeconds(Math.Max(num * bombArmorMult, 1.0));
			_stun.TryParalyze(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), knockdownTime, refresh: false);
			double knockoutMinusArmor = Math.Max(knockoutValue * bombArmorMult * 0.5, 0.5);
			if (args.Explosion == ProtoId<ExplosionPrototype>.op_Implicit("GrenadeLauncher"))
			{
				knockoutMinusArmor *= 0.2;
			}
			knockoutMinusArmor = Math.Min(knockoutMinusArmor, 5.0);
			TimeSpan knockoutTime = TimeSpan.FromSeconds(knockoutMinusArmor);
			_sizeStun.TryKnockOut(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), knockoutTime, refresh: false);
			_dazed.TryDaze(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), knockoutTime * 2.0, refresh: false, null, stutter: true);
			_statusEffects.TryAddStatusEffect<RMCBlindedComponent>(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), ProtoId<StatusEffectPrototype>.op_Implicit(BlindKey), ent.Comp.BlurTime, false, (StatusEffectsComponent?)null, false);
		}
		else if (factor > 0.0 && ent.Comp.Weak)
		{
			TimeSpan stunTime = TimeSpan.FromSeconds(factor / 2.5);
			_stun.TryStun(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), stunTime, refresh: true);
			_stun.TryKnockdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), stunTime, refresh: true);
			if ((int)size < 5)
			{
				_slow.TrySlowdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(factor));
				_slow.TrySuperSlowdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(factor / 2.0));
			}
			else
			{
				_slow.TrySlowdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(factor / 3.0));
			}
			_sizeStun.KnockBack(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), args.Epicenter, 1f, 1f, 5f, ignoreSize: true);
		}
		else if (factor > 10.0)
		{
			factor /= 5.0;
			TimeSpan stunTime2 = TimeSpan.FromSeconds(factor / 5.0);
			_stun.TryStun(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), stunTime2, refresh: true);
			_stun.TryKnockdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), stunTime2, refresh: true);
			if ((int)size < 5)
			{
				_slow.TrySlowdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(factor));
				_slow.TrySuperSlowdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(factor / 2.0));
			}
			else
			{
				_slow.TrySlowdown(Entity<StunOnExplosionReceivedComponent>.op_Implicit(ent), TimeSpan.FromSeconds(factor / 3.0));
			}
		}
	}

	private void OnDestroyedByExplosionReceived(Entity<DestroyedByExplosionTypeComponent> ent, ref ExplosionReceivedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Explosion != ent.Comp.Explosion) && !(args.Damage.GetTotal() < ent.Comp.Threshold) && !((EntitySystem)this).TerminatingOrDeleted(Entity<DestroyedByExplosionTypeComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			((EntitySystem)this).QueueDel((EntityUid?)Entity<DestroyedByExplosionTypeComponent>.op_Implicit(ent));
		}
	}

	private void OnMobGibbedByExplosionReceived(Entity<MobGibbedByExplosionTypeComponent> ent, ref ExplosionReceivedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (Array.IndexOf(ent.Comp.Explosions, args.Explosion) == -1)
		{
			return;
		}
		FixedPoint2 total = FixedPoint2.Zero;
		foreach (var (text2, amount) in args.Damage.DamageDict)
		{
			if (!(ProtoId<DamageTypePrototype>.op_Implicit(text2) == StructuralDamage))
			{
				total += amount;
			}
		}
		if (!(total < ent.Comp.Threshold) && !_net.IsClient && !((EntitySystem)this).TerminatingOrDeleted(Entity<MobGibbedByExplosionTypeComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			_body.GibBody(Entity<MobGibbedByExplosionTypeComponent>.op_Implicit(ent));
		}
	}

	public void DoEffect(Entity<CMExplosionEffectComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? shockWave = ent.Comp.ShockWave;
		if (shockWave.HasValue)
		{
			EntProtoId shockwave = shockWave.GetValueOrDefault();
			((EntitySystem)this).SpawnNextToOrDrop(EntProtoId.op_Implicit(shockwave), Entity<CMExplosionEffectComponent>.op_Implicit(ent), (TransformComponent)null, (ComponentRegistry)null);
		}
		shockWave = ent.Comp.Explosion;
		if (shockWave.HasValue)
		{
			EntProtoId explosion = shockWave.GetValueOrDefault();
			((EntitySystem)this).SpawnNextToOrDrop(EntProtoId.op_Implicit(explosion), Entity<CMExplosionEffectComponent>.op_Implicit(ent), (TransformComponent)null, (ComponentRegistry)null);
		}
		if (ent.Comp.MaxShrapnel <= 0)
		{
			return;
		}
		foreach (EntProtoId effect in ent.Comp.ShrapnelEffects)
		{
			int shrapnelCount = _random.Next(ent.Comp.MinShrapnel, ent.Comp.MaxShrapnel);
			for (int i = 0; i < shrapnelCount; i++)
			{
				Angle angle = _random.NextAngle();
				Vector2 direction = Vector2Helpers.Normalized(((Angle)(ref angle)).ToVec()) * 10f;
				EntityUid shrapnel = ((EntitySystem)this).SpawnNextToOrDrop(EntProtoId.op_Implicit(effect), Entity<CMExplosionEffectComponent>.op_Implicit(ent), (TransformComponent)null, (ComponentRegistry)null);
				_throwing.TryThrow(shrapnel, direction, ent.Comp.ShrapnelSpeed / 10f);
			}
		}
	}

	public void TryDoEffect(Entity<CMExplosionEffectComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CMExplosionEffectComponent>(Entity<CMExplosionEffectComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			DoEffect(Entity<CMExplosionEffectComponent>.op_Implicit((Entity<CMExplosionEffectComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	public virtual void QueueExplosion(MapCoordinates epicenter, string typeId, float totalIntensity, float slope, float maxTileIntensity, EntityUid? cause, float tileBreakScale = 1f, int maxTileBreak = int.MaxValue, bool canCreateVacuum = true, bool addLog = true, float? vehicleLightDamage = null, float? vehicleHeavyDamage = null)
	{
	}

	public virtual void TriggerExplosive(EntityUid uid, bool delete = true, float? totalIntensity = null, float? radius = null, EntityUid? user = null)
	{
	}
}
