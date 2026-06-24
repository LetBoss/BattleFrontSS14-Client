// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.EntityInsertConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityInsertConstructionGraphStep : 
  ConstructionGraphStep,
  ISerializationGenerated<EntityInsertConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("store", false, 1, false, false, null)]
  public string Store { get; private set; } = string.Empty;

  public abstract bool EntityValid(
    EntityUid uid,
    IEntityManager entityManager,
    IComponentFactory compFactory);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref EntityInsertConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ConstructionGraphStep target1 = (ConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntityInsertConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<EntityInsertConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Store == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Store, ref str, hookCtx, false, context))
      str = this.Store;
    target.Store = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref EntityInsertConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityInsertConstructionGraphStep target1 = (EntityInsertConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ConstructionGraphStep) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityInsertConstructionGraphStep target1 = (EntityInsertConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EntityInsertConstructionGraphStep ConstructionGraphStep.Instantiate()
  {
    throw new NotImplementedException();
  }
}
