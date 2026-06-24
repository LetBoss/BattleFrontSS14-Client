// Decompiled with JetBrains decompiler
// Type: Content.Shared.Polymorph.PolymorphActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Polymorph;

public sealed class PolymorphActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<PolymorphActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Robust.Shared.Prototypes.ProtoId<PolymorphPrototype>? ProtoId;

  public PolymorphActionEvent(Robust.Shared.Prototypes.ProtoId<PolymorphPrototype> protoId)
    : this()
  {
    this.ProtoId = new Robust.Shared.Prototypes.ProtoId<PolymorphPrototype>?(protoId);
  }

  public PolymorphActionEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PolymorphActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PolymorphActionEvent) target1;
    if (serialization.TryCustomCopy<PolymorphActionEvent>(this, ref target, hookCtx, false, context))
      return;
    Robust.Shared.Prototypes.ProtoId<PolymorphPrototype>? target2 = new Robust.Shared.Prototypes.ProtoId<PolymorphPrototype>?();
    if (!serialization.TryCustomCopy<Robust.Shared.Prototypes.ProtoId<PolymorphPrototype>?>(this.ProtoId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Robust.Shared.Prototypes.ProtoId<PolymorphPrototype>?>(this.ProtoId, hookCtx, context);
    target.ProtoId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PolymorphActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref InstantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PolymorphActionEvent target1 = (PolymorphActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (InstantActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PolymorphActionEvent target1 = (PolymorphActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PolymorphActionEvent InstantActionEvent.Instantiate() => new PolymorphActionEvent();
}
