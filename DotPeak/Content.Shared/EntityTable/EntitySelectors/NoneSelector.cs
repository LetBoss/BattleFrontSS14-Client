// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntitySelectors.NoneSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class NoneSelector : 
  EntityTableSelector,
  ISerializationGenerated<NoneSelector>,
  ISerializationGenerated
{
  protected override IEnumerable<EntProtoId> GetSpawnsImplementation(
    Random rand,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    yield break;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NoneSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTableSelector target1 = (EntityTableSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NoneSelector) target1;
    serialization.TryCustomCopy<NoneSelector>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NoneSelector target,
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
    NoneSelector target1 = (NoneSelector) target;
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
    NoneSelector target1 = (NoneSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NoneSelector EntityTableSelector.Instantiate() => new NoneSelector();
}
