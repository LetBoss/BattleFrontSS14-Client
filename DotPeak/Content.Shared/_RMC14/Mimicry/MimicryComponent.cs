// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mimicry.MimicryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Content.Shared.Maps;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Mimicry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class MimicryComponent : 
  Component,
  ISerializationGenerated<MimicryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Action = (EntProtoId) "ActionMimicrySurface";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ContentTileDefinition>? MimickedTile;
  [DataField(null, false, 1, false, false, null)]
  public float MaskSeconds = 5f;
  [DataField(null, false, 1, false, false, null)]
  public int MaxUses = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int UsesDone;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ContentTileDefinition>> SurfaceWhitelist = new List<ProtoId<ContentTileDefinition>>();
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Hood;
  [DataField(null, false, 1, false, false, null)]
  public string HoodSlot = "head";
  public EntityUid? HoodUid;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId HoodToggleAction = (EntProtoId) "ActionMimicryHood";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? HoodToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HoodDown;
  [DataField(null, false, 1, false, false, null)]
  public List<HumanoidVisualLayers> HoodHiddenLayers = new List<HumanoidVisualLayers>()
  {
    HumanoidVisualLayers.Hair,
    HumanoidVisualLayers.HeadTop,
    HumanoidVisualLayers.HeadSide
  };
  [DataField(null, false, 1, false, false, null)]
  public List<string> ExcludedSlots = new List<string>()
  {
    "suitstorage"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MimicryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MimicryComponent) target1;
    if (serialization.TryCustomCopy<MimicryComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Action, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Action, hookCtx, context);
    target.Action = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target3;
    ProtoId<ContentTileDefinition>? target4 = new ProtoId<ContentTileDefinition>?();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(this.MimickedTile, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(this.MimickedTile, hookCtx, context);
    target.MimickedTile = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaskSeconds, ref target5, hookCtx, false, context))
      target5 = this.MaskSeconds;
    target.MaskSeconds = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxUses, ref target6, hookCtx, false, context))
      target6 = this.MaxUses;
    target.MaxUses = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.UsesDone, ref target7, hookCtx, false, context))
      target7 = this.UsesDone;
    target.UsesDone = target7;
    List<ProtoId<ContentTileDefinition>> target8 = (List<ProtoId<ContentTileDefinition>>) null;
    if (this.SurfaceWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(this.SurfaceWhitelist, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(this.SurfaceWhitelist, hookCtx, context);
    target.SurfaceWhitelist = target8;
    EntProtoId? target9 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Hood, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId?>(this.Hood, hookCtx, context);
    target.Hood = target9;
    string target10 = (string) null;
    if (this.HoodSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HoodSlot, ref target10, hookCtx, false, context))
      target10 = this.HoodSlot;
    target.HoodSlot = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HoodToggleAction, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.HoodToggleAction, hookCtx, context);
    target.HoodToggleAction = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.HoodToggleActionEntity, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.HoodToggleActionEntity, hookCtx, context);
    target.HoodToggleActionEntity = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.HoodDown, ref target13, hookCtx, false, context))
      target13 = this.HoodDown;
    target.HoodDown = target13;
    List<HumanoidVisualLayers> target14 = (List<HumanoidVisualLayers>) null;
    if (this.HoodHiddenLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<HumanoidVisualLayers>>(this.HoodHiddenLayers, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<List<HumanoidVisualLayers>>(this.HoodHiddenLayers, hookCtx, context);
    target.HoodHiddenLayers = target14;
    List<string> target15 = (List<string>) null;
    if (this.ExcludedSlots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.ExcludedSlots, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<List<string>>(this.ExcludedSlots, hookCtx, context);
    target.ExcludedSlots = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MimicryComponent target,
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
    MimicryComponent target1 = (MimicryComponent) target;
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
    MimicryComponent target1 = (MimicryComponent) target;
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
    MimicryComponent target1 = (MimicryComponent) target;
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
  virtual MimicryComponent Component.Instantiate() => new MimicryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MimicryComponent_AutoState : IComponentState
  {
    public NetEntity? ActionEntity;
    public ProtoId<ContentTileDefinition>? MimickedTile;
    public int UsesDone;
    public NetEntity? HoodToggleActionEntity;
    public bool HoodDown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MimicryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MimicryComponent, ComponentGetState>(new ComponentEventRefHandler<MimicryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MimicryComponent, ComponentHandleState>(new ComponentEventRefHandler<MimicryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MimicryComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MimicryComponent.MimicryComponent_AutoState()
      {
        ActionEntity = this.GetNetEntity(component.ActionEntity),
        MimickedTile = component.MimickedTile,
        UsesDone = component.UsesDone,
        HoodToggleActionEntity = this.GetNetEntity(component.HoodToggleActionEntity),
        HoodDown = component.HoodDown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MimicryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MimicryComponent.MimicryComponent_AutoState current))
        return;
      component.ActionEntity = this.EnsureEntity<MimicryComponent>(current.ActionEntity, uid);
      component.MimickedTile = current.MimickedTile;
      component.UsesDone = current.UsesDone;
      component.HoodToggleActionEntity = this.EnsureEntity<MimicryComponent>(current.HoodToggleActionEntity, uid);
      component.HoodDown = current.HoodDown;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, MimicryComponent>(uid, component, ref args1);
    }
  }
}
