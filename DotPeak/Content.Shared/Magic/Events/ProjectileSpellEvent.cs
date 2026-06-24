// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.ProjectileSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class ProjectileSpellEvent : 
  WorldTargetActionEvent,
  ISerializationGenerated<ProjectileSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Prototype;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WorldTargetActionEvent target1 = (WorldTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileSpellEvent) target1;
    if (serialization.TryCustomCopy<ProjectileSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref WorldTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ProjectileSpellEvent target1 = (ProjectileSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (WorldTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ProjectileSpellEvent target1 = (ProjectileSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ProjectileSpellEvent WorldTargetActionEvent.Instantiate() => new ProjectileSpellEvent();
}
