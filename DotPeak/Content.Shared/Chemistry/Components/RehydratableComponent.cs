// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.RehydratableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[Access(new Type[] {typeof (RehydratableSystem)})]
public sealed class RehydratableComponent : 
  Component,
  ISerializationGenerated<RehydratableComponent>,
  ISerializationGenerated
{
  [DataField("catalyst", false, 1, false, false, null)]
  public ProtoId<ReagentPrototype> CatalystPrototype = ProtoId<ReagentPrototype>.op_Implicit("Water");
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 CatalystMinimum = FixedPoint2.Zero;
  [DataField(null, false, 1, true, false, null)]
  public List<EntProtoId> PossibleSpawns = new List<EntProtoId>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RehydratableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (RehydratableComponent) component;
    if (serialization.TryCustomCopy<RehydratableComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ReagentPrototype> protoId = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.CatalystPrototype, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.CatalystPrototype, hookCtx, context, false);
    target.CatalystPrototype = protoId;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CatalystMinimum, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.CatalystMinimum, hookCtx, context, false);
    target.CatalystMinimum = fixedPoint2;
    List<EntProtoId> entProtoIdList = (List<EntProtoId>) null;
    if (this.PossibleSpawns == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.PossibleSpawns, ref entProtoIdList, hookCtx, true, context))
      entProtoIdList = serialization.CreateCopy<List<EntProtoId>>(this.PossibleSpawns, hookCtx, context, false);
    target.PossibleSpawns = entProtoIdList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RehydratableComponent target,
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
    RehydratableComponent target1 = (RehydratableComponent) target;
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
    RehydratableComponent target1 = (RehydratableComponent) target;
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
    RehydratableComponent target1 = (RehydratableComponent) target;
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
  virtual RehydratableComponent Component.Instantiate() => new RehydratableComponent();
}
