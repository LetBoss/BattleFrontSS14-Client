// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Weapons.SharedCivSuppressionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG;
using Content.Shared._RMC14.Random;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
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
    base.Initialize();
    if (this._subscriptionsInitialized)
      return;
    this._subscriptionsInitialized = true;
    this.SubscribeLocalEvent<CivSuppressedComponent, SelfBeforeGunShotEvent>(new EntityEventRefHandler<CivSuppressedComponent, SelfBeforeGunShotEvent>(this.OnSelfBeforeGunShot));
    this.SubscribeLocalEvent<CivSuppressionPendingShotComponent, AmmoShotEvent>(new EntityEventRefHandler<CivSuppressionPendingShotComponent, AmmoShotEvent>(this.OnAmmoShot));
  }

  public float GetCurrentIntensity(EntityUid uid, CivSuppressedComponent? component = null)
  {
    return !this.Resolve<CivSuppressedComponent>(uid, ref component, false) ? 0.0f : this.GetCurrentIntensity(this.Timing.CurTime, component);
  }

  protected float GetCurrentIntensity(TimeSpan now, CivSuppressedComponent component)
  {
    if ((double) component.Intensity <= 0.0 || component.DecayTime <= TimeSpan.Zero)
      return 0.0f;
    TimeSpan timeSpan = now - component.LastAppliedAt;
    if (timeSpan < TimeSpan.Zero)
      timeSpan = TimeSpan.Zero;
    double totalSeconds = component.DecayTime.TotalSeconds;
    if (totalSeconds <= 0.0)
      return 0.0f;
    double num = 1.0 - timeSpan.TotalSeconds / totalSeconds;
    return Math.Clamp(component.Intensity * (float) num, 0.0f, 1f);
  }

  protected void ApplySuppression(
    EntityUid target,
    float amount,
    CivSuppressionEmitterComponent emitter,
    TimeSpan now,
    CivSuppressedComponent? suppressed = null)
  {
    if ((double) amount <= 0.0 || (double) emitter.MaxIntensity <= 0.0 || this.HasComp<PubgCharacterComponent>(target))
      return;
    if (suppressed == null)
      suppressed = this.EnsureComp<CivSuppressedComponent>(target);
    float currentIntensity = this.GetCurrentIntensity(now, suppressed);
    float num1 = (double) emitter.MaxIntensity <= 0.0 ? 0.0f : Math.Clamp(currentIntensity / emitter.MaxIntensity, 0.0f, 1f);
    float num2 = MathHelper.Lerp(Math.Clamp(emitter.FreshTargetSuppressionMultiplier, 0.0f, 1f), 1f, num1);
    float num3 = Math.Clamp(currentIntensity + amount * num2, 0.0f, emitter.MaxIntensity);
    suppressed.Intensity = num3;
    suppressed.VisualProfile = (double) currentIntensity > 0.0 ? (CivSuppressionVisualProfile) Math.Max((byte) suppressed.VisualProfile, (byte) emitter.VisualProfile) : emitter.VisualProfile;
    suppressed.LastAppliedAt = now;
    suppressed.DecayTime = emitter.DecayTime;
    suppressed.ShotPenaltyDegrees = (double) currentIntensity > 0.0 ? Math.Max(suppressed.ShotPenaltyDegrees, emitter.ShotPenaltyDegrees) : emitter.ShotPenaltyDegrees;
    suppressed.FreshTargetSuppressionMultiplier = (double) currentIntensity > 0.0 ? Math.Min(suppressed.FreshTargetSuppressionMultiplier, emitter.FreshTargetSuppressionMultiplier) : emitter.FreshTargetSuppressionMultiplier;
    suppressed.HighStressThreshold = (double) currentIntensity > 0.0 ? Math.Min(suppressed.HighStressThreshold, emitter.HighStressThreshold) : emitter.HighStressThreshold;
    suppressed.HighStressShotPenaltyMultiplier = (double) currentIntensity > 0.0 ? Math.Max(suppressed.HighStressShotPenaltyMultiplier, emitter.HighStressShotPenaltyMultiplier) : emitter.HighStressShotPenaltyMultiplier;
    suppressed.VisualShockMultiplier = (double) currentIntensity > 0.0 ? Math.Max(suppressed.VisualShockMultiplier, emitter.VisualShockMultiplier) : emitter.VisualShockMultiplier;
    suppressed.VisualSwayMultiplier = (double) currentIntensity > 0.0 ? Math.Max(suppressed.VisualSwayMultiplier, emitter.VisualSwayMultiplier) : emitter.VisualSwayMultiplier;
    suppressed.VisualRecoveryDelay = (double) currentIntensity > 0.0 ? TimeSpan.FromSeconds(Math.Max(suppressed.VisualRecoveryDelay.TotalSeconds, emitter.VisualRecoveryDelay.TotalSeconds)) : emitter.VisualRecoveryDelay;
    suppressed.VisualRingThreshold = (double) currentIntensity > 0.0 ? Math.Min(suppressed.VisualRingThreshold, emitter.VisualRingThreshold) : emitter.VisualRingThreshold;
    suppressed.VisualRingVolume = (double) currentIntensity > 0.0 ? Math.Max(suppressed.VisualRingVolume, emitter.VisualRingVolume) : emitter.VisualRingVolume;
    suppressed.VisualRingCooldown = (double) currentIntensity > 0.0 ? TimeSpan.FromSeconds(Math.Min(suppressed.VisualRingCooldown.TotalSeconds, emitter.VisualRingCooldown.TotalSeconds)) : emitter.VisualRingCooldown;
    suppressed.NextNearMissAt = now + emitter.NearMissCooldown;
    this.Dirty(target, (IComponent) suppressed);
  }

  private void OnSelfBeforeGunShot(
    Entity<CivSuppressedComponent> ent,
    ref SelfBeforeGunShotEvent args)
  {
    float currentIntensity = this.GetCurrentIntensity(ent.Owner, ent.Comp);
    if ((double) currentIntensity <= 0.0 || (double) ent.Comp.ShotPenaltyDegrees <= 0.0)
    {
      if (!this.HasComp<CivSuppressionPendingShotComponent>((EntityUid) args.Gun))
        return;
      this.RemComp<CivSuppressionPendingShotComponent>((EntityUid) args.Gun);
    }
    else
    {
      CivSuppressionPendingShotComponent pendingShotComponent = this.EnsureComp<CivSuppressionPendingShotComponent>((EntityUid) args.Gun);
      pendingShotComponent.Shooter = new EntityUid?(args.Shooter);
      pendingShotComponent.Intensity = currentIntensity;
      pendingShotComponent.ShotPenaltyDegrees = ent.Comp.ShotPenaltyDegrees;
      pendingShotComponent.HighStressThreshold = ent.Comp.HighStressThreshold;
      pendingShotComponent.HighStressShotPenaltyMultiplier = ent.Comp.HighStressShotPenaltyMultiplier;
    }
  }

  private void OnAmmoShot(Entity<CivSuppressionPendingShotComponent> ent, ref AmmoShotEvent args)
  {
    if ((double) ent.Comp.Intensity <= 0.0 || (double) ent.Comp.ShotPenaltyDegrees <= 0.0)
    {
      this.RemCompDeferred<CivSuppressionPendingShotComponent>(ent.Owner);
    }
    else
    {
      long seed = (long) this.Timing.CurTick.Value << 32 /*0x20*/ | (long) (uint) this.GetNetEntity(ent.Owner).Id;
      EntityUid? shooter = ent.Comp.Shooter;
      if (shooter.HasValue)
      {
        EntityUid valueOrDefault = shooter.GetValueOrDefault();
        seed ^= (long) (uint) this.GetNetEntity(valueOrDefault).Id;
      }
      Xoroshiro64S xoroshiro64S = new Xoroshiro64S(seed);
      foreach (EntityUid firedProjectile in args.FiredProjectiles)
      {
        PhysicsComponent comp1;
        if (this.TryComp<PhysicsComponent>(firedProjectile, out comp1))
        {
          Vector2 linearVelocity = comp1.LinearVelocity;
          if ((double) linearVelocity.LengthSquared() > 9.9999997473787516E-05)
          {
            float num1 = Math.Clamp(ent.Comp.HighStressThreshold, 0.0f, 0.98f);
            float num2 = Math.Max(ent.Comp.HighStressShotPenaltyMultiplier, 1f);
            float x = (double) ent.Comp.Intensity <= (double) num1 ? 0.0f : (float) (((double) ent.Comp.Intensity - (double) num1) / (1.0 - (double) num1));
            float num3 = (float) (1.0 + (double) x * (double) x * ((double) num2 - 1.0));
            float num4 = MathHelper.Lerp(0.18f, 1f, MathF.Sqrt(x));
            float max = MathHelper.Lerp(0.18f, 0.92f, x);
            Angle angle = Angle.FromDegrees((double) ent.Comp.ShotPenaltyDegrees * (double) ent.Comp.Intensity * (double) num4 * (double) num3 * (double) xoroshiro64S.NextFloat(-max, max));
            Vector2 velocity = ((Angle) ref angle).RotateVec(ref linearVelocity);
            this.Physics.SetLinearVelocity(firedProjectile, velocity, body: comp1);
            ProjectileComponent comp2;
            if (this.TryComp<ProjectileComponent>(firedProjectile, out comp2))
            {
              comp2.Angle = DirectionExtensions.ToWorldAngle(velocity);
              this.Dirty(firedProjectile, (IComponent) comp2);
            }
          }
        }
      }
      this.RemCompDeferred<CivSuppressionPendingShotComponent>(ent.Owner);
    }
  }
}
