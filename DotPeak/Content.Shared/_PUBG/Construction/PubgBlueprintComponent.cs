// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Construction.PubgBlueprintComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Construction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgBlueprintComponent : 
  Component,
  ISerializationGenerated<PubgBlueprintComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<RMCConstructionPrototype> Recipe;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PointsRequired = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Points;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Filled;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StackPrototype>? FillMaterial;
  [DataField(null, false, 1, false, false, null)]
  public int MaterialCost;
  [DataField(null, false, 1, false, false, null)]
  public Direction Direction;
  [DataField(null, false, 1, false, false, null)]
  public bool NoRotate;
  [DataField(null, false, 1, false, false, null)]
  public float BreakDamage = 30f;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Placer;
  public bool Completed;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgBlueprintComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgBlueprintComponent) target1;
    if (serialization.TryCustomCopy<PubgBlueprintComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<RMCConstructionPrototype> target2 = new ProtoId<RMCConstructionPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RMCConstructionPrototype>>(this.Recipe, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<RMCConstructionPrototype>>(this.Recipe, hookCtx, context);
    target.Recipe = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.PointsRequired, ref target3, hookCtx, false, context))
      target3 = this.PointsRequired;
    target.PointsRequired = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Points, ref target4, hookCtx, false, context))
      target4 = this.Points;
    target.Points = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Filled, ref target5, hookCtx, false, context))
      target5 = this.Filled;
    target.Filled = target5;
    ProtoId<StackPrototype>? target6 = new ProtoId<StackPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<StackPrototype>?>(this.FillMaterial, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<StackPrototype>?>(this.FillMaterial, hookCtx, context);
    target.FillMaterial = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaterialCost, ref target7, hookCtx, false, context))
      target7 = this.MaterialCost;
    target.MaterialCost = target7;
    Direction target8 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.Direction, ref target8, hookCtx, false, context))
      target8 = this.Direction;
    target.Direction = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.NoRotate, ref target9, hookCtx, false, context))
      target9 = this.NoRotate;
    target.NoRotate = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakDamage, ref target10, hookCtx, false, context))
      target10 = this.BreakDamage;
    target.BreakDamage = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Placer, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.Placer, hookCtx, context);
    target.Placer = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgBlueprintComponent target,
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
    PubgBlueprintComponent target1 = (PubgBlueprintComponent) target;
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
    PubgBlueprintComponent target1 = (PubgBlueprintComponent) target;
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
    PubgBlueprintComponent target1 = (PubgBlueprintComponent) target;
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
  virtual PubgBlueprintComponent Component.Instantiate() => new PubgBlueprintComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgBlueprintComponent_AutoState : IComponentState
  {
    public ProtoId<RMCConstructionPrototype> Recipe;
    public int PointsRequired;
    public int Points;
    public bool Filled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgBlueprintComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgBlueprintComponent, ComponentGetState>(new ComponentEventRefHandler<PubgBlueprintComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgBlueprintComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgBlueprintComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgBlueprintComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgBlueprintComponent.PubgBlueprintComponent_AutoState()
      {
        Recipe = component.Recipe,
        PointsRequired = component.PointsRequired,
        Points = component.Points,
        Filled = component.Filled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgBlueprintComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgBlueprintComponent.PubgBlueprintComponent_AutoState current))
        return;
      component.Recipe = current.Recipe;
      component.PointsRequired = current.PointsRequired;
      component.Points = current.Points;
      component.Filled = current.Filled;
    }
  }
}
