// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Cassette.SharedCassetteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Cassette;

public abstract class SharedCassetteSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private INetConfigurationManager _netConfig;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PlayerDetachedEvent>(new EntityEventHandler<PlayerDetachedEvent>(this.OnPlayerDetached));
    this.SubscribeLocalEvent<CassettePlayerComponent, GetItemActionsEvent>(new EntityEventRefHandler<CassettePlayerComponent, GetItemActionsEvent>(this.OnPlayerGetItemActions));
    this.SubscribeLocalEvent<CassettePlayerComponent, CassettePlayPauseActionEvent>(new EntityEventRefHandler<CassettePlayerComponent, CassettePlayPauseActionEvent>(this.OnPlayerPlayPause));
    this.SubscribeLocalEvent<CassettePlayerComponent, CassetteNextActionEvent>(new EntityEventRefHandler<CassettePlayerComponent, CassetteNextActionEvent>(this.OnPlayerNext));
    this.SubscribeLocalEvent<CassettePlayerComponent, CassetteRestartActionEvent>(new EntityEventRefHandler<CassettePlayerComponent, CassetteRestartActionEvent>(this.OnPlayerRestart));
    this.SubscribeLocalEvent<CassettePlayerComponent, InteractUsingEvent>(new EntityEventRefHandler<CassettePlayerComponent, InteractUsingEvent>(this.OnPlayerInteractUsing));
    this.SubscribeLocalEvent<CassettePlayerComponent, RMCStorageEjectHandItemEvent>(new EntityEventRefHandler<CassettePlayerComponent, RMCStorageEjectHandItemEvent>(this.OnPlayerEjectHand));
    this.SubscribeLocalEvent<CassettePlayerComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<CassettePlayerComponent, GetEquipmentVisualsEvent>(this.OnPlayerGetEquipmentVisuals), after: new Type[1]
    {
      typeof (ClothingSystem)
    });
    this.SubscribeLocalEvent<CassettePlayerComponent, GotUnequippedEvent>(new EntityEventRefHandler<CassettePlayerComponent, GotUnequippedEvent>(this.OnPlayerUnequipped));
    this.SubscribeLocalEvent<CassettePlayerComponent, ExaminedEvent>(new EntityEventRefHandler<CassettePlayerComponent, ExaminedEvent>(this.OnPlayerExamined));
    this.SubscribeLocalEvent<CassettePlayerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<CassettePlayerComponent, AfterAutoHandleStateEvent>(this.OnPlayerState));
    this.SubscribeLocalEvent<CassettePlayerComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<CassettePlayerComponent, EntRemovedFromContainerMessage>(this.OnPlayerRemovedFromContainer));
    this.SubscribeLocalEvent<CassettePlayerComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<CassettePlayerComponent, GetVerbsEvent<AlternativeVerb>>(this.OnPlayerGetVerbsAlternative));
    this.SubscribeLocalEvent<CassetteTapeComponent, ExaminedEvent>(new EntityEventRefHandler<CassetteTapeComponent, ExaminedEvent>(this.OnTapeExamined));
    this.SubscribeLocalEvent<CassetteTapeComponent, UseInHandEvent>(new EntityEventRefHandler<CassetteTapeComponent, UseInHandEvent>(this.OnPlayerUseInHand));
    this.SubscribeLocalEvent<CassetteTapeComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<CassetteTapeComponent, GetVerbsEvent<AlternativeVerb>>(this.OnTapeGetVerbsAlternative));
  }

  private void OnPlayerDetached(PlayerDetachedEvent ev)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) ev.Entity);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      CassettePlayerComponent comp;
      if (this.TryComp<CassettePlayerComponent>(container.ContainedEntity, out comp))
        this.StopAllAudio((Entity<CassettePlayerComponent>) (container.ContainedEntity.Value, comp));
    }
  }

  private void OnPlayerGetItemActions(
    Entity<CassettePlayerComponent> ent,
    ref GetItemActionsEvent args)
  {
    if (this._net.IsClient || args.InHands)
      return;
    SlotFlags? slotFlags = args.SlotFlags;
    if (!slotFlags.HasValue)
      return;
    SlotFlags valueOrDefault = slotFlags.GetValueOrDefault();
    if (!ent.Comp.Slots.HasFlag((Enum) valueOrDefault))
      return;
    args.AddAction(ref ent.Comp.PlayPauseAction, (string) ent.Comp.PlayPauseActionId);
    args.AddAction(ref ent.Comp.NextAction, (string) ent.Comp.NextActionId);
    args.AddAction(ref ent.Comp.RestartAction, (string) ent.Comp.RestartActionId);
    this.Dirty<CassettePlayerComponent>(ent);
  }

  private void OnPlayerPlayPause(
    Entity<CassettePlayerComponent> ent,
    ref CassettePlayPauseActionEvent args)
  {
    int totalSongs = this.GetTotalSongs(ent);
    Entity<CassetteTapeComponent>? tape = this.GetTape(ent);
    switch (ent.Comp.State)
    {
      case AudioState.Stopped:
        this.PlaySong(ent, args.Performer);
        this._popup.PopupClient(this.Loc.GetString("rmc-cassette-playing", ("player", (object) ent), ("current", (object) this.GetCurrentSongCount(ent)), ("total", (object) totalSongs)), (EntityUid) ent, new EntityUid?(args.Performer));
        break;
      case AudioState.Playing:
        if (this._net.IsServer)
          this._audio.SetState(ent.Comp.AudioStream, AudioState.Paused);
        else if (tape.HasValue)
        {
          CassetteTapeComponent comp = tape.GetValueOrDefault().Comp;
          if (comp != null && comp.Custom)
            this._audio.SetState(ent.Comp.CustomAudioStream, AudioState.Paused);
        }
        this._popup.PopupClient(this.Loc.GetString("rmc-cassette-pause", ("player", (object) ent)), (EntityUid) ent, new EntityUid?(args.Performer));
        ent.Comp.State = AudioState.Paused;
        break;
      case AudioState.Paused:
        if (this._net.IsServer)
          this._audio.SetState(ent.Comp.AudioStream, AudioState.Playing);
        else if (tape.HasValue)
        {
          CassetteTapeComponent comp = tape.GetValueOrDefault().Comp;
          if (comp != null && comp.Custom)
            this._audio.SetState(ent.Comp.CustomAudioStream, AudioState.Playing);
        }
        this._popup.PopupClient(this.Loc.GetString("rmc-cassette-resume", ("player", (object) ent), ("current", (object) this.GetCurrentSongCount(ent)), ("total", (object) totalSongs)), (EntityUid) ent, new EntityUid?(args.Performer));
        ent.Comp.State = AudioState.Playing;
        break;
    }
    this._audio.PlayLocal(ent.Comp.PlayPauseSound, (EntityUid) ent, new EntityUid?(args.Performer));
    this.Dirty<CassettePlayerComponent>(ent);
  }

  private void OnPlayerNext(Entity<CassettePlayerComponent> ent, ref CassetteNextActionEvent args)
  {
    this.PlaySong(ent, args.Performer, new int?(ent.Comp.Tape + 1));
    this._popup.PopupClient(this.Loc.GetString("rmc-cassette-change", ("current", (object) this.GetCurrentSongCount(ent)), ("total", (object) this.GetTotalSongs(ent))), (EntityUid) ent, new EntityUid?(args.Performer));
  }

  private void OnPlayerRestart(
    Entity<CassettePlayerComponent> ent,
    ref CassetteRestartActionEvent args)
  {
    this.PlaySong(ent, args.Performer);
    this._popup.PopupClient(this.Loc.GetString("rmc-cassette-restart", ("current", (object) this.GetCurrentSongCount(ent)), ("total", (object) this.GetTotalSongs(ent))), (EntityUid) ent, new EntityUid?(args.Performer));
  }

  private void PlaySong(Entity<CassettePlayerComponent> player, EntityUid actor, int? tape = null)
  {
    BaseContainer container;
    EntityUid? element;
    CassetteTapeComponent comp1;
    if (!this._container.TryGetContainer((EntityUid) player, player.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) || !this.TryComp<CassetteTapeComponent>(element, out comp1))
      return;
    this.StopAllAudio(player);
    tape.GetValueOrDefault();
    if (!tape.HasValue)
      tape = new int?(player.Comp.Tape);
    int? nullable1 = tape;
    int num = 0;
    if (!(nullable1.GetValueOrDefault() < num & nullable1.HasValue))
    {
      int? nullable2 = tape;
      int count = comp1.Songs.Count;
      if (!(nullable2.GetValueOrDefault() >= count & nullable2.HasValue))
        goto label_6;
    }
    tape = new int?(0);
label_6:
    if (comp1.Custom)
    {
      EntityUid? nullable3 = this.PlayCustomTrack(player, (Entity<CassetteTapeComponent>) (element.Value, comp1));
      if (nullable3.HasValue)
      {
        EntityUid valueOrDefault = nullable3.GetValueOrDefault();
        player.Comp.CustomAudioStream = new EntityUid?(valueOrDefault);
      }
      player.Comp.Tape = 0;
    }
    else if (this._net.IsServer)
    {
      SoundSpecifier song = comp1.Songs[tape.Value];
      AudioParams audioParams = player.Comp.AudioParams;
      ActorComponent comp2;
      if (this.TryComp<ActorComponent>(actor, out comp2))
      {
        float clientCvar = this._netConfig.GetClientCVar<float>(comp2.PlayerSession.Channel, RMCCVars.VolumeGainCassettes);
        audioParams = audioParams.WithVolume(SharedAudioSystem.GainToVolume(clientCvar));
      }
      CassettePlayerComponent comp3 = player.Comp;
      (EntityUid, AudioComponent)? nullable4 = this._audio.PlayGlobal(song, actor, new AudioParams?(audioParams));
      ref (EntityUid, AudioComponent)? local = ref nullable4;
      EntityUid? nullable5 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      comp3.AudioStream = nullable5;
    }
    player.Comp.Tape = tape.Value;
    player.Comp.State = AudioState.Playing;
    this.Dirty<CassettePlayerComponent>(player);
    this._item.VisualsChanged((EntityUid) player);
  }

  private void OnPlayerInteractUsing(
    Entity<CassettePlayerComponent> ent,
    ref InteractUsingEvent args)
  {
    if (!this.HasComp<CassetteTapeComponent>(args.Used))
      return;
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.ContainerId);
    EntityUid? containedEntity = container.ContainedEntity;
    if (containedEntity.HasValue)
      this._container.Remove((Entity<TransformComponent, MetaDataComponent>) containedEntity.Value, (BaseContainer) container);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) container);
    if (containedEntity.HasValue)
      this._hands.TryPickupAnyHand(args.User, containedEntity.Value);
    ent.Comp.Tape = 0;
    this.Dirty<CassettePlayerComponent>(ent);
    this._audio.PlayLocal(ent.Comp.InsertEjectSound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnPlayerEjectHand(
    Entity<CassettePlayerComponent> ent,
    ref RMCStorageEjectHandItemEvent args)
  {
    if (args.Handled || !this._hands.IsHolding((Entity<HandsComponent>) args.User, new EntityUid?((EntityUid) ent)) || !this.EjectTape(ent, args.User))
      return;
    args.Handled = true;
  }

  private void OnPlayerGetEquipmentVisuals(
    Entity<CassettePlayerComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    args.Layers.Add(("cassette", new PrototypeLayerData()
    {
      RsiPath = ent.Comp.WornSprite.RsiPath.ToString(),
      State = ent.Comp.WornSprite.RsiState
    }));
    if (ent.Comp.State != AudioState.Playing)
      return;
    args.Layers.Add(("cassette_music", new PrototypeLayerData()
    {
      RsiPath = ent.Comp.MusicSprite.RsiPath.ToString(),
      State = ent.Comp.MusicSprite.RsiState
    }));
  }

  private void OnPlayerUnequipped(Entity<CassettePlayerComponent> ent, ref GotUnequippedEvent args)
  {
    this.StopAllAudio(ent);
  }

  private void OnPlayerExamined(Entity<CassettePlayerComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("CassettePlayerComponent"))
    {
      Entity<CassetteTapeComponent> tape;
      if (this.TryGetTape(ent, out tape))
        args.PushMarkup(this.Loc.GetString("rmc-cassette-player-examine-tape", ("tape", (object) tape)));
      else
        args.PushMarkup(this.Loc.GetString("rmc-cassette-player-examine-none"));
    }
  }

  private void OnPlayerState(
    Entity<CassettePlayerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._item.VisualsChanged((EntityUid) ent);
  }

  private void OnPlayerRemovedFromContainer(
    Entity<CassettePlayerComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this._audio.Stop(this._net.IsServer ? ent.Comp.AudioStream : ent.Comp.CustomAudioStream);
    ent.Comp.State = AudioState.Stopped;
    ent.Comp.Tape = 0;
    this.Dirty<CassettePlayerComponent>(ent);
  }

  private void OnPlayerGetVerbsAlternative(
    Entity<CassettePlayerComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    if (this.HasComp<XenoComponent>(user))
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rmc-cassette-player-eject");
    alternativeVerb.Act = (Action) (() => this.EjectTape(ent, user));
    verbs.Add(alternativeVerb);
  }

  private void OnTapeExamined(Entity<CassetteTapeComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("CassetteTapeComponent"))
    {
      if (ent.Comp.Custom)
        args.PushMarkup(this.Loc.GetString("rmc-cassette-tape-custom"));
      else
        args.PushMarkup(this.Loc.GetString("rmc-cassette-tape-examine", ("total", (object) ent.Comp.Songs.Count)));
    }
  }

  protected virtual void OnPlayerUseInHand(
    Entity<CassetteTapeComponent> tape,
    ref UseInHandEvent args)
  {
    if (!tape.Comp.Custom)
      return;
    args.Handled = true;
    this.ChooseCustomTrack(tape);
  }

  private void OnTapeGetVerbsAlternative(
    Entity<CassetteTapeComponent> tape,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!tape.Comp.Custom)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rmc-cassette-tape-custom-choose");
    alternativeVerb.Act = (Action) (() => this.ChooseCustomTrack(tape));
    verbs.Add(alternativeVerb);
  }

  private bool TryGetTape(
    Entity<CassettePlayerComponent> player,
    out Entity<CassetteTapeComponent> tape)
  {
    tape = new Entity<CassetteTapeComponent>();
    BaseContainer container;
    EntityUid? element;
    CassetteTapeComponent comp;
    if (!this._container.TryGetContainer((EntityUid) player, player.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) || !this.TryComp<CassetteTapeComponent>(element, out comp))
      return false;
    tape = (Entity<CassetteTapeComponent>) (element.Value, comp);
    return true;
  }

  private Entity<CassetteTapeComponent>? GetTape(Entity<CassettePlayerComponent> player)
  {
    Entity<CassetteTapeComponent> tape;
    return !this.TryGetTape(player, out tape) ? new Entity<CassetteTapeComponent>?() : new Entity<CassetteTapeComponent>?(tape);
  }

  private int GetCurrentSongCount(Entity<CassettePlayerComponent> player) => player.Comp.Tape + 1;

  private int GetTotalSongs(Entity<CassettePlayerComponent> player)
  {
    Entity<CassetteTapeComponent> tape;
    if (!this.TryGetTape(player, out tape))
      return 0;
    int count = tape.Comp.Songs.Count;
    CassetteTapeComponent comp = tape.Comp;
    if (comp != null && comp.CustomTrack != null)
      ++count;
    return count;
  }

  protected virtual EntityUid? PlayCustomTrack(
    Entity<CassettePlayerComponent> player,
    Entity<CassetteTapeComponent> tape)
  {
    return new EntityUid?();
  }

  protected virtual void ChooseCustomTrack(Entity<CassetteTapeComponent> tape)
  {
  }

  private void StopAllAudio(Entity<CassettePlayerComponent> ent)
  {
    this._audio.Stop(ent.Comp.AudioStream);
    this._audio.Stop(ent.Comp.CustomAudioStream);
    ent.Comp.State = AudioState.Stopped;
    this.Dirty<CassettePlayerComponent>(ent);
    this._item.VisualsChanged((EntityUid) ent);
  }

  private bool EjectTape(Entity<CassettePlayerComponent> ent, EntityUid user)
  {
    BaseContainer container;
    EntityUid? element;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) || !this._container.Remove((Entity<TransformComponent, MetaDataComponent>) element.Value, container))
      return false;
    this._hands.TryPickupAnyHand(user, element.Value);
    this._audio.PlayLocal(ent.Comp.InsertEjectSound, (EntityUid) ent, new EntityUid?(user));
    return true;
  }
}
