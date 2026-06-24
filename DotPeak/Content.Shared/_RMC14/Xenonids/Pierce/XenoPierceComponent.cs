// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Pierce.XenoPierceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
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
namespace Content.Shared._RMC14.Xenonids.Pierce;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoPierceSystem)})]
public sealed class XenoPierceComponent : 
  Component,
  ISerializationGenerated<XenoPierceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AP = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MaxTargets = new int?(4);
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
  public FixedPoint2 Range = FixedPoint2.New(3);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RechargeTargetsRequired = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Blocker = (EntProtoId) "RMCEffectXenoTelegraphInvisible";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoPierceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoPierceComponent) target1;
    if (serialization.TryCustomCopy<XenoPierceComponent>(this, ref target, hookCtx, false, context))
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
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.AP, ref target3, hookCtx, false, context))
      target3 = this.AP;
    target.AP = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxTargets, ref target4, hookCtx, false, context))
      target4 = this.MaxTargets;
    target.MaxTargets = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AttackEffect, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.AttackEffect, hookCtx, context);
    target.AttackEffect = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    ProtoId<EmotePrototype> target7 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmoteCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.EmoteCooldown, hookCtx, context);
    target.EmoteCooldown = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Range, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.Range, hookCtx, context);
    target.Range = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.RechargeTargetsRequired, ref target10, hookCtx, false, context))
      target10 = this.RechargeTargetsRequired;
    target.RechargeTargetsRequired = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Blocker, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.Blocker, hookCtx, context);
    target.Blocker = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoPierceComponent target,
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
    XenoPierceComponent target1 = (XenoPierceComponent) target;
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
    XenoPierceComponent target1 = (XenoPierceComponent) target;
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
    XenoPierceComponent target1 = (XenoPierceComponent) target;
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
  virtual XenoPierceComponent Component.Instantiate() => new XenoPierceComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoPierceComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public int AP;
    public int? MaxTargets;
    public EntProtoId AttackEffect;
    public SoundSpecifier Sound;
    public ProtoId<EmotePrototype> Emote;
    public TimeSpan? EmoteCooldown;
    public FixedPoint2 Range;
    public int RechargeTargetsRequired;
    public EntProtoId Blocker;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoPierceComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoPierceComponent, ComponentGetState>(new ComponentEventRefHandler<XenoPierceComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoPierceComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoPierceComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoPierceComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoPierceComponent.XenoPierceComponent_AutoState()
      {
        Damage = component.Damage,
        AP = component.AP,
        MaxTargets = component.MaxTargets,
        AttackEffect = component.AttackEffect,
        Sound = component.Sound,
        Emote = component.Emote,
        EmoteCooldown = component.EmoteCooldown,
        Range = component.Range,
        RechargeTargetsRequired = component.RechargeTargetsRequired,
        Blocker = component.Blocker
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoPierceComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoPierceComponent.XenoPierceComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.AP = current.AP;
      component.MaxTargets = current.MaxTargets;
      component.AttackEffect = current.AttackEffect;
      component.Sound = current.Sound;
      component.Emote = current.Emote;
      component.EmoteCooldown = current.EmoteCooldown;
      component.Range = current.Range;
      component.RechargeTargetsRequired = current.RechargeTargetsRequired;
      component.Blocker = current.Blocker;
    }
  }
}
