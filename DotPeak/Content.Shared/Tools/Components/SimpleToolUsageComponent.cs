// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.SimpleToolUsageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SimpleToolUsageSystem)})]
public sealed class SimpleToolUsageComponent : 
  Component,
  ISerializationGenerated<SimpleToolUsageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> Quality = (ProtoId<ToolQualityPrototype>) "Slicing";
  [DataField(null, false, 1, false, false, null)]
  public float DoAfter = 5f;
  [DataField(null, false, 1, false, false, null)]
  public LocId? UsageVerb;
  [DataField(null, false, 1, false, false, null)]
  public LocId BlockedMessage = (LocId) "simple-tool-usage-blocked-message";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SimpleToolUsageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SimpleToolUsageComponent) target1;
    if (serialization.TryCustomCopy<SimpleToolUsageComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ToolQualityPrototype> target2 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.Quality, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.Quality, hookCtx, context);
    target.Quality = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DoAfter, ref target3, hookCtx, false, context))
      target3 = this.DoAfter;
    target.DoAfter = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UsageVerb, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.UsageVerb, hookCtx, context);
    target.UsageVerb = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.BlockedMessage, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.BlockedMessage, hookCtx, context);
    target.BlockedMessage = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SimpleToolUsageComponent target,
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
    SimpleToolUsageComponent target1 = (SimpleToolUsageComponent) target;
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
    SimpleToolUsageComponent target1 = (SimpleToolUsageComponent) target;
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
    SimpleToolUsageComponent target1 = (SimpleToolUsageComponent) target;
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
  virtual SimpleToolUsageComponent Component.Instantiate() => new SimpleToolUsageComponent();
}
