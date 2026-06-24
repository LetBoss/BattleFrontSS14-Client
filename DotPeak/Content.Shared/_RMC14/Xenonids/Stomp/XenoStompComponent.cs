// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Stomp.XenoStompComponent
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
namespace Content.Shared._RMC14.Xenonids.Stomp;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoStompSystem)})]
public sealed class XenoStompComponent : 
  Component,
  ISerializationGenerated<XenoStompComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 30;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(0.4);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ParalyzeUnderOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Slows = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SlowBigInsteadOfStun;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DebuffsHurtXenosMore = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShortRange = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 2.82f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? SelfEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_footstep_charge1.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoStompComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoStompComponent) target1;
    if (serialization.TryCustomCopy<XenoStompComponent>(this, ref target, hookCtx, false, context))
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
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ParalyzeUnderOnly, ref target5, hookCtx, false, context))
      target5 = this.ParalyzeUnderOnly;
    target.ParalyzeUnderOnly = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Slows, ref target6, hookCtx, false, context))
      target6 = this.Slows;
    target.Slows = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.SlowBigInsteadOfStun, ref target8, hookCtx, false, context))
      target8 = this.SlowBigInsteadOfStun;
    target.SlowBigInsteadOfStun = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.DebuffsHurtXenosMore, ref target9, hookCtx, false, context))
      target9 = this.DebuffsHurtXenosMore;
    target.DebuffsHurtXenosMore = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShortRange, ref target10, hookCtx, false, context))
      target10 = this.ShortRange;
    target.ShortRange = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target11, hookCtx, false, context))
      target11 = this.Range;
    target.Range = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target12;
    EntProtoId? target13 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.SelfEffect, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId?>(this.SelfEffect, hookCtx, context);
    target.SelfEffect = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoStompComponent target,
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
    XenoStompComponent target1 = (XenoStompComponent) target;
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
    XenoStompComponent target1 = (XenoStompComponent) target;
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
    XenoStompComponent target1 = (XenoStompComponent) target;
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
  virtual XenoStompComponent Component.Instantiate() => new XenoStompComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoStompComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public TimeSpan ParalyzeTime;
    public bool ParalyzeUnderOnly;
    public bool Slows;
    public TimeSpan SlowTime;
    public bool SlowBigInsteadOfStun;
    public bool DebuffsHurtXenosMore;
    public float ShortRange;
    public float Range;
    public TimeSpan Delay;
    public EntProtoId? SelfEffect;
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoStompComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoStompComponent, ComponentGetState>(new ComponentEventRefHandler<XenoStompComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoStompComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoStompComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoStompComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoStompComponent.XenoStompComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        ParalyzeTime = component.ParalyzeTime,
        ParalyzeUnderOnly = component.ParalyzeUnderOnly,
        Slows = component.Slows,
        SlowTime = component.SlowTime,
        SlowBigInsteadOfStun = component.SlowBigInsteadOfStun,
        DebuffsHurtXenosMore = component.DebuffsHurtXenosMore,
        ShortRange = component.ShortRange,
        Range = component.Range,
        Delay = component.Delay,
        SelfEffect = component.SelfEffect,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoStompComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoStompComponent.XenoStompComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.ParalyzeTime = current.ParalyzeTime;
      component.ParalyzeUnderOnly = current.ParalyzeUnderOnly;
      component.Slows = current.Slows;
      component.SlowTime = current.SlowTime;
      component.SlowBigInsteadOfStun = current.SlowBigInsteadOfStun;
      component.DebuffsHurtXenosMore = current.DebuffsHurtXenosMore;
      component.ShortRange = current.ShortRange;
      component.Range = current.Range;
      component.Delay = current.Delay;
      component.SelfEffect = current.SelfEffect;
      component.Sound = current.Sound;
    }
  }
}
