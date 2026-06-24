// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.OpenUiActionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.UserInterface;

public sealed class OpenUiActionEvent : 
  InstantActionEvent,
  ISerializationGenerated<OpenUiActionEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, typeof (EnumSerializer))]
  public Enum? Key { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OpenUiActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OpenUiActionEvent) target1;
    if (serialization.TryCustomCopy<OpenUiActionEvent>(this, ref target, hookCtx, false, context))
      return;
    Enum target2 = (Enum) null;
    if (!serialization.TryCustomCopy<Enum>(this.Key, ref target2, hookCtx, true, context))
      target2 = this.Key;
    target.Key = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OpenUiActionEvent target,
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
    OpenUiActionEvent target1 = (OpenUiActionEvent) target;
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
    OpenUiActionEvent target1 = (OpenUiActionEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual OpenUiActionEvent InstantActionEvent.Instantiate() => new OpenUiActionEvent();
}
