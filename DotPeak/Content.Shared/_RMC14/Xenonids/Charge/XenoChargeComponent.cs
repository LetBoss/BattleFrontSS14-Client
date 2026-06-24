// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.XenoChargeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoChargeSystem)})]
public sealed class XenoChargeComponent : 
  Component,
  ISerializationGenerated<XenoChargeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 20;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 8f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SlowRange = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(3.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ChargeDelay = TimeSpan.FromSeconds(1.2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_claw_block.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2? Charge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Strength = 20f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoChargeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoChargeComponent) target1;
    if (serialization.TryCustomCopy<XenoChargeComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target3, hookCtx, false, context))
    {
      if (this.Damage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target3, hookCtx, context, true);
    }
    target.Damage = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target4, hookCtx, false, context))
      target4 = this.Range;
    target.Range = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowRange, ref target5, hookCtx, false, context))
      target5 = this.SlowRange;
    target.SlowRange = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ChargeDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.ChargeDelay, hookCtx, context);
    target.ChargeDelay = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target9;
    Vector2? target10 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.Charge, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Vector2?>(this.Charge, hookCtx, context);
    target.Charge = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Strength, ref target11, hookCtx, false, context))
      target11 = this.Strength;
    target.Strength = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoChargeComponent target,
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
    XenoChargeComponent target1 = (XenoChargeComponent) target;
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
    XenoChargeComponent target1 = (XenoChargeComponent) target;
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
    XenoChargeComponent target1 = (XenoChargeComponent) target;
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
  virtual XenoChargeComponent Component.Instantiate() => new XenoChargeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoChargeComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public float Range;
    public float SlowRange;
    public TimeSpan SlowTime;
    public TimeSpan StunTime;
    public TimeSpan ChargeDelay;
    public SoundSpecifier Sound;
    public Vector2? Charge;
    public float Strength;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoChargeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoChargeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoChargeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoChargeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoChargeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoChargeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoChargeComponent.XenoChargeComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Range = component.Range,
        SlowRange = component.SlowRange,
        SlowTime = component.SlowTime,
        StunTime = component.StunTime,
        ChargeDelay = component.ChargeDelay,
        Sound = component.Sound,
        Charge = component.Charge,
        Strength = component.Strength
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoChargeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoChargeComponent.XenoChargeComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Range = current.Range;
      component.SlowRange = current.SlowRange;
      component.SlowTime = current.SlowTime;
      component.StunTime = current.StunTime;
      component.ChargeDelay = current.ChargeDelay;
      component.Sound = current.Sound;
      component.Charge = current.Charge;
      component.Strength = current.Strength;
    }
  }
}
