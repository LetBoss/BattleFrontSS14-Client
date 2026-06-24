// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.VapeDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
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
public sealed class VapeDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<VapeDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("solution", false, 1, true, false, null)]
  public Solution Solution;
  [DataField("forced", false, 1, true, false, null)]
  public bool Forced;

  private VapeDoAfterEvent()
  {
  }

  public VapeDoAfterEvent(Solution solution, bool forced)
  {
    this.Solution = solution;
    this.Forced = forced;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VapeDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VapeDoAfterEvent) target1;
    if (serialization.TryCustomCopy<VapeDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    Solution target2 = (Solution) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Solution>(this.Solution, ref target2, hookCtx, true, context))
    {
      if (this.Solution == null)
        target2 = (Solution) null;
      else
        serialization.CopyTo<Solution>(this.Solution, ref target2, hookCtx, context, true);
    }
    target.Solution = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Forced, ref target3, hookCtx, false, context))
      target3 = this.Forced;
    target.Forced = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VapeDoAfterEvent target,
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
    VapeDoAfterEvent target1 = (VapeDoAfterEvent) target;
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
    VapeDoAfterEvent target1 = (VapeDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual VapeDoAfterEvent DoAfterEvent.Instantiate() => new VapeDoAfterEvent();
}
