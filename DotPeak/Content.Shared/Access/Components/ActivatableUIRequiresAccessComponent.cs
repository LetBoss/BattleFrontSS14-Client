// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.ActivatableUIRequiresAccessComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (ActivatableUIRequiresAccessSystem)})]
public sealed class ActivatableUIRequiresAccessComponent : 
  Component,
  ISerializationGenerated<ActivatableUIRequiresAccessComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId? PopupMessage = LocId.op_Implicit("lock-comp-has-user-access-fail");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActivatableUIRequiresAccessComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ActivatableUIRequiresAccessComponent) component;
    if (serialization.TryCustomCopy<ActivatableUIRequiresAccessComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? nullable = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.PopupMessage, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<LocId?>(this.PopupMessage, hookCtx, context, false);
    target.PopupMessage = nullable;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActivatableUIRequiresAccessComponent target,
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
    ActivatableUIRequiresAccessComponent target1 = (ActivatableUIRequiresAccessComponent) target;
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
    ActivatableUIRequiresAccessComponent target1 = (ActivatableUIRequiresAccessComponent) target;
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
    ActivatableUIRequiresAccessComponent target1 = (ActivatableUIRequiresAccessComponent) target;
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
  virtual ActivatableUIRequiresAccessComponent Component.Instantiate()
  {
    return new ActivatableUIRequiresAccessComponent();
  }
}
