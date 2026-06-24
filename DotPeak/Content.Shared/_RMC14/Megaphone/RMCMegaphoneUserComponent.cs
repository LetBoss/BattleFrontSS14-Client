// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Megaphone.RMCMegaphoneUserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Speech;
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
namespace Content.Shared._RMC14.Megaphone;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCMegaphoneUserComponent : 
  Component,
  ISerializationGenerated<RMCMegaphoneUserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<SpeechSoundsPrototype> MegaphoneSpeechSound = (ProtoId<SpeechSoundsPrototype>) "RMCMegaphone";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<SpeechVerbPrototype> SpeechVerb = (ProtoId<SpeechVerbPrototype>) "Megaphone";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<SpeechVerbPrototype>? OriginalSpeechVerb;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<SpeechSoundsPrototype>? OriginalSpeechSounds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, ProtoId<SpeechVerbPrototype>>? OriginalSuffixSpeechVerbs;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, ProtoId<SpeechVerbPrototype>> SuffixSpeechVerbs = new Dictionary<string, ProtoId<SpeechVerbPrototype>>()
  {
    {
      "chat-speech-verb-suffix-exclamation-strong",
      (ProtoId<SpeechVerbPrototype>) "Megaphone"
    },
    {
      "chat-speech-verb-suffix-exclamation",
      (ProtoId<SpeechVerbPrototype>) "Megaphone"
    },
    {
      "chat-speech-verb-suffix-question",
      (ProtoId<SpeechVerbPrototype>) "Megaphone"
    },
    {
      "chat-speech-verb-suffix-stutter",
      (ProtoId<SpeechVerbPrototype>) "Megaphone"
    },
    {
      "chat-speech-verb-suffix-mumble",
      (ProtoId<SpeechVerbPrototype>) "Megaphone"
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCMegaphoneUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCMegaphoneUserComponent) target1;
    if (serialization.TryCustomCopy<RMCMegaphoneUserComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<SpeechSoundsPrototype> target2 = new ProtoId<SpeechSoundsPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SpeechSoundsPrototype>>(this.MegaphoneSpeechSound, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<SpeechSoundsPrototype>>(this.MegaphoneSpeechSound, hookCtx, context);
    target.MegaphoneSpeechSound = target2;
    ProtoId<SpeechVerbPrototype> target3 = new ProtoId<SpeechVerbPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SpeechVerbPrototype>>(this.SpeechVerb, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<SpeechVerbPrototype>>(this.SpeechVerb, hookCtx, context);
    target.SpeechVerb = target3;
    ProtoId<SpeechVerbPrototype>? target4 = new ProtoId<SpeechVerbPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<SpeechVerbPrototype>?>(this.OriginalSpeechVerb, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<SpeechVerbPrototype>?>(this.OriginalSpeechVerb, hookCtx, context);
    target.OriginalSpeechVerb = target4;
    ProtoId<SpeechSoundsPrototype>? target5 = new ProtoId<SpeechSoundsPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<SpeechSoundsPrototype>?>(this.OriginalSpeechSounds, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<SpeechSoundsPrototype>?>(this.OriginalSpeechSounds, hookCtx, context);
    target.OriginalSpeechSounds = target5;
    Dictionary<string, ProtoId<SpeechVerbPrototype>> target6 = (Dictionary<string, ProtoId<SpeechVerbPrototype>>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, ProtoId<SpeechVerbPrototype>>>(this.OriginalSuffixSpeechVerbs, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<string, ProtoId<SpeechVerbPrototype>>>(this.OriginalSuffixSpeechVerbs, hookCtx, context);
    target.OriginalSuffixSpeechVerbs = target6;
    Dictionary<string, ProtoId<SpeechVerbPrototype>> target7 = (Dictionary<string, ProtoId<SpeechVerbPrototype>>) null;
    if (this.SuffixSpeechVerbs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, ProtoId<SpeechVerbPrototype>>>(this.SuffixSpeechVerbs, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<string, ProtoId<SpeechVerbPrototype>>>(this.SuffixSpeechVerbs, hookCtx, context);
    target.SuffixSpeechVerbs = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCMegaphoneUserComponent target,
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
    RMCMegaphoneUserComponent target1 = (RMCMegaphoneUserComponent) target;
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
    RMCMegaphoneUserComponent target1 = (RMCMegaphoneUserComponent) target;
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
    RMCMegaphoneUserComponent target1 = (RMCMegaphoneUserComponent) target;
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
  virtual RMCMegaphoneUserComponent Component.Instantiate() => new RMCMegaphoneUserComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCMegaphoneUserComponent_AutoState : IComponentState
  {
    public ProtoId<SpeechSoundsPrototype> MegaphoneSpeechSound;
    public ProtoId<SpeechVerbPrototype> SpeechVerb;
    public ProtoId<SpeechVerbPrototype>? OriginalSpeechVerb;
    public ProtoId<SpeechSoundsPrototype>? OriginalSpeechSounds;
    public Dictionary<string, ProtoId<SpeechVerbPrototype>>? OriginalSuffixSpeechVerbs;
    public Dictionary<string, ProtoId<SpeechVerbPrototype>> SuffixSpeechVerbs;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCMegaphoneUserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCMegaphoneUserComponent, ComponentGetState>(new ComponentEventRefHandler<RMCMegaphoneUserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCMegaphoneUserComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCMegaphoneUserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCMegaphoneUserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCMegaphoneUserComponent.RMCMegaphoneUserComponent_AutoState()
      {
        MegaphoneSpeechSound = component.MegaphoneSpeechSound,
        SpeechVerb = component.SpeechVerb,
        OriginalSpeechVerb = component.OriginalSpeechVerb,
        OriginalSpeechSounds = component.OriginalSpeechSounds,
        OriginalSuffixSpeechVerbs = component.OriginalSuffixSpeechVerbs,
        SuffixSpeechVerbs = component.SuffixSpeechVerbs
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCMegaphoneUserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCMegaphoneUserComponent.RMCMegaphoneUserComponent_AutoState current))
        return;
      component.MegaphoneSpeechSound = current.MegaphoneSpeechSound;
      component.SpeechVerb = current.SpeechVerb;
      component.OriginalSpeechVerb = current.OriginalSpeechVerb;
      component.OriginalSpeechSounds = current.OriginalSpeechSounds;
      component.OriginalSuffixSpeechVerbs = current.OriginalSuffixSpeechVerbs == null ? (Dictionary<string, ProtoId<SpeechVerbPrototype>>) null : new Dictionary<string, ProtoId<SpeechVerbPrototype>>((IDictionary<string, ProtoId<SpeechVerbPrototype>>) current.OriginalSuffixSpeechVerbs);
      component.SuffixSpeechVerbs = current.SuffixSpeechVerbs == null ? (Dictionary<string, ProtoId<SpeechVerbPrototype>>) null : new Dictionary<string, ProtoId<SpeechVerbPrototype>>((IDictionary<string, ProtoId<SpeechVerbPrototype>>) current.SuffixSpeechVerbs);
    }
  }
}
