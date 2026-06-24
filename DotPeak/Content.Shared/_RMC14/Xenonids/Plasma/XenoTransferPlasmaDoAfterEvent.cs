// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Plasma.XenoTransferPlasmaDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Plasma;

[NetSerializable]
[Serializable]
public sealed class XenoTransferPlasmaDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<XenoTransferPlasmaDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Amount = FixedPoint2.New(50);

  public XenoTransferPlasmaDoAfterEvent(FixedPoint2 amount) => this.Amount = amount;

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  public XenoTransferPlasmaDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTransferPlasmaDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTransferPlasmaDoAfterEvent) target1;
    if (serialization.TryCustomCopy<XenoTransferPlasmaDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Amount, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Amount, hookCtx, context);
    target.Amount = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTransferPlasmaDoAfterEvent target,
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
    XenoTransferPlasmaDoAfterEvent target1 = (XenoTransferPlasmaDoAfterEvent) target;
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
    XenoTransferPlasmaDoAfterEvent target1 = (XenoTransferPlasmaDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoTransferPlasmaDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new XenoTransferPlasmaDoAfterEvent();
  }
}
