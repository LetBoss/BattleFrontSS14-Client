// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.AbsorbantDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Fluids;

[NetSerializable]
[Serializable]
public sealed class AbsorbantDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<AbsorbantDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("solution", false, 1, true, false, null)]
  public string TargetSolution;
  [DataField("message", false, 1, true, false, null)]
  public string Message;
  [DataField("sound", false, 1, true, false, null)]
  public SoundSpecifier Sound;
  [DataField("transferAmount", false, 1, true, false, null)]
  public FixedPoint2 TransferAmount;

  private AbsorbantDoAfterEvent()
  {
  }

  public AbsorbantDoAfterEvent(
    string targetSolution,
    string message,
    SoundSpecifier sound,
    FixedPoint2 transferAmount)
  {
    this.TargetSolution = targetSolution;
    this.Message = message;
    this.Sound = sound;
    this.TransferAmount = transferAmount;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AbsorbantDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AbsorbantDoAfterEvent) target1;
    if (serialization.TryCustomCopy<AbsorbantDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.TargetSolution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetSolution, ref target2, hookCtx, false, context))
      target2 = this.TargetSolution;
    target.TargetSolution = target2;
    string target3 = (string) null;
    if (this.Message == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Message, ref target3, hookCtx, false, context))
      target3 = this.Message;
    target.Message = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context);
    target.TransferAmount = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AbsorbantDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AbsorbantDoAfterEvent target1 = (AbsorbantDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AbsorbantDoAfterEvent target1 = (AbsorbantDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AbsorbantDoAfterEvent DoAfterEvent.Instantiate() => new AbsorbantDoAfterEvent();
}
