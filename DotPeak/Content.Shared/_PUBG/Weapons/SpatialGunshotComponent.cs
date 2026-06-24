// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Weapons.SpatialGunshotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Weapons;

[RegisterComponent]
[NetworkedComponent]
public sealed class SpatialGunshotComponent : 
  Component,
  ISerializationGenerated<SpatialGunshotComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? FarSound;
  [DataField(null, false, 1, false, false, null)]
  public float AudioRange = 50f;
  [DataField(null, false, 1, false, false, null)]
  public float NearAudioRange = 45f;
  [DataField(null, false, 1, false, false, null)]
  public float ConeAngle = 90f;
  [DataField(null, false, 1, false, false, null)]
  public float NearRange = 20f;
  [DataField(null, false, 1, false, false, null)]
  public float NearVolume = 6f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpatialGunshotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpatialGunshotComponent) target1;
    if (serialization.TryCustomCopy<SpatialGunshotComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FarSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.FarSound, hookCtx, context);
    target.FarSound = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AudioRange, ref target3, hookCtx, false, context))
      target3 = this.AudioRange;
    target.AudioRange = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearAudioRange, ref target4, hookCtx, false, context))
      target4 = this.NearAudioRange;
    target.NearAudioRange = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ConeAngle, ref target5, hookCtx, false, context))
      target5 = this.ConeAngle;
    target.ConeAngle = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearRange, ref target6, hookCtx, false, context))
      target6 = this.NearRange;
    target.NearRange = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NearVolume, ref target7, hookCtx, false, context))
      target7 = this.NearVolume;
    target.NearVolume = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpatialGunshotComponent target,
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
    SpatialGunshotComponent target1 = (SpatialGunshotComponent) target;
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
    SpatialGunshotComponent target1 = (SpatialGunshotComponent) target;
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
    SpatialGunshotComponent target1 = (SpatialGunshotComponent) target;
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
  virtual SpatialGunshotComponent Component.Instantiate() => new SpatialGunshotComponent();
}
