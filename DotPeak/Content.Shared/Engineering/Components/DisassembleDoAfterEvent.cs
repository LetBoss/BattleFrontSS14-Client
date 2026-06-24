// Decompiled with JetBrains decompiler
// Type: Content.Shared.Engineering.Components.DisassembleDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Engineering.Components;

[NetSerializable]
[Serializable]
public sealed class DisassembleDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<DisassembleDoAfterEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DisassembleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DisassembleDoAfterEvent) target1;
    serialization.TryCustomCopy<DisassembleDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DisassembleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SimpleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DisassembleDoAfterEvent target1 = (DisassembleDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SimpleDoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DisassembleDoAfterEvent target1 = (DisassembleDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DisassembleDoAfterEvent SimpleDoAfterEvent.Instantiate() => new DisassembleDoAfterEvent();
}
