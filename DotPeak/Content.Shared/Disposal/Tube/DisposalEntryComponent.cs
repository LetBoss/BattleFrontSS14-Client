// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Tube.DisposalEntryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Disposal.Unit;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Disposal.Tube;

[RegisterComponent]
[Access(new Type[] {typeof (SharedDisposalTubeSystem), typeof (SharedDisposalUnitSystem)})]
public sealed class DisposalEntryComponent : 
  Component,
  ISerializationGenerated<DisposalEntryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId HolderPrototypeId = EntProtoId.op_Implicit("DisposalHolder");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DisposalEntryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DisposalEntryComponent) component;
    if (serialization.TryCustomCopy<DisposalEntryComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HolderPrototypeId, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.HolderPrototypeId, hookCtx, context, false);
    target.HolderPrototypeId = entProtoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DisposalEntryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DisposalEntryComponent target1 = (DisposalEntryComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DisposalEntryComponent target1 = (DisposalEntryComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DisposalEntryComponent target1 = (DisposalEntryComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DisposalEntryComponent Component.Instantiate() => new DisposalEntryComponent();
}
