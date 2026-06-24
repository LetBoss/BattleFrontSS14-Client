// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.ConstructionGraphEdge
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Steps;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction;

[DataDefinition]
[Serializable]
public sealed class ConstructionGraphEdge : 
  ISerializationGenerated<ConstructionGraphEdge>,
  ISerializationGenerated
{
  [DataField("steps", false, 1, false, false, null)]
  private ConstructionGraphStep[] _steps = Array.Empty<ConstructionGraphStep>();
  [DataField("conditions", false, 1, false, true, null)]
  private IGraphCondition[] _conditions = Array.Empty<IGraphCondition>();
  [DataField("completed", false, 1, false, true, null)]
  private IGraphAction[] _completed = Array.Empty<IGraphAction>();

  [DataField("to", false, 1, true, false, null)]
  public string Target { get; private set; } = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<IGraphCondition> Conditions
  {
    get => (IReadOnlyList<IGraphCondition>) this._conditions;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<IGraphAction> Completed => (IReadOnlyList<IGraphAction>) this._completed;

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<ConstructionGraphStep> Steps
  {
    get => (IReadOnlyList<ConstructionGraphStep>) this._steps;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConstructionGraphEdge target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ConstructionGraphEdge>(this, ref target, hookCtx, false, context))
      return;
    ConstructionGraphStep[] constructionGraphStepArray = (ConstructionGraphStep[]) null;
    if (this._steps == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ConstructionGraphStep[]>(this._steps, ref constructionGraphStepArray, hookCtx, true, context))
      constructionGraphStepArray = serialization.CreateCopy<ConstructionGraphStep[]>(this._steps, hookCtx, context, false);
    target._steps = constructionGraphStepArray;
    IGraphCondition[] graphConditionArray = (IGraphCondition[]) null;
    if (this._conditions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IGraphCondition[]>(this._conditions, ref graphConditionArray, hookCtx, true, context))
      graphConditionArray = serialization.CreateCopy<IGraphCondition[]>(this._conditions, hookCtx, context, false);
    target._conditions = graphConditionArray;
    IGraphAction[] graphActionArray = (IGraphAction[]) null;
    if (this._completed == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IGraphAction[]>(this._completed, ref graphActionArray, hookCtx, true, context))
      graphActionArray = serialization.CreateCopy<IGraphAction[]>(this._completed, hookCtx, context, false);
    target._completed = graphActionArray;
    string str = (string) null;
    if (this.Target == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Target, ref str, hookCtx, false, context))
      str = this.Target;
    target.Target = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConstructionGraphEdge target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ConstructionGraphEdge target1 = (ConstructionGraphEdge) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ConstructionGraphEdge Instantiate() => new ConstructionGraphEdge();
}
