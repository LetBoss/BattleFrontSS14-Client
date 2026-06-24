// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.FlashReactionEffect
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
namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class FlashReactionEffect : 
  EventEntityEffect<FlashReactionEffect>,
  ISerializationGenerated<FlashReactionEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RangePerUnit = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxRange = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float SlowTo = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? FlashEffectPrototype = (EntProtoId?) "ReactionFlash";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/flash.ogg");

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-flash-reaction-effect", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlashReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<FlashReactionEffect> target1 = (EventEntityEffect<FlashReactionEffect>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FlashReactionEffect) target1;
    if (serialization.TryCustomCopy<FlashReactionEffect>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangePerUnit, ref target2, hookCtx, false, context))
      target2 = this.RangePerUnit;
    target.RangePerUnit = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRange, ref target3, hookCtx, false, context))
      target3 = this.MaxRange;
    target.MaxRange = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowTo, ref target4, hookCtx, false, context))
      target4 = this.SlowTo;
    target.SlowTo = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.FlashEffectPrototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.FlashEffectPrototype, hookCtx, context);
    target.FlashEffectPrototype = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlashReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<FlashReactionEffect> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlashReactionEffect target1 = (FlashReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<FlashReactionEffect>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlashReactionEffect target1 = (FlashReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FlashReactionEffect EventEntityEffect<FlashReactionEffect>.Instantiate()
  {
    return new FlashReactionEffect();
  }
}
