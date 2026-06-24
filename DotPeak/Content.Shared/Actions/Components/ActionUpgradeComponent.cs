// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Components.ActionUpgradeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ActionUpgradeSystem)})]
[EntityCategory(new string[] {"Actions"})]
public sealed class ActionUpgradeComponent : 
  Component,
  ISerializationGenerated<ActionUpgradeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Level = 1;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<int, EntProtoId> EffectedLevels = new Dictionary<int, EntProtoId>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActionUpgradeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ActionUpgradeComponent) component;
    if (serialization.TryCustomCopy<ActionUpgradeComponent>(this, ref target, hookCtx, false, context))
      return;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Level, ref num, hookCtx, false, context))
      num = this.Level;
    target.Level = num;
    Dictionary<int, EntProtoId> dictionary = (Dictionary<int, EntProtoId>) null;
    if (this.EffectedLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, EntProtoId>>(this.EffectedLevels, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<int, EntProtoId>>(this.EffectedLevels, hookCtx, context, false);
    target.EffectedLevels = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActionUpgradeComponent target,
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
    ActionUpgradeComponent target1 = (ActionUpgradeComponent) target;
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
    ActionUpgradeComponent target1 = (ActionUpgradeComponent) target;
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
    ActionUpgradeComponent target1 = (ActionUpgradeComponent) target;
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
  virtual ActionUpgradeComponent Component.Instantiate() => new ActionUpgradeComponent();
}
