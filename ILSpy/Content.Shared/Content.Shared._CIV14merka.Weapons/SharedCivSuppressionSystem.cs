using System;
using System.Numerics;
using Content.Shared._PUBG;
using Content.Shared._RMC14.Random;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._CIV14merka.Weapons;

public abstract class SharedCivSuppressionSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected SharedPhysicsSystem Physics;

	private bool _subscriptionsInitialized;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		if (!_subscriptionsInitialized)
		{
			_subscriptionsInitialized = true;
			((EntitySystem)this).SubscribeLocalEvent<CivSuppressedComponent, SelfBeforeGunShotEvent>((EntityEventRefHandler<CivSuppressedComponent, SelfBeforeGunShotEvent>)OnSelfBeforeGunShot, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeLocalEvent<CivSuppressionPendingShotComponent, AmmoShotEvent>((EntityEventRefHandler<CivSuppressionPendingShotComponent, AmmoShotEvent>)OnAmmoShot, (Type[])null, (Type[])null);
		}
	}

	public float GetCurrentIntensity(EntityUid uid, CivSuppressedComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CivSuppressedComponent>(uid, ref component, false))
		{
			return 0f;
		}
		return GetCurrentIntensity(Timing.CurTime, component);
	}

	protected float GetCurrentIntensity(TimeSpan now, CivSuppressedComponent component)
	{
		if (component.Intensity <= 0f || component.DecayTime <= TimeSpan.Zero)
		{
			return 0f;
		}
		TimeSpan elapsed = now - component.LastAppliedAt;
		if (elapsed < TimeSpan.Zero)
		{
			elapsed = TimeSpan.Zero;
		}
		double decay = component.DecayTime.TotalSeconds;
		if (decay <= 0.0)
		{
			return 0f;
		}
		double remaining = 1.0 - elapsed.TotalSeconds / decay;
		return Math.Clamp((float)((double)component.Intensity * remaining), 0f, 1f);
	}

	protected void ApplySuppression(EntityUid target, float amount, CivSuppressionEmitterComponent emitter, TimeSpan now, CivSuppressedComponent? suppressed = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		if (!(amount <= 0f) && !(emitter.MaxIntensity <= 0f) && !((EntitySystem)this).HasComp<PubgCharacterComponent>(target))
		{
			if (suppressed == null)
			{
				suppressed = ((EntitySystem)this).EnsureComp<CivSuppressedComponent>(target);
			}
			float current = GetCurrentIntensity(now, suppressed);
			float currentFraction = ((emitter.MaxIntensity <= 0f) ? 0f : Math.Clamp(current / emitter.MaxIntensity, 0f, 1f));
			float gainScale = MathHelper.Lerp(Math.Clamp(emitter.FreshTargetSuppressionMultiplier, 0f, 1f), 1f, currentFraction);
			float next = Math.Clamp(current + amount * gainScale, 0f, emitter.MaxIntensity);
			suppressed.Intensity = next;
			suppressed.VisualProfile = ((current > 0f) ? ((CivSuppressionVisualProfile)Math.Max((byte)suppressed.VisualProfile, (byte)emitter.VisualProfile)) : emitter.VisualProfile);
			suppressed.LastAppliedAt = now;
			suppressed.DecayTime = emitter.DecayTime;
			suppressed.ShotPenaltyDegrees = ((current > 0f) ? Math.Max(suppressed.ShotPenaltyDegrees, emitter.ShotPenaltyDegrees) : emitter.ShotPenaltyDegrees);
			suppressed.FreshTargetSuppressionMultiplier = ((current > 0f) ? Math.Min(suppressed.FreshTargetSuppressionMultiplier, emitter.FreshTargetSuppressionMultiplier) : emitter.FreshTargetSuppressionMultiplier);
			suppressed.HighStressThreshold = ((current > 0f) ? Math.Min(suppressed.HighStressThreshold, emitter.HighStressThreshold) : emitter.HighStressThreshold);
			suppressed.HighStressShotPenaltyMultiplier = ((current > 0f) ? Math.Max(suppressed.HighStressShotPenaltyMultiplier, emitter.HighStressShotPenaltyMultiplier) : emitter.HighStressShotPenaltyMultiplier);
			suppressed.VisualShockMultiplier = ((current > 0f) ? Math.Max(suppressed.VisualShockMultiplier, emitter.VisualShockMultiplier) : emitter.VisualShockMultiplier);
			suppressed.VisualSwayMultiplier = ((current > 0f) ? Math.Max(suppressed.VisualSwayMultiplier, emitter.VisualSwayMultiplier) : emitter.VisualSwayMultiplier);
			suppressed.VisualRecoveryDelay = ((current > 0f) ? TimeSpan.FromSeconds(Math.Max(suppressed.VisualRecoveryDelay.TotalSeconds, emitter.VisualRecoveryDelay.TotalSeconds)) : emitter.VisualRecoveryDelay);
			suppressed.VisualRingThreshold = ((current > 0f) ? Math.Min(suppressed.VisualRingThreshold, emitter.VisualRingThreshold) : emitter.VisualRingThreshold);
			suppressed.VisualRingVolume = ((current > 0f) ? Math.Max(suppressed.VisualRingVolume, emitter.VisualRingVolume) : emitter.VisualRingVolume);
			suppressed.VisualRingCooldown = ((current > 0f) ? TimeSpan.FromSeconds(Math.Min(suppressed.VisualRingCooldown.TotalSeconds, emitter.VisualRingCooldown.TotalSeconds)) : emitter.VisualRingCooldown);
			suppressed.NextNearMissAt = now + emitter.NearMissCooldown;
			((EntitySystem)this).Dirty(target, (IComponent)(object)suppressed, (MetaDataComponent)null);
		}
	}

	private void OnSelfBeforeGunShot(Entity<CivSuppressedComponent> ent, ref SelfBeforeGunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		float intensity = GetCurrentIntensity(ent.Owner, ent.Comp);
		if (intensity <= 0f || ent.Comp.ShotPenaltyDegrees <= 0f)
		{
			if (((EntitySystem)this).HasComp<CivSuppressionPendingShotComponent>(Entity<GunComponent>.op_Implicit(args.Gun)))
			{
				((EntitySystem)this).RemComp<CivSuppressionPendingShotComponent>(Entity<GunComponent>.op_Implicit(args.Gun));
			}
			return;
		}
		CivSuppressionPendingShotComponent civSuppressionPendingShotComponent = ((EntitySystem)this).EnsureComp<CivSuppressionPendingShotComponent>(Entity<GunComponent>.op_Implicit(args.Gun));
		civSuppressionPendingShotComponent.Shooter = args.Shooter;
		civSuppressionPendingShotComponent.Intensity = intensity;
		civSuppressionPendingShotComponent.ShotPenaltyDegrees = ent.Comp.ShotPenaltyDegrees;
		civSuppressionPendingShotComponent.HighStressThreshold = ent.Comp.HighStressThreshold;
		civSuppressionPendingShotComponent.HighStressShotPenaltyMultiplier = ent.Comp.HighStressShotPenaltyMultiplier;
	}

	private void OnAmmoShot(Entity<CivSuppressionPendingShotComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Intensity <= 0f || ent.Comp.ShotPenaltyDegrees <= 0f)
		{
			((EntitySystem)this).RemCompDeferred<CivSuppressionPendingShotComponent>(ent.Owner);
			return;
		}
		long seed = Timing.CurTick.Value;
		seed <<= 32;
		seed |= (uint)((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null).Id;
		EntityUid? shooter = ent.Comp.Shooter;
		if (shooter.HasValue)
		{
			EntityUid shooter2 = shooter.GetValueOrDefault();
			seed ^= (uint)((EntitySystem)this).GetNetEntity(shooter2, (MetaDataComponent)null).Id;
		}
		Xoroshiro64S random = new Xoroshiro64S(seed);
		PhysicsComponent physics = default(PhysicsComponent);
		ProjectileComponent projectileComponent = default(ProjectileComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (!((EntitySystem)this).TryComp<PhysicsComponent>(projectile, ref physics))
			{
				continue;
			}
			Vector2 velocity = physics.LinearVelocity;
			if (!(velocity.LengthSquared() <= 0.0001f))
			{
				float highStressThreshold = Math.Clamp(ent.Comp.HighStressThreshold, 0f, 0.98f);
				float highStressMultiplier = Math.Max(ent.Comp.HighStressShotPenaltyMultiplier, 1f);
				float highStress = ((ent.Comp.Intensity <= highStressThreshold) ? 0f : ((ent.Comp.Intensity - highStressThreshold) / (1f - highStressThreshold)));
				float penaltyScale = 1f + highStress * highStress * (highStressMultiplier - 1f);
				float basePenaltyFraction = MathHelper.Lerp(0.18f, 1f, MathF.Sqrt(highStress));
				float spreadRange = MathHelper.Lerp(0.18f, 0.92f, highStress);
				Angle spread = Angle.FromDegrees((double)(ent.Comp.ShotPenaltyDegrees * ent.Comp.Intensity * basePenaltyFraction * penaltyScale * random.NextFloat(0f - spreadRange, spreadRange)));
				Vector2 adjustedVelocity = ((Angle)(ref spread)).RotateVec(ref velocity);
				Physics.SetLinearVelocity(projectile, adjustedVelocity, true, true, (FixturesComponent)null, physics);
				if (((EntitySystem)this).TryComp<ProjectileComponent>(projectile, ref projectileComponent))
				{
					projectileComponent.Angle = DirectionExtensions.ToWorldAngle(adjustedVelocity);
					((EntitySystem)this).Dirty(projectile, (IComponent)(object)projectileComponent, (MetaDataComponent)null);
				}
			}
		}
		((EntitySystem)this).RemCompDeferred<CivSuppressionPendingShotComponent>(ent.Owner);
	}
}
