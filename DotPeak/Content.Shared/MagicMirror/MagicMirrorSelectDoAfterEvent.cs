// Decompiled with JetBrains decompiler
// Type: Content.Shared.MagicMirror.MagicMirrorSelectDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.MagicMirror;

[NetSerializable]
[Serializable]
public sealed class MagicMirrorSelectDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<MagicMirrorSelectDoAfterEvent>,
  ISerializationGenerated
{
  public MagicMirrorCategory Category;
  public int Slot;
  public string Marking = string.Empty;

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MagicMirrorSelectDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MagicMirrorSelectDoAfterEvent) target1;
    serialization.TryCustomCopy<MagicMirrorSelectDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MagicMirrorSelectDoAfterEvent target,
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
    MagicMirrorSelectDoAfterEvent target1 = (MagicMirrorSelectDoAfterEvent) target;
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
    MagicMirrorSelectDoAfterEvent target1 = (MagicMirrorSelectDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MagicMirrorSelectDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new MagicMirrorSelectDoAfterEvent();
  }
}
