// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Entrenching.CivBlindageShovelComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Entrenching;

[RegisterComponent]
public sealed class CivBlindageShovelComponent : 
  Component,
  ISerializationGenerated<CivBlindageShovelComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float DigDelay = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairDelay = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("CIVBlindageShovel", new AudioParams?(AudioParams.Default.WithVolume(2f)));
  [DataField(null, false, 1, false, false, null)]
  public float SoundInterval = 0.45f;
  [DataField(null, false, 1, false, false, null)]
  public float MinPitch = 0.95f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxPitch = 1.08f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivBlindageShovelComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivBlindageShovelComponent) target1;
    if (serialization.TryCustomCopy<CivBlindageShovelComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DigDelay, ref target2, hookCtx, false, context))
      target2 = this.DigDelay;
    target.DigDelay = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairDelay, ref target3, hookCtx, false, context))
      target3 = this.RepairDelay;
    target.RepairDelay = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SoundInterval, ref target5, hookCtx, false, context))
      target5 = this.SoundInterval;
    target.SoundInterval = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinPitch, ref target6, hookCtx, false, context))
      target6 = this.MinPitch;
    target.MinPitch = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxPitch, ref target7, hookCtx, false, context))
      target7 = this.MaxPitch;
    target.MaxPitch = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivBlindageShovelComponent target,
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
    CivBlindageShovelComponent target1 = (CivBlindageShovelComponent) target;
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
    CivBlindageShovelComponent target1 = (CivBlindageShovelComponent) target;
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
    CivBlindageShovelComponent target1 = (CivBlindageShovelComponent) target;
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
  virtual CivBlindageShovelComponent Component.Instantiate() => new CivBlindageShovelComponent();
}
