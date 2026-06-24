// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Impale.XenoImpaleComponent
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
namespace Content.Shared._RMC14.Xenonids.Impale;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoImpaleSystem)})]
public sealed class XenoImpaleComponent : 
  Component,
  ISerializationGenerated<XenoImpaleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PlasmaCost = 80 /*0x50*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Animation = (EntProtoId) "RMCEffectTailHit";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_tail_attack.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AP = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype>? Emote = (ProtoId<EmotePrototype>?) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? EmoteCooldown = new TimeSpan?(TimeSpan.FromSeconds(5L));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SecondImpaleTime = TimeSpan.FromSeconds(0.4);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoImpaleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoImpaleComponent) target1;
    if (serialization.TryCustomCopy<XenoImpaleComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = this.PlasmaCost;
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
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Animation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Animation, hookCtx, context);
    target.Animation = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.AP, ref target6, hookCtx, false, context))
      target6 = this.AP;
    target.AP = target6;
    ProtoId<EmotePrototype>? target7 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.Emote, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.Emote, hookCtx, context);
    target.Emote = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmoteCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.EmoteCooldown, hookCtx, context);
    target.EmoteCooldown = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SecondImpaleTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.SecondImpaleTime, hookCtx, context);
    target.SecondImpaleTime = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoImpaleComponent target,
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
    XenoImpaleComponent target1 = (XenoImpaleComponent) target;
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
    XenoImpaleComponent target1 = (XenoImpaleComponent) target;
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
    XenoImpaleComponent target1 = (XenoImpaleComponent) target;
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
  virtual XenoImpaleComponent Component.Instantiate() => new XenoImpaleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoImpaleComponent_AutoState : IComponentState
  {
    public int PlasmaCost;
    public DamageSpecifier Damage;
    public EntProtoId Animation;
    public SoundSpecifier Sound;
    public int AP;
    public ProtoId<EmotePrototype>? Emote;
    public TimeSpan? EmoteCooldown;
    public TimeSpan SecondImpaleTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoImpaleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoImpaleComponent, ComponentGetState>(new ComponentEventRefHandler<XenoImpaleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoImpaleComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoImpaleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoImpaleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoImpaleComponent.XenoImpaleComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Damage = component.Damage,
        Animation = component.Animation,
        Sound = component.Sound,
        AP = component.AP,
        Emote = component.Emote,
        EmoteCooldown = component.EmoteCooldown,
        SecondImpaleTime = component.SecondImpaleTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoImpaleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoImpaleComponent.XenoImpaleComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Damage = current.Damage;
      component.Animation = current.Animation;
      component.Sound = current.Sound;
      component.AP = current.AP;
      component.Emote = current.Emote;
      component.EmoteCooldown = current.EmoteCooldown;
      component.SecondImpaleTime = current.SecondImpaleTime;
    }
  }
}
