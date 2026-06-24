// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Blitz.XenoBlitzComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
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
namespace Content.Shared._RMC14.Xenonids.Blitz;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoBlitzComponent : 
  Component,
  ISerializationGenerated<XenoBlitzComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PlasmaCost = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BaseUseDelay = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FinishedUseDelay = TimeSpan.FromSeconds(11L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Dashed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SlashReady;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlashAroundAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectExtraSlash";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("AlienClaw");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlashDashTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionToReset = (EntProtoId) "ActionXenoBlitz";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HitsToRecharge = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FirstPartActivatedAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoBlitzComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoBlitzComponent) target1;
    if (serialization.TryCustomCopy<XenoBlitzComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = this.PlasmaCost;
    target.PlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BaseUseDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.BaseUseDelay, hookCtx, context);
    target.BaseUseDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FinishedUseDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FinishedUseDelay, hookCtx, context);
    target.FinishedUseDelay = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Dashed, ref target5, hookCtx, false, context))
      target5 = this.Dashed;
    target.Dashed = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.SlashReady, ref target6, hookCtx, false, context))
      target6 = this.SlashReady;
    target.SlashReady = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlashAroundAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.SlashAroundAt, hookCtx, context);
    target.SlashAroundAt = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target8, hookCtx, false, context))
    {
      if (this.Damage == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target8, hookCtx, context, true);
    }
    target.Damage = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlashDashTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.SlashDashTime, hookCtx, context);
    target.SlashDashTime = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target12, hookCtx, false, context))
      target12 = this.Range;
    target.Range = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionToReset, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.ActionToReset, hookCtx, context);
    target.ActionToReset = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.HitsToRecharge, ref target14, hookCtx, false, context))
      target14 = this.HitsToRecharge;
    target.HitsToRecharge = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FirstPartActivatedAt, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.FirstPartActivatedAt, hookCtx, context);
    target.FirstPartActivatedAt = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoBlitzComponent target,
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
    XenoBlitzComponent target1 = (XenoBlitzComponent) target;
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
    XenoBlitzComponent target1 = (XenoBlitzComponent) target;
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
    XenoBlitzComponent target1 = (XenoBlitzComponent) target;
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
  virtual XenoBlitzComponent Component.Instantiate() => new XenoBlitzComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoBlitzComponent_AutoState : IComponentState
  {
    public int PlasmaCost;
    public TimeSpan BaseUseDelay;
    public TimeSpan FinishedUseDelay;
    public bool Dashed;
    public bool SlashReady;
    public TimeSpan SlashAroundAt;
    public DamageSpecifier Damage;
    public EntProtoId Effect;
    public SoundSpecifier Sound;
    public TimeSpan SlashDashTime;
    public float Range;
    public EntProtoId ActionToReset;
    public int HitsToRecharge;
    public TimeSpan FirstPartActivatedAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoBlitzComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoBlitzComponent, ComponentGetState>(new ComponentEventRefHandler<XenoBlitzComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoBlitzComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoBlitzComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoBlitzComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoBlitzComponent.XenoBlitzComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        BaseUseDelay = component.BaseUseDelay,
        FinishedUseDelay = component.FinishedUseDelay,
        Dashed = component.Dashed,
        SlashReady = component.SlashReady,
        SlashAroundAt = component.SlashAroundAt,
        Damage = component.Damage,
        Effect = component.Effect,
        Sound = component.Sound,
        SlashDashTime = component.SlashDashTime,
        Range = component.Range,
        ActionToReset = component.ActionToReset,
        HitsToRecharge = component.HitsToRecharge,
        FirstPartActivatedAt = component.FirstPartActivatedAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoBlitzComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoBlitzComponent.XenoBlitzComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.BaseUseDelay = current.BaseUseDelay;
      component.FinishedUseDelay = current.FinishedUseDelay;
      component.Dashed = current.Dashed;
      component.SlashReady = current.SlashReady;
      component.SlashAroundAt = current.SlashAroundAt;
      component.Damage = current.Damage;
      component.Effect = current.Effect;
      component.Sound = current.Sound;
      component.SlashDashTime = current.SlashDashTime;
      component.Range = current.Range;
      component.ActionToReset = current.ActionToReset;
      component.HitsToRecharge = current.HitsToRecharge;
      component.FirstPartActivatedAt = current.FirstPartActivatedAt;
    }
  }
}
