// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.ConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction.Steps;

[ImplicitDataDefinitionForInheritors]
[Serializable]
public abstract class ConstructionGraphStep : 
  ISerializationGenerated<ConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("completed", false, 1, false, true, null)]
  private IGraphAction[] _completed = Array.Empty<IGraphAction>();

  [DataField("doAfter", false, 1, false, false, null)]
  public float DoAfter { get; private set; }

  public IReadOnlyList<IGraphAction> Completed => (IReadOnlyList<IGraphAction>) this._completed;

  public abstract void DoExamine(ExaminedEvent examinedEvent);

  public abstract ConstructionGuideEntry GenerateGuideEntry();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref ConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    IGraphAction[] graphActionArray = (IGraphAction[]) null;
    if (this._completed == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IGraphAction[]>(this._completed, ref graphActionArray, hookCtx, true, context))
      graphActionArray = serialization.CreateCopy<IGraphAction[]>(this._completed, hookCtx, context, false);
    target._completed = graphActionArray;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DoAfter, ref num, hookCtx, false, context))
      num = this.DoAfter;
    target.DoAfter = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref ConstructionGraphStep target,
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
    ConstructionGraphStep target1 = (ConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual ConstructionGraphStep Instantiate() => throw new NotImplementedException();
}
