// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.ToolRefinableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
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
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ToolRefinablSystem)})]
public sealed class ToolRefinableComponent : 
  Component,
  ISerializationGenerated<ToolRefinableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<EntitySpawnEntry> RefineResult = new List<EntitySpawnEntry>();
  [DataField(null, false, 1, false, false, null)]
  public float RefineTime = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float RefineFuel = 3f;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> QualityNeeded = (ProtoId<ToolQualityPrototype>) "Welding";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToolRefinableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ToolRefinableComponent) target1;
    if (serialization.TryCustomCopy<ToolRefinableComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntitySpawnEntry> target2 = (List<EntitySpawnEntry>) null;
    if (this.RefineResult == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.RefineResult, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.RefineResult, hookCtx, context);
    target.RefineResult = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RefineTime, ref target3, hookCtx, false, context))
      target3 = this.RefineTime;
    target.RefineTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RefineFuel, ref target4, hookCtx, false, context))
      target4 = this.RefineFuel;
    target.RefineFuel = target4;
    ProtoId<ToolQualityPrototype> target5 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.QualityNeeded, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.QualityNeeded, hookCtx, context);
    target.QualityNeeded = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToolRefinableComponent target,
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
    ToolRefinableComponent target1 = (ToolRefinableComponent) target;
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
    ToolRefinableComponent target1 = (ToolRefinableComponent) target;
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
    ToolRefinableComponent target1 = (ToolRefinableComponent) target;
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
  virtual ToolRefinableComponent Component.Instantiate() => new ToolRefinableComponent();
}
