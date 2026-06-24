// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.Components.VocalComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.Humanoid;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Speech.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VocalComponent : 
  Component,
  ISerializationGenerated<VocalComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>? Sounds;
  [DataField("screamId", false, 1, false, false, typeof (PrototypeIdSerializer<EmotePrototype>))]
  [AutoNetworkedField]
  public string ScreamId = "Scream";
  [DataField("wilhelm", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Wilhelm = (SoundSpecifier) new SoundPathSpecifier("/Audio/Voice/Human/wilhelm_scream.ogg");
  [DataField("wilhelmProbability", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WilhelmProbability = 0.0002f;
  [DataField("screamAction", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  [AutoNetworkedField]
  public string? ScreamAction = "ActionScream";
  [DataField("screamActionEntity", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ScreamActionEntity;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public ProtoId<EmoteSoundsPrototype>? EmoteSounds;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VocalComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VocalComponent) target1;
    if (serialization.TryCustomCopy<VocalComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Sex, ProtoId<EmoteSoundsPrototype>> target2 = (Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>) null;
    if (!serialization.TryCustomCopy<Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>>(this.Sounds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>>(this.Sounds, hookCtx, context);
    target.Sounds = target2;
    string target3 = (string) null;
    if (this.ScreamId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ScreamId, ref target3, hookCtx, false, context))
      target3 = this.ScreamId;
    target.ScreamId = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.Wilhelm == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Wilhelm, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Wilhelm, hookCtx, context);
    target.Wilhelm = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WilhelmProbability, ref target5, hookCtx, false, context))
      target5 = this.WilhelmProbability;
    target.WilhelmProbability = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ScreamAction, ref target6, hookCtx, false, context))
      target6 = this.ScreamAction;
    target.ScreamAction = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ScreamActionEntity, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.ScreamActionEntity, hookCtx, context);
    target.ScreamActionEntity = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VocalComponent target,
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
    VocalComponent target1 = (VocalComponent) target;
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
    VocalComponent target1 = (VocalComponent) target;
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
    VocalComponent target1 = (VocalComponent) target;
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
  virtual VocalComponent Component.Instantiate() => new VocalComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VocalComponent_AutoState : IComponentState
  {
    public Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>? Sounds;
    public string ScreamId;
    public SoundSpecifier Wilhelm;
    public float WilhelmProbability;
    public string? ScreamAction;
    public NetEntity? ScreamActionEntity;
    public ProtoId<EmoteSoundsPrototype>? EmoteSounds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VocalComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VocalComponent, ComponentGetState>(new ComponentEventRefHandler<VocalComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VocalComponent, ComponentHandleState>(new ComponentEventRefHandler<VocalComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, VocalComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new VocalComponent.VocalComponent_AutoState()
      {
        Sounds = component.Sounds,
        ScreamId = component.ScreamId,
        Wilhelm = component.Wilhelm,
        WilhelmProbability = component.WilhelmProbability,
        ScreamAction = component.ScreamAction,
        ScreamActionEntity = this.GetNetEntity(component.ScreamActionEntity),
        EmoteSounds = component.EmoteSounds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VocalComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VocalComponent.VocalComponent_AutoState current))
        return;
      component.Sounds = current.Sounds == null ? (Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>) null : new Dictionary<Sex, ProtoId<EmoteSoundsPrototype>>((IDictionary<Sex, ProtoId<EmoteSoundsPrototype>>) current.Sounds);
      component.ScreamId = current.ScreamId;
      component.Wilhelm = current.Wilhelm;
      component.WilhelmProbability = current.WilhelmProbability;
      component.ScreamAction = current.ScreamAction;
      component.ScreamActionEntity = this.EnsureEntity<VocalComponent>(current.ScreamActionEntity, uid);
      component.EmoteSounds = current.EmoteSounds;
    }
  }
}
