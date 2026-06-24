// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Roles.JobGroupComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Roles;

[RegisterComponent]
[NetworkedComponent]
public sealed class JobGroupComponent : 
  Component,
  ISerializationGenerated<JobGroupComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Name;
  [DataField(null, false, 1, true, false, null)]
  public Color Color;
  [DataField(null, false, 1, true, false, null)]
  public HashSet<ProtoId<JobPrototype>> Jobs = new HashSet<ProtoId<JobPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JobGroupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JobGroupComponent) target1;
    if (serialization.TryCustomCopy<JobGroupComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Name, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.Name, hookCtx, context);
    target.Name = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target3;
    HashSet<ProtoId<JobPrototype>> target4 = (HashSet<ProtoId<JobPrototype>>) null;
    if (this.Jobs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<JobPrototype>>>(this.Jobs, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<JobPrototype>>>(this.Jobs, hookCtx, context);
    target.Jobs = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JobGroupComponent target,
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
    JobGroupComponent target1 = (JobGroupComponent) target;
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
    JobGroupComponent target1 = (JobGroupComponent) target;
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
    JobGroupComponent target1 = (JobGroupComponent) target;
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
  virtual JobGroupComponent Component.Instantiate() => new JobGroupComponent();
}
