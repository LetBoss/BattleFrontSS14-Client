// Decompiled with JetBrains decompiler
// Type: Content.Shared.SprayPainter.SprayPainterPipeDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.SprayPainter;

[NetSerializable]
[Serializable]
public sealed class SprayPainterPipeDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<SprayPainterPipeDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Color Color;

  public SprayPainterPipeDoAfterEvent(Color color) => this.Color = color;

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  public SprayPainterPipeDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SprayPainterPipeDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SprayPainterPipeDoAfterEvent) target1;
    if (serialization.TryCustomCopy<SprayPainterPipeDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SprayPainterPipeDoAfterEvent target,
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
    SprayPainterPipeDoAfterEvent target1 = (SprayPainterPipeDoAfterEvent) target;
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
    SprayPainterPipeDoAfterEvent target1 = (SprayPainterPipeDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SprayPainterPipeDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new SprayPainterPipeDoAfterEvent();
  }
}
