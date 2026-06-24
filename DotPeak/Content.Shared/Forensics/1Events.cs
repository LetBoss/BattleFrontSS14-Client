// Decompiled with JetBrains decompiler
// Type: Content.Shared.Forensics.ForensicPadDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Forensics;

[NetSerializable]
[Serializable]
public sealed class ForensicPadDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<ForensicPadDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("sample", false, 1, true, false, null)]
  public string Sample;

  private ForensicPadDoAfterEvent()
  {
  }

  public ForensicPadDoAfterEvent(string sample) => this.Sample = sample;

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ForensicPadDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ForensicPadDoAfterEvent) target1;
    if (serialization.TryCustomCopy<ForensicPadDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Sample == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Sample, ref target2, hookCtx, false, context))
      target2 = this.Sample;
    target.Sample = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ForensicPadDoAfterEvent target,
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
    ForensicPadDoAfterEvent target1 = (ForensicPadDoAfterEvent) target;
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
    ForensicPadDoAfterEvent target1 = (ForensicPadDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ForensicPadDoAfterEvent DoAfterEvent.Instantiate() => new ForensicPadDoAfterEvent();
}
