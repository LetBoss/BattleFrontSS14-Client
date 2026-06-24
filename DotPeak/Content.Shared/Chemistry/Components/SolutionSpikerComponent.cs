// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionSpikerComponent
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
[Access(new Type[] {typeof (SolutionSpikerSystem)})]
public sealed class SolutionSpikerComponent : 
  Component,
  ISerializationGenerated<SolutionSpikerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string SourceSolution = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreEmpty;
  [DataField(null, false, 1, false, false, null)]
  public bool Delete = true;
  [DataField(null, false, 1, false, false, null)]
  public LocId Popup = LocId.op_Implicit("spike-solution-generic");
  [DataField(null, false, 1, false, false, null)]
  public LocId PopupEmpty = LocId.op_Implicit("spike-solution-empty-generic");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionSpikerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionSpikerComponent) component;
    if (serialization.TryCustomCopy<SolutionSpikerComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.SourceSolution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SourceSolution, ref str, hookCtx, false, context))
      str = this.SourceSolution;
    target.SourceSolution = str;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreEmpty, ref flag1, hookCtx, false, context))
      flag1 = this.IgnoreEmpty;
    target.IgnoreEmpty = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Delete, ref flag2, hookCtx, false, context))
      flag2 = this.Delete;
    target.Delete = flag2;
    LocId locId1 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref locId1, hookCtx, false, context))
      locId1 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context, false);
    target.Popup = locId1;
    LocId locId2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.PopupEmpty, ref locId2, hookCtx, false, context))
      locId2 = serialization.CreateCopy<LocId>(this.PopupEmpty, hookCtx, context, false);
    target.PopupEmpty = locId2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionSpikerComponent target,
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
    SolutionSpikerComponent target1 = (SolutionSpikerComponent) target;
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
    SolutionSpikerComponent target1 = (SolutionSpikerComponent) target;
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
    SolutionSpikerComponent target1 = (SolutionSpikerComponent) target;
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
  virtual SolutionSpikerComponent Component.Instantiate() => new SolutionSpikerComponent();
}
