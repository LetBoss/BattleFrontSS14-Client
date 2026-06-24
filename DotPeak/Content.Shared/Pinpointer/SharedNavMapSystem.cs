// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.SharedNavMapSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Pinpointer;

public abstract class SharedNavMapSystem : EntitySystem
{
  public const int Categories = 3;
  public const int Directions = 4;
  public const int ChunkSize = 8;
  public const int ArraySize = 64 /*0x40*/;
  public const int AllDirMask = 15;
  public const int AirlockMask = 3840 /*0x0F00*/;
  public const int WallMask = 240 /*0xF0*/;
  public const int FloorMask = 15;
  [Robust.Shared.IoC.Dependency]
  private TagSystem _tagSystem;
  [Robust.Shared.IoC.Dependency]
  private INetManager _net;
  private static readonly ProtoId<TagPrototype>[] WallTags = new ProtoId<TagPrototype>[2]
  {
    (ProtoId<TagPrototype>) "Wall",
    (ProtoId<TagPrototype>) "Window"
  };
  private Robust.Shared.GameObjects.EntityQuery<NavMapDoorComponent> _doorQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NavMapComponent, ComponentGetState>(new ComponentEventRefHandler<NavMapComponent, ComponentGetState>(this.OnGetState));
    this._doorQuery = this.GetEntityQuery<NavMapDoorComponent>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int GetTileIndex(Vector2i relativeTile) => relativeTile.X * 8 + relativeTile.Y;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2i GetTileFromIndex(int index) => new Vector2i(index / 8, index % 8);

  public NavMapChunkType GetEntityType(EntityUid uid)
  {
    if (this._doorQuery.HasComp(uid))
      return NavMapChunkType.Airlock;
    return this._tagSystem.HasAnyTag(uid, SharedNavMapSystem.WallTags) ? NavMapChunkType.Wall : NavMapChunkType.Invalid;
  }

  protected bool TryCreateNavMapBeaconData(
    EntityUid uid,
    NavMapBeaconComponent component,
    TransformComponent xform,
    MetaDataComponent meta,
    [NotNullWhen(true)] out SharedNavMapSystem.NavMapBeacon? beaconData)
  {
    beaconData = new SharedNavMapSystem.NavMapBeacon?();
    if (!component.Enabled || !xform.GridUid.HasValue || !xform.Anchored)
      return false;
    string Text = component.Text;
    if (string.IsNullOrEmpty(Text))
      Text = meta.EntityName;
    beaconData = new SharedNavMapSystem.NavMapBeacon?(new SharedNavMapSystem.NavMapBeacon(meta.NetEntity, component.Color, Text, xform.LocalPosition));
    return true;
  }

  public void AddOrUpdateNavMapRegion(
    EntityUid uid,
    NavMapComponent component,
    NetEntity regionOwner,
    SharedNavMapSystem.NavMapRegionProperties regionProperties)
  {
    SharedNavMapSystem.NavMapRegionProperties regionProperties1;
    if ((!component.RegionProperties.TryGetValue(regionOwner, out regionProperties1) ? 1 : (regionProperties1 != regionProperties ? 1 : 0)) == 0)
      return;
    component.RegionProperties[regionOwner] = regionProperties;
    if (!this._net.IsServer)
      return;
    this.Dirty(uid, (IComponent) component);
  }

  public void RemoveNavMapRegion(EntityUid uid, NavMapComponent component, NetEntity regionOwner)
  {
    if (!(component.RegionProperties.Remove(regionOwner) | component.RegionOverlays.Remove(regionOwner)))
      return;
    HashSet<Vector2i> vector2iSet;
    if (component.RegionOwnerToChunkTable.TryGetValue(regionOwner, out vector2iSet))
    {
      foreach (Vector2i key in vector2iSet)
      {
        HashSet<NetEntity> netEntitySet;
        if (component.ChunkToRegionOwnerTable.TryGetValue(key, out netEntitySet))
          netEntitySet.Remove(regionOwner);
      }
      component.RegionOwnerToChunkTable.Remove(regionOwner);
    }
    if (!this._net.IsServer)
      return;
    this.Dirty(uid, (IComponent) component);
  }

  public Dictionary<NetEntity, NavMapRegionOverlay> GetNavMapRegionOverlays(
    EntityUid uid,
    NavMapComponent component,
    Enum uiKey)
  {
    Dictionary<NetEntity, NavMapRegionOverlay> mapRegionOverlays = new Dictionary<NetEntity, NavMapRegionOverlay>();
    foreach ((NetEntity key, NavMapRegionOverlay mapRegionOverlay) in component.RegionOverlays)
    {
      if (mapRegionOverlay.UiKey.Equals((object) uiKey))
        mapRegionOverlays.Add(key, mapRegionOverlay);
    }
    return mapRegionOverlays;
  }

  private void OnGetState(EntityUid uid, NavMapComponent component, ref ComponentGetState args)
  {
    if (args.FromTick <= component.CreationTick)
    {
      Dictionary<Vector2i, int[]> chunks = new Dictionary<Vector2i, int[]>(component.Chunks.Count);
      foreach ((Vector2i key, NavMapChunk navMapChunk) in component.Chunks)
        chunks.Add(key, navMapChunk.TileData);
      args.State = (IComponentState) new SharedNavMapSystem.NavMapState(chunks, component.Beacons, component.RegionProperties);
    }
    else
    {
      Dictionary<Vector2i, int[]> modifiedChunks = new Dictionary<Vector2i, int[]>();
      foreach ((Vector2i key, NavMapChunk navMapChunk) in component.Chunks)
      {
        if (!(navMapChunk.LastUpdate < args.FromTick))
          modifiedChunks.Add(key, navMapChunk.TileData);
      }
      args.State = (IComponentState) new SharedNavMapSystem.NavMapDeltaState(modifiedChunks, component.Beacons, component.RegionProperties, new HashSet<Vector2i>((IEnumerable<Vector2i>) component.Chunks.Keys));
    }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class NavMapState(
    Dictionary<Vector2i, int[]> chunks,
    Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> beacons,
    Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> regions) : ComponentState
  {
    public Dictionary<Vector2i, int[]> Chunks = chunks;
    public Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> Beacons = beacons;
    public Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> Regions = regions;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class NavMapDeltaState(
    Dictionary<Vector2i, int[]> modifiedChunks,
    Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> beacons,
    Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> regions,
    HashSet<Vector2i> allChunks) : 
    ComponentState,
    IComponentDeltaState<SharedNavMapSystem.NavMapState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<Vector2i, int[]> ModifiedChunks = modifiedChunks;
    public Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> Beacons = beacons;
    public Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> Regions = regions;
    public HashSet<Vector2i> AllChunks = allChunks;

    public void ApplyToFullState(SharedNavMapSystem.NavMapState state)
    {
      foreach (Vector2i key in state.Chunks.Keys)
      {
        if (!this.AllChunks.Contains(key))
          state.Chunks.Remove(key);
      }
      foreach ((Vector2i key, int[] sourceArray) in this.ModifiedChunks)
      {
        int[] destinationArray;
        if (!state.Chunks.TryGetValue(key, out destinationArray))
          state.Chunks[key] = destinationArray = new int[sourceArray.Length];
        Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
      }
      state.Beacons.Clear();
      foreach ((NetEntity key3, SharedNavMapSystem.NavMapBeacon navMapBeacon1) in this.Beacons)
      {
        NetEntity key2 = key3;
        SharedNavMapSystem.NavMapBeacon navMapBeacon2 = navMapBeacon1;
        state.Beacons.Add(key2, navMapBeacon2);
      }
      state.Regions.Clear();
      SharedNavMapSystem.NavMapRegionProperties regionProperties2;
      foreach ((key3, regionProperties2) in this.Regions)
      {
        NetEntity key4 = key3;
        SharedNavMapSystem.NavMapRegionProperties regionProperties3 = regionProperties2;
        state.Regions.Add(key4, regionProperties3);
      }
    }

    public SharedNavMapSystem.NavMapState CreateNewFullState(SharedNavMapSystem.NavMapState state)
    {
      Dictionary<Vector2i, int[]> chunks = new Dictionary<Vector2i, int[]>(state.Chunks.Count);
      foreach ((Vector2i key, int[] numArray) in state.Chunks)
      {
        int[] destinationArray1 = numArray;
        if (this.AllChunks.Contains(key))
        {
          chunks[key] = numArray = new int[64 /*0x40*/];
          int[] sourceArray = numArray;
          int[] destinationArray2;
          if (this.ModifiedChunks.TryGetValue(key, out destinationArray2))
            Array.Copy((Array) sourceArray, (Array) destinationArray2, 64 /*0x40*/);
          else
            Array.Copy((Array) sourceArray, (Array) destinationArray1, 64 /*0x40*/);
        }
      }
      return new SharedNavMapSystem.NavMapState(chunks, new Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon>((IDictionary<NetEntity, SharedNavMapSystem.NavMapBeacon>) this.Beacons), new Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties>((IDictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties>) this.Regions));
    }
  }

  [NetSerializable]
  [Serializable]
  public record struct NavMapBeacon(NetEntity NetEnt, Color Color, string Text, Vector2 Position);

  [NetSerializable]
  [Serializable]
  public record struct NavMapRegionProperties(NetEntity Owner, Enum UiKey, HashSet<Vector2i> Seeds)
  {
    public Color Color = Color.White;
    public int MaxArea = 625;
    public int MaxRadius = 25;

    public NetEntity Owner { get; set; } = Owner;

    public Enum UiKey { get; set; } = UiKey;

    public HashSet<Vector2i> Seeds { get; set; } = Seeds;
  }
}
