// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunSpinupSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunSpinupSystem : EntitySystem
{
  private const float ClientWindupSafetyPadding = 0.05f;
  private const float ModifierRefreshEpsilon = 0.01f;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunSpinupComponent, ComponentStartup>(new EntityEventRefHandler<GunSpinupComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<GunSpinupComponent, AttemptShootEvent>(new EntityEventRefHandler<GunSpinupComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<GunSpinupComponent, GunShotEvent>(new EntityEventRefHandler<GunSpinupComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<GunSpinupComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunSpinupComponent, GunRefreshModifiersEvent>(this.OnRefreshModifiers));
  }

  private void OnStartup(Entity<GunSpinupComponent> ent, ref ComponentStartup args)
  {
    ent.Comp.LastUpdate = this._timing.CurTime;
    ent.Comp.CurrentSpinLevel = ent.Comp.MinSpinLevel;
    ent.Comp.LastAppliedRate = -1f;
    ent.Comp.LastAppliedScatter = -1f;
    ent.Comp.WasFiring = false;
    ent.Comp.StartSoundPlayed = false;
    ent.Comp.LastAttemptAt = new TimeSpan?();
    ent.Comp.PendingWindupUntil = new TimeSpan?();
    ent.Comp.LastLoopSoundAt = new TimeSpan?();
    GunComponent comp;
    if (!this.TryComp<GunComponent>((EntityUid) ent, out comp))
      return;
    this._gun.RefreshModifiers((Entity<GunComponent>) ((EntityUid) ent, comp));
  }

  private void OnAttemptShoot(Entity<GunSpinupComponent> ent, ref AttemptShootEvent args)
  {
    if (this._net.IsClient && !this._timing.IsFirstTimePredicted || args.Cancelled || (double) ent.Comp.InitialWindupDelay <= 0.0)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (GunSpinupSystem.IsSpinActive(ent.Comp, curTime))
      return;
    TimeSpan? nullable;
    if (!ent.Comp.PendingWindupUntil.HasValue)
    {
      nullable = ent.Comp.LastAttemptAt;
      if (nullable.HasValue)
      {
        TimeSpan valueOrDefault = nullable.GetValueOrDefault();
        if ((curTime - valueOrDefault).TotalSeconds > (double) ent.Comp.InitialWindupResetGap)
        {
          ent.Comp.PendingWindupUntil = new TimeSpan?();
          ent.Comp.StartSoundPlayed = false;
        }
      }
    }
    ent.Comp.LastAttemptAt = new TimeSpan?(curTime);
    nullable = ent.Comp.PendingWindupUntil;
    TimeSpan timeSpan1;
    if (nullable.HasValue)
    {
      timeSpan1 = nullable.GetValueOrDefault();
    }
    else
    {
      timeSpan1 = curTime + TimeSpan.FromSeconds((double) ent.Comp.InitialWindupDelay);
      ent.Comp.PendingWindupUntil = new TimeSpan?(timeSpan1);
      if (!ent.Comp.StartSoundPlayed && ent.Comp.StartSound != null)
      {
        this._audio.PlayPredicted(ent.Comp.StartSound, (EntityUid) ent, new EntityUid?(args.User));
        ent.Comp.StartSoundPlayed = true;
      }
    }
    TimeSpan timeSpan2 = timeSpan1 + (this._net.IsClient ? TimeSpan.FromSeconds(0.05000000074505806) : TimeSpan.Zero);
    if (curTime >= timeSpan2)
    {
      ent.Comp.PendingWindupUntil = new TimeSpan?();
    }
    else
    {
      args.Cancelled = true;
      args.ResetCooldown = true;
    }
  }

  private void OnGunShot(Entity<GunSpinupComponent> ent, ref GunShotEvent args)
  {
    TimeSpan curTime = this._timing.CurTime;
    int num = GunSpinupSystem.IsSpinActive(ent.Comp, curTime) ? 1 : 0;
    ent.Comp.LastShotAt = new TimeSpan?(curTime);
    ent.Comp.PendingWindupUntil = new TimeSpan?();
    if (num == 0 && ent.Comp.StartSound != null && !ent.Comp.StartSoundPlayed)
      this._audio.PlayPredicted(ent.Comp.StartSound, (EntityUid) ent, new EntityUid?(args.User));
    if (ent.Comp.LoopSound != null && (!ent.Comp.LastLoopSoundAt.HasValue || (curTime - ent.Comp.LastLoopSoundAt.Value).TotalSeconds >= (double) MathF.Max(ent.Comp.LoopSoundCooldown, 0.0f)))
    {
      this._audio.PlayPredicted(ent.Comp.LoopSound, (EntityUid) ent, new EntityUid?(args.User));
      ent.Comp.LastLoopSoundAt = new TimeSpan?(curTime);
    }
    ent.Comp.StartSoundPlayed = true;
  }

  private void OnRefreshModifiers(Entity<GunSpinupComponent> ent, ref GunRefreshModifiersEvent args)
  {
    float rateMultiplier = GunSpinupSystem.GetRateMultiplier(ent.Comp);
    float num = rateMultiplier / MathF.Max(ent.Comp.BaseShotDelay, 0.01f);
    Angle angle = Angle.FromDegrees((double) MathF.Max(ent.Comp.BaseScatter / MathF.Max(rateMultiplier, 1f), 0.0f));
    args.FireRate = num;
    args.MinAngle = angle;
    args.MaxAngle = angle;
    args.AngleIncrease = Angle.Zero;
    args.AngleDecay = Angle.Zero;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<GunSpinupComponent, GunComponent> entityQueryEnumerator = this.EntityQueryEnumerator<GunSpinupComponent, GunComponent>();
    EntityUid uid;
    GunSpinupComponent comp1;
    GunComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      float totalSeconds = (float) (curTime - comp1.LastUpdate).TotalSeconds;
      if ((double) totalSeconds > 0.0)
      {
        comp1.LastUpdate = curTime;
        bool flag1 = GunSpinupSystem.IsFiring(comp1, curTime);
        bool flag2 = GunSpinupSystem.IsSpinActive(comp1, curTime);
        if (comp1.WasFiring && !flag2 && comp1.StopSound != null)
          this._audio.PlayPvs(comp1.StopSound, uid);
        if (comp1.WasFiring && !flag2)
          comp1.StartSoundPlayed = false;
        comp1.WasFiring = flag2;
        float num1 = MathF.Max(comp1.MaxSpinLevel - comp1.MinSpinLevel, 0.0f);
        if ((double) num1 > 0.0)
        {
          float num2 = Math.Clamp(comp1.CurrentSpinLevel, comp1.MinSpinLevel, comp1.MaxSpinLevel);
          if (flag1)
          {
            float num3 = num1 / MathF.Max(comp1.SpinUpTime, 0.01f);
            num2 += num3 * totalSeconds;
          }
          else if (!flag2)
          {
            float num4 = num1 / MathF.Max(comp1.SpinDownTime, 0.01f);
            num2 -= num4 * totalSeconds;
          }
          float level = Math.Clamp(num2, comp1.MinSpinLevel, comp1.MaxSpinLevel);
          comp1.CurrentSpinLevel = level;
          float rateMultiplier = GunSpinupSystem.GetRateMultiplier(comp1, level);
          float num5 = comp1.BaseScatter / MathF.Max(rateMultiplier, 1f);
          if ((double) Math.Abs(rateMultiplier - comp1.LastAppliedRate) >= 0.0099999997764825821 || (double) Math.Abs(num5 - comp1.LastAppliedScatter) >= 0.0099999997764825821)
          {
            comp1.LastAppliedRate = rateMultiplier;
            comp1.LastAppliedScatter = num5;
            this._gun.RefreshModifiers((Entity<GunComponent>) (uid, comp2));
          }
        }
      }
    }
  }

  private static bool IsFiring(GunSpinupComponent comp, TimeSpan now)
  {
    TimeSpan? lastShotAt = comp.LastShotAt;
    if (!lastShotAt.HasValue)
      return false;
    TimeSpan valueOrDefault = lastShotAt.GetValueOrDefault();
    float rateMultiplier = GunSpinupSystem.GetRateMultiplier(comp, comp.CurrentSpinLevel);
    float num = comp.BaseShotDelay / MathF.Max(rateMultiplier, 1f) + MathF.Max(comp.FireWindowPadding, 0.0f);
    return (now - valueOrDefault).TotalSeconds <= (double) num;
  }

  private static bool IsSpinActive(GunSpinupComponent comp, TimeSpan now)
  {
    TimeSpan? lastShotAt = comp.LastShotAt;
    if (!lastShotAt.HasValue)
      return false;
    TimeSpan valueOrDefault = lastShotAt.GetValueOrDefault();
    return (now - valueOrDefault).TotalSeconds <= (double) MathF.Max(comp.GraceAfterStop, 0.0f);
  }

  private static float GetRateMultiplier(GunSpinupComponent comp)
  {
    return GunSpinupSystem.GetRateMultiplier(comp, comp.CurrentSpinLevel);
  }

  private static float GetRateMultiplier(GunSpinupComponent comp, float level)
  {
    if (comp.RateTiers.Length == 0)
      return 1f;
    if (comp.RateTiers.Length == 1)
      return MathF.Max((float) comp.RateTiers[0], 1f);
    double x = (double) Math.Clamp(level, 1f, (float) comp.RateTiers.Length);
    int index1 = Math.Clamp((int) MathF.Floor((float) x) - 1, 0, comp.RateTiers.Length - 1);
    int index2 = Math.Min(index1 + 1, comp.RateTiers.Length - 1);
    float num1 = (float) x - (float) (index1 + 1);
    float num2 = MathF.Max((float) comp.RateTiers[index1], 1f);
    float num3 = MathF.Max((float) comp.RateTiers[index2], 1f);
    return num2 + (num3 - num2) * num1;
  }
}
