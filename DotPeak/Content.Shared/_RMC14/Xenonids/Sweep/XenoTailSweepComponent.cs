// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Sweep.XenoTailSweepComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Sweep;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoTailSweepSystem)})]
public sealed class XenoTailSweepComponent : 
  Component,
  ISerializationGenerated<XenoTailSweepComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float KnockBackDistance = 1f;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("XenoTailSwipe");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HitEffect = (EntProtoId) "CMEffectPunch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier HitSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_claw_block.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailSweepComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailSweepComponent) target1;
    if (serialization.TryCustomCopy<XenoTailSweepComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.KnockBackDistance, ref target4, hookCtx, false, context))
      target4 = this.KnockBackDistance;
    target.KnockBackDistance = target4;
    DamageSpecifier target5 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target5, hookCtx, false, context))
    {
      if (this.Damage == null)
        target5 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target5, hookCtx, context);
    }
    target.Damage = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HitEffect, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.HitEffect, hookCtx, context);
    target.HitEffect = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.HitSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HitSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.HitSound, hookCtx, context);
    target.HitSound = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailSweepComponent target,
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
    XenoTailSweepComponent target1 = (XenoTailSweepComponent) target;
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
    XenoTailSweepComponent target1 = (XenoTailSweepComponent) target;
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
    XenoTailSweepComponent target1 = (XenoTailSweepComponent) target;
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
  virtual XenoTailSweepComponent Component.Instantiate() => new XenoTailSweepComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTailSweepComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public float Range;
    public float KnockBackDistance;
    public TimeSpan ParalyzeTime;
    public SoundSpecifier Sound;
    public EntProtoId HitEffect;
    public SoundSpecifier HitSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTailSweepComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTailSweepComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTailSweepComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTailSweepComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTailSweepComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTailSweepComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTailSweepComponent.XenoTailSweepComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Range = component.Range,
        KnockBackDistance = component.KnockBackDistance,
        ParalyzeTime = component.ParalyzeTime,
        Sound = component.Sound,
        HitEffect = component.HitEffect,
        HitSound = component.HitSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTailSweepComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTailSweepComponent.XenoTailSweepComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Range = current.Range;
      component.KnockBackDistance = current.KnockBackDistance;
      component.ParalyzeTime = current.ParalyzeTime;
      component.Sound = current.Sound;
      component.HitEffect = current.HitEffect;
      component.HitSound = current.HitSound;
    }
  }
}
