// Decompiled with JetBrains decompiler
// Type: Content.Shared.HealthExaminable.HealthExaminableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
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
namespace Content.Shared.HealthExaminable;

[RegisterComponent]
[Access(new Type[] {typeof (HealthExaminableSystem)})]
public sealed class HealthExaminableComponent : 
  Component,
  ISerializationGenerated<HealthExaminableComponent>,
  ISerializationGenerated
{
  public List<FixedPoint2> Thresholds = new List<FixedPoint2>()
  {
    FixedPoint2.New(8),
    FixedPoint2.New(15),
    FixedPoint2.New(30),
    FixedPoint2.New(50),
    FixedPoint2.New(75),
    FixedPoint2.New(100),
    FixedPoint2.New(200)
  };
  [DataField(null, false, 1, true, false, null)]
  public HashSet<ProtoId<DamageTypePrototype>> ExaminableTypes;
  [DataField(null, false, 1, false, false, null)]
  public string LocPrefix = "carbon";
  [DataField(null, false, 1, false, false, null)]
  public bool ExamineShowEmpty = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HealthExaminableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HealthExaminableComponent) target1;
    if (serialization.TryCustomCopy<HealthExaminableComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<DamageTypePrototype>> target2 = (HashSet<ProtoId<DamageTypePrototype>>) null;
    if (this.ExaminableTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<DamageTypePrototype>>>(this.ExaminableTypes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<DamageTypePrototype>>>(this.ExaminableTypes, hookCtx, context);
    target.ExaminableTypes = target2;
    string target3 = (string) null;
    if (this.LocPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LocPrefix, ref target3, hookCtx, false, context))
      target3 = this.LocPrefix;
    target.LocPrefix = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExamineShowEmpty, ref target4, hookCtx, false, context))
      target4 = this.ExamineShowEmpty;
    target.ExamineShowEmpty = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HealthExaminableComponent target,
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
    HealthExaminableComponent target1 = (HealthExaminableComponent) target;
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
    HealthExaminableComponent target1 = (HealthExaminableComponent) target;
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
    HealthExaminableComponent target1 = (HealthExaminableComponent) target;
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
  virtual HealthExaminableComponent Component.Instantiate() => new HealthExaminableComponent();
}
