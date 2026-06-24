// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.AreaReactionEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class AreaReactionEffect : 
  EventEntityEffect<AreaReactionEffect>,
  ISerializationGenerated<AreaReactionEffect>,
  ISerializationGenerated
{
  [DataField("duration", false, 1, false, false, null)]
  public float Duration = 10f;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 OverflowThreshold = FixedPoint2.New(2.5);
  [DataField("prototypeId", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string PrototypeId;
  [DataField("sound", false, 1, true, false, null)]
  public SoundSpecifier Sound;

  public override bool ShouldLog => true;

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-area-reaction", ("duration", (object) this.Duration));
  }

  public override LogImpact LogImpact => LogImpact.High;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AreaReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<AreaReactionEffect> target1 = (EventEntityEffect<AreaReactionEffect>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AreaReactionEffect) target1;
    if (serialization.TryCustomCopy<AreaReactionEffect>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Duration, ref target2, hookCtx, false, context))
      target2 = this.Duration;
    target.Duration = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.OverflowThreshold, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.OverflowThreshold, hookCtx, context);
    target.OverflowThreshold = target3;
    string target4 = (string) null;
    if (this.PrototypeId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PrototypeId, ref target4, hookCtx, false, context))
      target4 = this.PrototypeId;
    target.PrototypeId = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AreaReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<AreaReactionEffect> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AreaReactionEffect target1 = (AreaReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<AreaReactionEffect>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AreaReactionEffect target1 = (AreaReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AreaReactionEffect EventEntityEffect<AreaReactionEffect>.Instantiate()
  {
    return new AreaReactionEffect();
  }
}
