// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Cassette.CassetteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Cassette;
using Content.Shared._RMC14.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;

#nullable enable
namespace Content.Client._RMC14.Cassette;

public sealed class CassetteSystem : SharedCassetteSystem
{
  [Dependency]
  private AudioSystem _audio;
  [Dependency]
  private IAudioManager _audioManager;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IFileDialogManager _dialogs;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IGameTiming _timing;
  private float _gain;
  private readonly Dictionary<AudioStream, string> _names = new Dictionary<AudioStream, string>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, RMCCVars.VolumeGainCassettes, new Action<float>(this.SetGain), true);
    try
    {
      foreach (EntityPrototype enumeratePrototype in this._prototype.EnumeratePrototypes<EntityPrototype>())
      {
        CassetteTapeComponent cassetteTapeComponent;
        if (enumeratePrototype.TryGetComponent<CassetteTapeComponent>(ref cassetteTapeComponent, this._compFactory))
        {
          foreach (SoundSpecifier song in cassetteTapeComponent.Songs)
          {
            AudioResource audioResource;
            this._resourceCache.TryGetResource<AudioResource>(new ResPath(((SharedAudioSystem) this._audio).GetAudioPath(((SharedAudioSystem) this._audio).ResolveSound(song))), ref audioResource);
          }
        }
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error preloading cassette songs:\n{ex}");
    }
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev) => this._names.Clear();

  private void SetGain(float gain)
  {
    this._gain = gain;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(localEntity.GetValueOrDefault()));
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      CassettePlayerComponent cassettePlayerComponent;
      if (containedEntity.HasValue && this.TryComp<CassettePlayerComponent>(containedEntity.GetValueOrDefault(), ref cassettePlayerComponent))
      {
        this.SetAudioGain(cassettePlayerComponent.AudioStream);
        this.SetAudioGain(cassettePlayerComponent.CustomAudioStream);
      }
    }
  }

  protected override EntityUid? PlayCustomTrack(
    Entity<CassettePlayerComponent> player,
    Entity<CassetteTapeComponent> tape)
  {
    base.PlayCustomTrack(player, tape);
    if (!(tape.Comp.CustomTrack is AudioStream customTrack))
      return new EntityUid?();
    if (!this._timing.IsFirstTimePredicted)
      return new EntityUid?();
    string str;
    if (!this._names.TryGetValue(customTrack, out str))
      return new EntityUid?();
    AudioParams audioParams = ((AudioParams) ref player.Comp.AudioParams).WithVolume(SharedAudioSystem.GainToVolume(this._gain));
    (EntityUid, AudioComponent)? nullable = this._audio.PlayGlobal(customTrack, (ResolvedSoundSpecifier) new ResolvedPathSpecifier(str), new AudioParams?(audioParams));
    ref (EntityUid, AudioComponent)? local = ref nullable;
    return !local.HasValue ? new EntityUid?() : new EntityUid?(local.GetValueOrDefault().Item1);
  }

  protected override async void ChooseCustomTrack(Entity<CassetteTapeComponent> tape)
  {
    CassetteSystem cassetteSystem = this;
    try
    {
      if (!cassetteSystem._timing.IsFirstTimePredicted)
        return;
      FileDialogFilters fileDialogFilters = new FileDialogFilters(new FileDialogFilters.Group[1]
      {
        new FileDialogFilters.Group(new string[1]{ "ogg" })
      });
      Stream stream = await cassetteSystem._dialogs.OpenFile(fileDialogFilters, FileAccess.ReadWrite, new FileShare?());
      object obj = (object) null;
      int num = 0;
      try
      {
        if (stream != null)
        {
          AudioStream key = cassetteSystem._audioManager.LoadAudioOggVorbis(stream, (string) null);
          tape.Comp.CustomTrack = (object) key;
          string str = $"/Audio/_RMC14/_CustomCassetteUploads/upload_{cassetteSystem._names.Count}.ogg";
          cassetteSystem._resourceCache.CacheResource<AudioResource>(str, new AudioResource(key));
          cassetteSystem._names[key] = str;
        }
        else
          num = 1;
      }
      catch (object ex)
      {
        obj = ex;
      }
      if (stream != null)
        await stream.DisposeAsync();
      object obj1 = obj;
      if (obj1 != null)
      {
        if (!(obj1 is Exception source))
          throw obj1;
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num == 1)
        return;
      obj = (object) null;
    }
    catch (Exception ex)
    {
      cassetteSystem.Log.Error($"Error choosing custom cassette track:\n{ex}");
    }
  }

  private void SetAudioGain(EntityUid? audio)
  {
    AudioComponent audioComponent1;
    if (!this.TryComp<AudioComponent>(audio, ref audioComponent1))
      return;
    AudioComponent audioComponent2 = audioComponent1;
    AudioParams audioParams1 = audioComponent1.Params;
    ((AudioParams) ref audioParams1).Volume = SharedAudioSystem.GainToVolume(this._gain);
    AudioParams audioParams2 = audioParams1;
    audioComponent2.Params = audioParams2;
  }
}
