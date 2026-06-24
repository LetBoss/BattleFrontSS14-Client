// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.ClientGlobalSoundSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Audio;

public sealed class ClientGlobalSoundSystem : SharedGlobalSoundSystem
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private SharedAudioSystem _audio;
  private bool _adminAudioEnabled = true;
  private List<EntityUid?> _adminAudio = new List<EntityUid?>(1);
  private bool _eventAudioEnabled = true;
  private Dictionary<StationEventMusicType, EntityUid?> _eventAudio = new Dictionary<StationEventMusicType, EntityUid?>(1);

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<AdminSoundEvent>(new EntityEventHandler<AdminSoundEvent>(this.PlayAdminSound), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._cfg, CCVars.AdminSoundsEnabled, new Action<bool>(this.ToggleAdminSound), true);
    this.SubscribeNetworkEvent<StationEventMusicEvent>(new EntityEventHandler<StationEventMusicEvent>(this.PlayStationEventMusic), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<Content.Shared.Audio.StopStationEventMusic>(new EntityEventHandler<Content.Shared.Audio.StopStationEventMusic>(this.StopStationEventMusic), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._cfg, CCVars.EventMusicEnabled, new Action<bool>(this.ToggleStationEventMusic), true);
    this.SubscribeNetworkEvent<GameGlobalSoundEvent>(new EntityEventHandler<GameGlobalSoundEvent>(this.PlayGameSound), (Type[]) null, (Type[]) null);
  }

  private void OnRoundRestart(RoundRestartCleanupEvent ev) => this.ClearAudio();

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.ClearAudio();
  }

  private void ClearAudio()
  {
    foreach (EntityUid? nullable in this._adminAudio)
      this._audio.Stop(nullable, (AudioComponent) null);
    this._adminAudio.Clear();
    foreach (EntityUid? nullable in this._eventAudio.Values)
      this._audio.Stop(nullable, (AudioComponent) null);
    this._eventAudio.Clear();
  }

  private void PlayAdminSound(AdminSoundEvent soundEvent)
  {
    if (!this._adminAudioEnabled)
      return;
    this._adminAudio.Add(this._audio.PlayGlobal(soundEvent.Specifier, Filter.Local(), false, soundEvent.AudioParams).Item1);
  }

  private void PlayStationEventMusic(StationEventMusicEvent soundEvent)
  {
    if (!this._eventAudioEnabled || this._eventAudio.ContainsKey(soundEvent.Type))
      return;
    (EntityUid, AudioComponent)? nullable = this._audio.PlayGlobal(soundEvent.Specifier, Filter.Local(), false, soundEvent.AudioParams);
    this._eventAudio.Add(soundEvent.Type, nullable.Item1);
  }

  private void PlayGameSound(GameGlobalSoundEvent soundEvent)
  {
    this._audio.PlayGlobal(soundEvent.Specifier, Filter.Local(), false, soundEvent.AudioParams);
  }

  private void StopStationEventMusic(Content.Shared.Audio.StopStationEventMusic soundEvent)
  {
    EntityUid? nullable;
    if (!this._eventAudio.TryGetValue(soundEvent.Type, out nullable))
      return;
    this._audio.Stop(nullable, (AudioComponent) null);
    this._eventAudio.Remove(soundEvent.Type);
  }

  private void ToggleAdminSound(bool enabled)
  {
    this._adminAudioEnabled = enabled;
    if (this._adminAudioEnabled)
      return;
    foreach (EntityUid? nullable in this._adminAudio)
      this._audio.Stop(nullable, (AudioComponent) null);
    this._adminAudio.Clear();
  }

  private void ToggleStationEventMusic(bool enabled)
  {
    this._eventAudioEnabled = enabled;
    if (this._eventAudioEnabled)
      return;
    foreach (KeyValuePair<StationEventMusicType, EntityUid?> keyValuePair in this._eventAudio)
      this._audio.Stop(keyValuePair.Value, (AudioComponent) null);
    this._eventAudio.Clear();
  }
}
