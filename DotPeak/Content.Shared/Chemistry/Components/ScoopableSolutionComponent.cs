// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.ScoopableSolutionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ScoopableSolutionSystem)})]
public sealed class ScoopableSolutionComponent : 
  Component,
  ISerializationGenerated<ScoopableSolutionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "default";
  [DataField(null, false, 1, false, false, null)]
  public bool Delete = true;
  [DataField(null, false, 1, false, false, null)]
  public LocId Popup = LocId.op_Implicit("scoopable-component-popup");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ScoopableSolutionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ScoopableSolutionComponent) component;
    if (serialization.TryCustomCopy<ScoopableSolutionComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref str, hookCtx, false, context))
      str = this.Solution;
    target.Solution = str;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Delete, ref flag, hookCtx, false, context))
      flag = this.Delete;
    target.Delete = flag;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context, false);
    target.Popup = locId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ScoopableSolutionComponent target,
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
    ScoopableSolutionComponent target1 = (ScoopableSolutionComponent) target;
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
    ScoopableSolutionComponent target1 = (ScoopableSolutionComponent) target;
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
    ScoopableSolutionComponent target1 = (ScoopableSolutionComponent) target;
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
  virtual ScoopableSolutionComponent Component.Instantiate() => new ScoopableSolutionComponent();
}
