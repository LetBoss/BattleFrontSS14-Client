// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Headbutt.XenoHeadbuttComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
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
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Headbutt;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoHeadbuttSystem)})]
public sealed class XenoHeadbuttComponent : 
  Component,
  ISerializationGenerated<XenoHeadbuttComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 10;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowForce = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CrestFortifiedThrowAdd = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "CMEffectPunch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_claw_block.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2? Charge;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AP = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier CrestedDamageReduction = new DamageSpecifier();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoHeadbuttComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoHeadbuttComponent) target1;
    if (serialization.TryCustomCopy<XenoHeadbuttComponent>(this, ref target, hookCtx, false, context))
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
    if (!serialization.TryCustomCopy<float>(this.ThrowForce, ref target5, hookCtx, false, context))
      target5 = this.ThrowForce;
    target.ThrowForce = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CrestFortifiedThrowAdd, ref target6, hookCtx, false, context))
      target6 = this.CrestFortifiedThrowAdd;
    target.CrestFortifiedThrowAdd = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target8;
    Vector2? target9 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.Charge, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2?>(this.Charge, hookCtx, context);
    target.Charge = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.AP, ref target10, hookCtx, false, context))
      target10 = this.AP;
    target.AP = target10;
    DamageSpecifier target11 = (DamageSpecifier) null;
    if (this.CrestedDamageReduction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CrestedDamageReduction, ref target11, hookCtx, false, context))
    {
      if (this.CrestedDamageReduction == null)
        target11 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CrestedDamageReduction, ref target11, hookCtx, context, true);
    }
    target.CrestedDamageReduction = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoHeadbuttComponent target,
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
    XenoHeadbuttComponent target1 = (XenoHeadbuttComponent) target;
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
    XenoHeadbuttComponent target1 = (XenoHeadbuttComponent) target;
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
    XenoHeadbuttComponent target1 = (XenoHeadbuttComponent) target;
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
  virtual XenoHeadbuttComponent Component.Instantiate() => new XenoHeadbuttComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoHeadbuttComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public float Range;
    public float ThrowForce;
    public float CrestFortifiedThrowAdd;
    public EntProtoId Effect;
    public SoundSpecifier Sound;
    public Vector2? Charge;
    public int AP;
    public DamageSpecifier CrestedDamageReduction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoHeadbuttComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoHeadbuttComponent, ComponentGetState>(new ComponentEventRefHandler<XenoHeadbuttComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoHeadbuttComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoHeadbuttComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoHeadbuttComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoHeadbuttComponent.XenoHeadbuttComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Range = component.Range,
        ThrowForce = component.ThrowForce,
        CrestFortifiedThrowAdd = component.CrestFortifiedThrowAdd,
        Effect = component.Effect,
        Sound = component.Sound,
        Charge = component.Charge,
        AP = component.AP,
        CrestedDamageReduction = component.CrestedDamageReduction
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoHeadbuttComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoHeadbuttComponent.XenoHeadbuttComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Range = current.Range;
      component.ThrowForce = current.ThrowForce;
      component.CrestFortifiedThrowAdd = current.CrestFortifiedThrowAdd;
      component.Effect = current.Effect;
      component.Sound = current.Sound;
      component.Charge = current.Charge;
      component.AP = current.AP;
      component.CrestedDamageReduction = current.CrestedDamageReduction;
    }
  }
}
