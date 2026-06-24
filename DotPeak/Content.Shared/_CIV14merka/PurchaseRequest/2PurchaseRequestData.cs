// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.PurchaseRequest.CatalogItem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.PurchaseRequest;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class CatalogItem : ISerializationGenerated<CatalogItem>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string ItemPrototype = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public int Price;
  [DataField(null, false, 1, false, false, null)]
  public int PackQuantity = 1;
  [DataField(null, false, 1, false, false, null)]
  public bool IsWeapon;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CatalogItem target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CatalogItem>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ItemPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ItemPrototype, ref target1, hookCtx, false, context))
      target1 = this.ItemPrototype;
    target.ItemPrototype = target1;
    string target2 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target2, hookCtx, false, context))
      target2 = this.Name;
    target.Name = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Price, ref target3, hookCtx, false, context))
      target3 = this.Price;
    target.Price = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.PackQuantity, ref target4, hookCtx, false, context))
      target4 = this.PackQuantity;
    target.PackQuantity = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsWeapon, ref target5, hookCtx, false, context))
      target5 = this.IsWeapon;
    target.IsWeapon = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CatalogItem target,
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
    CatalogItem target1 = (CatalogItem) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CatalogItem Instantiate() => new CatalogItem();
}
