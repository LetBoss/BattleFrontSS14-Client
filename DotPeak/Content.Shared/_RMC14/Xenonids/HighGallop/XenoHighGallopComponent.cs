// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.HighGallop.XenoHighGallopComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.Tag;
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
namespace Content.Shared._RMC14.Xenonids.HighGallop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoHighGallopComponent : 
  Component,
  ISerializationGenerated<XenoHighGallopComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TelegraphEffect = (EntProtoId) "RMCEffectXenoTelegraphRed";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TelegraphEffectEdge = (EntProtoId) "RMCEffectXenoTelegraphRedSmall";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Width = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Height = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunDuration = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_footstep_charge3.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> Emote = (ProtoId<EmotePrototype>) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<TagPrototype> Flingable = (ProtoId<TagPrototype>) "Grenade";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlingDistance = 3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoHighGallopComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoHighGallopComponent) target1;
    if (serialization.TryCustomCopy<XenoHighGallopComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TelegraphEffect, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.TelegraphEffect, hookCtx, context);
    target.TelegraphEffect = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TelegraphEffectEdge, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.TelegraphEffectEdge, hookCtx, context);
    target.TelegraphEffectEdge = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Width, ref target4, hookCtx, false, context))
      target4 = this.Width;
    target.Width = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Height, ref target5, hookCtx, false, context))
      target5 = this.Height;
    target.Height = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.StunDuration, hookCtx, context);
    target.StunDuration = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target8;
    ProtoId<EmotePrototype> target9 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target9;
    ProtoId<TagPrototype> target10 = new ProtoId<TagPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(this.Flingable, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<TagPrototype>>(this.Flingable, hookCtx, context);
    target.Flingable = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlingDistance, ref target11, hookCtx, false, context))
      target11 = this.FlingDistance;
    target.FlingDistance = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoHighGallopComponent target,
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
    XenoHighGallopComponent target1 = (XenoHighGallopComponent) target;
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
    XenoHighGallopComponent target1 = (XenoHighGallopComponent) target;
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
    XenoHighGallopComponent target1 = (XenoHighGallopComponent) target;
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
  virtual XenoHighGallopComponent Component.Instantiate() => new XenoHighGallopComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoHighGallopComponent_AutoState : IComponentState
  {
    public EntProtoId TelegraphEffect;
    public EntProtoId TelegraphEffectEdge;
    public float Width;
    public float Height;
    public TimeSpan StunDuration;
    public TimeSpan SlowDuration;
    public SoundSpecifier Sound;
    public ProtoId<EmotePrototype> Emote;
    public ProtoId<TagPrototype> Flingable;
    public float FlingDistance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoHighGallopComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoHighGallopComponent, ComponentGetState>(new ComponentEventRefHandler<XenoHighGallopComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoHighGallopComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoHighGallopComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoHighGallopComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoHighGallopComponent.XenoHighGallopComponent_AutoState()
      {
        TelegraphEffect = component.TelegraphEffect,
        TelegraphEffectEdge = component.TelegraphEffectEdge,
        Width = component.Width,
        Height = component.Height,
        StunDuration = component.StunDuration,
        SlowDuration = component.SlowDuration,
        Sound = component.Sound,
        Emote = component.Emote,
        Flingable = component.Flingable,
        FlingDistance = component.FlingDistance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoHighGallopComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoHighGallopComponent.XenoHighGallopComponent_AutoState current))
        return;
      component.TelegraphEffect = current.TelegraphEffect;
      component.TelegraphEffectEdge = current.TelegraphEffectEdge;
      component.Width = current.Width;
      component.Height = current.Height;
      component.StunDuration = current.StunDuration;
      component.SlowDuration = current.SlowDuration;
      component.Sound = current.Sound;
      component.Emote = current.Emote;
      component.Flingable = current.Flingable;
      component.FlingDistance = current.FlingDistance;
    }
  }
}
