// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Movement.FootstepCacheSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Movement;

public sealed class FootstepCacheSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  private readonly Dictionary<(EntityUid grid, Vector2i tile), FootstepCacheSystem.CachedTileFootstep> _tileCache = new Dictionary<(EntityUid, Vector2i), FootstepCacheSystem.CachedTileFootstep>();
  private TimeSpan _lastCleanup;
  private const float CleanupInterval = 30f;
  private int _cacheHits;
  private int _cacheMisses;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    if ((curTime - this._lastCleanup).TotalSeconds <= 30.0)
      return;
    this.CleanupExpiredCache(curTime);
    this._lastCleanup = curTime;
  }

  public bool TryGetCachedSound(
    EntityUid uid,
    EntityUid gridUid,
    Vector2i tile,
    out SoundSpecifier? sound)
  {
    sound = (SoundSpecifier) null;
    FootstepCacheComponent comp;
    if (!this.TryComp<FootstepCacheComponent>(uid, out comp))
      return false;
    TimeSpan curTime = this._timing.CurTime;
    Vector2i? lastTile = comp.LastTile;
    Vector2i vector2i = tile;
    if ((lastTile.HasValue ? (Vector2i.op_Equality(lastTile.GetValueOrDefault(), vector2i) ? 1 : 0) : 0) != 0)
    {
      EntityUid? cachedGridUid = comp.CachedGridUid;
      EntityUid entityUid = gridUid;
      if ((cachedGridUid.HasValue ? (cachedGridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && curTime - comp.CacheTime < comp.CacheDuration)
      {
        sound = comp.CachedSound;
        ++this._cacheHits;
        return true;
      }
    }
    FootstepCacheSystem.CachedTileFootstep cachedTileFootstep;
    if (this._tileCache.TryGetValue((gridUid, tile), out cachedTileFootstep))
    {
      if (curTime - cachedTileFootstep.CacheTime < TimeSpan.FromSeconds(60L))
      {
        sound = cachedTileFootstep.Sound;
        comp.LastTile = new Vector2i?(tile);
        comp.CachedSound = sound;
        comp.CacheTime = curTime;
        comp.CachedGridUid = new EntityUid?(gridUid);
        ++this._cacheHits;
        return true;
      }
      this._tileCache.Remove((gridUid, tile));
    }
    ++this._cacheMisses;
    return false;
  }

  public void SetCachedSound(
    EntityUid uid,
    EntityUid gridUid,
    Vector2i tile,
    SoundSpecifier? sound)
  {
    FootstepCacheComponent footstepCacheComponent = this.EnsureComp<FootstepCacheComponent>(uid);
    TimeSpan curTime = this._timing.CurTime;
    footstepCacheComponent.LastTile = new Vector2i?(tile);
    footstepCacheComponent.CachedSound = sound;
    footstepCacheComponent.CacheTime = curTime;
    footstepCacheComponent.CachedGridUid = new EntityUid?(gridUid);
    this._tileCache[(gridUid, tile)] = new FootstepCacheSystem.CachedTileFootstep()
    {
      Sound = sound,
      CacheTime = curTime
    };
  }

  public void InvalidateCache(EntityUid uid)
  {
    FootstepCacheComponent comp;
    if (!this.TryComp<FootstepCacheComponent>(uid, out comp))
      return;
    comp.LastTile = new Vector2i?();
    comp.CachedSound = (SoundSpecifier) null;
    comp.CacheTime = TimeSpan.Zero;
    comp.CachedGridUid = new EntityUid?();
  }

  public void InvalidateTileCache(EntityUid gridUid, Vector2i tile)
  {
    this._tileCache.Remove((gridUid, tile));
    Robust.Shared.GameObjects.EntityQueryEnumerator<FootstepCacheComponent> entityQueryEnumerator = this.EntityQueryEnumerator<FootstepCacheComponent>();
    EntityUid uid;
    FootstepCacheComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? cachedGridUid = comp1.CachedGridUid;
      EntityUid entityUid = gridUid;
      if ((cachedGridUid.HasValue ? (cachedGridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
      {
        Vector2i? lastTile = comp1.LastTile;
        Vector2i vector2i = tile;
        if ((lastTile.HasValue ? (Vector2i.op_Equality(lastTile.GetValueOrDefault(), vector2i) ? 1 : 0) : 0) != 0)
          this.InvalidateCache(uid);
      }
    }
  }

  private void OnGridRemoved(GridRemovalEvent ev)
  {
    List<(EntityUid, Vector2i)> valueTupleList = new List<(EntityUid, Vector2i)>();
    foreach ((EntityUid grid, Vector2i tile) key in this._tileCache.Keys)
    {
      if (key.grid == ev.EntityUid)
        valueTupleList.Add(key);
    }
    foreach ((EntityUid, Vector2i) key in valueTupleList)
      this._tileCache.Remove(key);
    Robust.Shared.GameObjects.EntityQueryEnumerator<FootstepCacheComponent> entityQueryEnumerator = this.EntityQueryEnumerator<FootstepCacheComponent>();
    EntityUid uid;
    FootstepCacheComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? cachedGridUid = comp1.CachedGridUid;
      EntityUid entityUid = ev.EntityUid;
      if ((cachedGridUid.HasValue ? (cachedGridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        this.InvalidateCache(uid);
    }
  }

  private void CleanupExpiredCache(TimeSpan curTime)
  {
    List<(EntityUid, Vector2i)> valueTupleList = new List<(EntityUid, Vector2i)>();
    foreach (((EntityUid grid, Vector2i tile) key, FootstepCacheSystem.CachedTileFootstep cachedTileFootstep) in this._tileCache)
    {
      if ((curTime - cachedTileFootstep.CacheTime).TotalSeconds > 60.0)
        valueTupleList.Add(key);
    }
    foreach ((EntityUid, Vector2i) key in valueTupleList)
      this._tileCache.Remove(key);
  }

  public void ClearAllCache()
  {
    this._tileCache.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<FootstepCacheComponent> entityQueryEnumerator = this.EntityQueryEnumerator<FootstepCacheComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out FootstepCacheComponent _))
      this.InvalidateCache(uid);
    this._cacheHits = 0;
    this._cacheMisses = 0;
  }

  public FootstepCacheSystem.CacheStats GetCacheStats()
  {
    int num = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<FootstepCacheComponent> entityQueryEnumerator = this.EntityQueryEnumerator<FootstepCacheComponent>();
    while (entityQueryEnumerator.MoveNext(out EntityUid _, out FootstepCacheComponent _))
      ++num;
    return new FootstepCacheSystem.CacheStats()
    {
      TileCacheCount = this._tileCache.Count,
      EntityCacheCount = num,
      CacheHits = this._cacheHits,
      CacheMisses = this._cacheMisses,
      HitRate = this._cacheHits + this._cacheMisses > 0 ? (double) this._cacheHits / (double) (this._cacheHits + this._cacheMisses) : 0.0
    };
  }

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
}
