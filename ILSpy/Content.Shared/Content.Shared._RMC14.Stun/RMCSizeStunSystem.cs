using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stamina;
using Content.Shared.Coordinates;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Flash;
using Content.Shared.Interaction;
using Content.Shared.Pointing;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Speech.Muting;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Stun;

public sealed class RMCSizeStunSystem : EntitySystem
{
	private const double DazedMultiplierSmallXeno = 0.7;

	private const double DazedMultiplierBigXeno = 1.2;

	private static readonly ProtoId<StatusEffectPrototype> KnockedOut = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	[Dependency]
	private RMCDazedSystem _dazed;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedFlashSystem _flash;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private RMCStaminaSystem _stamina;

	[Dependency]
	private StandingStateSystem _stand;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<Entity<MarineComponent>> _marines = new HashSet<Entity<MarineComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCStunOnHitComponent, MapInitEvent>((EntityEventRefHandler<RMCStunOnHitComponent, MapInitEvent>)OnSizeStunMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStunOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCStunOnHitComponent, ProjectileHitEvent>)OnHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStunOnHitComponent, RMCTriggerEvent>((EntityEventRefHandler<RMCStunOnHitComponent, RMCTriggerEvent>)OnTrigger, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStunOnTriggerComponent, RMCTriggerEvent>((EntityEventRefHandler<RMCStunOnTriggerComponent, RMCTriggerEvent>)OnStunOnTrigger, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCUnconsciousComponent, ComponentStartup>((EntityEventRefHandler<RMCUnconsciousComponent, ComponentStartup>)OnUnconsciousStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCUnconsciousComponent, ComponentShutdown>((EntityEventRefHandler<RMCUnconsciousComponent, ComponentShutdown>)OnUnconsciousEnd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCUnconsciousComponent, StatusEffectEndedEvent>((EntityEventRefHandler<RMCUnconsciousComponent, StatusEffectEndedEvent>)OnUnconsciousUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCUnconsciousComponent, PointAttemptEvent>((EntityEventRefHandler<RMCUnconsciousComponent, PointAttemptEvent>)OnUnconsciousPointAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCKnockOutOnCollideComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCKnockOutOnCollideComponent, ProjectileHitEvent>)OnKnockOutCollideProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCKnockOutOnCollideComponent, ThrowDoHitEvent>((EntityEventRefHandler<RMCKnockOutOnCollideComponent, ThrowDoHitEvent>)OnKnockOutCollideThrowHit, (Type[])null, (Type[])null);
	}

	public bool IsHumanoidSized(Entity<RMCSizeComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return (int)ent.Comp.Size <= 1;
	}

	public bool IsHumanoidSized(RMCSizes size)
	{
		return (int)size <= 1;
	}

	public bool IsXenoSized(Entity<RMCSizeComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RMCSizeComponent>(Entity<RMCSizeComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		return (int)ent.Comp.Size >= 2;
	}

	public bool IsXenoSized(RMCSizes size)
	{
		return (int)size >= 2;
	}

	public bool TryGetSize(EntityUid ent, out RMCSizes size)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		size = RMCSizes.Small;
		RMCSizeComponent sizeComp = default(RMCSizeComponent);
		if (!((EntitySystem)this).TryComp<RMCSizeComponent>(ent, ref sizeComp))
		{
			return false;
		}
		size = sizeComp.Size;
		return true;
	}

	private void OnSizeStunMapInit(Entity<RMCStunOnHitComponent> projectile, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		projectile.Comp.ShotFrom = _transform.GetMapCoordinates(projectile.Owner, (TransformComponent)null);
		((EntitySystem)this).Dirty<RMCStunOnHitComponent>(projectile, (MetaDataComponent)null);
	}

	private void OnHit(Entity<RMCStunOnHitComponent> bullet, ref ProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		if (!bullet.Comp.ShotFrom.HasValue)
		{
			return;
		}
		RMCSizeComponent size = default(RMCSizeComponent);
		foreach (RMCStunOnHit stun in bullet.Comp.Stuns)
		{
			if (_entityWhitelist.IsWhitelistFail(stun.Whitelist, args.Target))
			{
				continue;
			}
			float distance = (_transform.GetMoverCoordinates(args.Target).Position - bullet.Comp.ShotFrom.Value.Position).Length();
			if (distance > stun.MaxRange || _stand.IsDown(args.Target) || !((EntitySystem)this).TryComp<RMCSizeComponent>(args.Target, ref size))
			{
				break;
			}
			KnockBack(args.Target, bullet.Comp.ShotFrom, stun.KnockBackPowerMin, stun.KnockBackPowerMax, stun.KnockBackSpeed);
			if (_net.IsClient)
			{
				break;
			}
			double dazeMultiplier = 1.0;
			if ((int)size.Size >= 5)
			{
				dazeMultiplier = 1.2;
			}
			else if ((int)size.Size <= 3 && IsXenoSized(Entity<RMCSizeComponent>.op_Implicit((args.Target, size))))
			{
				dazeMultiplier = 0.7;
			}
			_dazed.TryDaze(args.Target, stun.DazeTime * dazeMultiplier);
			if (IsXenoSized(Entity<RMCSizeComponent>.op_Implicit((args.Target, size))))
			{
				TimeSpan stunTime = stun.StunTime;
				TimeSpan superSlow = stun.SuperSlowTime;
				TimeSpan slow = stun.SlowTime;
				if (stun.LosesEffectWithRange)
				{
					stunTime -= TimeSpan.FromSeconds(distance / 50f);
					superSlow -= TimeSpan.FromSeconds(distance / 10f);
					slow -= TimeSpan.FromSeconds(distance / 5f);
				}
				if (stun.SlowsEffectBigXenos || (int)size.Size < 5)
				{
					ApplyEffects(args.Target, stunTime, slow, superSlow);
				}
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-stun-shaken"), args.Target, args.Target, PopupType.MediumCaution);
			}
			else
			{
				_stamina.DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit(args.Target), args.Damage.GetTotal().Float());
			}
		}
	}

	private void ApplyEffects(EntityUid uid, TimeSpan stun, TimeSpan slow, TimeSpan superSlow)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_slow.TrySlowdown(uid, slow);
		_slow.TrySuperSlowdown(uid, superSlow);
		RMCSizeComponent size = default(RMCSizeComponent);
		if (((EntitySystem)this).TryComp<RMCSizeComponent>(uid, ref size) && (int)size.Size < 5)
		{
			_stun.TryParalyze(uid, stun, refresh: true);
		}
	}

	public void KnockBack(EntityUid target, MapCoordinates? knockedBackFrom, float knockBackPowerMin = 1f, float knockBackPowerMax = 1f, float knockBackSpeed = 5f, bool ignoreSize = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		RMCSizeComponent size = default(RMCSizeComponent);
		if (((((EntitySystem)this).TryComp<RMCSizeComponent>(target, ref size) && (int)size.Size < 5) || ignoreSize) && knockedBackFrom.HasValue)
		{
			PhysicsComponent physics = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(target, ref physics))
			{
				_physics.SetLinearVelocity(target, Vector2.Zero, true, true, (FixturesComponent)null, physics);
				_physics.SetAngularVelocity(target, 0f, true, (FixturesComponent)null, physics);
			}
			Vector2 vec = _transform.GetMoverCoordinates(target).Position - knockedBackFrom.Value.Position;
			if (vec.Length() != 0f)
			{
				_rmcPulling.TryStopPullsOn(target);
				float knockBackPower = _random.NextFloat(knockBackPowerMin, knockBackPowerMax);
				Vector2 direction = Vector2Helpers.Normalized(vec) * knockBackPower;
				_throwing.TryThrow(target, direction, knockBackSpeed, null, 2f, null, compensateFriction: true, recoil: true, animated: false, playSound: false);
			}
		}
	}

	private void OnTrigger(Entity<RMCStunOnHitComponent> ent, ref RMCTriggerEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(Entity<RMCStunOnHitComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<RMCStunOnHitComponent>.op_Implicit(ent)));
		foreach (RMCStunOnHit stun in ent.Comp.Stuns)
		{
			foreach (Entity<StatusEffectsComponent> target in _entityLookup.GetEntitiesInRange<StatusEffectsComponent>(moverCoordinates, stun.StunArea, (LookupFlags)110))
			{
				if (!_entityWhitelist.IsWhitelistFail(stun.Whitelist, Entity<StatusEffectsComponent>.op_Implicit(target)))
				{
					ApplyEffects(Entity<StatusEffectsComponent>.op_Implicit(target), stun.StunTime, stun.SlowTime, stun.SuperSlowTime);
					KnockBack(Entity<StatusEffectsComponent>.op_Implicit(target), ent.Comp.ShotFrom, stun.KnockBackPowerMin, stun.KnockBackPowerMax, stun.KnockBackSpeed);
					break;
				}
			}
		}
	}

	private void OnStunOnTrigger(Entity<RMCStunOnTriggerComponent> ent, ref RMCTriggerEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		_marines.Clear();
		_entityLookup.GetEntitiesInRange<MarineComponent>(ent.Owner.ToCoordinates(), ent.Comp.Range, _marines, (LookupFlags)110);
		foreach (Entity<MarineComponent> target in _marines)
		{
			if (ent.Comp.Filters != null)
			{
				bool passedFilter = false;
				foreach (RMCStunOnTriggerFilter filter in ent.Comp.Filters)
				{
					if (!_entityWhitelist.IsWhitelistFail(filter.Whitelist, Entity<MarineComponent>.op_Implicit(target)))
					{
						float probability = filter.Probability ?? ent.Comp.Probability;
						float range = filter.Range ?? ent.Comp.Range;
						TimeSpan stun = filter.Stun ?? ent.Comp.Stun;
						TimeSpan flash = filter.Flash ?? ent.Comp.Flash;
						TimeSpan flashAdditionalStunTime = filter.FlashAdditionalStunTime ?? ent.Comp.FlashAdditionalStunTime;
						TimeSpan paralyze = filter.Paralyze ?? ent.Comp.Paralyze;
						Stun(ent, Entity<MarineComponent>.op_Implicit(target), args.User, probability, range, stun, flash, flashAdditionalStunTime, paralyze);
						passedFilter = true;
						break;
					}
				}
				if (passedFilter)
				{
					continue;
				}
			}
			Stun(ent, Entity<MarineComponent>.op_Implicit(target), args.User, ent.Comp.Probability, ent.Comp.Range, ent.Comp.Stun, ent.Comp.Flash, ent.Comp.FlashAdditionalStunTime, ent.Comp.Paralyze);
		}
		args.Handled = true;
	}

	private void Stun(Entity<RMCStunOnTriggerComponent> ent, EntityUid target, EntityUid? user, float probability, float range, TimeSpan stun, TimeSpan flash, TimeSpan flashAdditionalStunTime, TimeSpan paralyze)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = ((EntitySystem)this).Transform(target).Coordinates;
		if (RandomExtensions.Prob(_random, probability) && _interaction.InRangeUnobstructed(Entity<RMCStunOnTriggerComponent>.op_Implicit(ent), coordinates, range))
		{
			if (_flash.Flash(target, user, Entity<RMCStunOnTriggerComponent>.op_Implicit(ent), (float)flash.TotalMilliseconds, 0.8f, displayPopup: false))
			{
				stun += flashAdditionalStunTime;
				paralyze += flashAdditionalStunTime;
			}
			if (stun > TimeSpan.Zero)
			{
				_stun.TryStun(target, stun, refresh: true);
				_stun.TryKnockdown(target, stun, refresh: true);
			}
			if (paralyze > TimeSpan.Zero)
			{
				TryKnockOut(target, paralyze);
			}
		}
	}

	public bool TryKnockOut(EntityUid uid, TimeSpan duration, bool refresh = true, StatusEffectsComponent? status = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (duration <= TimeSpan.Zero)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!_status.TryAddStatusEffect<RMCUnconsciousComponent>(uid, ProtoId<StatusEffectPrototype>.op_Implicit(KnockedOut), duration, refresh, (StatusEffectsComponent?)null, false))
		{
			return false;
		}
		return true;
	}

	private void OnUnconsciousStart(Entity<RMCUnconsciousComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<StunnedComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		((EntitySystem)this).EnsureComp<KnockedDownComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		((EntitySystem)this).EnsureComp<TemporaryBlindnessComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		((EntitySystem)this).EnsureComp<MutedComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		((EntitySystem)this).EnsureComp<DeafComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
	}

	private void OnUnconsciousEnd(Entity<RMCUnconsciousComponent> ent, ref ComponentShutdown args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (!_status.TryGetTime(Entity<RMCUnconsciousComponent>.op_Implicit(ent), "Stun", out var statusTime) || statusTime.Value.Item2 < time)
		{
			((EntitySystem)this).RemCompDeferred<StunnedComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		}
		if (!_status.TryGetTime(Entity<RMCUnconsciousComponent>.op_Implicit(ent), "KnockedDown", out statusTime) || statusTime.Value.Item2 < time)
		{
			((EntitySystem)this).RemCompDeferred<KnockedDownComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		}
		if (!_status.TryGetTime(Entity<RMCUnconsciousComponent>.op_Implicit(ent), "TemporaryBlindness", out statusTime) || statusTime.Value.Item2 < time)
		{
			((EntitySystem)this).RemCompDeferred<TemporaryBlindnessComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		}
		if (!_status.TryGetTime(Entity<RMCUnconsciousComponent>.op_Implicit(ent), "Muted", out statusTime) || statusTime.Value.Item2 < time)
		{
			((EntitySystem)this).RemCompDeferred<MutedComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		}
		if (!_status.TryGetTime(Entity<RMCUnconsciousComponent>.op_Implicit(ent), "Deaf", out statusTime) || statusTime.Value.Item2 < time)
		{
			((EntitySystem)this).RemCompDeferred<DeafComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		}
	}

	private void OnUnconsciousUpdate(Entity<RMCUnconsciousComponent> ent, ref StatusEffectEndedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (IsKnockedOut(Entity<RMCUnconsciousComponent>.op_Implicit(ent)))
		{
			((EntitySystem)this).EnsureComp<StunnedComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
			((EntitySystem)this).EnsureComp<KnockedDownComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
			((EntitySystem)this).EnsureComp<TemporaryBlindnessComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
			((EntitySystem)this).EnsureComp<MutedComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
			((EntitySystem)this).EnsureComp<DeafComponent>(Entity<RMCUnconsciousComponent>.op_Implicit(ent));
		}
	}

	private void OnUnconsciousPointAttempt(Entity<RMCUnconsciousComponent> ent, ref PointAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (IsKnockedOut(Entity<RMCUnconsciousComponent>.op_Implicit(ent)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnKnockOutCollideProjectileHit(Entity<RMCKnockOutOnCollideComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TryKnockOut(args.Target, ent.Comp.ParalyzeTime);
	}

	private void OnKnockOutCollideThrowHit(Entity<RMCKnockOutOnCollideComponent> ent, ref ThrowDoHitEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TryKnockOut(args.Target, ent.Comp.ParalyzeTime);
	}

	public bool IsKnockedOut(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _status.HasStatusEffect(uid, ProtoId<StatusEffectPrototype>.op_Implicit(KnockedOut));
	}
}
