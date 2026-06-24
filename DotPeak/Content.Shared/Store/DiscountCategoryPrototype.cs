// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.DiscountCategoryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Store;

[Prototype(null, 1)]
[DataDefinition]
public sealed class DiscountCategoryPrototype : 
  IPrototype,
  ISerializationGenerated<DiscountCategoryPrototype>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int Weight { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int? MaxItems { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DiscountCategoryPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DiscountCategoryPrototype>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Weight, ref target2, hookCtx, false, context))
      target2 = this.Weight;
    target.Weight = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxItems, ref target3, hookCtx, false, context))
      target3 = this.MaxItems;
    target.MaxItems = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DiscountCategoryPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DiscountCategoryPrototype target1 = (DiscountCategoryPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public DiscountCategoryPrototype Instantiate() => new DiscountCategoryPrototype();
}
