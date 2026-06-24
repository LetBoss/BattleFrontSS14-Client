// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.IGraphCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction;

[ImplicitDataDefinitionForInheritors]
public interface IGraphCondition : ISerializationGenerated<IGraphCondition>, ISerializationGenerated
{
  bool Condition(EntityUid uid, IEntityManager entityManager);

  bool DoExamine(ExaminedEvent args);

  IEnumerable<ConstructionGuideEntry> GenerateGuideEntry();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void InternalCopy(
    ref IGraphCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<IGraphCondition>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void Copy(
    ref IGraphCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IGraphCondition target1 = (IGraphCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  IGraphCondition Instantiate() => throw new NotImplementedException();
}
