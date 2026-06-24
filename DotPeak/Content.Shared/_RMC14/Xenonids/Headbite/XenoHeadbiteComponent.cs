// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Headbite.XenoHeadbiteComponent
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
namespace Content.Shared._RMC14.Xenonids.Headbite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoHeadbiteSystem)})]
public sealed class XenoHeadbiteComponent : 
  Component,
  ISerializationGenerated<XenoHeadbiteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HeadbiteDelay = TimeSpan.FromSeconds(0.8);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HealDelay = TimeSpan.FromSeconds(0.05);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier HitSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_bite2.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HealEffect = (EntProtoId) "RMCEffectHealHeadbite";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HealAmount = 150;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HeadbiteEffect = (EntProtoId) "RMCEffectHeadbite";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> Emote = (ProtoId<EmotePrototype>) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? EmoteCooldown = new TimeSpan?(TimeSpan.FromSeconds(5L));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan JitterTime = TimeSpan.FromSeconds(3L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoHeadbiteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoHeadbiteComponent) target1;
    if (serialization.TryCustomCopy<XenoHeadbiteComponent>(this, ref target, hookCtx, false, context))
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
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HeadbiteDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.HeadbiteDelay, hookCtx, context);
    target.HeadbiteDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HealDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.HealDelay, hookCtx, context);
    target.HealDelay = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.HitSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HitSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.HitSound, hookCtx, context);
    target.HitSound = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HealEffect, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.HealEffect, hookCtx, context);
    target.HealEffect = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.HealAmount, ref target7, hookCtx, false, context))
      target7 = this.HealAmount;
    target.HealAmount = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HeadbiteEffect, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.HeadbiteEffect, hookCtx, context);
    target.HeadbiteEffect = target8;
    ProtoId<EmotePrototype> target9 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmoteCooldown, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.EmoteCooldown, hookCtx, context);
    target.EmoteCooldown = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JitterTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.JitterTime, hookCtx, context);
    target.JitterTime = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoHeadbiteComponent target,
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
    XenoHeadbiteComponent target1 = (XenoHeadbiteComponent) target;
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
    XenoHeadbiteComponent target1 = (XenoHeadbiteComponent) target;
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
    XenoHeadbiteComponent target1 = (XenoHeadbiteComponent) target;
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
  virtual XenoHeadbiteComponent Component.Instantiate() => new XenoHeadbiteComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoHeadbiteComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public TimeSpan HeadbiteDelay;
    public TimeSpan HealDelay;
    public SoundSpecifier HitSound;
    public EntProtoId HealEffect;
    public int HealAmount;
    public EntProtoId HeadbiteEffect;
    public ProtoId<EmotePrototype> Emote;
    public TimeSpan? EmoteCooldown;
    public TimeSpan JitterTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoHeadbiteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoHeadbiteComponent, ComponentGetState>(new ComponentEventRefHandler<XenoHeadbiteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoHeadbiteComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoHeadbiteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoHeadbiteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoHeadbiteComponent.XenoHeadbiteComponent_AutoState()
      {
        Damage = component.Damage,
        HeadbiteDelay = component.HeadbiteDelay,
        HealDelay = component.HealDelay,
        HitSound = component.HitSound,
        HealEffect = component.HealEffect,
        HealAmount = component.HealAmount,
        HeadbiteEffect = component.HeadbiteEffect,
        Emote = component.Emote,
        EmoteCooldown = component.EmoteCooldown,
        JitterTime = component.JitterTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoHeadbiteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoHeadbiteComponent.XenoHeadbiteComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.HeadbiteDelay = current.HeadbiteDelay;
      component.HealDelay = current.HealDelay;
      component.HitSound = current.HitSound;
      component.HealEffect = current.HealEffect;
      component.HealAmount = current.HealAmount;
      component.HeadbiteEffect = current.HeadbiteEffect;
      component.Emote = current.Emote;
      component.EmoteCooldown = current.EmoteCooldown;
      component.JitterTime = current.JitterTime;
    }
  }
}
