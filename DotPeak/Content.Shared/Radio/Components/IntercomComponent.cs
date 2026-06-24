// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.Components.IntercomComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
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
namespace Content.Shared.Radio.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class IntercomComponent : 
  Component,
  ISerializationGenerated<IntercomComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool RequiresPower = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SpeakerEnabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MicrophoneEnabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<RadioChannelPrototype>? CurrentChannel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<RadioChannelPrototype>> SupportedChannels = new List<ProtoId<RadioChannelPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntercomComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntercomComponent) target1;
    if (serialization.TryCustomCopy<IntercomComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresPower, ref target2, hookCtx, false, context))
      target2 = this.RequiresPower;
    target.RequiresPower = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpeakerEnabled, ref target3, hookCtx, false, context))
      target3 = this.SpeakerEnabled;
    target.SpeakerEnabled = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.MicrophoneEnabled, ref target4, hookCtx, false, context))
      target4 = this.MicrophoneEnabled;
    target.MicrophoneEnabled = target4;
    ProtoId<RadioChannelPrototype>? target5 = new ProtoId<RadioChannelPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>?>(this.CurrentChannel, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>?>(this.CurrentChannel, hookCtx, context);
    target.CurrentChannel = target5;
    List<ProtoId<RadioChannelPrototype>> target6 = (List<ProtoId<RadioChannelPrototype>>) null;
    if (this.SupportedChannels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<RadioChannelPrototype>>>(this.SupportedChannels, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<ProtoId<RadioChannelPrototype>>>(this.SupportedChannels, hookCtx, context);
    target.SupportedChannels = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntercomComponent target,
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
    IntercomComponent target1 = (IntercomComponent) target;
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
    IntercomComponent target1 = (IntercomComponent) target;
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
    IntercomComponent target1 = (IntercomComponent) target;
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
  virtual IntercomComponent Component.Instantiate() => new IntercomComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IntercomComponent_AutoState : IComponentState
  {
    public bool SpeakerEnabled;
    public bool MicrophoneEnabled;
    public ProtoId<RadioChannelPrototype>? CurrentChannel;
    public List<ProtoId<RadioChannelPrototype>> SupportedChannels;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IntercomComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IntercomComponent, ComponentGetState>(new ComponentEventRefHandler<IntercomComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IntercomComponent, ComponentHandleState>(new ComponentEventRefHandler<IntercomComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, IntercomComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new IntercomComponent.IntercomComponent_AutoState()
      {
        SpeakerEnabled = component.SpeakerEnabled,
        MicrophoneEnabled = component.MicrophoneEnabled,
        CurrentChannel = component.CurrentChannel,
        SupportedChannels = component.SupportedChannels
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IntercomComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IntercomComponent.IntercomComponent_AutoState current))
        return;
      component.SpeakerEnabled = current.SpeakerEnabled;
      component.MicrophoneEnabled = current.MicrophoneEnabled;
      component.CurrentChannel = current.CurrentChannel;
      component.SupportedChannels = current.SupportedChannels == null ? (List<ProtoId<RadioChannelPrototype>>) null : new List<ProtoId<RadioChannelPrototype>>((IEnumerable<ProtoId<RadioChannelPrototype>>) current.SupportedChannels);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, IntercomComponent>(uid, component, ref args1);
    }
  }
}
