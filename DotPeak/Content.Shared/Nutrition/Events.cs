// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.ConsumeDoAfterEvent
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
namespace Content.Shared.Nutrition;

[NetSerializable]
[Serializable]
public sealed class ConsumeDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<ConsumeDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("solution", false, 1, true, false, null)]
  public string Solution;
  [DataField("flavorMessage", false, 1, true, false, null)]
  public string FlavorMessage;

  private ConsumeDoAfterEvent()
  {
  }

  public ConsumeDoAfterEvent(string solution, string flavorMessage)
  {
    this.Solution = solution;
    this.FlavorMessage = flavorMessage;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConsumeDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ConsumeDoAfterEvent) target1;
    if (serialization.TryCustomCopy<ConsumeDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    string target3 = (string) null;
    if (this.FlavorMessage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FlavorMessage, ref target3, hookCtx, false, context))
      target3 = this.FlavorMessage;
    target.FlavorMessage = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConsumeDoAfterEvent target,
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
    ConsumeDoAfterEvent target1 = (ConsumeDoAfterEvent) target;
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
    ConsumeDoAfterEvent target1 = (ConsumeDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ConsumeDoAfterEvent DoAfterEvent.Instantiate() => new ConsumeDoAfterEvent();
}
