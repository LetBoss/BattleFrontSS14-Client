// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.SpeechComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Speech;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SpeechComponent : 
  Component,
  ISerializationGenerated<SpeechComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SpeechSystem)}, Friend = AccessPermissions.ReadWrite, Other = AccessPermissions.Read)]
  public bool Enabled = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SpeechSoundsPrototype>? SpeechSounds;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SpeechVerbPrototype> SpeechVerb = (ProtoId<SpeechVerbPrototype>) "Default";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<EmotePrototype>> AllowedEmotes = new List<ProtoId<EmotePrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, ProtoId<SpeechVerbPrototype>> SuffixSpeechVerbs = new Dictionary<string, ProtoId<SpeechVerbPrototype>>()
  {
    {
      "chat-speech-verb-suffix-exclamation-strong",
      (ProtoId<SpeechVerbPrototype>) "DefaultExclamationStrong"
    },
    {
      "chat-speech-verb-suffix-exclamation",
      (ProtoId<SpeechVerbPrototype>) "DefaultExclamation"
    },
    {
      "chat-speech-verb-suffix-question",
      (ProtoId<SpeechVerbPrototype>) "DefaultQuestion"
    },
    {
      "chat-speech-verb-suffix-stutter",
      (ProtoId<SpeechVerbPrototype>) "DefaultStutter"
    },
    {
      "chat-speech-verb-suffix-mumble",
      (ProtoId<SpeechVerbPrototype>) "DefaultMumble"
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public AudioParams AudioParams = AudioParams.Default.WithVolume(-2f).WithRolloffFactor(4.5f);
  public TimeSpan LastTimeSoundPlayed = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public float SpeechBubbleOffset;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public float SoundCooldownTime { get; set; } = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpeechComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpeechComponent) target1;
    if (serialization.TryCustomCopy<SpeechComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    ProtoId<SpeechSoundsPrototype>? target3 = new ProtoId<SpeechSoundsPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<SpeechSoundsPrototype>?>(this.SpeechSounds, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<SpeechSoundsPrototype>?>(this.SpeechSounds, hookCtx, context);
    target.SpeechSounds = target3;
    ProtoId<SpeechVerbPrototype> target4 = new ProtoId<SpeechVerbPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SpeechVerbPrototype>>(this.SpeechVerb, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<SpeechVerbPrototype>>(this.SpeechVerb, hookCtx, context);
    target.SpeechVerb = target4;
    List<ProtoId<EmotePrototype>> target5 = (List<ProtoId<EmotePrototype>>) null;
    if (this.AllowedEmotes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<EmotePrototype>>>(this.AllowedEmotes, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<ProtoId<EmotePrototype>>>(this.AllowedEmotes, hookCtx, context);
    target.AllowedEmotes = target5;
    Dictionary<string, ProtoId<SpeechVerbPrototype>> target6 = (Dictionary<string, ProtoId<SpeechVerbPrototype>>) null;
    if (this.SuffixSpeechVerbs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, ProtoId<SpeechVerbPrototype>>>(this.SuffixSpeechVerbs, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<string, ProtoId<SpeechVerbPrototype>>>(this.SuffixSpeechVerbs, hookCtx, context);
    target.SuffixSpeechVerbs = target6;
    AudioParams target7 = new AudioParams();
    if (!serialization.TryCustomCopy<AudioParams>(this.AudioParams, ref target7, hookCtx, false, context))
      serialization.CopyTo<AudioParams>(this.AudioParams, ref target7, hookCtx, context);
    target.AudioParams = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SoundCooldownTime, ref target8, hookCtx, false, context))
      target8 = this.SoundCooldownTime;
    target.SoundCooldownTime = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeechBubbleOffset, ref target9, hookCtx, false, context))
      target9 = this.SpeechBubbleOffset;
    target.SpeechBubbleOffset = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpeechComponent target,
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
    SpeechComponent target1 = (SpeechComponent) target;
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
    SpeechComponent target1 = (SpeechComponent) target;
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
    SpeechComponent target1 = (SpeechComponent) target;
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
  virtual SpeechComponent Component.Instantiate() => new SpeechComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpeechComponent_AutoState : IComponentState
  {
    public bool Enabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpeechComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpeechComponent, ComponentGetState>(new ComponentEventRefHandler<SpeechComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpeechComponent, ComponentHandleState>(new ComponentEventRefHandler<SpeechComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, SpeechComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpeechComponent.SpeechComponent_AutoState()
      {
        Enabled = component.Enabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpeechComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpeechComponent.SpeechComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
    }
  }
}
