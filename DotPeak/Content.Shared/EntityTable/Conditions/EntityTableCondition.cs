// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.Conditions.EntityTableCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.EntityTable.Conditions;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityTableCondition : 
  ISerializationGenerated<EntityTableCondition>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Invert;

  public bool Evaluate(
    EntityTableSelector root,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx)
  {
    return this.EvaluateImplementation(root, entMan, proto, ctx) ^ this.Invert;
  }

  protected abstract bool EvaluateImplementation(
    EntityTableSelector root,
    IEntityManager entMan,
    IPrototypeManager proto,
    EntityTableContext ctx);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EntityTableCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntityTableCondition>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Invert, ref target1, hookCtx, false, context))
      target1 = this.Invert;
    target.Invert = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EntityTableCondition target,
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
    EntityTableCondition target1 = (EntityTableCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual EntityTableCondition Instantiate() => throw new NotImplementedException();
}
