// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailJab.XenoTailJabComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
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
namespace Content.Shared._RMC14.Xenonids.TailJab;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoTailJabSystem)})]
public sealed class XenoTailJabComponent : 
  Component,
  ISerializationGenerated<XenoTailJabComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId AttackEffect = (EntProtoId) "RMCEffectExtraSlash";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_tail_attack.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> Emote = (ProtoId<EmotePrototype>) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? EmoteCooldown = new TimeSpan?(TimeSpan.FromSeconds(5L));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowdownTime = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WallSlamSlowdownTime = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WallSlamStunTime = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowRange = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailJabComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailJabComponent) target1;
    if (serialization.TryCustomCopy<XenoTailJabComponent>(this, ref target, hookCtx, false, context))
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
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AttackEffect, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.AttackEffect, hookCtx, context);
    target.AttackEffect = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target4;
    ProtoId<EmotePrototype> target5 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target5;
    TimeSpan? target6 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmoteCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan?>(this.EmoteCooldown, hookCtx, context);
    target.EmoteCooldown = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowdownTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.SlowdownTime, hookCtx, context);
    target.SlowdownTime = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WallSlamSlowdownTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.WallSlamSlowdownTime, hookCtx, context);
    target.WallSlamSlowdownTime = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WallSlamStunTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.WallSlamStunTime, hookCtx, context);
    target.WallSlamStunTime = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowRange, ref target10, hookCtx, false, context))
      target10 = this.ThrowRange;
    target.ThrowRange = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailJabComponent target,
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
    XenoTailJabComponent target1 = (XenoTailJabComponent) target;
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
    XenoTailJabComponent target1 = (XenoTailJabComponent) target;
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
    XenoTailJabComponent target1 = (XenoTailJabComponent) target;
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
  virtual XenoTailJabComponent Component.Instantiate() => new XenoTailJabComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTailJabComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public EntProtoId AttackEffect;
    public SoundSpecifier Sound;
    public ProtoId<EmotePrototype> Emote;
    public TimeSpan? EmoteCooldown;
    public TimeSpan SlowdownTime;
    public TimeSpan WallSlamSlowdownTime;
    public TimeSpan WallSlamStunTime;
    public float ThrowRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTailJabComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTailJabComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTailJabComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTailJabComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTailJabComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTailJabComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTailJabComponent.XenoTailJabComponent_AutoState()
      {
        Damage = component.Damage,
        AttackEffect = component.AttackEffect,
        Sound = component.Sound,
        Emote = component.Emote,
        EmoteCooldown = component.EmoteCooldown,
        SlowdownTime = component.SlowdownTime,
        WallSlamSlowdownTime = component.WallSlamSlowdownTime,
        WallSlamStunTime = component.WallSlamStunTime,
        ThrowRange = component.ThrowRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTailJabComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTailJabComponent.XenoTailJabComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.AttackEffect = current.AttackEffect;
      component.Sound = current.Sound;
      component.Emote = current.Emote;
      component.EmoteCooldown = current.EmoteCooldown;
      component.SlowdownTime = current.SlowdownTime;
      component.WallSlamSlowdownTime = current.WallSlamSlowdownTime;
      component.WallSlamStunTime = current.WallSlamStunTime;
      component.ThrowRange = current.ThrowRange;
    }
  }
}
