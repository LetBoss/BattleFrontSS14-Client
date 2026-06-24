// Decompiled with JetBrains decompiler
// Type: Content.Shared.SprayPainter.SprayPainterDoorDoAfterEvent
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
namespace Content.Shared.SprayPainter;

[NetSerializable]
[Serializable]
public sealed class SprayPainterDoorDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<SprayPainterDoorDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Sprite;
  [DataField(null, false, 1, false, false, null)]
  public string? Department;

  public SprayPainterDoorDoAfterEvent(string sprite, string? department)
  {
    this.Sprite = sprite;
    this.Department = department;
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  public SprayPainterDoorDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SprayPainterDoorDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SprayPainterDoorDoAfterEvent) target1;
    if (serialization.TryCustomCopy<SprayPainterDoorDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Sprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Sprite, ref target2, hookCtx, false, context))
      target2 = this.Sprite;
    target.Sprite = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Department, ref target3, hookCtx, false, context))
      target3 = this.Department;
    target.Department = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SprayPainterDoorDoAfterEvent target,
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
    SprayPainterDoorDoAfterEvent target1 = (SprayPainterDoorDoAfterEvent) target;
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
    SprayPainterDoorDoAfterEvent target1 = (SprayPainterDoorDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SprayPainterDoorDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new SprayPainterDoorDoAfterEvent();
  }
}
