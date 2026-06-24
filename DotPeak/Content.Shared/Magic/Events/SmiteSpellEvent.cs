// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.SmiteSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class SmiteSpellEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<SmiteSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteNonBrainParts = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SmiteSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SmiteSpellEvent) target1;
    if (serialization.TryCustomCopy<SmiteSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteNonBrainParts, ref target2, hookCtx, false, context))
      target2 = this.DeleteNonBrainParts;
    target.DeleteNonBrainParts = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SmiteSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SmiteSpellEvent target1 = (SmiteSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SmiteSpellEvent target1 = (SmiteSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SmiteSpellEvent EntityTargetActionEvent.Instantiate() => new SmiteSpellEvent();
}
