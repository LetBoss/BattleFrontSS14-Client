// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Boombox.PubgBoomboxCacheSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Boombox;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Content.Client._PUBG.Boombox;

public sealed class PubgBoomboxCacheSystem : EntitySystem
{
  [Dependency]
  private IResourceManager _res;
  private static readonly ResPath CacheDir = new ResPath("/BoomboxCache");
  private const int MaxTrackBytes = 67108864 /*0x04000000*/;
  private static readonly TimeSpan DownloadStallTimeout = TimeSpan.FromSeconds(30L);
  private readonly Dictionary<string, byte[]> _memory = new Dictionary<string, byte[]>();
  private readonly Dictionary<string, PubgBoomboxCacheSystem.DownloadState> _downloads = new Dictionary<string, PubgBoomboxCacheSystem.DownloadState>();
  private float _stallCheckTimer;

  public event Action<string>? TrackReady;

  public event Action<string>? TrackFailed;

  public event Action<string, float>? TrackProgress;

  public virtual void Initialize()
  {
    base.Initialize();
    this._res.UserData.CreateDir(PubgBoomboxCacheSystem.CacheDir);
    this.SubscribeNetworkEvent<PubgBoomboxFileChunkEvent>(new EntityEventHandler<PubgBoomboxFileChunkEvent>(this.OnChunk), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgBoomboxFileErrorEvent>(new EntityEventHandler<PubgBoomboxFileErrorEvent>(this.OnFileError), (Type[]) null, (Type[]) null);
  }

  public bool IsDownloading(string trackId) => this._downloads.ContainsKey(trackId);

  public float GetProgress(string trackId)
  {
    PubgBoomboxCacheSystem.DownloadState downloadState;
    return !this._downloads.TryGetValue(trackId, out downloadState) || downloadState.Buffer == null || downloadState.Buffer.Length == 0 ? 0.0f : (float) downloadState.Received / (float) downloadState.Buffer.Length;
  }

  public bool HasTrack(string trackId)
  {
    return this._memory.ContainsKey(trackId) || this._res.UserData.Exists(PubgBoomboxCacheSystem.DiskPath(trackId));
  }

  public bool TryGetTrack(string trackId, out byte[] data)
  {
    byte[] numArray;
    if (this._memory.TryGetValue(trackId, out numArray))
    {
      data = numArray;
      return true;
    }
    ResPath resPath = PubgBoomboxCacheSystem.DiskPath(trackId);
    if (this._res.UserData.Exists(resPath))
    {
      try
      {
        using (Stream stream = this._res.UserData.Open(resPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          using (MemoryStream destination = new MemoryStream())
          {
            stream.CopyTo((Stream) destination);
            data = destination.ToArray();
            this._memory[trackId] = data;
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        this.Log.Warning($"[Boombox] Failed to read cached track {trackId}: {ex.Message}");
      }
    }
    data = Array.Empty<byte>();
    return false;
  }

  public void EnsureTrack(string trackId)
  {
    if (string.IsNullOrEmpty(trackId) || this.HasTrack(trackId) || this._downloads.ContainsKey(trackId))
      return;
    this._downloads[trackId] = new PubgBoomboxCacheSystem.DownloadState();
    this.RaiseNetworkEvent((EntityEventArgs) new PubgBoomboxFileRequestEvent(trackId));
  }

  private void OnChunk(PubgBoomboxFileChunkEvent ev)
  {
    PubgBoomboxCacheSystem.DownloadState downloadState1;
    if (!this._downloads.TryGetValue(ev.TrackId, out downloadState1))
      return;
    if (ev.TotalSize <= 0 || ev.TotalSize > 67108864 /*0x04000000*/)
    {
      this._downloads.Remove(ev.TrackId);
      Action<string> trackFailed = this.TrackFailed;
      if (trackFailed == null)
        return;
      trackFailed(ev.TrackId);
    }
    else
    {
      PubgBoomboxCacheSystem.DownloadState downloadState2 = downloadState1;
      if (downloadState2.Buffer == null)
        downloadState2.Buffer = new byte[ev.TotalSize];
      if (downloadState1.Buffer.Length != ev.TotalSize || ev.Offset < 0 || ev.Offset + ev.Data.Length > downloadState1.Buffer.Length)
      {
        this._downloads.Remove(ev.TrackId);
        Action<string> trackFailed = this.TrackFailed;
        if (trackFailed == null)
          return;
        trackFailed(ev.TrackId);
      }
      else
      {
        Array.Copy((Array) ev.Data, 0, (Array) downloadState1.Buffer, ev.Offset, ev.Data.Length);
        downloadState1.Received += ev.Data.Length;
        downloadState1.LastProgress = DateTime.UtcNow;
        Action<string, float> trackProgress = this.TrackProgress;
        if (trackProgress != null)
          trackProgress(ev.TrackId, (float) downloadState1.Received / (float) downloadState1.Buffer.Length);
        if (downloadState1.Received < downloadState1.Buffer.Length)
          return;
        this._downloads.Remove(ev.TrackId);
        this._memory[ev.TrackId] = downloadState1.Buffer;
        this.WriteToDisk(ev.TrackId, downloadState1.Buffer);
        Action<string> trackReady = this.TrackReady;
        if (trackReady == null)
          return;
        trackReady(ev.TrackId);
      }
    }
  }

  private void OnFileError(PubgBoomboxFileErrorEvent ev)
  {
    if (!this._downloads.Remove(ev.TrackId))
      return;
    Action<string> trackFailed = this.TrackFailed;
    if (trackFailed == null)
      return;
    trackFailed(ev.TrackId);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    if (this._downloads.Count == 0)
      return;
    this._stallCheckTimer += frameTime;
    if ((double) this._stallCheckTimer < 5.0)
      return;
    this._stallCheckTimer = 0.0f;
    DateTime utcNow = DateTime.UtcNow;
    List<string> stringList = (List<string>) null;
    foreach ((string key, PubgBoomboxCacheSystem.DownloadState downloadState) in this._downloads)
    {
      if (utcNow - downloadState.LastProgress > PubgBoomboxCacheSystem.DownloadStallTimeout)
      {
        if (stringList == null)
          stringList = new List<string>();
        stringList.Add(key);
      }
    }
    if (stringList == null)
      return;
    foreach (string key in stringList)
    {
      this.Log.Warning($"[Boombox] Download of {key} stalled, dropping");
      this._downloads.Remove(key);
      Action<string> trackFailed = this.TrackFailed;
      if (trackFailed != null)
        trackFailed(key);
    }
  }

  private void WriteToDisk(string trackId, byte[] data)
  {
    try
    {
      using (Stream stream = this._res.UserData.Open(PubgBoomboxCacheSystem.DiskPath(trackId), FileMode.Create, FileAccess.Write, FileShare.None))
        stream.Write((ReadOnlySpan<byte>) data);
    }
    catch (Exception ex)
    {
      this.Log.Warning($"[Boombox] Failed to write track {trackId} to disk: {ex.Message}");
    }
  }

  private static ResPath DiskPath(string trackId)
  {
    return ResPath.op_Division(PubgBoomboxCacheSystem.CacheDir, trackId + ".ogg");
  }

  private sealed class DownloadState
  {
    public byte[]? Buffer;
    public int Received;
    public DateTime LastProgress = DateTime.UtcNow;
  }
}
