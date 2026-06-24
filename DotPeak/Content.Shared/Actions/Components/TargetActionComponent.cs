// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Components.TargetActionComponent
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
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedActionsSystem)})]
[EntityCategory(new string[] {"Actions"})]
public sealed class TargetActionComponent : 
  Component,
  ISerializationGenerated<TargetActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Repeat;
  [DataField(null, false, 1, false, false, null)]
  public bool DeselectOnMiss;
  [DataField(null, false, 1, false, false, null)]
  public bool CheckCanAccess = true;
  [DataField(null, false, 1, false, false, null)]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public bool InteractOnMiss;
  [DataField(null, false, 1, false, false, null)]
  public bool TargetingIndicator = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TargetActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (TargetActionComponent) component;
    if (serialization.TryCustomCopy<TargetActionComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Repeat, ref flag1, hookCtx, false, context))
      flag1 = this.Repeat;
    target.Repeat = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeselectOnMiss, ref flag2, hookCtx, false, context))
      flag2 = this.DeselectOnMiss;
    target.DeselectOnMiss = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CheckCanAccess, ref flag3, hookCtx, false, context))
      flag3 = this.CheckCanAccess;
    target.CheckCanAccess = flag3;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref num, hookCtx, false, context))
      num = this.Range;
    target.Range = num;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.InteractOnMiss, ref flag4, hookCtx, false, context))
      flag4 = this.InteractOnMiss;
    target.InteractOnMiss = flag4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.TargetingIndicator, ref flag5, hookCtx, false, context))
      flag5 = this.TargetingIndicator;
    target.TargetingIndicator = flag5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TargetActionComponent target,
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
    TargetActionComponent target1 = (TargetActionComponent) target;
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
    TargetActionComponent target1 = (TargetActionComponent) target;
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
    TargetActionComponent target1 = (TargetActionComponent) target;
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
  virtual TargetActionComponent Component.Instantiate() => new TargetActionComponent();
}
