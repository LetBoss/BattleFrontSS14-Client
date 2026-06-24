// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.CargoSlipComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cargo.Components;

[RegisterComponent]
public sealed class CargoSlipComponent : 
  Component,
  ISerializationGenerated<CargoSlipComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CargoProductPrototype> Product;
  [DataField(null, false, 1, false, false, null)]
  public string Requester;
  [DataField(null, false, 1, false, false, null)]
  public string Reason;
  [DataField(null, false, 1, false, false, null)]
  public int OrderQuantity;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CargoAccountPrototype> Account;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoSlipComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CargoSlipComponent) component;
    if (serialization.TryCustomCopy<CargoSlipComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<CargoProductPrototype> protoId1 = new ProtoId<CargoProductPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoProductPrototype>>(this.Product, ref protoId1, hookCtx, false, context))
      protoId1 = serialization.CreateCopy<ProtoId<CargoProductPrototype>>(this.Product, hookCtx, context, false);
    target.Product = protoId1;
    string str1 = (string) null;
    if (this.Requester == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Requester, ref str1, hookCtx, false, context))
      str1 = this.Requester;
    target.Requester = str1;
    string str2 = (string) null;
    if (this.Reason == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Reason, ref str2, hookCtx, false, context))
      str2 = this.Reason;
    target.Reason = str2;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.OrderQuantity, ref num, hookCtx, false, context))
      num = this.OrderQuantity;
    target.OrderQuantity = num;
    ProtoId<CargoAccountPrototype> protoId2 = new ProtoId<CargoAccountPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(this.Account, ref protoId2, hookCtx, false, context))
      protoId2 = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(this.Account, hookCtx, context, false);
    target.Account = protoId2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoSlipComponent target,
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
    CargoSlipComponent target1 = (CargoSlipComponent) target;
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
    CargoSlipComponent target1 = (CargoSlipComponent) target;
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
    CargoSlipComponent target1 = (CargoSlipComponent) target;
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
  virtual CargoSlipComponent Component.Instantiate() => new CargoSlipComponent();
}
