// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.InstantSpawnSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class InstantSpawnSpellEvent : 
  InstantActionEvent,
  ISerializationGenerated<InstantSpawnSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Prototype;
  [DataField(null, false, 1, false, false, null)]
  public bool PreventCollideWithCaster = true;
  [DataField(null, false, 1, false, false, null)]
  public MagicInstantSpawnData PosData = (MagicInstantSpawnData) new TargetCasterPos();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InstantSpawnSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InstantSpawnSpellEvent) target1;
    if (serialization.TryCustomCopy<InstantSpawnSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreventCollideWithCaster, ref target3, hookCtx, false, context))
      target3 = this.PreventCollideWithCaster;
    target.PreventCollideWithCaster = target3;
    MagicInstantSpawnData target4 = (MagicInstantSpawnData) null;
    if (this.PosData == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<MagicInstantSpawnData>(this.PosData, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<MagicInstantSpawnData>(this.PosData, hookCtx, context);
    target.PosData = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InstantSpawnSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref InstantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantSpawnSpellEvent target1 = (InstantSpawnSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (InstantActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantSpawnSpellEvent target1 = (InstantSpawnSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual InstantSpawnSpellEvent InstantActionEvent.Instantiate() => new InstantSpawnSpellEvent();
}
