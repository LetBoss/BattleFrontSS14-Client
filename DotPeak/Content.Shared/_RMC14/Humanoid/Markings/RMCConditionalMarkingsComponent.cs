// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Humanoid.Markings.RMCConditionalMarkingsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Humanoid.Markings;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCConditionalMarkingsComponent : 
  Component,
  ISerializationGenerated<RMCConditionalMarkingsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<Sex, List<ProtoId<MarkingPrototype>>> Markings;
  [DataField(null, false, 1, false, false, null)]
  public MarkingCategories TargetCategory;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCConditionalMarkingsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCConditionalMarkingsComponent) target1;
    if (serialization.TryCustomCopy<RMCConditionalMarkingsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Sex, List<ProtoId<MarkingPrototype>>> target2 = (Dictionary<Sex, List<ProtoId<MarkingPrototype>>>) null;
    if (this.Markings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Sex, List<ProtoId<MarkingPrototype>>>>(this.Markings, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Sex, List<ProtoId<MarkingPrototype>>>>(this.Markings, hookCtx, context);
    target.Markings = target2;
    MarkingCategories target3 = MarkingCategories.Special;
    if (!serialization.TryCustomCopy<MarkingCategories>(this.TargetCategory, ref target3, hookCtx, false, context))
      target3 = this.TargetCategory;
    target.TargetCategory = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCConditionalMarkingsComponent target,
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
    RMCConditionalMarkingsComponent target1 = (RMCConditionalMarkingsComponent) target;
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
    RMCConditionalMarkingsComponent target1 = (RMCConditionalMarkingsComponent) target;
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
    RMCConditionalMarkingsComponent target1 = (RMCConditionalMarkingsComponent) target;
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
  virtual RMCConditionalMarkingsComponent Component.Instantiate()
  {
    return new RMCConditionalMarkingsComponent();
  }
}
