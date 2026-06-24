// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.OverrideSellComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cargo.Components;

[RegisterComponent]
public sealed class OverrideSellComponent : 
  Component,
  ISerializationGenerated<OverrideSellComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<CargoAccountPrototype> OverrideAccount;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OverrideSellComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (OverrideSellComponent) component;
    if (serialization.TryCustomCopy<OverrideSellComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<CargoAccountPrototype> protoId = new ProtoId<CargoAccountPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(this.OverrideAccount, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(this.OverrideAccount, hookCtx, context, false);
    target.OverrideAccount = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OverrideSellComponent target,
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
    OverrideSellComponent target1 = (OverrideSellComponent) target;
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
    OverrideSellComponent target1 = (OverrideSellComponent) target;
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
    OverrideSellComponent target1 = (OverrideSellComponent) target;
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
  virtual OverrideSellComponent Component.Instantiate() => new OverrideSellComponent();
}
