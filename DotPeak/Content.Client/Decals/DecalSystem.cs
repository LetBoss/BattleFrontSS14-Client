// Decompiled with JetBrains decompiler
// Type: Content.Client.Decals.DecalSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Decals.Overlays;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Decals;

public sealed class DecalSystem : SharedDecalSystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private SpriteSystem _sprites;
  private DecalOverlay? _overlay;
  private HashSet<uint> _removedUids = new HashSet<uint>();
  private readonly List<Vector2i> _removedChunks = new List<Vector2i>();

  public override void Initialize()
  {
    base.Initialize();
    this._overlay = new DecalOverlay(this._sprites, (IEntityManager) this.EntityManager, this.PrototypeManager);
    this._overlayManager.AddOverlay((Overlay) this._overlay);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DecalGridComponent, ComponentHandleState>(new ComponentEventRefHandler<DecalGridComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<DecalChunkUpdateEvent>(new EntityEventHandler<DecalChunkUpdateEvent>(this.OnChunkUpdate), (Type[]) null, (Type[]) null);
  }

  public void ToggleOverlay()
  {
    if (this._overlay == null)
      return;
    if (this._overlayManager.HasOverlay<DecalOverlay>())
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    else
      this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay == null)
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
  }

  protected override void OnDecalRemoved(
    EntityUid gridId,
    uint decalId,
    DecalGridComponent component,
    Vector2i indices,
    DecalGridComponent.DecalChunk chunk)
  {
    base.OnDecalRemoved(gridId, decalId, component, indices, chunk);
    chunk.Decals.Remove(decalId);
  }

  private void OnHandleState(
    EntityUid gridUid,
    DecalGridComponent gridComp,
    ref ComponentHandleState args)
  {
    this._removedChunks.Clear();
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> updatedGridChunks;
    switch (((ComponentHandleState) ref args).Current)
    {
      case DecalGridDeltaState decalGridDeltaState:
        updatedGridChunks = decalGridDeltaState.ModifiedChunks;
        using (Dictionary<Vector2i, DecalGridComponent.DecalChunk>.KeyCollection.Enumerator enumerator = gridComp.ChunkCollection.ChunkCollection.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!decalGridDeltaState.AllChunks.Contains(current))
              this._removedChunks.Add(current);
          }
          break;
        }
      case DecalGridState decalGridState:
        updatedGridChunks = decalGridState.Chunks;
        using (Dictionary<Vector2i, DecalGridComponent.DecalChunk>.KeyCollection.Enumerator enumerator = gridComp.ChunkCollection.ChunkCollection.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!decalGridState.Chunks.ContainsKey(current))
              this._removedChunks.Add(current);
          }
          break;
        }
      default:
        return;
    }
    if (this._removedChunks.Count > 0)
      this.RemoveChunks(gridUid, gridComp, (IEnumerable<Vector2i>) this._removedChunks);
    if (updatedGridChunks.Count <= 0)
      return;
    this.UpdateChunks(gridUid, gridComp, updatedGridChunks);
  }

  private void OnChunkUpdate(DecalChunkUpdateEvent ev)
  {
    foreach ((NetEntity key, Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary) in ev.Data)
    {
      NetEntity netEntity = key;
      Dictionary<Vector2i, DecalGridComponent.DecalChunk> updatedGridChunks = dictionary;
      if (updatedGridChunks.Count != 0)
      {
        EntityUid entity = this.GetEntity(netEntity);
        DecalGridComponent gridComp;
        if (!this.TryComp<DecalGridComponent>(entity, ref gridComp))
          this.Log.Error($"Received decal information for an entity without a decal component: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity))}");
        else
          this.UpdateChunks(entity, gridComp, updatedGridChunks);
      }
    }
    HashSet<Vector2i> vector2iSet2;
    foreach ((key, vector2iSet2) in ev.RemovedChunks)
    {
      NetEntity netEntity = key;
      HashSet<Vector2i> chunks = vector2iSet2;
      if (chunks.Count != 0)
      {
        EntityUid entity = this.GetEntity(netEntity);
        DecalGridComponent gridComp;
        if (!this.TryComp<DecalGridComponent>(entity, ref gridComp))
          this.Log.Error($"Received decal information for an entity without a decal component: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity))}");
        else
          this.RemoveChunks(entity, gridComp, (IEnumerable<Vector2i>) chunks);
      }
    }
  }

  private void UpdateChunks(
    EntityUid gridId,
    DecalGridComponent gridComp,
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> updatedGridChunks)
  {
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = gridComp.ChunkCollection.ChunkCollection;
    foreach ((Vector2i vector2i, DecalGridComponent.DecalChunk decalChunk) in updatedGridChunks)
    {
      DecalGridComponent.DecalChunk chunk;
      if (chunkCollection.TryGetValue(vector2i, out chunk))
      {
        this._removedUids.Clear();
        this._removedUids.UnionWith((IEnumerable<uint>) chunk.Decals.Keys);
        this._removedUids.ExceptWith((IEnumerable<uint>) decalChunk.Decals.Keys);
        foreach (uint removedUid in this._removedUids)
        {
          this.OnDecalRemoved(gridId, removedUid, gridComp, vector2i, chunk);
          gridComp.DecalIndex.Remove(removedUid);
        }
      }
      chunkCollection[vector2i] = decalChunk;
      foreach ((uint key, Decal _) in decalChunk.Decals)
        gridComp.DecalIndex[key] = vector2i;
    }
  }

  private void RemoveChunks(
    EntityUid gridId,
    DecalGridComponent gridComp,
    IEnumerable<Vector2i> chunks)
  {
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = gridComp.ChunkCollection.ChunkCollection;
    foreach (Vector2i chunk1 in chunks)
    {
      DecalGridComponent.DecalChunk chunk2;
      if (chunkCollection.TryGetValue(chunk1, out chunk2))
      {
        foreach (uint key in chunk2.Decals.Keys)
        {
          this.OnDecalRemoved(gridId, key, gridComp, chunk1, chunk2);
          gridComp.DecalIndex.Remove(key);
        }
        chunkCollection.Remove(chunk1);
      }
    }
  }
}
