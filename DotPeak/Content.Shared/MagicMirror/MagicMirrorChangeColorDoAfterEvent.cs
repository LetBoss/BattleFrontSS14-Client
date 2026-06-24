// Decompiled with JetBrains decompiler
// Type: Content.Shared.MagicMirror.MagicMirrorChangeColorDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.MagicMirror;

[NetSerializable]
[Serializable]
public sealed class MagicMirrorChangeColorDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<MagicMirrorChangeColorDoAfterEvent>,
  ISerializationGenerated
{
  public MagicMirrorCategory Category;
  public int Slot;
  public List<Color> Colors = new List<Color>();

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MagicMirrorChangeColorDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MagicMirrorChangeColorDoAfterEvent) target1;
    serialization.TryCustomCopy<MagicMirrorChangeColorDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MagicMirrorChangeColorDoAfterEvent target,
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
    MagicMirrorChangeColorDoAfterEvent target1 = (MagicMirrorChangeColorDoAfterEvent) target;
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
    MagicMirrorChangeColorDoAfterEvent target1 = (MagicMirrorChangeColorDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MagicMirrorChangeColorDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new MagicMirrorChangeColorDoAfterEvent();
  }
}
