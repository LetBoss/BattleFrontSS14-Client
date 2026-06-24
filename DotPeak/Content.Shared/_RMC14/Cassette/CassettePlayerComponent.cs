// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Cassette.CassettePlayerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Cassette;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedCassetteSystem)})]
public sealed class CassettePlayerComponent : 
  Component,
  ISerializationGenerated<CassettePlayerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId PlayPauseActionId = (EntProtoId) "RMCActionCassettePlayPause";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? PlayPauseAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId NextActionId = (EntProtoId) "RMCActionCassetteNext";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? NextAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId RestartActionId = (EntProtoId) "RMCActionCassetteRestart";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? RestartAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slots = SlotFlags.EARS;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "rmc_cassette_player";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? AudioStream;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? CustomAudioStream;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AudioState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AudioParams AudioParams = AudioParams.Default.WithVolume(-6f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Tape;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier PlayPauseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier InsertEjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/handcuffs.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi WornSprite = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Objects/Devices/cassette_player.rsi"), "mob_overlay");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi MusicSprite = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Objects/Devices/cassette_player.rsi"), "music");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CassettePlayerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CassettePlayerComponent) target1;
    if (serialization.TryCustomCopy<CassettePlayerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.PlayPauseActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.PlayPauseActionId, hookCtx, context);
    target.PlayPauseActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.PlayPauseAction, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.PlayPauseAction, hookCtx, context);
    target.PlayPauseAction = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.NextActionId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.NextActionId, hookCtx, context);
    target.NextActionId = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.NextAction, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.NextAction, hookCtx, context);
    target.NextAction = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RestartActionId, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.RestartActionId, hookCtx, context);
    target.RestartActionId = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.RestartAction, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.RestartAction, hookCtx, context);
    target.RestartAction = target7;
    SlotFlags target8 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref target8, hookCtx, false, context))
      target8 = this.Slots;
    target.Slots = target8;
    string target9 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target9, hookCtx, false, context))
      target9 = this.ContainerId;
    target.ContainerId = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AudioStream, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.AudioStream, hookCtx, context);
    target.AudioStream = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CustomAudioStream, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.CustomAudioStream, hookCtx, context);
    target.CustomAudioStream = target11;
    AudioState target12 = AudioState.Stopped;
    if (!serialization.TryCustomCopy<AudioState>(this.State, ref target12, hookCtx, false, context))
      target12 = this.State;
    target.State = target12;
    AudioParams target13 = new AudioParams();
    if (!serialization.TryCustomCopy<AudioParams>(this.AudioParams, ref target13, hookCtx, false, context))
      serialization.CopyTo<AudioParams>(this.AudioParams, ref target13, hookCtx, context);
    target.AudioParams = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.Tape, ref target14, hookCtx, false, context))
      target14 = this.Tape;
    target.Tape = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (this.PlayPauseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PlayPauseSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.PlayPauseSound, hookCtx, context);
    target.PlayPauseSound = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (this.InsertEjectSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsertEjectSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.InsertEjectSound, hookCtx, context);
    target.InsertEjectSound = target16;
    SpriteSpecifier.Rsi target17 = (SpriteSpecifier.Rsi) null;
    if (this.WornSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.WornSprite, ref target17, hookCtx, false, context))
    {
      if (this.WornSprite == null)
        target17 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.WornSprite, ref target17, hookCtx, context, true);
    }
    target.WornSprite = target17;
    SpriteSpecifier.Rsi target18 = (SpriteSpecifier.Rsi) null;
    if (this.MusicSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.MusicSprite, ref target18, hookCtx, false, context))
    {
      if (this.MusicSprite == null)
        target18 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.MusicSprite, ref target18, hookCtx, context, true);
    }
    target.MusicSprite = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CassettePlayerComponent target,
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
    CassettePlayerComponent target1 = (CassettePlayerComponent) target;
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
    CassettePlayerComponent target1 = (CassettePlayerComponent) target;
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
    CassettePlayerComponent target1 = (CassettePlayerComponent) target;
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
  virtual CassettePlayerComponent Component.Instantiate() => new CassettePlayerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CassettePlayerComponent_AutoState : IComponentState
  {
    public EntProtoId PlayPauseActionId;
    public NetEntity? PlayPauseAction;
    public EntProtoId NextActionId;
    public NetEntity? NextAction;
    public EntProtoId RestartActionId;
    public NetEntity? RestartAction;
    public SlotFlags Slots;
    public string ContainerId;
    public NetEntity? AudioStream;
    public AudioState State;
    public AudioParams AudioParams;
    public int Tape;
    public SoundSpecifier PlayPauseSound;
    public SoundSpecifier InsertEjectSound;
    public SpriteSpecifier.Rsi WornSprite;
    public SpriteSpecifier.Rsi MusicSprite;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CassettePlayerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CassettePlayerComponent, ComponentGetState>(new ComponentEventRefHandler<CassettePlayerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CassettePlayerComponent, ComponentHandleState>(new ComponentEventRefHandler<CassettePlayerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CassettePlayerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CassettePlayerComponent.CassettePlayerComponent_AutoState()
      {
        PlayPauseActionId = component.PlayPauseActionId,
        PlayPauseAction = this.GetNetEntity(component.PlayPauseAction),
        NextActionId = component.NextActionId,
        NextAction = this.GetNetEntity(component.NextAction),
        RestartActionId = component.RestartActionId,
        RestartAction = this.GetNetEntity(component.RestartAction),
        Slots = component.Slots,
        ContainerId = component.ContainerId,
        AudioStream = this.GetNetEntity(component.AudioStream),
        State = component.State,
        AudioParams = component.AudioParams,
        Tape = component.Tape,
        PlayPauseSound = component.PlayPauseSound,
        InsertEjectSound = component.InsertEjectSound,
        WornSprite = component.WornSprite,
        MusicSprite = component.MusicSprite
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CassettePlayerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CassettePlayerComponent.CassettePlayerComponent_AutoState current))
        return;
      component.PlayPauseActionId = current.PlayPauseActionId;
      component.PlayPauseAction = this.EnsureEntity<CassettePlayerComponent>(current.PlayPauseAction, uid);
      component.NextActionId = current.NextActionId;
      component.NextAction = this.EnsureEntity<CassettePlayerComponent>(current.NextAction, uid);
      component.RestartActionId = current.RestartActionId;
      component.RestartAction = this.EnsureEntity<CassettePlayerComponent>(current.RestartAction, uid);
      component.Slots = current.Slots;
      component.ContainerId = current.ContainerId;
      component.AudioStream = this.EnsureEntity<CassettePlayerComponent>(current.AudioStream, uid);
      component.State = current.State;
      component.AudioParams = current.AudioParams;
      component.Tape = current.Tape;
      component.PlayPauseSound = current.PlayPauseSound;
      component.InsertEjectSound = current.InsertEjectSound;
      component.WornSprite = current.WornSprite;
      component.MusicSprite = current.MusicSprite;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CassettePlayerComponent>(uid, component, ref args1);
    }
  }
}
