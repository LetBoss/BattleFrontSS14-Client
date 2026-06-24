// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.Components.XenoArtifactComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Xenoarchaeology.Artifact.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedXenoArtifactSystem)})]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class XenoArtifactComponent : 
  Component,
  ISerializationGenerated<XenoArtifactComponent>,
  ISerializationGenerated
{
  public static string NodeContainerId = "node-container";
  [DataField(null, false, 1, false, false, null)]
  public bool IsGenerationRequired;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container NodeContainer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<NetEntity> CachedActiveNodes;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<NetEntity>> CachedSegments;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Suppressed;
  [DataField(null, false, 1, false, false, null)]
  public float PriceMultiplier;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnlockStateDuration;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnlockStateIncrementPerNode;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnlockStateRefractory;
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan NextUnlockTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity?[] NodeVertices;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<bool>> NodeAdjacencyMatrix;
  [DataField(null, false, 1, false, false, null)]
  public MinMax NodeCount;
  [DataField(null, false, 1, false, false, null)]
  public MinMax SegmentSize;
  [DataField(null, false, 1, false, false, null)]
  public MinMax NodesPerSegmentLayer;
  [DataField(null, false, 1, false, false, null)]
  public MinMax ScatterPerLayer;
  [DataField(null, false, 1, false, false, null)]
  public EntityTableSelector EffectsTable;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<WeightedRandomXenoArchTriggerPrototype> TriggerWeights;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ForceActivationSoundSpecifier;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<InstantActionComponent> SelfActivateAction;

  public int NodeAdjacencyMatrixRows => this.NodeAdjacencyMatrix.Count;

  public int NodeAdjacencyMatrixColumns
  {
    get
    {
      List<bool> boolList;
      return !this.NodeAdjacencyMatrix.TryGetValue<List<bool>>(0, out boolList) ? 0 : boolList.Count;
    }
  }

  public XenoArtifactComponent()
  {
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("ArtifactForceActivation");
    collectionSpecifier.Params = new AudioParams()
    {
      Variation = new float?(0.1f)
    };
    this.ForceActivationSoundSpecifier = (SoundSpecifier) collectionSpecifier;
    this.SelfActivateAction = (EntProtoId<InstantActionComponent>) "ActionArtifactActivate";
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoArtifactComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoArtifactComponent) target1;
    if (serialization.TryCustomCopy<XenoArtifactComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsGenerationRequired, ref target2, hookCtx, false, context))
      target2 = this.IsGenerationRequired;
    target.IsGenerationRequired = target2;
    List<NetEntity> target3 = (List<NetEntity>) null;
    if (this.CachedActiveNodes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<NetEntity>>(this.CachedActiveNodes, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<NetEntity>>(this.CachedActiveNodes, hookCtx, context);
    target.CachedActiveNodes = target3;
    List<List<NetEntity>> target4 = (List<List<NetEntity>>) null;
    if (this.CachedSegments == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<NetEntity>>>(this.CachedSegments, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<List<NetEntity>>>(this.CachedSegments, hookCtx, context);
    target.CachedSegments = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Suppressed, ref target5, hookCtx, false, context))
      target5 = this.Suppressed;
    target.Suppressed = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PriceMultiplier, ref target6, hookCtx, false, context))
      target6 = this.PriceMultiplier;
    target.PriceMultiplier = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockStateDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.UnlockStateDuration, hookCtx, context);
    target.UnlockStateDuration = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockStateIncrementPerNode, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.UnlockStateIncrementPerNode, hookCtx, context);
    target.UnlockStateIncrementPerNode = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockStateRefractory, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.UnlockStateRefractory, hookCtx, context);
    target.UnlockStateRefractory = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUnlockTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.NextUnlockTime, hookCtx, context);
    target.NextUnlockTime = target10;
    NetEntity?[] target11 = (NetEntity?[]) null;
    if (this.NodeVertices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<NetEntity?[]>(this.NodeVertices, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<NetEntity?[]>(this.NodeVertices, hookCtx, context);
    target.NodeVertices = target11;
    List<List<bool>> target12 = (List<List<bool>>) null;
    if (this.NodeAdjacencyMatrix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<bool>>>(this.NodeAdjacencyMatrix, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<List<bool>>>(this.NodeAdjacencyMatrix, hookCtx, context);
    target.NodeAdjacencyMatrix = target12;
    MinMax target13 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.NodeCount, ref target13, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.NodeCount, ref target13, hookCtx, context);
    target.NodeCount = target13;
    MinMax target14 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.SegmentSize, ref target14, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.SegmentSize, ref target14, hookCtx, context);
    target.SegmentSize = target14;
    MinMax target15 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.NodesPerSegmentLayer, ref target15, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.NodesPerSegmentLayer, ref target15, hookCtx, context);
    target.NodesPerSegmentLayer = target15;
    MinMax target16 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.ScatterPerLayer, ref target16, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.ScatterPerLayer, ref target16, hookCtx, context);
    target.ScatterPerLayer = target16;
    EntityTableSelector target17 = (EntityTableSelector) null;
    if (this.EffectsTable == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityTableSelector>(this.EffectsTable, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<EntityTableSelector>(this.EffectsTable, hookCtx, context);
    target.EffectsTable = target17;
    ProtoId<WeightedRandomXenoArchTriggerPrototype> target18 = new ProtoId<WeightedRandomXenoArchTriggerPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<WeightedRandomXenoArchTriggerPrototype>>(this.TriggerWeights, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<ProtoId<WeightedRandomXenoArchTriggerPrototype>>(this.TriggerWeights, hookCtx, context);
    target.TriggerWeights = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ForceActivationSoundSpecifier, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.ForceActivationSoundSpecifier, hookCtx, context);
    target.ForceActivationSoundSpecifier = target19;
    EntProtoId<InstantActionComponent> target20 = new EntProtoId<InstantActionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<InstantActionComponent>>(this.SelfActivateAction, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<EntProtoId<InstantActionComponent>>(this.SelfActivateAction, hookCtx, context);
    target.SelfActivateAction = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoArtifactComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoArtifactComponent target1 = (XenoArtifactComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoArtifactComponent target1 = (XenoArtifactComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoArtifactComponent target1 = (XenoArtifactComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoArtifactComponent Component.Instantiate() => new XenoArtifactComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoArtifactComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoArtifactComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoArtifactComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoArtifactComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUnlockTime += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoArtifactComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    List<NetEntity> CachedActiveNodes;
    public List<List<NetEntity>> CachedSegments;
    public bool Suppressed;
    public NetEntity?[] NodeVertices;
    public List<List<bool>> NodeAdjacencyMatrix;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoArtifactComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoArtifactComponent, ComponentGetState>(new ComponentEventRefHandler<XenoArtifactComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoArtifactComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoArtifactComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoArtifactComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoArtifactComponent.XenoArtifactComponent_AutoState()
      {
        CachedActiveNodes = component.CachedActiveNodes,
        CachedSegments = component.CachedSegments,
        Suppressed = component.Suppressed,
        NodeVertices = component.NodeVertices,
        NodeAdjacencyMatrix = component.NodeAdjacencyMatrix
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoArtifactComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoArtifactComponent.XenoArtifactComponent_AutoState current))
        return;
      component.CachedActiveNodes = current.CachedActiveNodes == null ? (List<NetEntity>) null : new List<NetEntity>((IEnumerable<NetEntity>) current.CachedActiveNodes);
      component.CachedSegments = current.CachedSegments == null ? (List<List<NetEntity>>) null : new List<List<NetEntity>>((IEnumerable<List<NetEntity>>) current.CachedSegments);
      component.Suppressed = current.Suppressed;
      component.NodeVertices = current.NodeVertices;
      component.NodeAdjacencyMatrix = current.NodeAdjacencyMatrix == null ? (List<List<bool>>) null : new List<List<bool>>((IEnumerable<List<bool>>) current.NodeAdjacencyMatrix);
    }
  }
}
