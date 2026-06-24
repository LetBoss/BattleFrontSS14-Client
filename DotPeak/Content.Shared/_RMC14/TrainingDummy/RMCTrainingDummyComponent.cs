// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TrainingDummy.RMCTrainingDummyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.TrainingDummy;

[RegisterComponent]
public sealed class RMCTrainingDummyComponent : 
  Component,
  ISerializationGenerated<RMCTrainingDummyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? RemoveComponents;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCTrainingDummyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCTrainingDummyComponent) target1;
    if (serialization.TryCustomCopy<RMCTrainingDummyComponent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.RemoveComponents, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.RemoveComponents, hookCtx, context);
    target.RemoveComponents = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCTrainingDummyComponent target,
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
    RMCTrainingDummyComponent target1 = (RMCTrainingDummyComponent) target;
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
    RMCTrainingDummyComponent target1 = (RMCTrainingDummyComponent) target;
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
    RMCTrainingDummyComponent target1 = (RMCTrainingDummyComponent) target;
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
  virtual RMCTrainingDummyComponent Component.Instantiate() => new RMCTrainingDummyComponent();
}
