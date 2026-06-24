// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.SharedDecalSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Decals;

public abstract class SharedDecalSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  protected IMapManager MapManager;
  protected bool PvsEnabled;
  public const int ChunkSize = 32 /*0x20*/;

  public static Vector2i GetChunkIndices(Vector2 coordinates)
  {
    return new Vector2i((int) Math.Floor((double) coordinates.X / 32.0), (int) Math.Floor((double) coordinates.Y / 32.0));
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInitialize), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DecalGridComponent, ComponentStartup>(new ComponentEventHandler<DecalGridComponent, ComponentStartup>((object) this, __methodptr(OnCompStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DecalGridComponent, ComponentGetState>(new ComponentEventRefHandler<DecalGridComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
  }

  private void OnGetState(EntityUid uid, DecalGridComponent component, ref ComponentGetState args)
  {
    if (this.PvsEnabled && !((ComponentGetState) ref args).ReplayState)
      return;
    if (GameTick.op_LessThanOrEqual(((ComponentGetState) ref args).FromTick, component.CreationTick) || GameTick.op_LessThanOrEqual(((ComponentGetState) ref args).FromTick, component.ForceTick))
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DecalGridState(component.ChunkCollection.ChunkCollection);
    }
    else
    {
      Dictionary<Vector2i, DecalGridComponent.DecalChunk> modifiedChunks = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
      foreach ((Vector2i key, DecalGridComponent.DecalChunk decalChunk) in component.ChunkCollection.ChunkCollection)
      {
        if (GameTick.op_GreaterThanOrEqual(decalChunk.LastModified, ((ComponentGetState) ref args).FromTick))
          modifiedChunks[key] = decalChunk;
      }
      ((ComponentGetState) ref args).State = (IComponentState) new DecalGridDeltaState(modifiedChunks, new HashSet<Vector2i>((IEnumerable<Vector2i>) component.ChunkCollection.ChunkCollection.Keys));
    }
  }

  private void OnGridInitialize(GridInitializeEvent msg)
  {
    this.EnsureComp<DecalGridComponent>(msg.EntityUid);
  }

  private void OnCompStartup(EntityUid uid, DecalGridComponent component, ComponentStartup args)
  {
    foreach ((Vector2i key1, DecalGridComponent.DecalChunk decalChunk) in component.ChunkCollection.ChunkCollection)
    {
      foreach (uint key2 in decalChunk.Decals.Keys)
        component.DecalIndex[key2] = key1;
    }
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  protected Dictionary<Vector2i, DecalGridComponent.DecalChunk>? ChunkCollection(
    EntityUid gridEuid,
    DecalGridComponent? comp = null)
  {
    return !this.Resolve<DecalGridComponent>(gridEuid, ref comp, true) ? (Dictionary<Vector2i, DecalGridComponent.DecalChunk>) null : comp.ChunkCollection.ChunkCollection;
  }

  protected virtual void DirtyChunk(
    EntityUid id,
    Vector2i chunkIndices,
    DecalGridComponent.DecalChunk chunk)
  {
  }

  protected bool RemoveDecalInternal(
    EntityUid gridId,
    uint decalId,
    [NotNullWhen(true)] out Decal? removed,
    DecalGridComponent? component = null)
  {
    removed = (Decal) null;
    Vector2i vector2i;
    DecalGridComponent.DecalChunk chunk;
    if (!this.Resolve<DecalGridComponent>(gridId, ref component, true) || !component.DecalIndex.Remove(decalId, out vector2i) || !component.ChunkCollection.ChunkCollection.TryGetValue(vector2i, out chunk) || !chunk.Decals.Remove(decalId, out removed))
      return false;
    if (chunk.Decals.Count == 0)
      component.ChunkCollection.ChunkCollection.Remove(vector2i);
    this.DirtyChunk(gridId, vector2i, chunk);
    this.OnDecalRemoved(gridId, decalId, component, vector2i, chunk);
    return true;
  }

  protected virtual void OnDecalRemoved(
    EntityUid gridId,
    uint decalId,
    DecalGridComponent component,
    Vector2i indices,
    DecalGridComponent.DecalChunk chunk)
  {
  }

  public virtual HashSet<(uint Index, Decal Decal)> GetDecalsInRange(
    EntityUid gridId,
    Vector2 position,
    float distance = 0.75f,
    Func<Decal, bool>? validDelegate = null)
  {
    return new HashSet<(uint, Decal)>();
  }

  public virtual bool RemoveDecal(EntityUid gridId, uint decalId, DecalGridComponent? component = null)
  {
    return true;
  }
}
