using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

public abstract class SharedNavMapSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class NavMapState(Dictionary<Vector2i, int[]> chunks, Dictionary<NetEntity, NavMapBeacon> beacons, Dictionary<NetEntity, NavMapRegionProperties> regions) : ComponentState
	{
		public Dictionary<Vector2i, int[]> Chunks = chunks;

		public Dictionary<NetEntity, NavMapBeacon> Beacons = beacons;

		public Dictionary<NetEntity, NavMapRegionProperties> Regions = regions;
	}

	[Serializable]
	[NetSerializable]
	protected sealed class NavMapDeltaState(Dictionary<Vector2i, int[]> modifiedChunks, Dictionary<NetEntity, NavMapBeacon> beacons, Dictionary<NetEntity, NavMapRegionProperties> regions, HashSet<Vector2i> allChunks) : ComponentState, IComponentDeltaState<NavMapState>, IComponentDeltaState, IComponentState
	{
		public Dictionary<Vector2i, int[]> ModifiedChunks = modifiedChunks;

		public Dictionary<NetEntity, NavMapBeacon> Beacons = beacons;

		public Dictionary<NetEntity, NavMapRegionProperties> Regions = regions;

		public HashSet<Vector2i> AllChunks = allChunks;

		public void ApplyToFullState(NavMapState state)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			foreach (Vector2i key in state.Chunks.Keys)
			{
				if (!AllChunks.Contains(key))
				{
					state.Chunks.Remove(key);
				}
			}
			foreach (var (index, data) in ModifiedChunks)
			{
				if (!state.Chunks.TryGetValue(index, out int[] stateValue))
				{
					stateValue = (state.Chunks[index] = new int[data.Length]);
				}
				Array.Copy(data, stateValue, data.Length);
			}
			state.Beacons.Clear();
			NetEntity key2;
			foreach (KeyValuePair<NetEntity, NavMapBeacon> beacon2 in Beacons)
			{
				beacon2.Deconstruct(out key2, out var value);
				NetEntity nuid = key2;
				NavMapBeacon beacon = value;
				state.Beacons.Add(nuid, beacon);
			}
			state.Regions.Clear();
			foreach (KeyValuePair<NetEntity, NavMapRegionProperties> region2 in Regions)
			{
				region2.Deconstruct(out key2, out var value2);
				NetEntity nuid2 = key2;
				NavMapRegionProperties region = value2;
				state.Regions.Add(nuid2, region);
			}
		}

		public NavMapState CreateNewFullState(NavMapState state)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<Vector2i, int[]> chunks = new Dictionary<Vector2i, int[]>(state.Chunks.Count);
			foreach (KeyValuePair<Vector2i, int[]> chunk in state.Chunks)
			{
				chunk.Deconstruct(out var key, out var value);
				Vector2i index = key;
				int[] data = value;
				if (AllChunks.Contains(index))
				{
					value = (chunks[index] = new int[64]);
					int[] newData = value;
					if (ModifiedChunks.TryGetValue(index, out int[] updatedData))
					{
						Array.Copy(newData, updatedData, 64);
					}
					else
					{
						Array.Copy(newData, data, 64);
					}
				}
			}
			return new NavMapState(chunks, new Dictionary<NetEntity, NavMapBeacon>(Beacons), new Dictionary<NetEntity, NavMapRegionProperties>(Regions));
		}
	}

	[Serializable]
	[NetSerializable]
	public record struct NavMapBeacon(NetEntity NetEnt, Color Color, string Text, Vector2 Position);

	[Serializable]
	[NetSerializable]
	public record struct NavMapRegionProperties(NetEntity Owner, Enum UiKey, HashSet<Vector2i> Seeds)
	{
		public Color Color = Color.White;

		public int MaxArea = 625;

		public int MaxRadius = 25;
	}

	public const int Categories = 3;

	public const int Directions = 4;

	public const int ChunkSize = 8;

	public const int ArraySize = 64;

	public const int AllDirMask = 15;

	public const int AirlockMask = 3840;

	public const int WallMask = 240;

	public const int FloorMask = 15;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private INetManager _net;

	private static readonly ProtoId<TagPrototype>[] WallTags = new ProtoId<TagPrototype>[2]
	{
		ProtoId<TagPrototype>.op_Implicit("Wall"),
		ProtoId<TagPrototype>.op_Implicit("Window")
	};

	private EntityQuery<NavMapDoorComponent> _doorQuery;

	public override void Initialize()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NavMapComponent, ComponentGetState>((ComponentEventRefHandler<NavMapComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		_doorQuery = ((EntitySystem)this).GetEntityQuery<NavMapDoorComponent>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTileIndex(Vector2i relativeTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return relativeTile.X * 8 + relativeTile.Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetTileFromIndex(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int num = index / 8;
		int y = index % 8;
		return new Vector2i(num, y);
	}

	public NavMapChunkType GetEntityType(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_doorQuery.HasComp(uid))
		{
			return NavMapChunkType.Airlock;
		}
		if (_tagSystem.HasAnyTag(uid, WallTags))
		{
			return NavMapChunkType.Wall;
		}
		return NavMapChunkType.Invalid;
	}

	protected bool TryCreateNavMapBeaconData(EntityUid uid, NavMapBeaconComponent component, TransformComponent xform, MetaDataComponent meta, [NotNullWhen(true)] out NavMapBeacon? beaconData)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		beaconData = null;
		if (!component.Enabled || !xform.GridUid.HasValue || !xform.Anchored)
		{
			return false;
		}
		string name = component.Text;
		if (string.IsNullOrEmpty(name))
		{
			name = meta.EntityName;
		}
		beaconData = new NavMapBeacon(meta.NetEntity, component.Color, name, xform.LocalPosition);
		return true;
	}

	public void AddOrUpdateNavMapRegion(EntityUid uid, NavMapComponent component, NetEntity regionOwner, NavMapRegionProperties regionProperties)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!component.RegionProperties.TryGetValue(regionOwner, out var oldProperties) || oldProperties != regionProperties)
		{
			component.RegionProperties[regionOwner] = regionProperties;
			if (_net.IsServer)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	public void RemoveNavMapRegion(EntityUid uid, NavMapComponent component, NetEntity regionOwner)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!(component.RegionProperties.Remove(regionOwner) | component.RegionOverlays.Remove(regionOwner)))
		{
			return;
		}
		if (component.RegionOwnerToChunkTable.TryGetValue(regionOwner, out HashSet<Vector2i> affectedChunks))
		{
			foreach (Vector2i affectedChunk in affectedChunks)
			{
				if (component.ChunkToRegionOwnerTable.TryGetValue(affectedChunk, out HashSet<NetEntity> regionOwners))
				{
					regionOwners.Remove(regionOwner);
				}
			}
			component.RegionOwnerToChunkTable.Remove(regionOwner);
		}
		if (_net.IsServer)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public Dictionary<NetEntity, NavMapRegionOverlay> GetNavMapRegionOverlays(EntityUid uid, NavMapComponent component, Enum uiKey)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<NetEntity, NavMapRegionOverlay> regionOverlays = new Dictionary<NetEntity, NavMapRegionOverlay>();
		foreach (var (regionOwner, regionOverlay) in component.RegionOverlays)
		{
			if (regionOverlay.UiKey.Equals(uiKey))
			{
				regionOverlays.Add(regionOwner, regionOverlay);
			}
		}
		return regionOverlays;
	}

	private void OnGetState(EntityUid uid, NavMapComponent component, ref ComponentGetState args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key;
		NavMapChunk value;
		Dictionary<Vector2i, int[]> chunks;
		if (((ComponentGetState)(ref args)).FromTick <= ((Component)component).CreationTick)
		{
			chunks = new Dictionary<Vector2i, int[]>(component.Chunks.Count);
			foreach (KeyValuePair<Vector2i, NavMapChunk> chunk3 in component.Chunks)
			{
				chunk3.Deconstruct(out key, out value);
				Vector2i origin = key;
				NavMapChunk chunk = value;
				chunks.Add(origin, chunk.TileData);
			}
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new NavMapState(chunks, component.Beacons, component.RegionProperties);
			return;
		}
		chunks = new Dictionary<Vector2i, int[]>();
		foreach (KeyValuePair<Vector2i, NavMapChunk> chunk4 in component.Chunks)
		{
			chunk4.Deconstruct(out key, out value);
			Vector2i origin2 = key;
			NavMapChunk chunk2 = value;
			if (!(chunk2.LastUpdate < ((ComponentGetState)(ref args)).FromTick))
			{
				chunks.Add(origin2, chunk2.TileData);
			}
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new NavMapDeltaState(chunks, component.Beacons, component.RegionProperties, new HashSet<Vector2i>(component.Chunks.Keys));
	}
}
