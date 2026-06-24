// Decompiled with JetBrains decompiler
// Type: Content.Shared.Slippery.SlipperyEffectEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Slippery;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class SlipperyEffectEntry : 
  ISerializationGenerated<SlipperyEffectEntry>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  public float LaunchForwardsMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public float RequiredSlipSpeed = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  public bool SuperSlippery;
  [DataField(null, false, 1, false, false, null)]
  public float SlipFriction;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SlipperyEffectEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SlipperyEffectEntry>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LaunchForwardsMultiplier, ref target2, hookCtx, false, context))
      target2 = this.LaunchForwardsMultiplier;
    target.LaunchForwardsMultiplier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RequiredSlipSpeed, ref target3, hookCtx, false, context))
      target3 = this.RequiredSlipSpeed;
    target.RequiredSlipSpeed = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.SuperSlippery, ref target4, hookCtx, false, context))
      target4 = this.SuperSlippery;
    target.SuperSlippery = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlipFriction, ref target5, hookCtx, false, context))
      target5 = this.SlipFriction;
    target.SlipFriction = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SlipperyEffectEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SlipperyEffectEntry target1 = (SlipperyEffectEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SlipperyEffectEntry Instantiate() => new SlipperyEffectEntry();
}
