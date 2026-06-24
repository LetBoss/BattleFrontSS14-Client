// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Barricade.Components.BarbedDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Barricade.Components;

[NetSerializable]
[Serializable]
public sealed class BarbedDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<BarbedDoAfterEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BarbedDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BarbedDoAfterEvent) target1;
    serialization.TryCustomCopy<BarbedDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BarbedDoAfterEvent target,
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
    BarbedDoAfterEvent target1 = (BarbedDoAfterEvent) target;
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
    BarbedDoAfterEvent target1 = (BarbedDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BarbedDoAfterEvent SimpleDoAfterEvent.Instantiate() => new BarbedDoAfterEvent();
}
