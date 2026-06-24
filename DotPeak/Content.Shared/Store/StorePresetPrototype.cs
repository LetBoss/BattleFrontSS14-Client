// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.StorePresetPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Store;

[Robust.Shared.Prototypes.Prototype(null, 1)]
[DataDefinition]
public sealed class StorePresetPrototype : 
  IPrototype,
  ISerializationGenerated<StorePresetPrototype>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("storeName", false, 1, true, false, null)]
  public string StoreName { get; private set; } = string.Empty;

  [DataField("categories", false, 1, false, false, typeof (PrototypeIdHashSetSerializer<StoreCategoryPrototype>))]
  public HashSet<string> Categories { get; private set; } = new HashSet<string>();

  [DataField("initialBalance", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, CurrencyPrototype>))]
  public System.Collections.Generic.Dictionary<string, FixedPoint2>? InitialBalance { get; private set; }

  [DataField("currencyWhitelist", false, 1, false, false, typeof (PrototypeIdHashSetSerializer<CurrencyPrototype>))]
  public HashSet<string> CurrencyWhitelist { get; private set; } = new HashSet<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StorePresetPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<StorePresetPrototype>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
    string target2 = (string) null;
    if (this.StoreName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StoreName, ref target2, hookCtx, false, context))
      target2 = this.StoreName;
    target.StoreName = target2;
    HashSet<string> target3 = (HashSet<string>) null;
    if (this.Categories == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.Categories, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<string>>(this.Categories, hookCtx, context);
    target.Categories = target3;
    System.Collections.Generic.Dictionary<string, FixedPoint2> target4 = (System.Collections.Generic.Dictionary<string, FixedPoint2>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this.InitialBalance, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this.InitialBalance, hookCtx, context);
    target.InitialBalance = target4;
    HashSet<string> target5 = (HashSet<string>) null;
    if (this.CurrencyWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.CurrencyWhitelist, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<string>>(this.CurrencyWhitelist, hookCtx, context);
    target.CurrencyWhitelist = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StorePresetPrototype target,
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
    StorePresetPrototype target1 = (StorePresetPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public StorePresetPrototype Instantiate() => new StorePresetPrototype();
}
