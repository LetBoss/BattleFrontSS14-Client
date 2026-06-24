using System;
using System.Collections.Generic;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared._PUBG.Movement;

public sealed class FootstepCacheSystem : EntitySystem
{
	private struct CachedTileFootstep
	{
		public SoundSpecifier? Sound;

		public TimeSpan CacheTime;
	}

	public struct CacheStats
	{
		public int TileCacheCount;

		public int EntityCacheCount;

		public int CacheHits;

		public int CacheMisses;

		public double HitRate;
	}

	[Dependency]
	private IGameTiming _timing;

	private readonly Dictionary<(EntityUid grid, Vector2i tile), CachedTileFootstep> _tileCache = new Dictionary<(EntityUid, Vector2i), CachedTileFootstep>();

	private TimeSpan _lastCleanup;

	private const float CleanupInterval = 30f;

	private int _cacheHits;

	private int _cacheMisses;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GridRemovalEvent>((EntityEventHandler<GridRemovalEvent>)OnGridRemoved, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		TimeSpan curTime = _timing.CurTime;
		if ((curTime - _lastCleanup).TotalSeconds > 30.0)
		{
			CleanupExpiredCache(curTime);
			_lastCleanup = curTime;
		}
	}

	public bool TryGetCachedSound(EntityUid uid, EntityUid gridUid, Vector2i tile, out SoundSpecifier? sound)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		sound = null;
		FootstepCacheComponent cache = default(FootstepCacheComponent);
		if (!((EntitySystem)this).TryComp<FootstepCacheComponent>(uid, ref cache))
		{
			return false;
		}
		TimeSpan curTime = _timing.CurTime;
		Vector2i? lastTile = cache.LastTile;
		if (lastTile.HasValue && lastTile.GetValueOrDefault() == tile)
		{
			EntityUid? cachedGridUid = cache.CachedGridUid;
			if (cachedGridUid.HasValue && cachedGridUid.GetValueOrDefault() == gridUid && curTime - cache.CacheTime < cache.CacheDuration)
			{
				sound = cache.CachedSound;
				_cacheHits++;
				return true;
			}
		}
		if (_tileCache.TryGetValue((gridUid, tile), out var tileData))
		{
			if (curTime - tileData.CacheTime < TimeSpan.FromSeconds(60L))
			{
				sound = tileData.Sound;
				cache.LastTile = tile;
				cache.CachedSound = sound;
				cache.CacheTime = curTime;
				cache.CachedGridUid = gridUid;
				_cacheHits++;
				return true;
			}
			_tileCache.Remove((gridUid, tile));
		}
		_cacheMisses++;
		return false;
	}

	public void SetCachedSound(EntityUid uid, EntityUid gridUid, Vector2i tile, SoundSpecifier? sound)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		FootstepCacheComponent footstepCacheComponent = ((EntitySystem)this).EnsureComp<FootstepCacheComponent>(uid);
		TimeSpan curTime = _timing.CurTime;
		footstepCacheComponent.LastTile = tile;
		footstepCacheComponent.CachedSound = sound;
		footstepCacheComponent.CacheTime = curTime;
		footstepCacheComponent.CachedGridUid = gridUid;
		_tileCache[(gridUid, tile)] = new CachedTileFootstep
		{
			Sound = sound,
			CacheTime = curTime
		};
	}

	public void InvalidateCache(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		FootstepCacheComponent cache = default(FootstepCacheComponent);
		if (((EntitySystem)this).TryComp<FootstepCacheComponent>(uid, ref cache))
		{
			cache.LastTile = null;
			cache.CachedSound = null;
			cache.CacheTime = TimeSpan.Zero;
			cache.CachedGridUid = null;
		}
	}

	public void InvalidateTileCache(EntityUid gridUid, Vector2i tile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		_tileCache.Remove((gridUid, tile));
		EntityQueryEnumerator<FootstepCacheComponent> query = ((EntitySystem)this).EntityQueryEnumerator<FootstepCacheComponent>();
		EntityUid uid = default(EntityUid);
		FootstepCacheComponent cache = default(FootstepCacheComponent);
		while (query.MoveNext(ref uid, ref cache))
		{
			EntityUid? cachedGridUid = cache.CachedGridUid;
			if (cachedGridUid.HasValue && cachedGridUid.GetValueOrDefault() == gridUid)
			{
				Vector2i? lastTile = cache.LastTile;
				if (lastTile.HasValue && lastTile.GetValueOrDefault() == tile)
				{
					InvalidateCache(uid);
				}
			}
		}
	}

	private void OnGridRemoved(GridRemovalEvent ev)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		List<(EntityUid, Vector2i)> toRemove = new List<(EntityUid, Vector2i)>();
		foreach (var key in _tileCache.Keys)
		{
			if (key.grid == ev.EntityUid)
			{
				toRemove.Add(key);
			}
		}
		foreach (var key2 in toRemove)
		{
			_tileCache.Remove(key2);
		}
		EntityQueryEnumerator<FootstepCacheComponent> query = ((EntitySystem)this).EntityQueryEnumerator<FootstepCacheComponent>();
		EntityUid uid = default(EntityUid);
		FootstepCacheComponent cache = default(FootstepCacheComponent);
		while (query.MoveNext(ref uid, ref cache))
		{
			EntityUid? cachedGridUid = cache.CachedGridUid;
			EntityUid entityUid = ev.EntityUid;
			if (cachedGridUid.HasValue && cachedGridUid.GetValueOrDefault() == entityUid)
			{
				InvalidateCache(uid);
			}
		}
	}

	private void CleanupExpiredCache(TimeSpan curTime)
	{
		List<(EntityUid, Vector2i)> toRemove = new List<(EntityUid, Vector2i)>();
		foreach (var (key, data) in _tileCache)
		{
			if ((curTime - data.CacheTime).TotalSeconds > 60.0)
			{
				toRemove.Add(key);
			}
		}
		foreach (var key2 in toRemove)
		{
			_tileCache.Remove(key2);
		}
	}

	public void ClearAllCache()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_tileCache.Clear();
		EntityQueryEnumerator<FootstepCacheComponent> query = ((EntitySystem)this).EntityQueryEnumerator<FootstepCacheComponent>();
		EntityUid uid = default(EntityUid);
		FootstepCacheComponent footstepCacheComponent = default(FootstepCacheComponent);
		while (query.MoveNext(ref uid, ref footstepCacheComponent))
		{
			InvalidateCache(uid);
		}
		_cacheHits = 0;
		_cacheMisses = 0;
	}

	public CacheStats GetCacheStats()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int entityCount = 0;
		EntityQueryEnumerator<FootstepCacheComponent> query = ((EntitySystem)this).EntityQueryEnumerator<FootstepCacheComponent>();
		EntityUid val = default(EntityUid);
		FootstepCacheComponent footstepCacheComponent = default(FootstepCacheComponent);
		while (query.MoveNext(ref val, ref footstepCacheComponent))
		{
			entityCount++;
		}
		return new CacheStats
		{
			TileCacheCount = _tileCache.Count,
			EntityCacheCount = entityCount,
			CacheHits = _cacheHits,
			CacheMisses = _cacheMisses,
			HitRate = ((_cacheHits + _cacheMisses > 0) ? ((double)_cacheHits / (double)(_cacheHits + _cacheMisses)) : 0.0)
		};
	}
}
