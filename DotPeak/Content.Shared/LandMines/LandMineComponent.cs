// Decompiled with JetBrains decompiler
// Type: Content.Shared.LandMines.LandMineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.LandMines;

[RegisterComponent]
public sealed class LandMineComponent : 
  Component,
  ISerializationGenerated<LandMineComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId? TriggerText = (LocId?) "land-mine-triggered";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan HandDisarmDelay = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ShovelDisarmDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? DisarmedItemSpawn;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LandMineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LandMineComponent) target1;
    if (serialization.TryCustomCopy<LandMineComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? target2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.TriggerText, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId?>(this.TriggerText, hookCtx, context);
    target.TriggerText = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HandDisarmDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.HandDisarmDelay, hookCtx, context);
    target.HandDisarmDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShovelDisarmDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ShovelDisarmDelay, hookCtx, context);
    target.ShovelDisarmDelay = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.DisarmedItemSpawn, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.DisarmedItemSpawn, hookCtx, context);
    target.DisarmedItemSpawn = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LandMineComponent target,
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
    LandMineComponent target1 = (LandMineComponent) target;
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
    LandMineComponent target1 = (LandMineComponent) target;
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
    LandMineComponent target1 = (LandMineComponent) target;
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
  virtual LandMineComponent Component.Instantiate() => new LandMineComponent();
}
