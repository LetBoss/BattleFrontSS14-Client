using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Radiation.Systems;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Radiation.Overlays;

public sealed class RadiationDebugOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entityManager;

	private readonly SharedMapSystem _mapSystem;

	private readonly RadiationSystem _radiation;

	private readonly Font _font;

	public override OverlaySpace Space => (OverlaySpace)6;

	public RadiationDebugOverlay()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		IoCManager.InjectDependencies<RadiationDebugOverlay>(this);
		_radiation = _entityManager.System<RadiationSystem>();
		_mapSystem = _entityManager.System<SharedMapSystem>();
		IResourceCache val = IoCManager.Resolve<IResourceCache>();
		_font = (Font)new VectorFont(val.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		OverlaySpace space = args.Space;
		if ((int)space != 2)
		{
			if ((int)space == 4)
			{
				DrawWorld(in args);
			}
		}
		else
		{
			DrawScreenRays(args);
			DrawScreenResistance(args);
		}
	}

	private void DrawScreenRays(OverlayDrawArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		List<DebugRadiationRay> rays = _radiation.Rays;
		if (rays == null || args.ViewportControl == null)
		{
			return;
		}
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		MapGridComponent val2 = default(MapGridComponent);
		foreach (DebugRadiationRay item3 in rays)
		{
			if (item3.MapId != args.MapId)
			{
				continue;
			}
			if (item3.ReachedDestination)
			{
				Vector2 vector = args.ViewportControl.WorldToScreen(item3.Destination);
				screenHandle.DrawString(_font, vector, (ReadOnlySpan<char>)item3.Rads.ToString("F2"), 2f, Color.White);
			}
			foreach (KeyValuePair<NetEntity, List<(Vector2i, float)>> blocker in item3.Blockers)
			{
				blocker.Deconstruct(out var key, out var value);
				NetEntity val = key;
				List<(Vector2i, float)> list = value;
				EntityUid entity = _entityManager.GetEntity(val);
				if (!_entityManager.TryGetComponent<MapGridComponent>(entity, ref val2))
				{
					continue;
				}
				foreach (var item4 in list)
				{
					Vector2i item = item4.Item1;
					float item2 = item4.Item2;
					Vector2 vector2 = _mapSystem.GridTileToWorldPos(entity, val2, item);
					Vector2 vector3 = args.ViewportControl.WorldToScreen(vector2);
					screenHandle.DrawString(_font, vector3, (ReadOnlySpan<char>)item2.ToString("F2"), 1.5f, Color.White);
				}
			}
		}
	}

	private void DrawScreenResistance(OverlayDrawArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<NetEntity, Dictionary<Vector2i, float>> resistanceGrids = _radiation.ResistanceGrids;
		if (resistanceGrids == null || args.ViewportControl == null)
		{
			return;
		}
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		MapGridComponent val2 = default(MapGridComponent);
		TransformComponent val3 = default(TransformComponent);
		foreach (KeyValuePair<NetEntity, Dictionary<Vector2i, float>> item in resistanceGrids)
		{
			item.Deconstruct(out var key, out var value);
			NetEntity val = key;
			Dictionary<Vector2i, float> dictionary = value;
			EntityUid entity = _entityManager.GetEntity(val);
			if (!_entityManager.TryGetComponent<MapGridComponent>(entity, ref val2) || (entityQuery.TryGetComponent(entity, ref val3) && val3.MapID != args.MapId))
			{
				continue;
			}
			Vector2 vector = new Vector2((int)val2.TileSize, -val2.TileSize) * 0.25f;
			foreach (KeyValuePair<Vector2i, float> item2 in dictionary)
			{
				item2.Deconstruct(out var key2, out var value2);
				Vector2i val4 = key2;
				float num = value2;
				Vector2 vector2 = _mapSystem.GridTileToLocal(entity, val2, val4).Position + vector;
				Vector2 vector3 = _mapSystem.LocalToWorld(entity, val2, vector2);
				Vector2 vector4 = args.ViewportControl.WorldToScreen(vector3);
				screenHandle.DrawString(_font, vector4, num.ToString("F2"), Color.White);
			}
		}
	}

	private void DrawWorld(in OverlayDrawArgs args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		List<DebugRadiationRay> rays = _radiation.Rays;
		if (rays == null)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		MapGridComponent val2 = default(MapGridComponent);
		foreach (DebugRadiationRay item2 in rays)
		{
			if (item2.MapId != args.MapId)
			{
				continue;
			}
			if (item2.ReachedDestination)
			{
				((DrawingHandleBase)worldHandle).DrawLine(item2.Source, item2.Destination, Color.Red);
				continue;
			}
			foreach (KeyValuePair<NetEntity, List<(Vector2i, float)>> blocker in item2.Blockers)
			{
				blocker.Deconstruct(out var key, out var value);
				NetEntity val = key;
				List<(Vector2i, float)> source = value;
				EntityUid entity = _entityManager.GetEntity(val);
				if (_entityManager.TryGetComponent<MapGridComponent>(entity, ref val2))
				{
					Vector2i item = source.Last().Item1;
					Vector2 vector = _mapSystem.GridTileToWorldPos(entity, val2, item);
					((DrawingHandleBase)worldHandle).DrawLine(item2.Source, vector, Color.Red);
				}
			}
		}
	}
}
