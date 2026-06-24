// Decompiled with JetBrains decompiler
// Type: Content.Shared.StoreDiscount.Components.StoreDiscountComponent
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
namespace Content.Shared.StoreDiscount.Components;

[RegisterComponent]
public sealed class StoreDiscountComponent : 
  Component,
  ISerializationGenerated<StoreDiscountComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public IReadOnlyList<StoreDiscountData> Discounts = (IReadOnlyList<StoreDiscountData>) Array.Empty<StoreDiscountData>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StoreDiscountComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StoreDiscountComponent) target1;
    if (serialization.TryCustomCopy<StoreDiscountComponent>(this, ref target, hookCtx, false, context))
      return;
    IReadOnlyList<StoreDiscountData> target2 = (IReadOnlyList<StoreDiscountData>) null;
    if (this.Discounts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IReadOnlyList<StoreDiscountData>>(this.Discounts, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<IReadOnlyList<StoreDiscountData>>(this.Discounts, hookCtx, context);
    target.Discounts = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StoreDiscountComponent target,
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
    StoreDiscountComponent target1 = (StoreDiscountComponent) target;
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
    StoreDiscountComponent target1 = (StoreDiscountComponent) target;
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
    StoreDiscountComponent target1 = (StoreDiscountComponent) target;
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
  virtual StoreDiscountComponent Component.Instantiate() => new StoreDiscountComponent();
}
