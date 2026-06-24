// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.CrusherShieldComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Shields;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class CrusherShieldComponent : 
  Component,
  ISerializationGenerated<CrusherShieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExplosionOffAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShieldOffAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ExplosionResistApplying;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ExplosionResistance = 1000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DamageReduction = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExplosionResistanceDuration = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShieldDuration = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = FixedPoint2.New(50);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Amount = FixedPoint2.New(200);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectEmpowerBrown";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CrusherShieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CrusherShieldComponent) target1;
    if (serialization.TryCustomCopy<CrusherShieldComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExplosionOffAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ExplosionOffAt, hookCtx, context);
    target.ExplosionOffAt = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShieldOffAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ShieldOffAt, hookCtx, context);
    target.ShieldOffAt = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExplosionResistApplying, ref target4, hookCtx, false, context))
      target4 = this.ExplosionResistApplying;
    target.ExplosionResistApplying = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExplosionResistance, ref target5, hookCtx, false, context))
      target5 = this.ExplosionResistance;
    target.ExplosionResistance = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.DamageReduction, ref target6, hookCtx, false, context))
      target6 = this.DamageReduction;
    target.DamageReduction = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExplosionResistanceDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ExplosionResistanceDuration, hookCtx, context);
    target.ExplosionResistanceDuration = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShieldDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.ShieldDuration, hookCtx, context);
    target.ShieldDuration = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target9;
    FixedPoint2 target10 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Amount, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<FixedPoint2>(this.Amount, hookCtx, context);
    target.Amount = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CrusherShieldComponent target,
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
    CrusherShieldComponent target1 = (CrusherShieldComponent) target;
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
    CrusherShieldComponent target1 = (CrusherShieldComponent) target;
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
    CrusherShieldComponent target1 = (CrusherShieldComponent) target;
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
  virtual CrusherShieldComponent Component.Instantiate() => new CrusherShieldComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CrusherShieldComponent_AutoState : IComponentState
  {
    public TimeSpan ExplosionOffAt;
    public TimeSpan ShieldOffAt;
    public bool ExplosionResistApplying;
    public int ExplosionResistance;
    public int DamageReduction;
    public TimeSpan ExplosionResistanceDuration;
    public TimeSpan ShieldDuration;
    public FixedPoint2 PlasmaCost;
    public FixedPoint2 Amount;
    public EntProtoId Effect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CrusherShieldComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CrusherShieldComponent, ComponentGetState>(new ComponentEventRefHandler<CrusherShieldComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CrusherShieldComponent, ComponentHandleState>(new ComponentEventRefHandler<CrusherShieldComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CrusherShieldComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CrusherShieldComponent.CrusherShieldComponent_AutoState()
      {
        ExplosionOffAt = component.ExplosionOffAt,
        ShieldOffAt = component.ShieldOffAt,
        ExplosionResistApplying = component.ExplosionResistApplying,
        ExplosionResistance = component.ExplosionResistance,
        DamageReduction = component.DamageReduction,
        ExplosionResistanceDuration = component.ExplosionResistanceDuration,
        ShieldDuration = component.ShieldDuration,
        PlasmaCost = component.PlasmaCost,
        Amount = component.Amount,
        Effect = component.Effect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CrusherShieldComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CrusherShieldComponent.CrusherShieldComponent_AutoState current))
        return;
      component.ExplosionOffAt = current.ExplosionOffAt;
      component.ShieldOffAt = current.ShieldOffAt;
      component.ExplosionResistApplying = current.ExplosionResistApplying;
      component.ExplosionResistance = current.ExplosionResistance;
      component.DamageReduction = current.DamageReduction;
      component.ExplosionResistanceDuration = current.ExplosionResistanceDuration;
      component.ShieldDuration = current.ShieldDuration;
      component.PlasmaCost = current.PlasmaCost;
      component.Amount = current.Amount;
      component.Effect = current.Effect;
    }
  }
}
