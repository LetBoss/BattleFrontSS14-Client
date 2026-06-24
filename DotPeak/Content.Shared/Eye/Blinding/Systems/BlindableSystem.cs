// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.BlindableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Camera;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class BlindableSystem : EntitySystem
{
  [Dependency]
  private BlurryVisionSystem _blurriness;
  [Dependency]
  private EyeClosingSystem _eyelids;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<BlindableComponent, RejuvenateEvent>(new EntityEventRefHandler<BlindableComponent, RejuvenateEvent>(this.OnRejuvenate));
    this.SubscribeLocalEvent<BlindableComponent, EyeDamageChangedEvent>(new EntityEventRefHandler<BlindableComponent, EyeDamageChangedEvent>(this.OnDamageChanged));
    this.SubscribeLocalEvent<BlindableComponent, GetEyePvsScaleAttemptEvent>(new EntityEventRefHandler<BlindableComponent, GetEyePvsScaleAttemptEvent>(this.OnGetEyePvsScaleAttemptEvent));
    this.SubscribeLocalEvent<BlindableComponent, GetEyeOffsetAttemptEvent>(new EntityEventRefHandler<BlindableComponent, GetEyeOffsetAttemptEvent>(this.OnGetEyeOffsetAttemptEvent));
  }

  private void OnRejuvenate(Entity<BlindableComponent> ent, ref RejuvenateEvent args)
  {
    this.AdjustEyeDamage((Entity<BlindableComponent>) (ent.Owner, ent.Comp), -ent.Comp.EyeDamage);
  }

  private void OnDamageChanged(Entity<BlindableComponent> ent, ref EyeDamageChangedEvent args)
  {
    this._blurriness.UpdateBlurMagnitude((Entity<BlindableComponent>) (ent.Owner, ent.Comp));
    this._eyelids.UpdateEyesClosable((Entity<BlindableComponent>) (ent.Owner, ent.Comp));
  }

  private void OnGetEyePvsScaleAttemptEvent(
    Entity<BlindableComponent> ent,
    ref GetEyePvsScaleAttemptEvent args)
  {
    if (!ent.Comp.IsBlind)
      return;
    args.Cancelled = true;
  }

  private void OnGetEyeOffsetAttemptEvent(
    Entity<BlindableComponent> ent,
    ref GetEyeOffsetAttemptEvent args)
  {
    if (!ent.Comp.IsBlind)
      return;
    args.Cancelled = true;
  }

  public void UpdateIsBlind(Entity<BlindableComponent?> blindable)
  {
    if (!this.Resolve<BlindableComponent>((EntityUid) blindable, ref blindable.Comp, false))
      return;
    int num1 = blindable.Comp.IsBlind ? 1 : 0;
    if (blindable.Comp.EyeDamage >= blindable.Comp.MaxDamage)
    {
      blindable.Comp.IsBlind = true;
    }
    else
    {
      CanSeeAttemptEvent args = new CanSeeAttemptEvent();
      this.RaiseLocalEvent<CanSeeAttemptEvent>(blindable.Owner, args);
      blindable.Comp.IsBlind = args.Blind;
    }
    int num2 = blindable.Comp.IsBlind ? 1 : 0;
    if (num1 == num2)
      return;
    BlindnessChangedEvent args1 = new BlindnessChangedEvent(blindable.Comp.IsBlind);
    this.RaiseLocalEvent<BlindnessChangedEvent>(blindable.Owner, ref args1);
    this.Dirty<BlindableComponent>(blindable);
  }

  public void AdjustEyeDamage(Entity<BlindableComponent?> blindable, int amount)
  {
    if (!this.Resolve<BlindableComponent>((EntityUid) blindable, ref blindable.Comp, false) || amount == 0)
      return;
    blindable.Comp.EyeDamage += amount;
    this.UpdateEyeDamage(blindable, true);
  }

  private void UpdateEyeDamage(Entity<BlindableComponent?> blindable, bool isDamageChanged)
  {
    if (!this.Resolve<BlindableComponent>((EntityUid) blindable, ref blindable.Comp, false))
      return;
    int eyeDamage = blindable.Comp.EyeDamage;
    blindable.Comp.EyeDamage = Math.Clamp(blindable.Comp.EyeDamage, blindable.Comp.MinDamage, blindable.Comp.MaxDamage);
    this.Dirty<BlindableComponent>(blindable);
    if (!isDamageChanged && eyeDamage == blindable.Comp.EyeDamage)
      return;
    this.UpdateIsBlind(blindable);
    EyeDamageChangedEvent args = new EyeDamageChangedEvent(blindable.Comp.EyeDamage);
    this.RaiseLocalEvent<EyeDamageChangedEvent>(blindable.Owner, ref args);
  }

  public void SetMinDamage(Entity<BlindableComponent?> blindable, int amount)
  {
    if (!this.Resolve<BlindableComponent>((EntityUid) blindable, ref blindable.Comp, false))
      return;
    blindable.Comp.MinDamage = amount;
    this.UpdateEyeDamage(blindable, false);
  }
}
