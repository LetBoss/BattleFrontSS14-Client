// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Retrieve.XenoRetrieveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Content.Shared.Chat.Prototypes;
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
namespace Content.Shared._RMC14.Xenonids.Retrieve;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoRetrieveSystem)})]
public sealed class XenoRetrieveComponent : 
  Component,
  ISerializationGenerated<XenoRetrieveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes SizeLimit = RMCSizes.Big;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(0.6);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Force = 15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_footstep_charge1.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype>? Emote = (ProtoId<EmotePrototype>?) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Visual = (EntProtoId) "RMCEffectXenoTelegraphGreen";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Visuals = new List<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRetrieveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRetrieveComponent) target1;
    if (serialization.TryCustomCopy<XenoRetrieveComponent>(this, ref target, hookCtx, false, context))
      return;
    RMCSizes target2 = RMCSizes.Small;
    if (!serialization.TryCustomCopy<RMCSizes>(this.SizeLimit, ref target2, hookCtx, false, context))
      target2 = this.SizeLimit;
    target.SizeLimit = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Force, ref target6, hookCtx, false, context))
      target6 = this.Force;
    target.Force = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
    ProtoId<EmotePrototype>? target8 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.Emote, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.Emote, hookCtx, context);
    target.Emote = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Visual, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.Visual, hookCtx, context);
    target.Visual = target9;
    List<EntityUid> target10 = (List<EntityUid>) null;
    if (this.Visuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Visuals, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<EntityUid>>(this.Visuals, hookCtx, context);
    target.Visuals = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRetrieveComponent target,
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
    XenoRetrieveComponent target1 = (XenoRetrieveComponent) target;
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
    XenoRetrieveComponent target1 = (XenoRetrieveComponent) target;
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
    XenoRetrieveComponent target1 = (XenoRetrieveComponent) target;
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
  virtual XenoRetrieveComponent Component.Instantiate() => new XenoRetrieveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoRetrieveComponent_AutoState : IComponentState
  {
    public RMCSizes SizeLimit;
    public TimeSpan Delay;
    public TimeSpan Cooldown;
    public float Range;
    public float Force;
    public SoundSpecifier Sound;
    public ProtoId<EmotePrototype>? Emote;
    public EntProtoId Visual;
    public List<NetEntity> Visuals;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoRetrieveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoRetrieveComponent, ComponentGetState>(new ComponentEventRefHandler<XenoRetrieveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoRetrieveComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoRetrieveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoRetrieveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoRetrieveComponent.XenoRetrieveComponent_AutoState()
      {
        SizeLimit = component.SizeLimit,
        Delay = component.Delay,
        Cooldown = component.Cooldown,
        Range = component.Range,
        Force = component.Force,
        Sound = component.Sound,
        Emote = component.Emote,
        Visual = component.Visual,
        Visuals = this.GetNetEntityList(component.Visuals)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoRetrieveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoRetrieveComponent.XenoRetrieveComponent_AutoState current))
        return;
      component.SizeLimit = current.SizeLimit;
      component.Delay = current.Delay;
      component.Cooldown = current.Cooldown;
      component.Range = current.Range;
      component.Force = current.Force;
      component.Sound = current.Sound;
      component.Emote = current.Emote;
      component.Visual = current.Visual;
      this.EnsureEntityList<XenoRetrieveComponent>(current.Visuals, uid, component.Visuals);
    }
  }
}
