// Decompiled with JetBrains decompiler
// Type: Content.Shared.Objectives.Components.StealTargetComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Objectives.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StealTargetComponent : 
  Component,
  ISerializationGenerated<StealTargetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<StealTargetGroupPrototype> StealGroup;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StealTargetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StealTargetComponent) target1;
    if (serialization.TryCustomCopy<StealTargetComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<StealTargetGroupPrototype> target2 = new ProtoId<StealTargetGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<StealTargetGroupPrototype>>(this.StealGroup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<StealTargetGroupPrototype>>(this.StealGroup, hookCtx, context);
    target.StealGroup = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StealTargetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StealTargetComponent target1 = (StealTargetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StealTargetComponent target1 = (StealTargetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StealTargetComponent target1 = (StealTargetComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StealTargetComponent Component.Instantiate() => new StealTargetComponent();
}
