// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntitySelectors.AllSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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

public sealed class AllSelector : 
  EntityTableSelector,
  ISerializationGenerated<AllSelector>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public List<EntityTableSelector> Children;

  protected override IEnumerable<EntProtoId> GetSpawnsImplementation(
    Random rand,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    foreach (EntityTableSelector child in this.Children)
    {
      IEnumerator<EntProtoId> enumerator = child.GetSpawns(rand, entMan, proto, ctx).GetEnumerator();
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      enumerator = (IEnumerator<EntProtoId>) null;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AllSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTableSelector target1 = (EntityTableSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AllSelector) target1;
    if (serialization.TryCustomCopy<AllSelector>(this, ref target, hookCtx, false, context))
      return;
    List<EntityTableSelector> target2 = (List<EntityTableSelector>) null;
    if (this.Children == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityTableSelector>>(this.Children, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityTableSelector>>(this.Children, hookCtx, context);
    target.Children = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AllSelector target,
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
    AllSelector target1 = (AllSelector) target;
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
    AllSelector target1 = (AllSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AllSelector EntityTableSelector.Instantiate() => new AllSelector();
}
