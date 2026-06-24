// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntitySelectors.NestedSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class NestedSelector : 
  EntityTableSelector,
  ISerializationGenerated<NestedSelector>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<EntityTablePrototype> TableId;

  protected override IEnumerable<EntProtoId> GetSpawnsImplementation(
    Random rand,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    return proto.Index<EntityTablePrototype>(this.TableId).Table.GetSpawns(rand, entMan, proto, ctx);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NestedSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTableSelector target1 = (EntityTableSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NestedSelector) target1;
    if (serialization.TryCustomCopy<NestedSelector>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<EntityTablePrototype> target2 = new ProtoId<EntityTablePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(this.TableId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(this.TableId, hookCtx, context);
    target.TableId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NestedSelector target,
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
    NestedSelector target1 = (NestedSelector) target;
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
    NestedSelector target1 = (NestedSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NestedSelector EntityTableSelector.Instantiate() => new NestedSelector();
}
