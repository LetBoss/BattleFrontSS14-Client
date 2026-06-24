// Decompiled with JetBrains decompiler
// Type: Content.Shared.NodeContainer.Node
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.NodeContainer.NodeGroups;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.NodeContainer;

[ImplicitDataDefinitionForInheritors]
public abstract class Node : ISerializationGenerated<Node>, ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public INodeGroup? NodeGroup;
  public bool Deleting;
  public readonly HashSet<Node> ReachableNodes = new HashSet<Node>();
  public int FloodGen;
  public int UndirectGen;
  public bool FlaggedForFlood;
  public int NetId;
  public string Name;

  [DataField("nodeGroupID", false, 1, false, false, null)]
  public NodeGroupID NodeGroupID { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid Owner { get; private set; }

  public virtual bool Connectable(IEntityManager entMan, TransformComponent? xform = null)
  {
    if (this.Deleting || entMan.IsQueuedForDeletion(this.Owner))
      return false;
    if (!this.NeedAnchored)
      return true;
    if (xform == null)
      xform = entMan.GetComponent<TransformComponent>(this.Owner);
    return xform.Anchored;
  }

  [DataField(null, false, 1, false, false, null)]
  public bool NeedAnchored { get; private set; } = true;

  public virtual void OnAnchorStateChanged(IEntityManager entityManager, bool anchored)
  {
  }

  public virtual void Initialize(EntityUid owner, IEntityManager entMan) => this.Owner = owner;

  public abstract IEnumerable<Node> GetReachableNodes(
    TransformComponent xform,
    EntityQuery<NodeContainerComponent> nodeQuery,
    EntityQuery<TransformComponent> xformQuery,
    MapGridComponent? grid,
    IEntityManager entMan);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref Node target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Node>(this, ref target, hookCtx, false, context))
      return;
    NodeGroupID target1 = NodeGroupID.Default;
    if (!serialization.TryCustomCopy<NodeGroupID>(this.NodeGroupID, ref target1, hookCtx, false, context))
      target1 = this.NodeGroupID;
    target.NodeGroupID = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedAnchored, ref target2, hookCtx, false, context))
      target2 = this.NeedAnchored;
    target.NeedAnchored = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Node target,
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
    Node target1 = (Node) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual Node Instantiate() => throw new NotImplementedException();
}
