// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunSpinupComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[Access(new Type[] {typeof (GunSpinupSystem)})]
public sealed class GunSpinupComponent : 
  Component,
  ISerializationGenerated<GunSpinupComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float BaseShotDelay = 0.7f;
  [DataField(null, false, 1, false, false, null)]
  public float BaseScatter = 18f;
  [DataField(null, false, 1, false, false, null)]
  public float SpinUpTime = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float GraceAfterStop = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float SpinDownTime = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float MinSpinLevel = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxSpinLevel = 11f;
  [DataField(null, false, 1, false, false, null)]
  public int[] RateTiers = new int[11]
  {
    1,
    1,
    2,
    2,
    3,
    3,
    3,
    4,
    4,
    4,
    5
  };
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? StartSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Vehicle/weapons/minigun_start.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? LoopSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/minigun.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? StopSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Vehicle/weapons/minigun_stop.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SelectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Vehicle/weapons/minigun_select.ogg");
  [DataField(null, false, 1, false, false, null)]
  public float LoopSoundCooldown = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float FireWindowPadding = 0.12f;
  [DataField(null, false, 1, false, false, null)]
  public float InitialWindupDelay;
  [DataField(null, false, 1, false, false, null)]
  public float InitialWindupResetGap = 0.2f;
  public TimeSpan LastUpdate;
  public TimeSpan? LastShotAt;
  public TimeSpan? LastAttemptAt;
  public TimeSpan? PendingWindupUntil;
  public TimeSpan? LastLoopSoundAt;
  public float CurrentSpinLevel = 1f;
  public float LastAppliedRate = -1f;
  public float LastAppliedScatter = -1f;
  public bool WasFiring;
  public bool StartSoundPlayed;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunSpinupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunSpinupComponent) target1;
    if (serialization.TryCustomCopy<GunSpinupComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseShotDelay, ref target2, hookCtx, false, context))
      target2 = this.BaseShotDelay;
    target.BaseShotDelay = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseScatter, ref target3, hookCtx, false, context))
      target3 = this.BaseScatter;
    target.BaseScatter = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpinUpTime, ref target4, hookCtx, false, context))
      target4 = this.SpinUpTime;
    target.SpinUpTime = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.GraceAfterStop, ref target5, hookCtx, false, context))
      target5 = this.GraceAfterStop;
    target.GraceAfterStop = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpinDownTime, ref target6, hookCtx, false, context))
      target6 = this.SpinDownTime;
    target.SpinDownTime = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinSpinLevel, ref target7, hookCtx, false, context))
      target7 = this.MinSpinLevel;
    target.MinSpinLevel = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxSpinLevel, ref target8, hookCtx, false, context))
      target8 = this.MaxSpinLevel;
    target.MaxSpinLevel = target8;
    int[] target9 = (int[]) null;
    if (this.RateTiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<int[]>(this.RateTiers, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<int[]>(this.RateTiers, hookCtx, context);
    target.RateTiers = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StartSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.StartSound, hookCtx, context);
    target.StartSound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LoopSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.LoopSound, hookCtx, context);
    target.LoopSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StopSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.StopSound, hookCtx, context);
    target.StopSound = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SelectSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.SelectSound, hookCtx, context);
    target.SelectSound = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LoopSoundCooldown, ref target14, hookCtx, false, context))
      target14 = this.LoopSoundCooldown;
    target.LoopSoundCooldown = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireWindowPadding, ref target15, hookCtx, false, context))
      target15 = this.FireWindowPadding;
    target.FireWindowPadding = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InitialWindupDelay, ref target16, hookCtx, false, context))
      target16 = this.InitialWindupDelay;
    target.InitialWindupDelay = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InitialWindupResetGap, ref target17, hookCtx, false, context))
      target17 = this.InitialWindupResetGap;
    target.InitialWindupResetGap = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunSpinupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GunSpinupComponent target1 = (GunSpinupComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GunSpinupComponent target1 = (GunSpinupComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GunSpinupComponent target1 = (GunSpinupComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GunSpinupComponent Component.Instantiate() => new GunSpinupComponent();
}
