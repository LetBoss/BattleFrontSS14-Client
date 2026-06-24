// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Components.MobStateActionsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mobs.Components;

[RegisterComponent]
public sealed class MobStateActionsComponent : 
  Component,
  ISerializationGenerated<MobStateActionsComponent>,
  ISerializationGenerated
{
  [DataField("actions", false, 1, false, false, null)]
  public Dictionary<MobState, List<string>> Actions = new Dictionary<MobState, List<string>>();
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> GrantedActions = new List<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobStateActionsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MobStateActionsComponent) target1;
    if (serialization.TryCustomCopy<MobStateActionsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<MobState, List<string>> target2 = (Dictionary<MobState, List<string>>) null;
    if (this.Actions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, List<string>>>(this.Actions, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<MobState, List<string>>>(this.Actions, hookCtx, context);
    target.Actions = target2;
    List<EntityUid> target3 = (List<EntityUid>) null;
    if (this.GrantedActions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.GrantedActions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntityUid>>(this.GrantedActions, hookCtx, context);
    target.GrantedActions = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobStateActionsComponent target,
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
    MobStateActionsComponent target1 = (MobStateActionsComponent) target;
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
    MobStateActionsComponent target1 = (MobStateActionsComponent) target;
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
    MobStateActionsComponent target1 = (MobStateActionsComponent) target;
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
  virtual MobStateActionsComponent Component.Instantiate() => new MobStateActionsComponent();
}
