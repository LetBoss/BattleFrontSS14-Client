// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntitySelectors.EntityTableSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.Conditions;
using Content.Shared.EntityTable.ValueSelector;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.EntityTable.EntitySelectors;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityTableSelector : 
  ISerializationGenerated<EntityTableSelector>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NumberSelector Rolls = (NumberSelector) new ConstantNumberSelector(1);
  [DataField(null, false, 1, false, false, null)]
  public float Weight = 1f;
  [DataField(null, false, 1, false, false, null)]
  public double Prob = 1.0;
  [DataField(null, false, 1, false, false, null)]
  public List<EntityTableCondition> Conditions = new List<EntityTableCondition>();
  [DataField(null, false, 1, false, false, null)]
  public bool RequireAll = true;

  public IEnumerable<EntProtoId> GetSpawns(
    Random rand,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    if (this.CheckConditions(entMan, proto, ctx))
    {
      int rolls = this.Rolls.Get(rand);
      for (int i = 0; i < rolls; ++i)
      {
        if (rand.NextDouble() < this.Prob)
        {
          foreach (EntProtoId spawn in this.GetSpawnsImplementation(rand, entMan, proto, ctx))
            yield return spawn;
        }
      }
    }
  }

  public bool CheckConditions(
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    if (this.Conditions.Count == 0)
      return true;
    bool flag1 = false;
    foreach (EntityTableCondition condition in this.Conditions)
    {
      bool flag2 = condition.Evaluate(this, entMan, proto, ctx);
      if (this.RequireAll && !flag2)
        return false;
      flag1 |= flag2;
    }
    return this.RequireAll || flag1;
  }

  protected abstract IEnumerable<EntProtoId> GetSpawnsImplementation(
    Random rand,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EntityTableSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntityTableSelector>(this, ref target, hookCtx, false, context))
      return;
    NumberSelector target1 = (NumberSelector) null;
    if (this.Rolls == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<NumberSelector>(this.Rolls, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<NumberSelector>(this.Rolls, hookCtx, context);
    target.Rolls = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Weight, ref target2, hookCtx, false, context))
      target2 = this.Weight;
    target.Weight = target2;
    double target3 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Prob, ref target3, hookCtx, false, context))
      target3 = this.Prob;
    target.Prob = target3;
    List<EntityTableCondition> target4 = (List<EntityTableCondition>) null;
    if (this.Conditions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityTableCondition>>(this.Conditions, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntityTableCondition>>(this.Conditions, hookCtx, context);
    target.Conditions = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireAll, ref target5, hookCtx, false, context))
      target5 = this.RequireAll;
    target.RequireAll = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EntityTableSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTableSelector target1 = (EntityTableSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual EntityTableSelector Instantiate() => throw new NotImplementedException();
}
