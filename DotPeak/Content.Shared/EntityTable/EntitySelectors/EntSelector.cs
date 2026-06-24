// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntitySelectors.EntSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.ValueSelector;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class EntSelector : 
  EntityTableSelector,
  ISerializationGenerated<EntSelector>,
  ISerializationGenerated
{
  public const string IdDataFieldTag = "id";
  [DataField("id", false, 1, true, false, null)]
  public EntProtoId Id;
  [DataField(null, false, 1, false, false, null)]
  public NumberSelector Amount = (NumberSelector) new ConstantNumberSelector(1);

  protected override IEnumerable<EntProtoId> GetSpawnsImplementation(
    Random rand,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    int num = this.Amount.Get(rand);
    for (int i = 0; i < num; ++i)
      yield return this.Id;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTableSelector target1 = (EntityTableSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntSelector) target1;
    if (serialization.TryCustomCopy<EntSelector>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Id, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Id, hookCtx, context);
    target.Id = target2;
    NumberSelector target3 = (NumberSelector) null;
    if (this.Amount == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<NumberSelector>(this.Amount, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<NumberSelector>(this.Amount, hookCtx, context);
    target.Amount = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTableSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntSelector target1 = (EntSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTableSelector) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntSelector target1 = (EntSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EntSelector EntityTableSelector.Instantiate() => new EntSelector();
}
