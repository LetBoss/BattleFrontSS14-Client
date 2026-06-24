// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Balance.CivCommanderComebackBonusTier
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Balance;

[DataDefinition]
public sealed class CivCommanderComebackBonusTier : 
  ISerializationGenerated<CivCommanderComebackBonusTier>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int LosingPercent;
  [DataField(null, false, 1, true, false, null)]
  public int Currency;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivCommanderComebackBonusTier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CivCommanderComebackBonusTier>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.LosingPercent, ref target1, hookCtx, false, context))
      target1 = this.LosingPercent;
    target.LosingPercent = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Currency, ref target2, hookCtx, false, context))
      target2 = this.Currency;
    target.Currency = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivCommanderComebackBonusTier target,
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
    CivCommanderComebackBonusTier target1 = (CivCommanderComebackBonusTier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CivCommanderComebackBonusTier Instantiate() => new CivCommanderComebackBonusTier();
}
