// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.VanguardShieldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Shields;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VanguardShieldComponent : 
  Component,
  ISerializationGenerated<VanguardShieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 RegenAmount = FixedPoint2.New(800);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ExplosionResistance = 75;
  [DataField(null, false, 1, false, false, null)]
  public bool WasHit;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextDecay;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DecayEvery = TimeSpan.FromSeconds(0.4);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DecayMult = 0.7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DecaySub = 50f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LastTimeHit;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RechargeTime = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LastRecharge;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BuffExtraTime = TimeSpan.FromSeconds(0.7);
  [DataField(null, false, 1, false, false, null)]
  public float DecayThreshold = 5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VanguardShieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VanguardShieldComponent) target1;
    if (serialization.TryCustomCopy<VanguardShieldComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RegenAmount, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.RegenAmount, hookCtx, context);
    target.RegenAmount = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExplosionResistance, ref target3, hookCtx, false, context))
      target3 = this.ExplosionResistance;
    target.ExplosionResistance = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.WasHit, ref target4, hookCtx, false, context))
      target4 = this.WasHit;
    target.WasHit = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDecay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextDecay, hookCtx, context);
    target.NextDecay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DecayEvery, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DecayEvery, hookCtx, context);
    target.DecayEvery = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecayMult, ref target7, hookCtx, false, context))
      target7 = this.DecayMult;
    target.DecayMult = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecaySub, ref target8, hookCtx, false, context))
      target8 = this.DecaySub;
    target.DecaySub = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastTimeHit, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.LastTimeHit, hookCtx, context);
    target.LastTimeHit = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RechargeTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.RechargeTime, hookCtx, context);
    target.RechargeTime = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastRecharge, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.LastRecharge, hookCtx, context);
    target.LastRecharge = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BuffExtraTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.BuffExtraTime, hookCtx, context);
    target.BuffExtraTime = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DecayThreshold, ref target13, hookCtx, false, context))
      target13 = this.DecayThreshold;
    target.DecayThreshold = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VanguardShieldComponent target,
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
    VanguardShieldComponent target1 = (VanguardShieldComponent) target;
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
    VanguardShieldComponent target1 = (VanguardShieldComponent) target;
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
    VanguardShieldComponent target1 = (VanguardShieldComponent) target;
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
  virtual VanguardShieldComponent Component.Instantiate() => new VanguardShieldComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VanguardShieldComponent_AutoState : IComponentState
  {
    public FixedPoint2 RegenAmount;
    public int ExplosionResistance;
    public float DecayMult;
    public float DecaySub;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VanguardShieldComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VanguardShieldComponent, ComponentGetState>(new ComponentEventRefHandler<VanguardShieldComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VanguardShieldComponent, ComponentHandleState>(new ComponentEventRefHandler<VanguardShieldComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VanguardShieldComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VanguardShieldComponent.VanguardShieldComponent_AutoState()
      {
        RegenAmount = component.RegenAmount,
        ExplosionResistance = component.ExplosionResistance,
        DecayMult = component.DecayMult,
        DecaySub = component.DecaySub
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VanguardShieldComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VanguardShieldComponent.VanguardShieldComponent_AutoState current))
        return;
      component.RegenAmount = current.RegenAmount;
      component.ExplosionResistance = current.ExplosionResistance;
      component.DecayMult = current.DecayMult;
      component.DecaySub = current.DecaySub;
    }
  }
}
