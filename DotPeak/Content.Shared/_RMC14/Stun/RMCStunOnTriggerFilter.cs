// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stun.RMCStunOnTriggerFilter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._RMC14.Stun;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RMCStunOnTriggerFilter : 
  ISerializationGenerated<RMCStunOnTriggerFilter>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float? Range;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? Stun;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? Paralyze;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? Flash;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? FlashAdditionalStunTime;
  [DataField(null, false, 1, false, false, null)]
  public float? Probability;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist Whitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCStunOnTriggerFilter target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCStunOnTriggerFilter>(this, ref target, hookCtx, false, context))
      return;
    float? target1 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Range, ref target1, hookCtx, false, context))
      target1 = this.Range;
    target.Range = target1;
    TimeSpan? target2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Stun, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan?>(this.Stun, hookCtx, context);
    target.Stun = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Paralyze, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.Paralyze, hookCtx, context);
    target.Paralyze = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Flash, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.Flash, hookCtx, context);
    target.Flash = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.FlashAdditionalStunTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.FlashAdditionalStunTime, hookCtx, context);
    target.FlashAdditionalStunTime = target5;
    float? target6 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Probability, ref target6, hookCtx, false, context))
      target6 = this.Probability;
    target.Probability = target6;
    EntityWhitelist target7 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target7 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, context, true);
    }
    target.Whitelist = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCStunOnTriggerFilter target,
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
    RMCStunOnTriggerFilter target1 = (RMCStunOnTriggerFilter) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCStunOnTriggerFilter Instantiate() => new RMCStunOnTriggerFilter();
}
