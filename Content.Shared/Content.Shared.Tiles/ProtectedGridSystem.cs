using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;

namespace Content.Shared.Tiles;

public sealed class ProtectedGridSystem : EntitySystem
{
	[Dependency]
	private SharedMapSystem _map;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ProtectedGridComponent, MapInitEvent>((EntityEventRefHandler<ProtectedGridComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProtectedGridComponent, FloorTileAttemptEvent>((EntityEventRefHandler<ProtectedGridComponent, FloorTileAttemptEvent>)OnFloorTileAttempt, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ProtectedGridComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(Entity<ProtectedGridComponent>.op_Implicit(ent), ref grid))
		{
			return;
		}
		ChunkIndicesEnumerator chunkEnumerator = default(ChunkIndicesEnumerator);
		((ChunkIndicesEnumerator)(ref chunkEnumerator))._002Ector(grid.LocalAABB, 8);
		Vector2i? chunk = default(Vector2i?);
		Vector2i index = default(Vector2i);
		while (((ChunkIndicesEnumerator)(ref chunkEnumerator)).MoveNext(ref chunk))
		{
			ulong flag = 0uL;
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					((Vector2i)(ref index))._002Ector(x + chunk.Value.X * 8, y + chunk.Value.Y * 8);
					TileRef tile = _map.GetTileRef(ent.Owner, grid, index);
					if (!((Tile)(ref tile.Tile)).IsEmpty)
					{
						ulong data = SharedMapSystem.ToBitmask(new Vector2i(x, y), (byte)8);
						flag |= data;
					}
				}
			}
			if (flag != 0L)
			{
				ent.Comp.BaseIndices[chunk.Value] = flag;
			}
		}
		((EntitySystem)this).Dirty<ProtectedGridComponent>(ent, (MetaDataComponent)null);
	}

	private void OnFloorTileAttempt(Entity<ProtectedGridComponent> ent, ref FloorTileAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector2i chunkOrigin = SharedMapSystem.GetChunkIndices(args.GridIndices, 8);
		if (!ent.Comp.BaseIndices.TryGetValue(chunkOrigin, out var data))
		{
			args.Cancelled = true;
		}
		else if (SharedMapSystem.FromBitmask(args.GridIndices, data, (byte)8))
		{
			args.Cancelled = true;
		}
	}
}
