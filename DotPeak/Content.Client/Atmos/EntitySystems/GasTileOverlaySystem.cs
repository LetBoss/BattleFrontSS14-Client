// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.GasTileOverlaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.Overlays;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class GasTileOverlaySystem : SharedGasTileOverlaySystem
{
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IOverlayManager _overlayMan;
  [Dependency]
  private SpriteSystem _spriteSys;
  [Dependency]
  private SharedTransformSystem _xformSys;
  private GasTileOverlay _overlay;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<SharedGasTileOverlaySystem.GasOverlayUpdateEvent>(new EntityEventHandler<SharedGasTileOverlaySystem.GasOverlayUpdateEvent>(this.HandleGasOverlayUpdate), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTileOverlayComponent, ComponentHandleState>(new ComponentEventRefHandler<GasTileOverlayComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    this._overlay = new GasTileOverlay(this, (IEntityManager) this.EntityManager, this._resourceCache, this.ProtoMan, this._spriteSys, this._xformSys);
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlayMan.RemoveOverlay<GasTileOverlay>();
  }

  private void OnHandleState(
    EntityUid gridUid,
    GasTileOverlayComponent comp,
    ref ComponentHandleState args)
  {
    Dictionary<Vector2i, GasOverlayChunk> dictionary;
    switch (((ComponentHandleState) ref args).Current)
    {
      case GasTileOverlayDeltaState overlayDeltaState:
        dictionary = overlayDeltaState.ModifiedChunks;
        using (Dictionary<Vector2i, GasOverlayChunk>.KeyCollection.Enumerator enumerator = comp.Chunks.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!overlayDeltaState.AllChunks.Contains(current))
              comp.Chunks.Remove(current);
          }
          break;
        }
      case GasTileOverlayState tileOverlayState:
        dictionary = tileOverlayState.Chunks;
        using (Dictionary<Vector2i, GasOverlayChunk>.KeyCollection.Enumerator enumerator = comp.Chunks.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!tileOverlayState.Chunks.ContainsKey(current))
              comp.Chunks.Remove(current);
          }
          break;
        }
      default:
        return;
    }
    foreach ((Vector2i key, GasOverlayChunk gasOverlayChunk) in dictionary)
      comp.Chunks[key] = gasOverlayChunk;
  }

  private void HandleGasOverlayUpdate(
    SharedGasTileOverlaySystem.GasOverlayUpdateEvent ev)
  {
    foreach ((NetEntity key, HashSet<Vector2i> vector2iSet1) in ev.RemovedChunks)
    {
      NetEntity netEntity = key;
      HashSet<Vector2i> vector2iSet2 = vector2iSet1;
      GasTileOverlayComponent overlayComponent;
      if (this.TryComp<GasTileOverlayComponent>(this.GetEntity(netEntity), ref overlayComponent))
      {
        foreach (Vector2i key2 in vector2iSet2)
          overlayComponent.Chunks.Remove(key2);
      }
    }
    List<GasOverlayChunk> gasOverlayChunkList2;
    foreach ((key, gasOverlayChunkList2) in ev.UpdatedChunks)
    {
      NetEntity netEntity = key;
      List<GasOverlayChunk> gasOverlayChunkList3 = gasOverlayChunkList2;
      GasTileOverlayComponent overlayComponent;
      if (this.TryComp<GasTileOverlayComponent>(this.GetEntity(netEntity), ref overlayComponent))
      {
        foreach (GasOverlayChunk gasOverlayChunk in gasOverlayChunkList3)
          overlayComponent.Chunks[gasOverlayChunk.Index] = gasOverlayChunk;
      }
    }
  }
}
