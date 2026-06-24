// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.PubgSkinShopOfferPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[DataDefinition]
public sealed class PubgSkinShopOfferPrototype : 
  ISerializationGenerated<PubgSkinShopOfferPrototype>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Currency = "premium";
  [DataField(null, false, 1, false, false, null)]
  public int Price;
  [DataField(null, false, 1, false, false, null)]
  public int? DurationDays;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgSkinShopOfferPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PubgSkinShopOfferPrototype>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Currency == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Currency, ref target1, hookCtx, false, context))
      target1 = this.Currency;
    target.Currency = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Price, ref target2, hookCtx, false, context))
      target2 = this.Price;
    target.Price = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.DurationDays, ref target3, hookCtx, false, context))
      target3 = this.DurationDays;
    target.DurationDays = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgSkinShopOfferPrototype target,
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
    PubgSkinShopOfferPrototype target1 = (PubgSkinShopOfferPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PubgSkinShopOfferPrototype Instantiate() => new PubgSkinShopOfferPrototype();
}
