// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Evolution.XenoOpenEvolutionsActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Evolution;

public sealed class XenoOpenEvolutionsActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<XenoOpenEvolutionsActionEvent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoOpenEvolutionsActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoOpenEvolutionsActionEvent) target1;
    serialization.TryCustomCopy<XenoOpenEvolutionsActionEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoOpenEvolutionsActionEvent target,
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
    XenoOpenEvolutionsActionEvent target1 = (XenoOpenEvolutionsActionEvent) target;
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
    XenoOpenEvolutionsActionEvent target1 = (XenoOpenEvolutionsActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoOpenEvolutionsActionEvent InstantActionEvent.Instantiate()
  {
    return new XenoOpenEvolutionsActionEvent();
  }
}
