// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Boombox.PubgBoomboxAudioSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Boombox;
using Content.Shared.CCVar;
using Robust.Client.Audio;
using Robust.Client.ResourceManagement;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Boombox;

public sealed class PubgBoomboxAudioSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IResourceManager _res;
  [Dependency]
  private AudioSystem _audio;
  [Dependency]
  private PubgBoomboxCacheSystem _cache;
  private static readonly ResPath Prefix = ResPath.op_Division(ResPath.Root, "PubgBoombox");
  private const int MaxLoadedResources = 3;
  private const float ResyncToleranceSeconds = 0.5f;
  private static readonly MemoryContentRoot ContentRoot = new MemoryContentRoot();
  private static bool _rootRegistered;
  private readonly Dictionary<string, AudioResource> _resources = new Dictionary<string, AudioResource>();
  private readonly List<string> _resourceOrder = new List<string>();
  private readonly Dictionary<EntityUid, PubgBoomboxAudioSystem.ActiveAudio> _active = new Dictionary<EntityUid, PubgBoomboxAudioSystem.ActiveAudio>();
  private bool _enabled = true;
  private float _gain = 1f;
  private float _pollTimer;

  public virtual void Initialize()
  {
    base.Initialize();
    if (!PubgBoomboxAudioSystem._rootRegistered)
    {
      this._res.AddRoot(PubgBoomboxAudioSystem.Prefix, (IContentRoot) PubgBoomboxAudioSystem.ContentRoot);
      PubgBoomboxAudioSystem._rootRegistered = true;
    }
    this._cfg.OnValueChanged<bool>(CCVars.PubgBoomboxSoundEnabled, new Action<bool>(this.OnEnabledChanged), true);
    this._cfg.OnValueChanged<float>(CCVars.PubgBoomboxVolume, new Action<float>(this.OnGainChanged), true);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PubgBoomboxComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<PubgBoomboxComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PubgBoomboxComponent, ComponentShutdown>(new EntityEventRefHandler<PubgBoomboxComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    this._cache.TrackReady += new Action<string>(this.OnTrackReady);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._cfg.UnsubValueChanged<bool>(CCVars.PubgBoomboxSoundEnabled, new Action<bool>(this.OnEnabledChanged));
    this._cfg.UnsubValueChanged<float>(CCVars.PubgBoomboxVolume, new Action<float>(this.OnGainChanged));
    this._cache.TrackReady -= new Action<string>(this.OnTrackReady);
  }

  private void OnEnabledChanged(bool enabled)
  {
    this._enabled = enabled;
    if (enabled)
      return;
    foreach (EntityUid uid in new List<EntityUid>((IEnumerable<EntityUid>) this._active.Keys))
      this.StopActive(uid);
  }

  private void OnGainChanged(float gain)
  {
    this._gain = gain;
    foreach (PubgBoomboxAudioSystem.ActiveAudio active in this._active.Values)
      this.ApplyLiveVolume(active);
  }

  private void ApplyLiveVolume(PubgBoomboxAudioSystem.ActiveAudio active)
  {
    AudioComponent audioComp = active.AudioComp;
    if (audioComp == null || !this.Exists(active.AudioEntity) || this.TerminatingOrDeleted(active.AudioEntity, (MetaDataComponent) null))
      return;
    ((SharedAudioSystem) this._audio).SetVolume(new EntityUid?(active.AudioEntity), SharedAudioSystem.GainToVolume(active.Volume * this._gain), audioComp);
  }

  private void OnHandleState(Entity<PubgBoomboxComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    this.Reconcile(ent);
  }

  private void OnShutdown(Entity<PubgBoomboxComponent> ent, ref ComponentShutdown args)
  {
    this.StopActive(Entity<PubgBoomboxComponent>.op_Implicit(ent));
  }

  private void OnTrackReady(string trackId)
  {
    EntityQueryEnumerator<PubgBoomboxComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PubgBoomboxComponent>();
    EntityUid entityUid;
    PubgBoomboxComponent boomboxComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref boomboxComponent))
    {
      if (boomboxComponent.Playing && boomboxComponent.TrackId == trackId)
        this.Reconcile(Entity<PubgBoomboxComponent>.op_Implicit((entityUid, boomboxComponent)));
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    this._pollTimer += frameTime;
    if ((double) this._pollTimer < 1.0)
      return;
    this._pollTimer = 0.0f;
    EntityQueryEnumerator<PubgBoomboxComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PubgBoomboxComponent>();
    EntityUid key;
    PubgBoomboxComponent boomboxComponent;
    while (entityQueryEnumerator.MoveNext(ref key, ref boomboxComponent))
    {
      if (boomboxComponent.Playing || this._active.ContainsKey(key))
        this.Reconcile(Entity<PubgBoomboxComponent>.op_Implicit((key, boomboxComponent)));
    }
  }

  private void Reconcile(Entity<PubgBoomboxComponent> ent)
  {
    EntityUid entityUid1;
    PubgBoomboxComponent boomboxComponent1;
    ent.Deconstruct(ref entityUid1, ref boomboxComponent1);
    EntityUid entityUid2 = entityUid1;
    PubgBoomboxComponent boomboxComponent2 = boomboxComponent1;
    float totalSeconds = (float) (this._timing.CurTime - boomboxComponent2.PlaybackStart).TotalSeconds;
    if ((!this._enabled || !boomboxComponent2.Playing || boomboxComponent2.TrackId == null || (double) totalSeconds < -1.0 ? 0 : ((double) totalSeconds < (double) boomboxComponent2.TrackDuration ? 1 : 0)) == 0)
    {
      this.StopActive(entityUid2);
    }
    else
    {
      string trackId = boomboxComponent2.TrackId;
      if (!this._cache.HasTrack(trackId))
      {
        this.StopActive(entityUid2);
        this._cache.EnsureTrack(trackId);
      }
      else
      {
        PubgBoomboxAudioSystem.ActiveAudio active;
        if (this._active.TryGetValue(entityUid2, out active) && active.TrackId == trackId && Math.Abs((active.PlaybackStart - boomboxComponent2.PlaybackStart).TotalSeconds) < 0.5 && !this.TerminatingOrDeleted(active.AudioEntity, (MetaDataComponent) null) && this.Exists(active.AudioEntity) && MathHelper.CloseTo(active.MaxDistance, boomboxComponent2.MaxDistance, 1E-07f))
        {
          if (MathHelper.CloseTo(active.Volume, boomboxComponent2.Volume, 1E-07f))
            return;
          active.Volume = boomboxComponent2.Volume;
          this.ApplyLiveVolume(active);
        }
        else
        {
          this.StopActive(entityUid2);
          AudioResource audioResource = this.LoadResource(trackId);
          if (audioResource == null)
            return;
          AudioParams audioParams1 = ((AudioParams) ref AudioParams.Default).WithVolume(SharedAudioSystem.GainToVolume(boomboxComponent2.Volume * this._gain));
          AudioParams audioParams2 = ((AudioParams) ref audioParams1).WithMaxDistance(boomboxComponent2.MaxDistance);
          ResolvedPathSpecifier resolvedPathSpecifier = new ResolvedPathSpecifier(ResPath.op_Division(PubgBoomboxAudioSystem.Prefix, trackId + ".ogg"));
          (EntityUid, AudioComponent)? nullable = this._audio.PlayEntity(audioResource.AudioStream, entityUid2, (ResolvedSoundSpecifier) resolvedPathSpecifier, new AudioParams?(audioParams2));
          if (!nullable.HasValue)
            return;
          EntityUid entityUid3 = nullable.Value.Item1;
          if ((double) totalSeconds > 0.25)
            ((SharedAudioSystem) this._audio).SetPlaybackPosition(new Entity<AudioComponent>?(Entity<AudioComponent>.op_Implicit(entityUid3)), totalSeconds);
          this._active[entityUid2] = new PubgBoomboxAudioSystem.ActiveAudio()
          {
            TrackId = trackId,
            AudioEntity = entityUid3,
            AudioComp = nullable.Value.Item2,
            PlaybackStart = boomboxComponent2.PlaybackStart,
            Volume = boomboxComponent2.Volume,
            MaxDistance = boomboxComponent2.MaxDistance
          };
        }
      }
    }
  }

  private void StopActive(EntityUid uid)
  {
    PubgBoomboxAudioSystem.ActiveAudio activeAudio;
    if (!this._active.Remove(uid, out activeAudio) || !this.Exists(activeAudio.AudioEntity) || this.TerminatingOrDeleted(activeAudio.AudioEntity, (MetaDataComponent) null))
      return;
    this.QueueDel(new EntityUid?(activeAudio.AudioEntity));
  }

  private AudioResource? LoadResource(string trackId)
  {
    AudioResource audioResource1;
    if (this._resources.TryGetValue(trackId, out audioResource1))
      return audioResource1;
    byte[] data;
    if (!this._cache.TryGetTrack(trackId, out data))
      return (AudioResource) null;
    ResPath resPath;
    // ISSUE: explicit constructor call
    ((ResPath) ref resPath).\u002Ector(trackId + ".ogg");
    try
    {
      PubgBoomboxAudioSystem.ContentRoot.AddOrUpdateFile(resPath, data);
      AudioResource audioResource2 = new AudioResource();
      ((BaseResource) audioResource2).Load(IoCManager.Instance, ResPath.op_Division(PubgBoomboxAudioSystem.Prefix, resPath));
      this._resources[trackId] = audioResource2;
      this._resourceOrder.Add(trackId);
      while (this._resourceOrder.Count > 3)
      {
        string key = this._resourceOrder[0];
        this._resourceOrder.RemoveAt(0);
        this._resources.Remove(key);
        PubgBoomboxAudioSystem.ContentRoot.RemoveFile(new ResPath(key + ".ogg"));
      }
      return audioResource2;
    }
    catch (Exception ex)
    {
      this.Log.Warning($"[Boombox] Failed to load track {trackId}: {ex.Message}");
      PubgBoomboxAudioSystem.ContentRoot.RemoveFile(resPath);
      return (AudioResource) null;
    }
  }

  private sealed class ActiveAudio
  {
    public string TrackId = string.Empty;
    public EntityUid AudioEntity;
    public AudioComponent? AudioComp;
    public TimeSpan PlaybackStart;
    public float Volume;
    public float MaxDistance;
  }
}
