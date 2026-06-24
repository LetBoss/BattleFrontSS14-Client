// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fling.XenoFlingComponent
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
namespace Content.Shared._RMC14.Xenonids.Fling;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoFlingSystem)})]
public sealed class XenoFlingComponent : 
  Component,
  ISerializationGenerated<XenoFlingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EnragedRange;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowSpeed = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "CMEffectPunch";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HealAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int EnragedHealAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HealDelay = TimeSpan.FromSeconds(0.05);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_claw_block.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFlingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFlingComponent) target1;
    if (serialization.TryCustomCopy<XenoFlingComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnragedRange, ref target4, hookCtx, false, context))
      target4 = this.EnragedRange;
    target.EnragedRange = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowSpeed, ref target5, hookCtx, false, context))
      target5 = this.ThrowSpeed;
    target.ThrowSpeed = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.HealAmount, ref target10, hookCtx, false, context))
      target10 = this.HealAmount;
    target.HealAmount = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.EnragedHealAmount, ref target11, hookCtx, false, context))
      target11 = this.EnragedHealAmount;
    target.EnragedHealAmount = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.HealDelay, hookCtx, context);
    target.HealDelay = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFlingComponent target,
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
    XenoFlingComponent target1 = (XenoFlingComponent) target;
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
    XenoFlingComponent target1 = (XenoFlingComponent) target;
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
    XenoFlingComponent target1 = (XenoFlingComponent) target;
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
  virtual XenoFlingComponent Component.Instantiate() => new XenoFlingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFlingComponent_AutoState : IComponentState
  {
    public float Range;
    public float EnragedRange;
    public float ThrowSpeed;
    public TimeSpan ParalyzeTime;
    public TimeSpan SlowTime;
    public TimeSpan DazeTime;
    public EntProtoId Effect;
    public int HealAmount;
    public int EnragedHealAmount;
    public TimeSpan HealDelay;
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFlingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFlingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFlingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFlingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFlingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFlingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFlingComponent.XenoFlingComponent_AutoState()
      {
        Range = component.Range,
        EnragedRange = component.EnragedRange,
        ThrowSpeed = component.ThrowSpeed,
        ParalyzeTime = component.ParalyzeTime,
        SlowTime = component.SlowTime,
        DazeTime = component.DazeTime,
        Effect = component.Effect,
        HealAmount = component.HealAmount,
        EnragedHealAmount = component.EnragedHealAmount,
        HealDelay = component.HealDelay,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFlingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFlingComponent.XenoFlingComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.EnragedRange = current.EnragedRange;
      component.ThrowSpeed = current.ThrowSpeed;
      component.ParalyzeTime = current.ParalyzeTime;
      component.SlowTime = current.SlowTime;
      component.DazeTime = current.DazeTime;
      component.Effect = current.Effect;
      component.HealAmount = current.HealAmount;
      component.EnragedHealAmount = current.EnragedHealAmount;
      component.HealDelay = current.HealDelay;
      component.Sound = current.Sound;
    }
  }
}
