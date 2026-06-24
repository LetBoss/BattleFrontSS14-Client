// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.RMCChemicalStorageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCChemistrySystem)})]
public sealed class RMCChemicalStorageComponent : 
  Component,
  ISerializationGenerated<RMCChemicalStorageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Energy = (FixedPoint2) 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxEnergy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BaseMax = (FixedPoint2) 100;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxPer = (FixedPoint2) 100;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId Network;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RechargeEvery = TimeSpan.FromSeconds(52.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Recharge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BaseRecharge = (FixedPoint2) 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 RechargePer = (FixedPoint2) 5;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan RechargeAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Updated;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCChemicalStorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCChemicalStorageComponent) target1;
    if (serialization.TryCustomCopy<RMCChemicalStorageComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Energy, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Energy, hookCtx, context);
    target.Energy = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxEnergy, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MaxEnergy, hookCtx, context);
    target.MaxEnergy = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BaseMax, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.BaseMax, hookCtx, context);
    target.BaseMax = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxPer, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.MaxPer, hookCtx, context);
    target.MaxPer = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Network, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.Network, hookCtx, context);
    target.Network = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RechargeEvery, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.RechargeEvery, hookCtx, context);
    target.RechargeEvery = target7;
    FixedPoint2 target8 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Recharge, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<FixedPoint2>(this.Recharge, hookCtx, context);
    target.Recharge = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BaseRecharge, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.BaseRecharge, hookCtx, context);
    target.BaseRecharge = target9;
    FixedPoint2 target10 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RechargePer, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<FixedPoint2>(this.RechargePer, hookCtx, context);
    target.RechargePer = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RechargeAt, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.RechargeAt, hookCtx, context);
    target.RechargeAt = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Updated, ref target12, hookCtx, false, context))
      target12 = this.Updated;
    target.Updated = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCChemicalStorageComponent target,
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
    RMCChemicalStorageComponent target1 = (RMCChemicalStorageComponent) target;
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
    RMCChemicalStorageComponent target1 = (RMCChemicalStorageComponent) target;
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
    RMCChemicalStorageComponent target1 = (RMCChemicalStorageComponent) target;
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
  virtual RMCChemicalStorageComponent Component.Instantiate() => new RMCChemicalStorageComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCChemicalStorageComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCChemicalStorageComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCChemicalStorageComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCChemicalStorageComponent component,
      ref EntityUnpausedEvent args)
    {
      component.RechargeAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCChemicalStorageComponent_AutoState : IComponentState
  {
    public FixedPoint2 Energy;
    public FixedPoint2 MaxEnergy;
    public FixedPoint2 BaseMax;
    public FixedPoint2 MaxPer;
    public EntProtoId Network;
    public TimeSpan RechargeEvery;
    public FixedPoint2 Recharge;
    public FixedPoint2 BaseRecharge;
    public FixedPoint2 RechargePer;
    public TimeSpan RechargeAt;
    public bool Updated;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCChemicalStorageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCChemicalStorageComponent, ComponentGetState>(new ComponentEventRefHandler<RMCChemicalStorageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCChemicalStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCChemicalStorageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      RMCChemicalStorageComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCChemicalStorageComponent.RMCChemicalStorageComponent_AutoState()
      {
        Energy = component.Energy,
        MaxEnergy = component.MaxEnergy,
        BaseMax = component.BaseMax,
        MaxPer = component.MaxPer,
        Network = component.Network,
        RechargeEvery = component.RechargeEvery,
        Recharge = component.Recharge,
        BaseRecharge = component.BaseRecharge,
        RechargePer = component.RechargePer,
        RechargeAt = component.RechargeAt,
        Updated = component.Updated
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCChemicalStorageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCChemicalStorageComponent.RMCChemicalStorageComponent_AutoState current))
        return;
      component.Energy = current.Energy;
      component.MaxEnergy = current.MaxEnergy;
      component.BaseMax = current.BaseMax;
      component.MaxPer = current.MaxPer;
      component.Network = current.Network;
      component.RechargeEvery = current.RechargeEvery;
      component.Recharge = current.Recharge;
      component.BaseRecharge = current.BaseRecharge;
      component.RechargePer = current.RechargePer;
      component.RechargeAt = current.RechargeAt;
      component.Updated = current.Updated;
    }
  }
}
