// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.PartAssemblyConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class PartAssemblyConstructionGraphStep : 
  ConstructionGraphStep,
  ISerializationGenerated<PartAssemblyConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string AssemblyId = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public LocId GuideString = LocId.op_Implicit("construction-guide-condition-part-assembly");

  public bool Condition(EntityUid uid, IEntityManager entityManager)
  {
    return entityManager.System<PartAssemblySystem>().IsAssemblyFinished(uid, this.AssemblyId);
  }

  public override void DoExamine(ExaminedEvent args)
  {
    args.PushMarkup(Loc.GetString(LocId.op_Implicit(this.GuideString)));
  }

  public override ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = LocId.op_Implicit(this.GuideString)
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PartAssemblyConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ConstructionGraphStep target1 = (ConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PartAssemblyConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<PartAssemblyConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.AssemblyId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AssemblyId, ref str, hookCtx, false, context))
      str = this.AssemblyId;
    target.AssemblyId = str;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.GuideString, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.GuideString, hookCtx, context, false);
    target.GuideString = locId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PartAssemblyConstructionGraphStep target,
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
    PartAssemblyConstructionGraphStep target1 = (PartAssemblyConstructionGraphStep) target;
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
    PartAssemblyConstructionGraphStep target1 = (PartAssemblyConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PartAssemblyConstructionGraphStep ConstructionGraphStep.Instantiate()
  {
    return new PartAssemblyConstructionGraphStep();
  }
}
