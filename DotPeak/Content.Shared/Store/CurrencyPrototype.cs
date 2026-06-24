// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.CurrencyPrototype
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
using System;

#nullable enable
namespace Content.Shared.Store;

[Robust.Shared.Prototypes.Prototype(null, 1)]
[DataDefinition]
public sealed class CurrencyPrototype : 
  IPrototype,
  ISerializationGenerated<CurrencyPrototype>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("displayName", false, 1, false, false, null)]
  public string DisplayName { get; private set; } = string.Empty;

  [DataField("cash", false, 1, false, false, typeof (PrototypeIdValueDictionarySerializer<FixedPoint2, EntityPrototype>))]
  public System.Collections.Generic.Dictionary<FixedPoint2, string>? Cash { get; private set; }

  [DataField("canWithdraw", false, 1, false, false, null)]
  public bool CanWithdraw { get; private set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CurrencyPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CurrencyPrototype>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
    string target2 = (string) null;
    if (this.DisplayName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DisplayName, ref target2, hookCtx, false, context))
      target2 = this.DisplayName;
    target.DisplayName = target2;
    System.Collections.Generic.Dictionary<FixedPoint2, string> target3 = (System.Collections.Generic.Dictionary<FixedPoint2, string>) null;
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<FixedPoint2, string>>(this.Cash, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<System.Collections.Generic.Dictionary<FixedPoint2, string>>(this.Cash, hookCtx, context);
    target.Cash = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanWithdraw, ref target4, hookCtx, false, context))
      target4 = this.CanWithdraw;
    target.CanWithdraw = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CurrencyPrototype target,
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
    CurrencyPrototype target1 = (CurrencyPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CurrencyPrototype Instantiate() => new CurrencyPrototype();
}
