using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Atmos;
using Content.Shared.Pinpointer;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Client.Pinpointer;

public sealed class NavMapSystem : SharedNavMapSystem
{
	private (AtmosDirection, Vector2i, AtmosDirection)[] _regionPropagationTable = new(AtmosDirection, Vector2i, AtmosDirection)[4]
	{
		(AtmosDirection.East, new Vector2i(1, 0), AtmosDirection.West),
		(AtmosDirection.West, new Vector2i(-1, 0), AtmosDirection.East),
		(AtmosDirection.North, new Vector2i(0, 1), AtmosDirection.South),
		(AtmosDirection.South, new Vector2i(0, -1), AtmosDirection.North)
	};

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NavMapComponent, ComponentHandleState>((ComponentEventRefHandler<NavMapComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, NavMapComponent component, ref ComponentHandleState args)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		IComponentState current = ((ComponentHandleState)(ref args)).Current;
		Dictionary<Vector2i, int[]> dictionary;
		Dictionary<NetEntity, NavMapBeacon> beacons;
		Dictionary<NetEntity, NavMapRegionProperties> regions;
		if (!(current is NavMapDeltaState navMapDeltaState))
		{
			if (!(current is NavMapState navMapState))
			{
				return;
			}
			dictionary = navMapState.Chunks;
			beacons = navMapState.Beacons;
			regions = navMapState.Regions;
			foreach (Vector2i key4 in component.Chunks.Keys)
			{
				if (!navMapState.Chunks.ContainsKey(key4))
				{
					component.Chunks.Remove(key4);
				}
			}
		}
		else
		{
			dictionary = navMapDeltaState.ModifiedChunks;
			beacons = navMapDeltaState.Beacons;
			regions = navMapDeltaState.Regions;
			foreach (Vector2i key5 in component.Chunks.Keys)
			{
				if (!navMapDeltaState.AllChunks.Contains(key5))
				{
					component.Chunks.Remove(key5);
				}
			}
		}
		List<NetEntity> first = component.RegionProperties.Keys.ToList();
		List<NetEntity> list = new List<NetEntity>();
		component.RegionProperties.Clear();
		NetEntity key;
		foreach (KeyValuePair<NetEntity, NavMapRegionProperties> item in regions)
		{
			item.Deconstruct(out key, out var value);
			NetEntity val = key;
			NavMapRegionProperties value2 = value;
			if (value2.Seeds.Any())
			{
				component.RegionProperties[val] = value2;
				list.Add(val);
				if (!component.RegionOverlays.ContainsKey(val) && !component.QueuedRegionsToFlood.Contains(val))
				{
					component.QueuedRegionsToFlood.Enqueue(val);
				}
			}
		}
		foreach (NetEntity item2 in first.Except(list))
		{
			RemoveNavMapRegion(uid, component, item2);
		}
		foreach (KeyValuePair<Vector2i, int[]> item3 in dictionary)
		{
			item3.Deconstruct(out var key2, out var value3);
			Vector2i val2 = key2;
			int[] array = value3;
			NavMapChunk navMapChunk = new NavMapChunk(val2);
			Array.Copy(array, navMapChunk.TileData, array.Length);
			component.Chunks[val2] = navMapChunk;
			if (!component.ChunkToRegionOwnerTable.TryGetValue(val2, out HashSet<NetEntity> value4))
			{
				continue;
			}
			foreach (NetEntity item4 in value4)
			{
				if (!component.QueuedRegionsToFlood.Contains(item4))
				{
					component.QueuedRegionsToFlood.Enqueue(item4);
				}
			}
		}
		component.Beacons.Clear();
		foreach (KeyValuePair<NetEntity, NavMapBeacon> item5 in beacons)
		{
			item5.Deconstruct(out key, out var value5);
			NetEntity key3 = key;
			NavMapBeacon value6 = value5;
			component.Beacons[key3] = value6;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<NavMapComponent> val = ((EntitySystem)this).AllEntityQuery<NavMapComponent>();
		EntityUid uid = default(EntityUid);
		NavMapComponent component = default(NavMapComponent);
		while (val.MoveNext(ref uid, ref component))
		{
			FloodFillNextEnqueuedRegion(uid, component);
		}
	}

	private void FloodFillNextEnqueuedRegion(EntityUid uid, NavMapComponent component)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (!component.QueuedRegionsToFlood.Any())
		{
			return;
		}
		NetEntity val = component.QueuedRegionsToFlood.Dequeue();
		if (!component.RegionProperties.TryGetValue(val, out var value) || !value.Seeds.Any())
		{
			FloodFillNextEnqueuedRegion(uid, component);
			return;
		}
		(HashSet<Vector2i>, HashSet<Vector2i>) tuple = FloodFillRegion(uid, component, value);
		HashSet<Vector2i> item = tuple.Item1;
		HashSet<Vector2i> item2 = tuple.Item2;
		List<(Vector2i, Vector2i)> mergedRegionTiles = GetMergedRegionTiles(item);
		NavMapRegionOverlay value2 = new NavMapRegionOverlay(value.UiKey, mergedRegionTiles)
		{
			Color = value.Color
		};
		component.RegionOverlays[val] = value2;
		if (component.RegionOwnerToChunkTable.TryGetValue(val, out HashSet<Vector2i> value3))
		{
			foreach (Vector2i item3 in value3)
			{
				if (component.ChunkToRegionOwnerTable.TryGetValue(item3, out HashSet<NetEntity> value4))
				{
					value4.Remove(val);
					component.ChunkToRegionOwnerTable[item3] = value4;
				}
			}
		}
		component.RegionOwnerToChunkTable[val] = item2;
		foreach (Vector2i item4 in item2)
		{
			if (!component.ChunkToRegionOwnerTable.TryGetValue(item4, out HashSet<NetEntity> value5))
			{
				value5 = new HashSet<NetEntity>();
			}
			value5.Add(val);
			component.ChunkToRegionOwnerTable[item4] = value5;
		}
	}

	private (HashSet<Vector2i>, HashSet<Vector2i>) FloodFillRegion(EntityUid uid, NavMapComponent component, NavMapRegionProperties regionProperties)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		if (!regionProperties.Seeds.Any())
		{
			return (new HashSet<Vector2i>(), new HashSet<Vector2i>());
		}
		HashSet<Vector2i> hashSet = new HashSet<Vector2i>();
		HashSet<Vector2i> hashSet2 = new HashSet<Vector2i>();
		Stack<Vector2i> stack = new Stack<Vector2i>();
		foreach (Vector2i seed in regionProperties.Seeds)
		{
			stack.Push(seed);
			while (stack.Count > 0)
			{
				if (hashSet2.Count > regionProperties.MaxArea)
				{
					return (new HashSet<Vector2i>(), new HashSet<Vector2i>());
				}
				Vector2i val = stack.Pop();
				Vector2i val2 = seed - val;
				if (((Vector2i)(ref val2)).Length > (float)regionProperties.MaxRadius || hashSet2.Contains(val))
				{
					continue;
				}
				Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(val, 8);
				int tileIndex = SharedNavMapSystem.GetTileIndex(SharedMapSystem.GetChunkRelative(val, 8));
				if (!component.Chunks.TryGetValue(chunkIndices, out NavMapChunk value))
				{
					continue;
				}
				int num = value.TileData[tileIndex];
				if ((0xF & num) == 0 || (0xF0 & num) == 240 || (0xF00 & num) == 3840)
				{
					continue;
				}
				hashSet2.Add(val);
				hashSet.Add(chunkIndices);
				(AtmosDirection, Vector2i, AtmosDirection)[] regionPropagationTable = _regionPropagationTable;
				for (int i = 0; i < regionPropagationTable.Length; i++)
				{
					var (direction, val3, direction2) = regionPropagationTable[i];
					if (!RegionCanPropagateInDirection(value, val, direction))
					{
						continue;
					}
					Vector2i val4 = val + val3;
					Vector2i chunkIndices2 = SharedMapSystem.GetChunkIndices(val4, 8);
					if (component.Chunks.TryGetValue(chunkIndices2, out NavMapChunk value2))
					{
						hashSet.Add(chunkIndices2);
						if (RegionCanPropagateInDirection(value2, val4, direction2))
						{
							stack.Push(val4);
						}
					}
				}
			}
		}
		return (hashSet2, hashSet);
	}

	private bool RegionCanPropagateInDirection(NavMapChunk chunk, Vector2i tile, AtmosDirection direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		int tileIndex = SharedNavMapSystem.GetTileIndex(SharedMapSystem.GetChunkRelative(tile, 8));
		int num = chunk.TileData[tileIndex];
		if ((0xF & num) == 0)
		{
			return false;
		}
		int num2 = (int)direction << 4;
		int num3 = (int)direction << 8;
		if ((num2 & num) > 0)
		{
			return false;
		}
		if ((num3 & num) > 0)
		{
			return false;
		}
		return true;
	}

	private List<(Vector2i, Vector2i)> GetMergedRegionTiles(HashSet<Vector2i> tiles)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (!tiles.Any())
		{
			return new List<(Vector2i, Vector2i)>();
		}
		IEnumerable<int> source = tiles.Select((Vector2i t) => t.X);
		int num = source.Min();
		int num2 = source.Max();
		IEnumerable<int> source2 = tiles.Select((Vector2i t) => t.Y);
		int num3 = source2.Min();
		int num4 = source2.Max();
		int[,] array = new int[num2 - num + 1, num4 - num3 + 1];
		foreach (Vector2i tile in tiles)
		{
			int num5 = tile.X - num;
			int num6 = tile.Y - num3;
			array[num5, num6] = 1;
		}
		return GetMergedRegionTiles(array, new Vector2i(num, num3));
	}

	private List<(Vector2i, Vector2i)> GetMergedRegionTiles(int[,] matrix, Vector2i offset)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		List<(Vector2i, Vector2i)> list = new List<(Vector2i, Vector2i)>();
		int length = matrix.GetLength(0);
		int length2 = matrix.GetLength(1);
		int[,] array = new int[length, length2];
		(Vector2i, Vector2i) tuple = (default(Vector2i), default(Vector2i));
		int num = 0;
		int num2 = 0;
		while (!IsArrayEmpty(matrix))
		{
			num2++;
			if (num2 > length * length2)
			{
				break;
			}
			array = new int[length, length2];
			tuple = (default(Vector2i), default(Vector2i));
			num = 0;
			for (int i = 0; i < length2; i++)
			{
				array[0, i] = matrix[0, i];
			}
			for (int j = 1; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					array[j, k] = ((matrix[j, k] == 1) ? (array[j - 1, k] + 1) : 0);
				}
			}
			for (int l = 0; l < length; l++)
			{
				for (int m = 0; m < length2; m++)
				{
					int num3 = array[l, m];
					int num4 = m;
					while (num4 >= 0 && array[l, num4] > 0)
					{
						num3 = Math.Min(num3, array[l, num4]);
						int num5 = Math.Max(num, num3 * (m - num4 + 1));
						if (num5 > num)
						{
							num = num5;
							tuple = (new Vector2i(l - num3 + 1, num4), new Vector2i(l, m));
						}
						num4--;
					}
				}
			}
			list.Add((tuple.Item1 + offset, tuple.Item2 + offset));
			for (int n = tuple.Item1.X; n <= tuple.Item2.X; n++)
			{
				for (int num6 = tuple.Item1.Y; num6 <= tuple.Item2.Y; num6++)
				{
					matrix[n, num6] = 0;
				}
			}
		}
		return list;
	}

	private bool IsArrayEmpty(int[,] matrix)
	{
		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				if (matrix[i, j] == 1)
				{
					return false;
				}
			}
		}
		return true;
	}
}
