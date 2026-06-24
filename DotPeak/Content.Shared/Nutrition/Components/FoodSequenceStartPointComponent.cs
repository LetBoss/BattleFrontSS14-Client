// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.FoodSequenceStartPointComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedFoodSequenceSystem)})]
public sealed class FoodSequenceStartPointComponent : 
  Component,
  ISerializationGenerated<FoodSequenceStartPointComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<TagPrototype> Key = (ProtoId<TagPrototype>) string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public int MaxLayers = 10;
  [DataField(null, false, 1, false, false, null)]
  public bool Finished;
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "food";
  [DataField(null, false, 1, false, false, null)]
  public LocId? NameGeneration;
  [DataField(null, false, 1, false, false, null)]
  public LocId? NamePrefix;
  [DataField(null, false, 1, false, false, null)]
  public string? ContentSeparator;
  [DataField(null, false, 1, false, false, null)]
  public LocId? NameSuffix;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<FoodSequenceVisualLayer> FoodLayers = new List<FoodSequenceVisualLayer>();
  [DataField(null, false, 1, false, false, null)]
  public bool InverseLayers;
  [DataField(null, false, 1, false, false, null)]
  public string TargetLayerMap = "foodSequenceLayers";
  [DataField(null, false, 1, false, false, null)]
  public Vector2 StartPosition = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Offset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 MaxLayerOffset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 MinLayerOffset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowHorizontalFlip = true;
  public HashSet<string> RevealedLayers = new HashSet<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoodSequenceStartPointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FoodSequenceStartPointComponent) target1;
    if (serialization.TryCustomCopy<FoodSequenceStartPointComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<TagPrototype> target2 = new ProtoId<TagPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(this.Key, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<TagPrototype>>(this.Key, hookCtx, context);
    target.Key = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLayers, ref target3, hookCtx, false, context))
      target3 = this.MaxLayers;
    target.MaxLayers = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Finished, ref target4, hookCtx, false, context))
      target4 = this.Finished;
    target.Finished = target4;
    string target5 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target5, hookCtx, false, context))
      target5 = this.Solution;
    target.Solution = target5;
    LocId? target6 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NameGeneration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId?>(this.NameGeneration, hookCtx, context);
    target.NameGeneration = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NamePrefix, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.NamePrefix, hookCtx, context);
    target.NamePrefix = target7;
    string target8 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ContentSeparator, ref target8, hookCtx, false, context))
      target8 = this.ContentSeparator;
    target.ContentSeparator = target8;
    LocId? target9 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NameSuffix, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId?>(this.NameSuffix, hookCtx, context);
    target.NameSuffix = target9;
    List<FoodSequenceVisualLayer> target10 = (List<FoodSequenceVisualLayer>) null;
    if (this.FoodLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<FoodSequenceVisualLayer>>(this.FoodLayers, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<FoodSequenceVisualLayer>>(this.FoodLayers, hookCtx, context);
    target.FoodLayers = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.InverseLayers, ref target11, hookCtx, false, context))
      target11 = this.InverseLayers;
    target.InverseLayers = target11;
    string target12 = (string) null;
    if (this.TargetLayerMap == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetLayerMap, ref target12, hookCtx, false, context))
      target12 = this.TargetLayerMap;
    target.TargetLayerMap = target12;
    Vector2 target13 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.StartPosition, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<Vector2>(this.StartPosition, hookCtx, context);
    target.StartPosition = target13;
    Vector2 target14 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target14;
    Vector2 target15 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.MaxLayerOffset, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<Vector2>(this.MaxLayerOffset, hookCtx, context);
    target.MaxLayerOffset = target15;
    Vector2 target16 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.MinLayerOffset, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<Vector2>(this.MinLayerOffset, hookCtx, context);
    target.MinLayerOffset = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowHorizontalFlip, ref target17, hookCtx, false, context))
      target17 = this.AllowHorizontalFlip;
    target.AllowHorizontalFlip = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoodSequenceStartPointComponent target,
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
    FoodSequenceStartPointComponent target1 = (FoodSequenceStartPointComponent) target;
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
    FoodSequenceStartPointComponent target1 = (FoodSequenceStartPointComponent) target;
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
    FoodSequenceStartPointComponent target1 = (FoodSequenceStartPointComponent) target;
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
  virtual FoodSequenceStartPointComponent Component.Instantiate()
  {
    return new FoodSequenceStartPointComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FoodSequenceStartPointComponent_AutoState : IComponentState
  {
    public List<FoodSequenceVisualLayer> FoodLayers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FoodSequenceStartPointComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FoodSequenceStartPointComponent, ComponentGetState>(new ComponentEventRefHandler<FoodSequenceStartPointComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FoodSequenceStartPointComponent, ComponentHandleState>(new ComponentEventRefHandler<FoodSequenceStartPointComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FoodSequenceStartPointComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FoodSequenceStartPointComponent.FoodSequenceStartPointComponent_AutoState()
      {
        FoodLayers = component.FoodLayers
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FoodSequenceStartPointComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FoodSequenceStartPointComponent.FoodSequenceStartPointComponent_AutoState current))
        return;
      component.FoodLayers = current.FoodLayers == null ? (List<FoodSequenceVisualLayer>) null : new List<FoodSequenceVisualLayer>((IEnumerable<FoodSequenceVisualLayer>) current.FoodLayers);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, FoodSequenceStartPointComponent>(uid, component, ref args1);
    }
  }
}
