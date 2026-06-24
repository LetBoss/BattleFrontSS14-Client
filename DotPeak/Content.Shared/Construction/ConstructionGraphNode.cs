// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.ConstructionGraphNode
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.NodeEntities;
using Content.Shared.Construction.Serialization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Construction;

[DataDefinition]
[Serializable]
public sealed class ConstructionGraphNode : 
  ISerializationGenerated<ConstructionGraphNode>,
  ISerializationGenerated
{
  [DataField("actions", false, 1, false, true, null)]
  private IGraphAction[] _actions = Array.Empty<IGraphAction>();
  [DataField("edges", false, 1, false, false, null)]
  private ConstructionGraphEdge[] _edges = Array.Empty<ConstructionGraphEdge>();
  [DataField("transform", false, 1, false, false, null)]
  public IGraphTransform[] TransformLogic = Array.Empty<IGraphTransform>();
  [DataField("doNotReplaceInheritingEntities", false, 1, false, false, null)]
  public bool DoNotReplaceInheritingEntities;

  [DataField("node", false, 1, true, false, null)]
  public string Name { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<ConstructionGraphEdge> Edges
  {
    get => (IReadOnlyList<ConstructionGraphEdge>) this._edges;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<IGraphAction> Actions => (IReadOnlyList<IGraphAction>) this._actions;

  [DataField("entity", false, 1, false, false, typeof (GraphNodeEntitySerializer))]
  public IGraphNodeEntity Entity { get; private set; } = (IGraphNodeEntity) new NullNodeEntity();

  public ConstructionGraphEdge? GetEdge(string target)
  {
    foreach (ConstructionGraphEdge edge in this._edges)
    {
      if (edge.Target == target)
        return edge;
    }
    return (ConstructionGraphEdge) null;
  }

  public int? GetEdgeIndex(string target)
  {
    for (int index = 0; index < this._edges.Length; ++index)
    {
      if (this._edges[index].Target == target)
        return new int?(index);
    }
    return new int?();
  }

  public bool TryGetEdge(string target, [NotNullWhen(true)] out ConstructionGraphEdge? edge)
  {
    return (edge = this.GetEdge(target)) != null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConstructionGraphNode target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ConstructionGraphNode>(this, ref target, hookCtx, false, context))
      return;
    IGraphAction[] graphActionArray = (IGraphAction[]) null;
    if (this._actions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IGraphAction[]>(this._actions, ref graphActionArray, hookCtx, true, context))
      graphActionArray = serialization.CreateCopy<IGraphAction[]>(this._actions, hookCtx, context, false);
    target._actions = graphActionArray;
    ConstructionGraphEdge[] constructionGraphEdgeArray = (ConstructionGraphEdge[]) null;
    if (this._edges == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ConstructionGraphEdge[]>(this._edges, ref constructionGraphEdgeArray, hookCtx, true, context))
      constructionGraphEdgeArray = serialization.CreateCopy<ConstructionGraphEdge[]>(this._edges, hookCtx, context, false);
    target._edges = constructionGraphEdgeArray;
    string str = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref str, hookCtx, false, context))
      str = this.Name;
    target.Name = str;
    IGraphTransform[] graphTransformArray = (IGraphTransform[]) null;
    if (this.TransformLogic == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IGraphTransform[]>(this.TransformLogic, ref graphTransformArray, hookCtx, true, context))
      graphTransformArray = serialization.CreateCopy<IGraphTransform[]>(this.TransformLogic, hookCtx, context, false);
    target.TransformLogic = graphTransformArray;
    IGraphNodeEntity graphNodeEntity = (IGraphNodeEntity) null;
    if (this.Entity == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IGraphNodeEntity>(this.Entity, ref graphNodeEntity, hookCtx, true, context))
      graphNodeEntity = serialization.CreateCopy<IGraphNodeEntity>(this.Entity, hookCtx, context, false);
    target.Entity = graphNodeEntity;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.DoNotReplaceInheritingEntities, ref flag, hookCtx, false, context))
      flag = this.DoNotReplaceInheritingEntities;
    target.DoNotReplaceInheritingEntities = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConstructionGraphNode target,
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
    ConstructionGraphNode target1 = (ConstructionGraphNode) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ConstructionGraphNode Instantiate() => new ConstructionGraphNode();
}
