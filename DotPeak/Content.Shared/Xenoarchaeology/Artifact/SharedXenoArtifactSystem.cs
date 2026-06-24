// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.SharedXenoArtifactSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.EntityTable;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.NameIdentifier;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.Prototypes;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact;

public abstract class SharedXenoArtifactSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  protected IRobustRandom RobustRandom;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private EntityTableSystem _entityTable;
  private Robust.Shared.GameObjects.EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoArtifactNodeComponent> _nodeQuery;
  [Dependency]
  private SharedAudioSystem _audio;
  private Robust.Shared.GameObjects.EntityQuery<XenoArtifactUnlockingComponent> _unlockingQuery;
  [Dependency]
  private UseDelaySystem _useDelay;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoArtifactComponent, ComponentStartup>(new EntityEventRefHandler<XenoArtifactComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<XenoArtifactComponent, ArtifactSelfActivateEvent>(new EntityEventRefHandler<XenoArtifactComponent, ArtifactSelfActivateEvent>(this.OnSelfActivate));
    this.InitializeNode();
    this.InitializeUnlock();
    this.InitializeXAT();
    this.InitializeXAE();
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this.UpdateUnlock(frameTime);
  }

  private void OnStartup(Entity<XenoArtifactComponent> ent, ref ComponentStartup args)
  {
    this._actions.AddAction((EntityUid) ent, (string) ent.Comp.SelfActivateAction);
    ent.Comp.NodeContainer = this._container.EnsureContainer<Container>((EntityUid) ent, XenoArtifactComponent.NodeContainerId);
  }

  private void OnSelfActivate(Entity<XenoArtifactComponent> ent, ref ArtifactSelfActivateEvent args)
  {
    args.Handled = this.TryActivateXenoArtifact(ent, new EntityUid?((EntityUid) ent), new EntityUid?(), this.Transform((EntityUid) ent).Coordinates, false);
  }

  public void SetSuppressed(Entity<XenoArtifactComponent> ent, bool val)
  {
    if (ent.Comp.Suppressed == val)
      return;
    ent.Comp.Suppressed = val;
    this.Dirty<XenoArtifactComponent>(ent);
  }

  public int GetIndex(Entity<XenoArtifactComponent> ent, EntityUid node)
  {
    int? index;
    if (this.TryGetIndex((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), node, out index))
      return index.Value;
    throw new ArgumentException($"node {this.ToPrettyString((Entity<MetaDataComponent>) node)} is not present in {this.ToPrettyString(new EntityUid?((EntityUid) ent))}");
  }

  public bool TryGetIndex(Entity<XenoArtifactComponent?> ent, EntityUid node, [NotNullWhen(true)] out int? index)
  {
    index = new int?();
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    for (int index1 = 0; index1 < ent.Comp.NodeVertices.Length; ++index1)
    {
      Entity<XenoArtifactNodeComponent>? node1;
      if (this.TryGetNode(ent, index1, out node1) && !(node != node1.Value.Owner))
      {
        index = new int?(index1);
        return true;
      }
    }
    return false;
  }

  public Entity<XenoArtifactNodeComponent> GetNode(Entity<XenoArtifactComponent> ent, int index)
  {
    EntityUid entity = this.GetEntity(ent.Comp.NodeVertices[index] ?? throw new ArgumentException($"index {index} does not correspond to an existing node in {this.ToPrettyString(new EntityUid?((EntityUid) ent))}"));
    return (Entity<XenoArtifactNodeComponent>) (entity, this.XenoArtifactNode(entity));
  }

  public bool TryGetNode(
    Entity<XenoArtifactComponent?> ent,
    int index,
    [NotNullWhen(true)] out Entity<XenoArtifactNodeComponent>? node)
  {
    node = new Entity<XenoArtifactNodeComponent>?();
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp) || index < 0 || index >= ent.Comp.NodeVertices.Length)
      return false;
    NetEntity? nodeVertex = ent.Comp.NodeVertices[index];
    if (nodeVertex.HasValue)
    {
      EntityUid entity = this.GetEntity(nodeVertex.GetValueOrDefault());
      node = new Entity<XenoArtifactNodeComponent>?((Entity<XenoArtifactNodeComponent>) (entity, this.XenoArtifactNode(entity)));
    }
    return node.HasValue;
  }

  public int GetFreeNodeIndex(Entity<XenoArtifactComponent> ent)
  {
    int length = ent.Comp.NodeVertices.Length;
    for (int freeNodeIndex = 0; freeNodeIndex < length; ++freeNodeIndex)
    {
      if (!ent.Comp.NodeVertices[freeNodeIndex].HasValue)
        return freeNodeIndex;
    }
    this.ResizeNodeGraph(ent, length + 1);
    return length;
  }

  public IEnumerable<Entity<XenoArtifactNodeComponent>> GetAllNodes(
    Entity<XenoArtifactComponent> ent)
  {
    SharedXenoArtifactSystem xenoArtifactSystem = this;
    NetEntity?[] nullableArray = ent.Comp.NodeVertices;
    for (int index = 0; index < nullableArray.Length; ++index)
    {
      NetEntity? nEntity = nullableArray[index];
      EntityUid? entity;
      if (xenoArtifactSystem.TryGetEntity(nEntity, out entity))
        yield return (Entity<XenoArtifactNodeComponent>) (entity.Value, xenoArtifactSystem.XenoArtifactNode(entity.Value));
    }
    nullableArray = (NetEntity?[]) null;
  }

  public IEnumerable<int> GetAllNodeIndices(Entity<XenoArtifactComponent> ent)
  {
    for (int i = 0; i < ent.Comp.NodeVertices.Length; ++i)
    {
      if (ent.Comp.NodeVertices[i].HasValue)
        yield return i;
    }
  }

  public bool AddEdge(Entity<XenoArtifactComponent?> ent, EntityUid from, EntityUid to, bool dirty = true)
  {
    int? index1;
    int? index2;
    return this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp) && this.TryGetIndex(ent, from, out index1) && this.TryGetIndex(ent, to, out index2) && this.AddEdge(ent, index1.Value, index2.Value, dirty);
  }

  public bool AddEdge(Entity<XenoArtifactComponent?> ent, int fromIdx, int toIdx, bool dirty = true)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp) || ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx])
      return false;
    ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx] = true;
    if (dirty)
      this.RebuildXenoArtifactMetaData(ent);
    return true;
  }

  public bool RemoveEdge(
    Entity<XenoArtifactComponent?> ent,
    EntityUid from,
    EntityUid to,
    bool dirty = true)
  {
    int? index1;
    int? index2;
    return this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp) && this.TryGetIndex(ent, from, out index1) && this.TryGetIndex(ent, to, out index2) && this.RemoveEdge(ent, index1.Value, index2.Value, dirty);
  }

  public bool RemoveEdge(Entity<XenoArtifactComponent?> ent, int fromIdx, int toIdx, bool dirty = true)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp) || !ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx])
      return false;
    ent.Comp.NodeAdjacencyMatrix[fromIdx][toIdx] = false;
    if (dirty)
      this.RebuildXenoArtifactMetaData(ent);
    return true;
  }

  public bool AddNode(
    Entity<XenoArtifactComponent?> ent,
    EntProtoId entProtoId,
    [NotNullWhen(true)] out Entity<XenoArtifactNodeComponent>? node,
    bool dirty = true)
  {
    node = new Entity<XenoArtifactNodeComponent>?();
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    EntityUid uid = this.Spawn((string) entProtoId);
    node = new Entity<XenoArtifactNodeComponent>?((Entity<XenoArtifactNodeComponent>) (uid, this.XenoArtifactNode(uid)));
    return this.AddNode(ent, (Entity<XenoArtifactNodeComponent>) ((EntityUid) node.Value, node.Value.Comp), dirty);
  }

  public bool AddNode(
    Entity<XenoArtifactComponent?> ent,
    Entity<XenoArtifactNodeComponent?> node,
    bool dirty = true)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    ref XenoArtifactNodeComponent local = ref node.Comp;
    if (local == null)
      local = this.XenoArtifactNode((EntityUid) node);
    node.Comp.Attached = new NetEntity?(this.GetNetEntity((EntityUid) ent));
    int freeNodeIndex = this.GetFreeNodeIndex((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp));
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) node.Owner, (BaseContainer) ent.Comp.NodeContainer);
    ent.Comp.NodeVertices[freeNodeIndex] = new NetEntity?(this.GetNetEntity((EntityUid) node));
    this.Dirty<XenoArtifactNodeComponent>(node);
    if (dirty)
      this.RebuildXenoArtifactMetaData(ent);
    return true;
  }

  public bool RemoveNode(
    Entity<XenoArtifactComponent?> ent,
    Entity<XenoArtifactNodeComponent?> node,
    bool dirty = true)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    ref XenoArtifactNodeComponent local = ref node.Comp;
    if (local == null)
      local = this.XenoArtifactNode((EntityUid) node);
    int? index;
    if (!this.TryGetIndex(ent, (EntityUid) node, out index))
      return false;
    this.RemoveAllNodeEdges(ent, index.Value, false);
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) node.Owner, (BaseContainer) ent.Comp.NodeContainer);
    node.Comp.Attached = new NetEntity?();
    ent.Comp.NodeVertices[index.Value] = new NetEntity?();
    if (dirty)
      this.RebuildXenoArtifactMetaData(ent);
    this.Dirty<XenoArtifactNodeComponent>(node);
    return true;
  }

  public void RemoveAllNodeEdges(Entity<XenoArtifactComponent?> ent, int nodeIdx, bool dirty = true)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return;
    foreach (int directPredecessorNode in this.GetDirectPredecessorNodes(ent, nodeIdx))
      this.RemoveEdge(ent, directPredecessorNode, nodeIdx, false);
    foreach (int directSuccessorNode in this.GetDirectSuccessorNodes(ent, nodeIdx))
      this.RemoveEdge(ent, nodeIdx, directSuccessorNode, false);
    if (!dirty)
      return;
    this.RebuildXenoArtifactMetaData(ent);
  }

  public HashSet<Entity<XenoArtifactNodeComponent>> GetDirectPredecessorNodes(
    Entity<XenoArtifactComponent?> ent,
    EntityUid node)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp, false))
      return new HashSet<Entity<XenoArtifactNodeComponent>>();
    int? index1;
    if (!this.TryGetIndex(ent, node, out index1))
      return new HashSet<Entity<XenoArtifactNodeComponent>>();
    HashSet<int> predecessorNodes1 = this.GetDirectPredecessorNodes(ent, index1.Value);
    HashSet<Entity<XenoArtifactNodeComponent>> predecessorNodes2 = new HashSet<Entity<XenoArtifactNodeComponent>>();
    foreach (int index2 in predecessorNodes1)
    {
      Entity<XenoArtifactNodeComponent>? node1;
      if (this.TryGetNode(ent, index2, out node1))
        predecessorNodes2.Add(node1.Value);
    }
    return predecessorNodes2;
  }

  public HashSet<int> GetDirectPredecessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<int>();
    HashSet<int> predecessorNodes = new HashSet<int>();
    for (int index = 0; index < ent.Comp.NodeAdjacencyMatrixRows; ++index)
    {
      if (ent.Comp.NodeAdjacencyMatrix[index][nodeIdx])
        predecessorNodes.Add(index);
    }
    return predecessorNodes;
  }

  public HashSet<Entity<XenoArtifactNodeComponent>> GetDirectSuccessorNodes(
    Entity<XenoArtifactComponent?> ent,
    EntityUid node)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<Entity<XenoArtifactNodeComponent>>();
    int? index1;
    if (!this.TryGetIndex(ent, node, out index1))
      return new HashSet<Entity<XenoArtifactNodeComponent>>();
    HashSet<int> directSuccessorNodes1 = this.GetDirectSuccessorNodes(ent, index1.Value);
    HashSet<Entity<XenoArtifactNodeComponent>> directSuccessorNodes2 = new HashSet<Entity<XenoArtifactNodeComponent>>();
    foreach (int index2 in directSuccessorNodes1)
    {
      Entity<XenoArtifactNodeComponent>? node1;
      if (this.TryGetNode(ent, index2, out node1))
        directSuccessorNodes2.Add(node1.Value);
    }
    return directSuccessorNodes2;
  }

  public HashSet<int> GetDirectSuccessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<int>();
    HashSet<int> directSuccessorNodes = new HashSet<int>();
    for (int index = 0; index < ent.Comp.NodeAdjacencyMatrixColumns; ++index)
    {
      if (ent.Comp.NodeAdjacencyMatrix[nodeIdx][index])
        directSuccessorNodes.Add(index);
    }
    return directSuccessorNodes;
  }

  public HashSet<Entity<XenoArtifactNodeComponent>> GetPredecessorNodes(
    Entity<XenoArtifactComponent?> ent,
    Entity<XenoArtifactNodeComponent> node)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<Entity<XenoArtifactNodeComponent>>();
    HashSet<int> predecessorNodes1 = this.GetPredecessorNodes(ent, this.GetIndex((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), (EntityUid) node));
    HashSet<Entity<XenoArtifactNodeComponent>> predecessorNodes2 = new HashSet<Entity<XenoArtifactNodeComponent>>();
    foreach (int index in predecessorNodes1)
      predecessorNodes2.Add(this.GetNode((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), index));
    return predecessorNodes2;
  }

  public HashSet<int> GetPredecessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<int>();
    HashSet<int> predecessorNodes1 = this.GetDirectPredecessorNodes(ent, nodeIdx);
    if (predecessorNodes1.Count == 0)
      return new HashSet<int>();
    HashSet<int> predecessorNodes2 = new HashSet<int>();
    foreach (int nodeIdx1 in predecessorNodes1)
    {
      predecessorNodes2.Add(nodeIdx1);
      foreach (int predecessorNode in this.GetPredecessorNodes(ent, nodeIdx1))
        predecessorNodes2.Add(predecessorNode);
    }
    return predecessorNodes2;
  }

  public HashSet<Entity<XenoArtifactNodeComponent>> GetSuccessorNodes(
    Entity<XenoArtifactComponent?> ent,
    Entity<XenoArtifactNodeComponent> node)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<Entity<XenoArtifactNodeComponent>>();
    HashSet<int> successorNodes1 = this.GetSuccessorNodes(ent, this.GetIndex((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), (EntityUid) node));
    HashSet<Entity<XenoArtifactNodeComponent>> successorNodes2 = new HashSet<Entity<XenoArtifactNodeComponent>>();
    foreach (int index in successorNodes1)
      successorNodes2.Add(this.GetNode((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), index));
    return successorNodes2;
  }

  public HashSet<int> GetSuccessorNodes(Entity<XenoArtifactComponent?> ent, int nodeIdx)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return new HashSet<int>();
    HashSet<int> directSuccessorNodes = this.GetDirectSuccessorNodes(ent, nodeIdx);
    if (directSuccessorNodes.Count == 0)
      return new HashSet<int>();
    HashSet<int> successorNodes = new HashSet<int>();
    foreach (int nodeIdx1 in directSuccessorNodes)
    {
      successorNodes.Add(nodeIdx1);
      foreach (int successorNode in this.GetSuccessorNodes(ent, nodeIdx1))
        successorNodes.Add(successorNode);
    }
    return successorNodes;
  }

  public bool NodeHasEdge(
    Entity<XenoArtifactComponent?> ent,
    Entity<XenoArtifactNodeComponent?> from,
    Entity<XenoArtifactNodeComponent?> to)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    int index1 = this.GetIndex((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), (EntityUid) from);
    int index2 = this.GetIndex((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), (EntityUid) to);
    return ent.Comp.NodeAdjacencyMatrix[index1][index2];
  }

  protected void ResizeNodeGraph(Entity<XenoArtifactComponent> ent, int newSize)
  {
    Array.Resize<NetEntity?>(ref ent.Comp.NodeVertices, newSize);
    while (ent.Comp.NodeAdjacencyMatrix.Count < newSize)
      ent.Comp.NodeAdjacencyMatrix.Add(new List<bool>());
    foreach (List<bool> boolList in ent.Comp.NodeAdjacencyMatrix)
    {
      while (boolList.Count < newSize)
        boolList.Add(false);
    }
    this.Dirty<XenoArtifactComponent>(ent);
  }

  private void CancelUnlockingOnGraphStructureChange(Entity<XenoArtifactComponent> ent)
  {
    XenoArtifactUnlockingComponent comp;
    if (!this.TryComp<XenoArtifactUnlockingComponent>((EntityUid) ent, out comp))
      return;
    this.CancelUnlockingState((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>) ((EntityUid) ent, comp, ent.Comp));
  }

  private void InitializeNode()
  {
    this.SubscribeLocalEvent<XenoArtifactNodeComponent, MapInitEvent>(new EntityEventRefHandler<XenoArtifactNodeComponent, MapInitEvent>(this.OnNodeMapInit));
    this._xenoArtifactQuery = this.GetEntityQuery<XenoArtifactComponent>();
    this._nodeQuery = this.GetEntityQuery<XenoArtifactNodeComponent>();
  }

  private void OnNodeMapInit(Entity<XenoArtifactNodeComponent> ent, ref MapInitEvent args)
  {
    XenoArtifactNodeComponent artifactNodeComponent = (XenoArtifactNodeComponent) ent;
    artifactNodeComponent.MaxDurability -= artifactNodeComponent.MaxDurabilityCanDecreaseBy.Next(this.RobustRandom);
    this.SetNodeDurability((Entity<XenoArtifactNodeComponent>) ((EntityUid) ent, (XenoArtifactNodeComponent) ent), artifactNodeComponent.MaxDurability);
  }

  public XenoArtifactNodeComponent XenoArtifactNode(EntityUid uid)
  {
    return (XenoArtifactNodeComponent) this._nodeQuery.Get(uid);
  }

  public void SetNodeUnlocked(Entity<XenoArtifactNodeComponent?> ent)
  {
    if (!this.Resolve<XenoArtifactNodeComponent>((EntityUid) ent, ref ent.Comp))
      return;
    NetEntity? attached = ent.Comp.Attached;
    if (!attached.HasValue)
      return;
    EntityUid entity = this.GetEntity(attached.GetValueOrDefault());
    XenoArtifactComponent comp;
    if (!this.TryComp<XenoArtifactComponent>(entity, out comp))
      return;
    this.SetNodeUnlocked((Entity<XenoArtifactComponent>) (entity, comp), (Entity<XenoArtifactNodeComponent>) ((EntityUid) ent, ent.Comp));
  }

  public void SetNodeUnlocked(
    Entity<XenoArtifactComponent> artifact,
    Entity<XenoArtifactNodeComponent> node)
  {
    if (!node.Comp.Locked)
      return;
    node.Comp.Locked = false;
    this.RebuildCachedActiveNodes((Entity<XenoArtifactComponent>) ((EntityUid) artifact, (XenoArtifactComponent) artifact));
    this.Dirty<XenoArtifactNodeComponent>(node);
  }

  public void AdjustNodeDurability(Entity<XenoArtifactNodeComponent?> ent, int durabilityDelta)
  {
    if (!this.Resolve<XenoArtifactNodeComponent>((EntityUid) ent, ref ent.Comp))
      return;
    this.SetNodeDurability(ent, ent.Comp.Durability + durabilityDelta);
  }

  public void SetNodeDurability(Entity<XenoArtifactNodeComponent?> ent, int durability)
  {
    if (!this.Resolve<XenoArtifactNodeComponent>((EntityUid) ent, ref ent.Comp))
      return;
    ent.Comp.Durability = Math.Clamp(durability, 0, ent.Comp.MaxDurability);
    this.UpdateNodeResearchValue((Entity<XenoArtifactNodeComponent>) ((EntityUid) ent, ent.Comp));
    this.Dirty<XenoArtifactNodeComponent>(ent);
  }

  public Entity<XenoArtifactNodeComponent> CreateNode(
    Entity<XenoArtifactComponent> ent,
    ProtoId<XenoArchTriggerPrototype> trigger,
    int depth = 0)
  {
    XenoArchTriggerPrototype trigger1 = this.PrototypeManager.Index<XenoArchTriggerPrototype>(trigger);
    return this.CreateNode(ent, trigger1, depth);
  }

  public Entity<XenoArtifactNodeComponent> CreateNode(
    Entity<XenoArtifactComponent> ent,
    XenoArchTriggerPrototype trigger,
    int depth = 0)
  {
    EntProtoId entProtoId = this._entityTable.GetSpawns(ent.Comp.EffectsTable).First<EntProtoId>();
    Entity<XenoArtifactNodeComponent>? node;
    this.AddNode((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), entProtoId, out node, false);
    XenoArtifactNodeComponent comp = node.Value.Comp;
    comp.Depth = depth;
    comp.TriggerTip = new LocId?(trigger.Tip);
    this.EntityManager.AddComponents((EntityUid) node.Value, trigger.Components, true);
    this.Dirty<XenoArtifactNodeComponent>(node.Value);
    return node.Value;
  }

  public bool HasUnlockedPredecessor(Entity<XenoArtifactComponent> ent, EntityUid node)
  {
    HashSet<Entity<XenoArtifactNodeComponent>> predecessorNodes = this.GetDirectPredecessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), node);
    if (predecessorNodes.Count == 0)
      return true;
    foreach (Entity<XenoArtifactNodeComponent> entity in predecessorNodes)
    {
      if (entity.Comp.Locked)
        return false;
    }
    return true;
  }

  public bool IsNodeActive(Entity<XenoArtifactComponent> ent, EntityUid node)
  {
    return ent.Comp.CachedActiveNodes.Contains(this.GetNetEntity(node));
  }

  public List<Entity<XenoArtifactNodeComponent>> GetActiveNodes(Entity<XenoArtifactComponent> ent)
  {
    return ent.Comp.CachedActiveNodes.Select<NetEntity, Entity<XenoArtifactNodeComponent>>((Func<NetEntity, Entity<XenoArtifactNodeComponent>>) (activeNode => this._nodeQuery.Get(this.GetEntity(activeNode)))).ToList<Entity<XenoArtifactNodeComponent>>();
  }

  public int GetResearchValue(Entity<XenoArtifactNodeComponent> ent)
  {
    return ent.Comp.Locked ? 0 : Math.Max(0, ent.Comp.ResearchValue - ent.Comp.ConsumedResearchValue);
  }

  public void SetConsumedResearchValue(Entity<XenoArtifactNodeComponent> ent, int value)
  {
    ent.Comp.ConsumedResearchValue = value;
    this.Dirty<XenoArtifactNodeComponent>(ent);
  }

  public string GetNodeId(EntityUid uid)
  {
    NameIdentifierComponent identifierComponent = this.CompOrNull<NameIdentifierComponent>(uid);
    return (identifierComponent != null ? identifierComponent.Identifier : 0).ToString("D3");
  }

  public List<List<Entity<XenoArtifactNodeComponent>>> GetSegments(Entity<XenoArtifactComponent> ent)
  {
    List<List<Entity<XenoArtifactNodeComponent>>> segments = new List<List<Entity<XenoArtifactNodeComponent>>>();
    foreach (List<NetEntity> cachedSegment in ent.Comp.CachedSegments)
    {
      List<Entity<XenoArtifactNodeComponent>> entityList = new List<Entity<XenoArtifactNodeComponent>>();
      foreach (NetEntity netEntity in cachedSegment)
      {
        EntityUid entity = this.GetEntity(netEntity);
        entityList.Add((Entity<XenoArtifactNodeComponent>) (entity, this.XenoArtifactNode(entity)));
      }
      segments.Add(entityList);
    }
    return segments;
  }

  public Dictionary<int, List<Entity<XenoArtifactNodeComponent>>> GetDepthOrderedNodes(
    IEnumerable<Entity<XenoArtifactNodeComponent>> nodes)
  {
    Dictionary<int, List<Entity<XenoArtifactNodeComponent>>> depthOrderedNodes = new Dictionary<int, List<Entity<XenoArtifactNodeComponent>>>();
    foreach (Entity<XenoArtifactNodeComponent> node in nodes)
    {
      List<Entity<XenoArtifactNodeComponent>> entityList;
      if (!depthOrderedNodes.TryGetValue(node.Comp.Depth, out entityList))
      {
        entityList = new List<Entity<XenoArtifactNodeComponent>>();
        depthOrderedNodes.Add(node.Comp.Depth, entityList);
      }
      entityList.Add(node);
    }
    return depthOrderedNodes;
  }

  public void RebuildXenoArtifactMetaData(Entity<XenoArtifactComponent?> artifact)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) artifact, ref artifact.Comp))
      return;
    this.RebuildCachedActiveNodes(artifact);
    this.RebuildCachedSegments(artifact);
    foreach (Entity<XenoArtifactNodeComponent> allNode in this.GetAllNodes((Entity<XenoArtifactComponent>) ((EntityUid) artifact, artifact.Comp)))
      this.RebuildNodeMetaData(allNode);
    this.CancelUnlockingOnGraphStructureChange((Entity<XenoArtifactComponent>) ((EntityUid) artifact, artifact.Comp));
  }

  public void RebuildNodeMetaData(Entity<XenoArtifactNodeComponent> node)
  {
    this.UpdateNodeResearchValue(node);
  }

  public void RebuildCachedActiveNodes(Entity<XenoArtifactComponent?> ent)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return;
    ent.Comp.CachedActiveNodes.Clear();
    foreach (Entity<XenoArtifactNodeComponent> allNode in this.GetAllNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp)))
    {
      if (!allNode.Comp.Locked)
      {
        HashSet<Entity<XenoArtifactNodeComponent>> directSuccessorNodes = this.GetDirectSuccessorNodes(ent, (EntityUid) allNode);
        if (directSuccessorNodes.Count != 0)
        {
          bool flag = false;
          foreach (Entity<XenoArtifactNodeComponent> entity in directSuccessorNodes)
          {
            if (!entity.Comp.Locked)
            {
              flag = true;
              break;
            }
          }
          if (flag)
            continue;
        }
        NetEntity netEntity = this.GetNetEntity((EntityUid) allNode);
        ent.Comp.CachedActiveNodes.Add(netEntity);
      }
    }
    this.Dirty<XenoArtifactComponent>(ent);
  }

  public void RebuildCachedSegments(Entity<XenoArtifactComponent?> ent)
  {
    if (!this.Resolve<XenoArtifactComponent>((EntityUid) ent, ref ent.Comp))
      return;
    ent.Comp.CachedSegments.Clear();
    List<Entity<XenoArtifactNodeComponent>> list = this.GetAllNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp)).ToList<Entity<XenoArtifactNodeComponent>>();
    IEnumerable<List<NetEntity>> collection = this.GetSegmentsFromNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, ent.Comp), list).Select<List<Entity<XenoArtifactNodeComponent>>, List<NetEntity>>((Func<List<Entity<XenoArtifactNodeComponent>>, List<NetEntity>>) (s => s.Select<Entity<XenoArtifactNodeComponent>, NetEntity>((Func<Entity<XenoArtifactNodeComponent>, NetEntity>) (n => this.GetNetEntity((EntityUid) n))).ToList<NetEntity>()));
    ent.Comp.CachedSegments.AddRange(collection);
    this.Dirty<XenoArtifactComponent>(ent);
  }

  public IEnumerable<List<Entity<XenoArtifactNodeComponent>>> GetSegmentsFromNodes(
    Entity<XenoArtifactComponent> ent,
    List<Entity<XenoArtifactNodeComponent>> nodes)
  {
    List<List<Entity<XenoArtifactNodeComponent>>> otherSegments = new List<List<Entity<XenoArtifactNodeComponent>>>();
    foreach (Entity<XenoArtifactNodeComponent> node in nodes)
    {
      List<Entity<XenoArtifactNodeComponent>> segment = new List<Entity<XenoArtifactNodeComponent>>();
      this.GetSegmentNodesRecursive(ent, node, segment, otherSegments);
      if (segment.Count != 0)
        otherSegments.Add(segment);
    }
    return (IEnumerable<List<Entity<XenoArtifactNodeComponent>>>) otherSegments;
  }

  private void GetSegmentNodesRecursive(
    Entity<XenoArtifactComponent> ent,
    Entity<XenoArtifactNodeComponent> node,
    List<Entity<XenoArtifactNodeComponent>> segment,
    List<List<Entity<XenoArtifactNodeComponent>>> otherSegments)
  {
    if (otherSegments.Any<List<Entity<XenoArtifactNodeComponent>>>((Func<List<Entity<XenoArtifactNodeComponent>>, bool>) (s => s.Contains(node))) || segment.Contains(node))
      return;
    segment.Add(node);
    foreach (Entity<XenoArtifactNodeComponent> directPredecessorNode in this.GetDirectPredecessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), (EntityUid) node))
      this.GetSegmentNodesRecursive(ent, directPredecessorNode, segment, otherSegments);
    foreach (Entity<XenoArtifactNodeComponent> directSuccessorNode in this.GetDirectSuccessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), (EntityUid) node))
      this.GetSegmentNodesRecursive(ent, directSuccessorNode, segment, otherSegments);
  }

  public void UpdateNodeResearchValue(Entity<XenoArtifactNodeComponent> node)
  {
    XenoArtifactNodeComponent artifactNodeComponent = (XenoArtifactNodeComponent) node;
    if (!artifactNodeComponent.Attached.HasValue)
    {
      artifactNodeComponent.ResearchValue = 0;
    }
    else
    {
      Entity<XenoArtifactComponent> ent = this._xenoArtifactQuery.Get(this.GetEntity(artifactNodeComponent.Attached.Value));
      List<Entity<XenoArtifactNodeComponent>> activeNodes = this.GetActiveNodes(ent);
      float num1 = MathF.Pow((float) artifactNodeComponent.Durability / (float) artifactNodeComponent.MaxDurability, 2f);
      Entity<XenoArtifactNodeComponent> entity = node;
      float num2 = activeNodes.Contains(entity) ? 1f - num1 : 1f + num1;
      HashSet<Entity<XenoArtifactNodeComponent>> predecessorNodes = this.GetPredecessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), node);
      artifactNodeComponent.ResearchValue = (int) (Math.Pow(1.25, Math.Pow((double) predecessorNodes.Count, 1.5)) * (double) artifactNodeComponent.BasePointValue * (double) num2);
    }
  }

  private void InitializeUnlock()
  {
    this._unlockingQuery = this.GetEntityQuery<XenoArtifactUnlockingComponent>();
    this.SubscribeLocalEvent<XenoArtifactUnlockingComponent, MapInitEvent>(new EntityEventRefHandler<XenoArtifactUnlockingComponent, MapInitEvent>(this.OnUnlockingStarted));
  }

  private void UpdateUnlock(float _)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoArtifactUnlockingComponent, XenoArtifactComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoArtifactUnlockingComponent, XenoArtifactComponent>();
    EntityUid uid;
    XenoArtifactUnlockingComponent comp1;
    XenoArtifactComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(this._timing.CurTime < comp1.EndTime))
        this.FinishUnlockingState((Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent>) (uid, comp1, comp2));
    }
  }

  public bool CanUnlockNode(Entity<XenoArtifactNodeComponent?> ent)
  {
    if (!this.Resolve<XenoArtifactNodeComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    EntityUid? entity = this.GetEntity(ent.Comp.Attached);
    XenoArtifactComponent comp;
    return this.TryComp<XenoArtifactComponent>(entity, out comp) && !comp.Suppressed && this.HasUnlockedPredecessor((Entity<XenoArtifactComponent>) (entity.Value, comp), (EntityUid) ent) && (ent.Comp.Locked || this.GetSuccessorNodes((Entity<XenoArtifactComponent>) (entity.Value, comp), (Entity<XenoArtifactNodeComponent>) (ent.Owner, ent.Comp)).Count != 0);
  }

  public void FinishUnlockingState(
    Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent)
  {
    XenoArtifactComponent artifactComponent = (XenoArtifactComponent) ent;
    XenoArtifactUnlockingComponent unlockingComponent = (XenoArtifactUnlockingComponent) ent;
    Entity<XenoArtifactNodeComponent>? node;
    string messageId;
    SoundSpecifier sound;
    if (this.TryGetNodeFromUnlockState(ent, out node))
    {
      this.SetNodeUnlocked((Entity<XenoArtifactComponent>) ((EntityUid) ent, artifactComponent), node.Value);
      this.ActivateNode((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), (Entity<XenoArtifactNodeComponent>) ((EntityUid) node.Value, (XenoArtifactNodeComponent) node.Value), new EntityUid?(), new EntityUid?(), this.Transform((EntityUid) ent).Coordinates, false);
      messageId = "artifact-unlock-state-end-success";
      sound = unlockingComponent.UnlockActivationSuccessfulSound;
    }
    else
    {
      messageId = "artifact-unlock-state-end-failure";
      sound = unlockingComponent.UnlockActivationFailedSound;
    }
    if (this._net.IsServer)
    {
      this._popup.PopupEntity(this.Loc.GetString(messageId), (EntityUid) ent);
      this._audio.PlayPvs(sound, ent.Owner);
    }
    this.RemComp((EntityUid) ent, (IComponent) unlockingComponent);
    this.RaiseUnlockingFinished(ent, node);
    artifactComponent.NextUnlockTime = this._timing.CurTime + artifactComponent.UnlockStateRefractory;
  }

  public void CancelUnlockingState(
    Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent)
  {
    this.RemComp((EntityUid) ent, (IComponent) ent.Comp1);
    this.RaiseUnlockingFinished(ent, new Entity<XenoArtifactNodeComponent>?());
  }

  public bool TryGetNodeFromUnlockState(
    Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent,
    [NotNullWhen(true)] out Entity<XenoArtifactNodeComponent>? node)
  {
    node = new Entity<XenoArtifactNodeComponent>?();
    ValueList<Entity<XenoArtifactNodeComponent>> list = new ValueList<Entity<XenoArtifactNodeComponent>>();
    XenoArtifactUnlockingComponent comp1 = ent.Comp1;
    foreach (int allNodeIndex in this.GetAllNodeIndices((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent)))
    {
      XenoArtifactComponent comp2 = ent.Comp2;
      Entity<XenoArtifactNodeComponent> node1 = this.GetNode((Entity<XenoArtifactComponent>) ((EntityUid) ent, comp2), allNodeIndex);
      if (node1.Comp.Locked && this.CanUnlockNode((Entity<XenoArtifactNodeComponent>) ((EntityUid) node1, (XenoArtifactNodeComponent) node1)))
      {
        HashSet<int> predecessorNodes = this.GetPredecessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, comp2), allNodeIndex);
        predecessorNodes.Add(allNodeIndex);
        if (!ent.Comp1.ArtifexiumApplied)
        {
          if (predecessorNodes.Count == comp1.TriggeredNodeIndexes.Count && comp1.TriggeredNodeIndexes.All<int>(new Func<int, bool>(predecessorNodes.Contains)))
          {
            node = new Entity<XenoArtifactNodeComponent>?(node1);
            return true;
          }
        }
        else if (comp1.TriggeredNodeIndexes.All<int>(new Func<int, bool>(predecessorNodes.Contains)) && predecessorNodes.Count - 1 == comp1.TriggeredNodeIndexes.Count)
          list.Add(node1);
      }
    }
    if (list.Count != 0)
      node = new Entity<XenoArtifactNodeComponent>?(this.RobustRandom.Pick<Entity<XenoArtifactNodeComponent>>(list));
    return node.HasValue;
  }

  private void OnUnlockingStarted(Entity<XenoArtifactUnlockingComponent> ent, ref MapInitEvent args)
  {
    ArtifactUnlockingStartedEvent args1 = new ArtifactUnlockingStartedEvent();
    this.RaiseLocalEvent<ArtifactUnlockingStartedEvent>(ent.Owner, ref args1);
  }

  private void RaiseUnlockingFinished(
    Entity<XenoArtifactUnlockingComponent, XenoArtifactComponent> ent,
    Entity<XenoArtifactNodeComponent>? node)
  {
    ArtifactUnlockingFinishedEvent args;
    ref ArtifactUnlockingFinishedEvent local = ref args;
    Entity<XenoArtifactNodeComponent>? nullable = node;
    EntityUid? UnlockedNode = nullable.HasValue ? new EntityUid?((EntityUid) nullable.GetValueOrDefault()) : new EntityUid?();
    local = new ArtifactUnlockingFinishedEvent(UnlockedNode);
    this.RaiseLocalEvent<ArtifactUnlockingFinishedEvent>(ent.Owner, ref args);
  }

  private void InitializeXAE()
  {
    this.SubscribeLocalEvent<XenoArtifactComponent, UseInHandEvent>(new EntityEventRefHandler<XenoArtifactComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<XenoArtifactComponent, AfterInteractEvent>(new EntityEventRefHandler<XenoArtifactComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<XenoArtifactComponent, ActivateInWorldEvent>(new EntityEventRefHandler<XenoArtifactComponent, ActivateInWorldEvent>(this.OnActivateInWorld));
  }

  private void OnUseInHand(Entity<XenoArtifactComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryActivateXenoArtifact(ent, new EntityUid?(args.User), new EntityUid?(args.User), this.Transform(args.User).Coordinates);
  }

  private void OnAfterInteract(Entity<XenoArtifactComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    args.Handled = this.TryActivateXenoArtifact(ent, new EntityUid?(args.User), args.Target, args.ClickLocation);
  }

  private void OnActivateInWorld(Entity<XenoArtifactComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    args.Handled = this.TryActivateXenoArtifact(ent, new EntityUid?(args.User), new EntityUid?(args.Target), this.Transform(args.Target).Coordinates);
  }

  public bool TryActivateXenoArtifact(
    Entity<XenoArtifactComponent> artifact,
    EntityUid? user,
    EntityUid? target,
    EntityCoordinates coordinates,
    bool consumeDurability = true)
  {
    XenoArtifactComponent artifactComponent = (XenoArtifactComponent) artifact;
    UseDelayComponent comp;
    if (artifactComponent.Suppressed || this.TryComp<UseDelayComponent>((EntityUid) artifact, out comp) && !this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) artifact, comp), true))
      return false;
    bool flag = false;
    foreach (Entity<XenoArtifactNodeComponent> activeNode in this.GetActiveNodes(artifact))
      flag |= this.ActivateNode(artifact, activeNode, user, target, coordinates, consumeDurability);
    if (!flag)
    {
      this._popup.PopupClient(this.Loc.GetString("artifact-activation-fail"), (EntityUid) artifact, user);
      return false;
    }
    XenoArtifactActivatedEvent args = new XenoArtifactActivatedEvent(artifact, user, target, coordinates);
    this.RaiseLocalEvent<XenoArtifactActivatedEvent>((EntityUid) artifact, ref args);
    if (user.HasValue)
      this._audio.PlayPredicted(artifactComponent.ForceActivationSoundSpecifier, (EntityUid) artifact, user);
    else
      this._audio.PlayPvs(artifactComponent.ForceActivationSoundSpecifier, (EntityUid) artifact);
    return true;
  }

  public bool ActivateNode(
    Entity<XenoArtifactComponent> artifact,
    Entity<XenoArtifactNodeComponent> node,
    EntityUid? user,
    EntityUid? target,
    EntityCoordinates coordinates,
    bool consumeDurability = true)
  {
    if (node.Comp.Degraded)
      return false;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(24, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) artifact.Owner), "ToPrettyString(artifact.Owner)");
    logStringHandler.AppendLiteral(" node ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) node)), "ToPrettyString(node)");
    logStringHandler.AppendLiteral(" got activated at ");
    logStringHandler.AppendFormatted<EntityCoordinates>(coordinates, nameof (coordinates));
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.ArtifactNode, LogImpact.Low, ref local);
    if (consumeDurability)
      this.AdjustNodeDurability((Entity<XenoArtifactNodeComponent>) ((EntityUid) node, node.Comp), -1);
    XenoArtifactNodeActivatedEvent args = new XenoArtifactNodeActivatedEvent(artifact, node, user, target, coordinates);
    this.RaiseLocalEvent<XenoArtifactNodeActivatedEvent>((EntityUid) node, ref args);
    return true;
  }

  private void InitializeXAT()
  {
    this.XATRelayLocalEvent<DamageChangedEvent>();
    this.XATRelayLocalEvent<InteractUsingEvent>();
    this.XATRelayLocalEvent<PullStartedMessage>();
    this.XATRelayLocalEvent<AttackedEvent>();
    this.XATRelayLocalEvent<XATToolUseDoAfterEvent>();
    this.XATRelayLocalEvent<InteractHandEvent>();
    this.XATRelayLocalEvent<ReactionEntityEvent>();
    this.XATRelayLocalEvent<LandEvent>();
    this.SubscribeLocalEvent<XenoArtifactComponent, ExaminedEvent>(new EntityEventRefHandler<XenoArtifactComponent, ExaminedEvent>(this.OnExamined));
  }

  protected void XATRelayLocalEvent<T>() where T : notnull
  {
    this.SubscribeLocalEvent<XenoArtifactComponent, T>(new EntityEventRefHandler<XenoArtifactComponent, T>(this.RelayEventToNodes<T>));
  }

  private void OnExamined(Entity<XenoArtifactComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("XenoArtifactComponent"))
      this.RelayEventToNodes<ExaminedEvent>(ent, ref args);
  }

  protected void RelayEventToNodes<T>(Entity<XenoArtifactComponent> ent, ref T args) where T : notnull
  {
    XenoArchNodeRelayedEvent<T> args1 = new XenoArchNodeRelayedEvent<T>(ent, args);
    foreach (Entity<XenoArtifactNodeComponent> allNode in this.GetAllNodes(ent))
      this.RaiseLocalEvent<XenoArchNodeRelayedEvent<T>>((EntityUid) allNode, ref args1);
  }

  public void TriggerXenoArtifact(
    Entity<XenoArtifactComponent> ent,
    Entity<XenoArtifactNodeComponent>? node,
    bool force = false)
  {
    if (!force && this._timing.CurTime < ent.Comp.NextUnlockTime)
      return;
    XenoArtifactUnlockingComponent component;
    if (!this._unlockingQuery.TryGetComponent((EntityUid) ent, out component))
    {
      component = this.EnsureComp<XenoArtifactUnlockingComponent>((EntityUid) ent);
      component.EndTime = this._timing.CurTime + ent.Comp.UnlockStateDuration;
      this.Log.Debug($"{this.ToPrettyString(new EntityUid?((EntityUid) ent))} entered unlocking state");
      if (this._net.IsServer)
        this._popup.PopupEntity(this.Loc.GetString("artifact-unlock-state-begin"), (EntityUid) ent);
      this.Dirty<XenoArtifactComponent>(ent);
    }
    else if (node.HasValue)
    {
      int index = this.GetIndex(ent, (EntityUid) node.Value);
      HashSet<int> predecessorNodeIndices = this.GetPredecessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), index);
      HashSet<int> successorNodeIndices = this.GetSuccessorNodes((Entity<XenoArtifactComponent>) ((EntityUid) ent, (XenoArtifactComponent) ent), index);
      if (component.TriggeredNodeIndexes.Count == 0 || component.TriggeredNodeIndexes.All<int>((Func<int, bool>) (x => predecessorNodeIndices.Contains(x) || successorNodeIndices.Contains(x))))
        component.EndTime += ent.Comp.UnlockStateIncrementPerNode;
    }
    if (!node.HasValue || !component.TriggeredNodeIndexes.Add(this.GetIndex(ent, (EntityUid) node.Value)))
      return;
    this.Dirty((EntityUid) ent, (IComponent) component);
  }

  public void SetArtifexiumApplied(Entity<XenoArtifactUnlockingComponent> ent, bool val)
  {
    ent.Comp.ArtifexiumApplied = val;
    this.Dirty<XenoArtifactUnlockingComponent>(ent);
  }
}
