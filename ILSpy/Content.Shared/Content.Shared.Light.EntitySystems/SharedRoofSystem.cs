using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Shared.Light.EntitySystems;

public abstract class SharedRoofSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private AreaSystem _area;

	private HashSet<Entity<IsRoofComponent>> _roofSet = new HashSet<Entity<IsRoofComponent>>();

	public bool IsRooved(Entity<MapGridComponent, RoofComponent> grid, Vector2i index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		RoofComponent comp = grid.Comp2;
		Vector2i chunkOrigin = SharedMapSystem.GetChunkIndices(index, 8);
		if (comp.Data.TryGetValue(chunkOrigin, out var bitMask))
		{
			Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(index, 8);
			ulong bitFlag = (ulong)(1L << chunkRelative.X + chunkRelative.Y * 8);
			if ((bitMask & bitFlag) == bitFlag)
			{
				return true;
			}
		}
		_roofSet.Clear();
		_lookup.GetLocalEntitiesIntersecting<IsRoofComponent>(grid.Owner, index, _roofSet, -0.04f, (LookupFlags)110, (MapGridComponent)null);
		foreach (Entity<IsRoofComponent> item in _roofSet)
		{
			if (item.Comp.Enabled)
			{
				return true;
			}
		}
		return false;
	}

	public Color? GetColor(Entity<MapGridComponent, RoofComponent> grid, Vector2i index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		RoofComponent roof = grid.Comp2;
		Vector2i chunkOrigin = SharedMapSystem.GetChunkIndices(index, 8);
		if (roof.Data.TryGetValue(chunkOrigin, out var bitMask))
		{
			Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(index, 8);
			ulong bitFlag = (ulong)(1L << chunkRelative.X + chunkRelative.Y * 8);
			if ((bitMask & bitFlag) == bitFlag)
			{
				return roof.Color;
			}
		}
		_roofSet.Clear();
		_lookup.GetLocalEntitiesIntersecting<IsRoofComponent>(grid.Owner, index, _roofSet, -0.04f, (LookupFlags)110, (MapGridComponent)null);
		foreach (Entity<IsRoofComponent> isRoofEnt in _roofSet)
		{
			if (isRoofEnt.Comp.Enabled)
			{
				return (Color)(((_003F?)isRoofEnt.Comp.Color) ?? roof.Color);
			}
		}
		if (_area.IsLightBlocked(Entity<MapGridComponent, RoofComponent>.op_Implicit(grid), index))
		{
			return roof.Color;
		}
		return null;
	}

	public void SetRoof(Entity<MapGridComponent?, RoofComponent?> grid, Vector2i index, bool value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MapGridComponent, RoofComponent>(Entity<MapGridComponent, RoofComponent>.op_Implicit(grid), ref grid.Comp1, ref grid.Comp2, false))
		{
			return;
		}
		Vector2i chunkOrigin = SharedMapSystem.GetChunkIndices(index, 8);
		RoofComponent roof = grid.Comp2;
		if (!roof.Data.TryGetValue(chunkOrigin, out var chunkData))
		{
			if (!value)
			{
				return;
			}
			chunkData = 0uL;
		}
		Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(index, 8);
		ulong bitFlag = (ulong)(1L << chunkRelative.X + chunkRelative.Y * 8);
		if (value)
		{
			if ((chunkData & bitFlag) == bitFlag)
			{
				return;
			}
			chunkData |= bitFlag;
		}
		else
		{
			if ((chunkData & bitFlag) == 0L)
			{
				return;
			}
			chunkData &= ~bitFlag;
		}
		roof.Data[chunkOrigin] = chunkData;
		((EntitySystem)this).Dirty(grid.Owner, (IComponent)(object)roof, (MetaDataComponent)null);
	}
}
