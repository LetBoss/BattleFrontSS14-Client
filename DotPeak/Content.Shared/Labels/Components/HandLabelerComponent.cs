// Decompiled with JetBrains decompiler
// Type: Content.Shared.Labels.Components.HandLabelerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Labels.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Labels.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedHandLabelerSystem)})]
public sealed class HandLabelerComponent : 
  Component,
  ISerializationGenerated<HandLabelerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  [DataField(null, false, 1, false, false, null)]
  public string AssignedLabel = string.Empty;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public int MaxLabelChars = 50;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist Whitelist = new EntityWhitelist();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HandLabelerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HandLabelerComponent) target1;
    if (serialization.TryCustomCopy<HandLabelerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.AssignedLabel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AssignedLabel, ref target2, hookCtx, false, context))
      target2 = this.AssignedLabel;
    target.AssignedLabel = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLabelChars, ref target3, hookCtx, false, context))
      target3 = this.MaxLabelChars;
    target.MaxLabelChars = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, context, true);
    }
    target.Whitelist = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HandLabelerComponent target,
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
    HandLabelerComponent target1 = (HandLabelerComponent) target;
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
    HandLabelerComponent target1 = (HandLabelerComponent) target;
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
    HandLabelerComponent target1 = (HandLabelerComponent) target;
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
  virtual HandLabelerComponent Component.Instantiate() => new HandLabelerComponent();
}
