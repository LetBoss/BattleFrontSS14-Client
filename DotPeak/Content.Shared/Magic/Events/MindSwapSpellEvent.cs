// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.MindSwapSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class MindSwapSpellEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<MindSwapSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PerformerStunDuration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TargetStunDuration = TimeSpan.FromSeconds(10L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MindSwapSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MindSwapSpellEvent) target1;
    if (serialization.TryCustomCopy<MindSwapSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PerformerStunDuration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.PerformerStunDuration, hookCtx, context);
    target.PerformerStunDuration = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetStunDuration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.TargetStunDuration, hookCtx, context);
    target.TargetStunDuration = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MindSwapSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MindSwapSpellEvent target1 = (MindSwapSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MindSwapSpellEvent target1 = (MindSwapSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MindSwapSpellEvent EntityTargetActionEvent.Instantiate() => new MindSwapSpellEvent();
}
