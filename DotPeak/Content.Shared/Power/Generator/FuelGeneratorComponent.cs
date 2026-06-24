// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.FuelGeneratorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Power.Generator;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedGeneratorSystem)})]
public sealed class FuelGeneratorComponent : 
  Component,
  ISerializationGenerated<FuelGeneratorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool On;
  [DataField(null, false, 1, false, false, null)]
  public float TargetPower = 15000f;
  [DataField(null, false, 1, false, false, null)]
  [GuidebookData]
  public float MaxTargetPower = 30000f;
  [DataField(null, false, 1, false, false, null)]
  public float MinTargetPower = 1000f;
  [DataField(null, false, 1, false, false, null)]
  public float OptimalPower = 15000f;
  [DataField(null, false, 1, false, false, null)]
  public float OptimalBurnRate = 0.0166666675f;
  [DataField(null, false, 1, false, false, null)]
  public float FuelEfficiencyConstant = 1.3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FuelGeneratorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FuelGeneratorComponent) target1;
    if (serialization.TryCustomCopy<FuelGeneratorComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.On, ref target2, hookCtx, false, context))
      target2 = this.On;
    target.On = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TargetPower, ref target3, hookCtx, false, context))
      target3 = this.TargetPower;
    target.TargetPower = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTargetPower, ref target4, hookCtx, false, context))
      target4 = this.MaxTargetPower;
    target.MaxTargetPower = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinTargetPower, ref target5, hookCtx, false, context))
      target5 = this.MinTargetPower;
    target.MinTargetPower = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OptimalPower, ref target6, hookCtx, false, context))
      target6 = this.OptimalPower;
    target.OptimalPower = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OptimalBurnRate, ref target7, hookCtx, false, context))
      target7 = this.OptimalBurnRate;
    target.OptimalBurnRate = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FuelEfficiencyConstant, ref target8, hookCtx, false, context))
      target8 = this.FuelEfficiencyConstant;
    target.FuelEfficiencyConstant = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FuelGeneratorComponent target,
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
    FuelGeneratorComponent target1 = (FuelGeneratorComponent) target;
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
    FuelGeneratorComponent target1 = (FuelGeneratorComponent) target;
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
    FuelGeneratorComponent target1 = (FuelGeneratorComponent) target;
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
  virtual FuelGeneratorComponent Component.Instantiate() => new FuelGeneratorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FuelGeneratorComponent_AutoState : IComponentState
  {
    public bool On;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FuelGeneratorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FuelGeneratorComponent, ComponentGetState>(new ComponentEventRefHandler<FuelGeneratorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FuelGeneratorComponent, ComponentHandleState>(new ComponentEventRefHandler<FuelGeneratorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FuelGeneratorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FuelGeneratorComponent.FuelGeneratorComponent_AutoState()
      {
        On = component.On
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FuelGeneratorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FuelGeneratorComponent.FuelGeneratorComponent_AutoState current))
        return;
      component.On = current.On;
    }
  }
}
