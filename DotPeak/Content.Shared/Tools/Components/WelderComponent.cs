// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.WelderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Tools.Systems;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedToolSystem)})]
public sealed class WelderComponent : 
  Component,
  ISerializationGenerated<WelderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  public float WelderTimer;
  [DataField(null, false, 1, false, false, null)]
  public string FuelSolutionName = "Welder";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ReagentPrototype> FuelReagent = (ProtoId<ReagentPrototype>) "WeldingFuel";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 FuelConsumption = FixedPoint2.New(1f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 FuelLitCost = FixedPoint2.New(0.5f);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier WelderRefill = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/refill.ogg");
  [DataField(null, false, 1, false, false, null)]
  public bool TankSafe;
  [DataField(null, false, 1, false, false, null)]
  public float WelderUpdateTimer = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WelderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WelderComponent) target1;
    if (serialization.TryCustomCopy<WelderComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WelderTimer, ref target3, hookCtx, false, context))
      target3 = this.WelderTimer;
    target.WelderTimer = target3;
    string target4 = (string) null;
    if (this.FuelSolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FuelSolutionName, ref target4, hookCtx, false, context))
      target4 = this.FuelSolutionName;
    target.FuelSolutionName = target4;
    ProtoId<ReagentPrototype> target5 = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.FuelReagent, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.FuelReagent, hookCtx, context);
    target.FuelReagent = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FuelConsumption, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.FuelConsumption, hookCtx, context);
    target.FuelConsumption = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FuelLitCost, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.FuelLitCost, hookCtx, context);
    target.FuelLitCost = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.WelderRefill == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WelderRefill, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.WelderRefill, hookCtx, context);
    target.WelderRefill = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.TankSafe, ref target9, hookCtx, false, context))
      target9 = this.TankSafe;
    target.TankSafe = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WelderUpdateTimer, ref target10, hookCtx, false, context))
      target10 = this.WelderUpdateTimer;
    target.WelderUpdateTimer = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WelderComponent target,
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
    WelderComponent target1 = (WelderComponent) target;
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
    WelderComponent target1 = (WelderComponent) target;
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
    WelderComponent target1 = (WelderComponent) target;
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
  virtual WelderComponent Component.Instantiate() => new WelderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WelderComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public FixedPoint2 FuelConsumption;
    public FixedPoint2 FuelLitCost;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WelderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WelderComponent, ComponentGetState>(new ComponentEventRefHandler<WelderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WelderComponent, ComponentHandleState>(new ComponentEventRefHandler<WelderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, WelderComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new WelderComponent.WelderComponent_AutoState()
      {
        Enabled = component.Enabled,
        FuelConsumption = component.FuelConsumption,
        FuelLitCost = component.FuelLitCost
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WelderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WelderComponent.WelderComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.FuelConsumption = current.FuelConsumption;
      component.FuelLitCost = current.FuelLitCost;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, WelderComponent>(uid, component, ref args1);
    }
  }
}
