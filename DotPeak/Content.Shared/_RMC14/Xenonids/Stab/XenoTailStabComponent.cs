// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Stab.XenoTailStabComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Stab;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoTailStabSystem)})]
public sealed class XenoTailStabComponent : 
  Component,
  ISerializationGenerated<XenoTailStabComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TailAnimationId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HitAnimationId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TailRange;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier TailDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier SoundHit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier SoundMiss;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ChargeTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BigDazeTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>? Inject;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Toggle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InjectNeuro;

  public XenoTailStabComponent()
  {
    SoundCollectionSpecifier collectionSpecifier1 = new SoundCollectionSpecifier("XenoBite");
    collectionSpecifier1.Params = AudioParams.Default.WithVariation(new float?(0.15f)).WithVolume(-3f);
    this.SoundHit = (SoundSpecifier) collectionSpecifier1;
    SoundCollectionSpecifier collectionSpecifier2 = new SoundCollectionSpecifier("XenoTailSwipe");
    collectionSpecifier2.Params = AudioParams.Default.WithVariation(new float?(0.15f));
    this.SoundMiss = (SoundSpecifier) collectionSpecifier2;
    this.ChargeTime = 1f;
    this.DazeTime = TimeSpan.FromSeconds(1L);
    this.BigDazeTime = TimeSpan.FromSeconds(3L);
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailStabComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailStabComponent) target1;
    if (serialization.TryCustomCopy<XenoTailStabComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TailAnimationId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.TailAnimationId, hookCtx, context);
    target.TailAnimationId = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HitAnimationId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.HitAnimationId, hookCtx, context);
    target.HitAnimationId = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TailRange, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.TailRange, hookCtx, context);
    target.TailRange = target4;
    DamageSpecifier target5 = (DamageSpecifier) null;
    if (this.TailDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.TailDamage, ref target5, hookCtx, false, context))
    {
      if (this.TailDamage == null)
        target5 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.TailDamage, ref target5, hookCtx, context, true);
    }
    target.TailDamage = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.SoundHit == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundHit, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.SoundHit, hookCtx, context);
    target.SoundHit = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.SoundMiss == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundMiss, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.SoundMiss, hookCtx, context);
    target.SoundMiss = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ChargeTime, ref target8, hookCtx, false, context))
      target8 = this.ChargeTime;
    target.ChargeTime = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BigDazeTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.BigDazeTime, hookCtx, context);
    target.BigDazeTime = target10;
    Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> target11 = (Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>>(this.Inject, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>>(this.Inject, hookCtx, context);
    target.Inject = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Toggle, ref target12, hookCtx, false, context))
      target12 = this.Toggle;
    target.Toggle = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.InjectNeuro, ref target13, hookCtx, false, context))
      target13 = this.InjectNeuro;
    target.InjectNeuro = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailStabComponent target,
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
    XenoTailStabComponent target1 = (XenoTailStabComponent) target;
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
    XenoTailStabComponent target1 = (XenoTailStabComponent) target;
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
    XenoTailStabComponent target1 = (XenoTailStabComponent) target;
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
  virtual XenoTailStabComponent Component.Instantiate() => new XenoTailStabComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTailStabComponent_AutoState : IComponentState
  {
    public EntProtoId TailAnimationId;
    public EntProtoId HitAnimationId;
    public FixedPoint2 TailRange;
    public SoundSpecifier SoundHit;
    public SoundSpecifier SoundMiss;
    public float ChargeTime;
    public TimeSpan DazeTime;
    public TimeSpan BigDazeTime;
    public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>? Inject;
    public bool Toggle;
    public bool InjectNeuro;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTailStabComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTailStabComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTailStabComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTailStabComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTailStabComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTailStabComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTailStabComponent.XenoTailStabComponent_AutoState()
      {
        TailAnimationId = component.TailAnimationId,
        HitAnimationId = component.HitAnimationId,
        TailRange = component.TailRange,
        SoundHit = component.SoundHit,
        SoundMiss = component.SoundMiss,
        ChargeTime = component.ChargeTime,
        DazeTime = component.DazeTime,
        BigDazeTime = component.BigDazeTime,
        Inject = component.Inject,
        Toggle = component.Toggle,
        InjectNeuro = component.InjectNeuro
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTailStabComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTailStabComponent.XenoTailStabComponent_AutoState current))
        return;
      component.TailAnimationId = current.TailAnimationId;
      component.HitAnimationId = current.HitAnimationId;
      component.TailRange = current.TailRange;
      component.SoundHit = current.SoundHit;
      component.SoundMiss = current.SoundMiss;
      component.ChargeTime = current.ChargeTime;
      component.DazeTime = current.DazeTime;
      component.BigDazeTime = current.BigDazeTime;
      component.Inject = current.Inject == null ? (Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>) null : new Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>((IDictionary<ProtoId<ReagentPrototype>, FixedPoint2>) current.Inject);
      component.Toggle = current.Toggle;
      component.InjectNeuro = current.InjectNeuro;
    }
  }
}
