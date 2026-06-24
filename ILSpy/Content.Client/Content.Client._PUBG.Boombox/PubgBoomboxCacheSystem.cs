using System;
using System.Collections.Generic;
using System.IO;
using Content.Shared._PUBG.Boombox;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.Boombox;

public sealed class PubgBoomboxCacheSystem : EntitySystem
{
	private sealed class DownloadState
	{
		public byte[]? Buffer;

		public int Received;

		public DateTime LastProgress = DateTime.UtcNow;
	}

	[Dependency]
	private IResourceManager _res;

	private static readonly ResPath CacheDir = new ResPath("/BoomboxCache");

	private const int MaxTrackBytes = 67108864;

	private static readonly TimeSpan DownloadStallTimeout = TimeSpan.FromSeconds(30L);

	private readonly Dictionary<string, byte[]> _memory = new Dictionary<string, byte[]>();

	private readonly Dictionary<string, DownloadState> _downloads = new Dictionary<string, DownloadState>();

	private float _stallCheckTimer;

	public event Action<string>? TrackReady;

	public event Action<string>? TrackFailed;

	public event Action<string, float>? TrackProgress;

	public override void Initialize()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_res.UserData.CreateDir(CacheDir);
		((EntitySystem)this).SubscribeNetworkEvent<PubgBoomboxFileChunkEvent>((EntityEventHandler<PubgBoomboxFileChunkEvent>)OnChunk, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgBoomboxFileErrorEvent>((EntityEventHandler<PubgBoomboxFileErrorEvent>)OnFileError, (Type[])null, (Type[])null);
	}

	public bool IsDownloading(string trackId)
	{
		return _downloads.ContainsKey(trackId);
	}

	public float GetProgress(string trackId)
	{
		if (!_downloads.TryGetValue(trackId, out DownloadState value) || value.Buffer == null || value.Buffer.Length == 0)
		{
			return 0f;
		}
		return (float)value.Received / (float)value.Buffer.Length;
	}

	public bool HasTrack(string trackId)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (_memory.ContainsKey(trackId))
		{
			return true;
		}
		return _res.UserData.Exists(DiskPath(trackId));
	}

	public bool TryGetTrack(string trackId, out byte[] data)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_memory.TryGetValue(trackId, out byte[] value))
		{
			data = value;
			return true;
		}
		ResPath val = DiskPath(trackId);
		if (_res.UserData.Exists(val))
		{
			try
			{
				using Stream stream = _res.UserData.Open(val, FileMode.Open, FileAccess.Read, FileShare.Read);
				using MemoryStream memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				data = memoryStream.ToArray();
				_memory[trackId] = data;
				return true;
			}
			catch (Exception ex)
			{
				((EntitySystem)this).Log.Warning("[Boombox] Failed to read cached track " + trackId + ": " + ex.Message);
			}
		}
		data = Array.Empty<byte>();
		return false;
	}

	public void EnsureTrack(string trackId)
	{
		if (!string.IsNullOrEmpty(trackId) && !HasTrack(trackId) && !_downloads.ContainsKey(trackId))
		{
			_downloads[trackId] = new DownloadState();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgBoomboxFileRequestEvent(trackId));
		}
	}

	private void OnChunk(PubgBoomboxFileChunkEvent ev)
	{
		if (!_downloads.TryGetValue(ev.TrackId, out DownloadState value))
		{
			return;
		}
		if (ev.TotalSize <= 0 || ev.TotalSize > 67108864)
		{
			_downloads.Remove(ev.TrackId);
			this.TrackFailed?.Invoke(ev.TrackId);
			return;
		}
		DownloadState downloadState = value;
		if (downloadState.Buffer == null)
		{
			downloadState.Buffer = new byte[ev.TotalSize];
		}
		if (value.Buffer.Length != ev.TotalSize || ev.Offset < 0 || ev.Offset + ev.Data.Length > value.Buffer.Length)
		{
			_downloads.Remove(ev.TrackId);
			this.TrackFailed?.Invoke(ev.TrackId);
			return;
		}
		Array.Copy(ev.Data, 0, value.Buffer, ev.Offset, ev.Data.Length);
		value.Received += ev.Data.Length;
		value.LastProgress = DateTime.UtcNow;
		this.TrackProgress?.Invoke(ev.TrackId, (float)value.Received / (float)value.Buffer.Length);
		if (value.Received >= value.Buffer.Length)
		{
			_downloads.Remove(ev.TrackId);
			_memory[ev.TrackId] = value.Buffer;
			WriteToDisk(ev.TrackId, value.Buffer);
			this.TrackReady?.Invoke(ev.TrackId);
		}
	}

	private void OnFileError(PubgBoomboxFileErrorEvent ev)
	{
		if (_downloads.Remove(ev.TrackId))
		{
			this.TrackFailed?.Invoke(ev.TrackId);
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		((EntitySystem)this).FrameUpdate(frameTime);
		if (_downloads.Count == 0)
		{
			return;
		}
		_stallCheckTimer += frameTime;
		if (_stallCheckTimer < 5f)
		{
			return;
		}
		_stallCheckTimer = 0f;
		DateTime utcNow = DateTime.UtcNow;
		List<string> list = null;
		foreach (var (item, downloadState2) in _downloads)
		{
			if (utcNow - downloadState2.LastProgress > DownloadStallTimeout)
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(item);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (string item2 in list)
		{
			((EntitySystem)this).Log.Warning("[Boombox] Download of " + item2 + " stalled, dropping");
			_downloads.Remove(item2);
			this.TrackFailed?.Invoke(item2);
		}
	}

	private void WriteToDisk(string trackId, byte[] data)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			using Stream stream = _res.UserData.Open(DiskPath(trackId), FileMode.Create, FileAccess.Write, FileShare.None);
			stream.Write(data);
		}
		catch (Exception ex)
		{
			((EntitySystem)this).Log.Warning("[Boombox] Failed to write track " + trackId + " to disk: " + ex.Message);
		}
	}

	private static ResPath DiskPath(string trackId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return CacheDir / (trackId + ".ogg");
	}
}
