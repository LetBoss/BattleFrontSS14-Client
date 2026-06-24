// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitPlanterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Xenonids.Fruit.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedXenoFruitSystem)})]
public sealed class XenoFruitPlanterComponent : 
  Component,
  ISerializationGenerated<XenoFruitPlanterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> CanPlant;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? FruitChoice;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier PlantSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxFruitAllowed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> PlantedFruit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FruitPickingMultiplier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FruitFeedingMultiplier;

  public XenoFruitPlanterComponent()
  {
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("RMCResinBuild");
    collectionSpecifier.Params = AudioParams.Default.WithVolume(-10f);
    this.PlantSound = (SoundSpecifier) collectionSpecifier;
    this.MaxFruitAllowed = 3;
    this.PlantedFruit = new List<EntityUid>();
    this.FruitPickingMultiplier = 1f;
    this.FruitFeedingMultiplier = 1f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitPlanterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitPlanterComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitPlanterComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntProtoId> target2 = (List<EntProtoId>) null;
    if (this.CanPlant == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.CanPlant, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntProtoId>>(this.CanPlant, hookCtx, context);
    target.CanPlant = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.FruitChoice, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.FruitChoice, hookCtx, context);
    target.FruitChoice = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.PlantSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PlantSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.PlantSound, hookCtx, context);
    target.PlantSound = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFruitAllowed, ref target5, hookCtx, false, context))
      target5 = this.MaxFruitAllowed;
    target.MaxFruitAllowed = target5;
    List<EntityUid> target6 = (List<EntityUid>) null;
    if (this.PlantedFruit == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.PlantedFruit, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<EntityUid>>(this.PlantedFruit, hookCtx, context);
    target.PlantedFruit = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FruitPickingMultiplier, ref target7, hookCtx, false, context))
      target7 = this.FruitPickingMultiplier;
    target.FruitPickingMultiplier = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FruitFeedingMultiplier, ref target8, hookCtx, false, context))
      target8 = this.FruitFeedingMultiplier;
    target.FruitFeedingMultiplier = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitPlanterComponent target,
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
    XenoFruitPlanterComponent target1 = (XenoFruitPlanterComponent) target;
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
    XenoFruitPlanterComponent target1 = (XenoFruitPlanterComponent) target;
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
    XenoFruitPlanterComponent target1 = (XenoFruitPlanterComponent) target;
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
  virtual XenoFruitPlanterComponent Component.Instantiate() => new XenoFruitPlanterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitPlanterComponent_AutoState : IComponentState
  {
    public List<EntProtoId> CanPlant;
    public EntProtoId? FruitChoice;
    public SoundSpecifier PlantSound;
    public int MaxFruitAllowed;
    public List<NetEntity> PlantedFruit;
    public float FruitPickingMultiplier;
    public float FruitFeedingMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitPlanterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitPlanterComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitPlanterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitPlanterComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitPlanterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFruitPlanterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitPlanterComponent.XenoFruitPlanterComponent_AutoState()
      {
        CanPlant = component.CanPlant,
        FruitChoice = component.FruitChoice,
        PlantSound = component.PlantSound,
        MaxFruitAllowed = component.MaxFruitAllowed,
        PlantedFruit = this.GetNetEntityList(component.PlantedFruit),
        FruitPickingMultiplier = component.FruitPickingMultiplier,
        FruitFeedingMultiplier = component.FruitFeedingMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitPlanterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitPlanterComponent.XenoFruitPlanterComponent_AutoState current))
        return;
      component.CanPlant = current.CanPlant == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.CanPlant);
      component.FruitChoice = current.FruitChoice;
      component.PlantSound = current.PlantSound;
      component.MaxFruitAllowed = current.MaxFruitAllowed;
      this.EnsureEntityList<XenoFruitPlanterComponent>(current.PlantedFruit, uid, component.PlantedFruit);
      component.FruitPickingMultiplier = current.FruitPickingMultiplier;
      component.FruitFeedingMultiplier = current.FruitFeedingMultiplier;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoFruitPlanterComponent>(uid, component, ref args1);
    }
  }
}
