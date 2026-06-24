// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.RespawnTowers.PubgRespawnTowerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.RespawnTowers;

[RegisterComponent]
public sealed class PubgRespawnTowerComponent : 
  Component,
  ISerializationGenerated<PubgRespawnTowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float ActivationDelay = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float ActivationRadius = 3f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 ReviveHp = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ActivationSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgRespawnTowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgRespawnTowerComponent) target1;
    if (serialization.TryCustomCopy<PubgRespawnTowerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ActivationDelay, ref target2, hookCtx, false, context))
      target2 = this.ActivationDelay;
    target.ActivationDelay = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ActivationRadius, ref target3, hookCtx, false, context))
      target3 = this.ActivationRadius;
    target.ActivationRadius = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReviveHp, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.ReviveHp, hookCtx, context);
    target.ReviveHp = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ActivationSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.ActivationSound, hookCtx, context);
    target.ActivationSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgRespawnTowerComponent target,
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
    PubgRespawnTowerComponent target1 = (PubgRespawnTowerComponent) target;
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
    PubgRespawnTowerComponent target1 = (PubgRespawnTowerComponent) target;
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
    PubgRespawnTowerComponent target1 = (PubgRespawnTowerComponent) target;
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
  virtual PubgRespawnTowerComponent Component.Instantiate() => new PubgRespawnTowerComponent();
}
