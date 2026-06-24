// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Flurry.XenoFlurryComponent
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
namespace Content.Shared._RMC14.Xenonids.Flurry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoFlurryComponent : 
  Component,
  ISerializationGenerated<XenoFlurryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MaxTargets = new int?(4);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HealAmount = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HealDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HealCharges = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EmoteDelay = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier SlashSound = (SoundSpecifier) new SoundCollectionSpecifier("AlienClaw");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> Emote = (ProtoId<EmotePrototype>) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId AttackEffect = (EntProtoId) "RMCEffectExtraSlash";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TelegraphEffect = (EntProtoId) "RMCEffectXenoTelegraphRed";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HealEffect = (EntProtoId) "RMCEffectHealFlurry";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFlurryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFlurryComponent) target1;
    if (serialization.TryCustomCopy<XenoFlurryComponent>(this, ref target, hookCtx, false, context))
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
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxTargets, ref target4, hookCtx, false, context))
      target4 = this.MaxTargets;
    target.MaxTargets = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.HealAmount, ref target5, hookCtx, false, context))
      target5 = this.HealAmount;
    target.HealAmount = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.HealDelay, hookCtx, context);
    target.HealDelay = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.HealCharges, ref target7, hookCtx, false, context))
      target7 = this.HealCharges;
    target.HealCharges = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EmoteDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.EmoteDelay, hookCtx, context);
    target.EmoteDelay = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.SlashSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SlashSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.SlashSound, hookCtx, context);
    target.SlashSound = target9;
    ProtoId<EmotePrototype> target10 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AttackEffect, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.AttackEffect, hookCtx, context);
    target.AttackEffect = target11;
    EntProtoId target12 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TelegraphEffect, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntProtoId>(this.TelegraphEffect, hookCtx, context);
    target.TelegraphEffect = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HealEffect, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.HealEffect, hookCtx, context);
    target.HealEffect = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFlurryComponent target,
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
    XenoFlurryComponent target1 = (XenoFlurryComponent) target;
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
    XenoFlurryComponent target1 = (XenoFlurryComponent) target;
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
    XenoFlurryComponent target1 = (XenoFlurryComponent) target;
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
  virtual XenoFlurryComponent Component.Instantiate() => new XenoFlurryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFlurryComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public float Range;
    public int? MaxTargets;
    public int HealAmount;
    public TimeSpan HealDelay;
    public int HealCharges;
    public TimeSpan EmoteDelay;
    public SoundSpecifier SlashSound;
    public ProtoId<EmotePrototype> Emote;
    public EntProtoId AttackEffect;
    public EntProtoId TelegraphEffect;
    public EntProtoId HealEffect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFlurryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFlurryComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFlurryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFlurryComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFlurryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFlurryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFlurryComponent.XenoFlurryComponent_AutoState()
      {
        Damage = component.Damage,
        Range = component.Range,
        MaxTargets = component.MaxTargets,
        HealAmount = component.HealAmount,
        HealDelay = component.HealDelay,
        HealCharges = component.HealCharges,
        EmoteDelay = component.EmoteDelay,
        SlashSound = component.SlashSound,
        Emote = component.Emote,
        AttackEffect = component.AttackEffect,
        TelegraphEffect = component.TelegraphEffect,
        HealEffect = component.HealEffect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFlurryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFlurryComponent.XenoFlurryComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.Range = current.Range;
      component.MaxTargets = current.MaxTargets;
      component.HealAmount = current.HealAmount;
      component.HealDelay = current.HealDelay;
      component.HealCharges = current.HealCharges;
      component.EmoteDelay = current.EmoteDelay;
      component.SlashSound = current.SlashSound;
      component.Emote = current.Emote;
      component.AttackEffect = current.AttackEffect;
      component.TelegraphEffect = current.TelegraphEffect;
      component.HealEffect = current.HealEffect;
    }
  }
}
