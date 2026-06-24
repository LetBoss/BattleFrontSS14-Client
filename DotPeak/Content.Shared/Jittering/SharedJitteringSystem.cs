// Decompiled with JetBrains decompiler
// Type: Content.Shared.Jittering.SharedJitteringSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Rejuvenate;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Jittering;

public abstract class SharedJitteringSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming GameTiming;
  [Dependency]
  protected StatusEffectsSystem StatusEffects;
  public float MaxAmplitude = 300f;
  public float MinAmplitude = 1f;
  public float MaxFrequency = 10f;
  public float MinFrequency = 1f;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<JitteringComponent, RejuvenateEvent>(new ComponentEventHandler<JitteringComponent, RejuvenateEvent>(this.OnRejuvenate));
  }

  private void OnRejuvenate(EntityUid uid, JitteringComponent component, RejuvenateEvent args)
  {
    this.RemCompDeferred<JitteringComponent>(uid);
  }

  public void DoJitter(
    EntityUid uid,
    TimeSpan time,
    bool refresh,
    float amplitude = 10f,
    float frequency = 4f,
    bool forceValueChange = false,
    StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false))
      return;
    amplitude = Math.Clamp(amplitude, this.MinAmplitude, this.MaxAmplitude);
    frequency = Math.Clamp(frequency, this.MinFrequency, this.MaxFrequency);
    if (!this.StatusEffects.TryAddStatusEffect<JitteringComponent>(uid, "Jitter", time, refresh, status))
      return;
    JitteringComponent jitteringComponent = this.Comp<JitteringComponent>(uid);
    if (forceValueChange || (double) jitteringComponent.Amplitude < (double) amplitude)
      jitteringComponent.Amplitude = amplitude;
    if (!forceValueChange && (double) jitteringComponent.Frequency >= (double) frequency)
      return;
    jitteringComponent.Frequency = frequency;
  }

  public void AddJitter(EntityUid uid, float amplitude = 10f, float frequency = 4f)
  {
    JitteringComponent jitteringComponent = this.EnsureComp<JitteringComponent>(uid);
    jitteringComponent.Amplitude = amplitude;
    jitteringComponent.Frequency = frequency;
    this.Dirty(uid, (IComponent) jitteringComponent);
  }
}
