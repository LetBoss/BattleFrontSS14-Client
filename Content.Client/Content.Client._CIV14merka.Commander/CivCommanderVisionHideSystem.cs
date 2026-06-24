using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionHideSystem : EntitySystem
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedMapSystem _maps;

	[Dependency]
	private SpriteSystem _sprites;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private CivCommanderVisionSystem _vision;

	public readonly List<(Entity<SpriteComponent?> Ent, float BaseAlpha)> CachedBaseAlphas = new List<(Entity<SpriteComponent>, float)>(64);

	private readonly HashSet<EntityUid> _seen = new HashSet<EntityUid>();

	private EntityUid _localEntity = EntityUid.Invalid;

	private MapId _mapId = MapId.Nullspace;

	public bool Prepare(MapId mapId)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		RestoreCachedAlphas();
		_seen.Clear();
		_localEntity = EntityUid.Invalid;
		_mapId = MapId.Nullspace;
		if (!_vision.Active || _vision.VisionRange <= 0f || mapId == MapId.Nullspace)
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return false;
		}
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent) || !civTeamMemberComponent.IsCommander || civTeamMemberComponent.TeamId <= 0)
		{
			return false;
		}
		_localEntity = localEntity.Value;
		_mapId = mapId;
		return true;
	}

	public void RestoreCachedAlphas()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (CachedBaseAlphas.Count == 0)
		{
			return;
		}
		foreach (var (val, num) in CachedBaseAlphas)
		{
			if (val.Comp != null)
			{
				SpriteSystem sprites = _sprites;
				Entity<SpriteComponent> val2 = val;
				Color color = val.Comp.Color;
				sprites.SetColor(val2, ((Color)(ref color)).WithAlpha(num));
			}
		}
		CachedBaseAlphas.Clear();
	}

	public void Apply()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!(_mapId == MapId.Nullspace))
		{
			EntityQueryEnumerator<TransformComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<TransformComponent, SpriteComponent>();
			EntityUid uid = default(EntityUid);
			TransformComponent xform = default(TransformComponent);
			SpriteComponent sprite = default(SpriteComponent);
			while (val.MoveNext(ref uid, ref xform, ref sprite))
			{
				ApplyEntity(uid, xform, sprite);
			}
		}
	}

	private void ApplyEntity(EntityUid uid, TransformComponent xform, SpriteComponent sprite)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!(uid == _localEntity) && _seen.Add(uid) && !(xform.MapID != _mapId) && sprite.Visible && (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(uid, ref civTeamMemberComponent) || !civTeamMemberComponent.IsCommander))
		{
			float num = (IsVisible(xform) ? sprite.Color.A : 0f);
			if (!(MathF.Abs(sprite.Color.A - num) <= 0.001f))
			{
				(EntityUid, SpriteComponent) tuple = (uid, sprite);
				CachedBaseAlphas.Add((Entity<SpriteComponent>.op_Implicit(tuple), sprite.Color.A));
				SpriteSystem sprites = _sprites;
				Entity<SpriteComponent> val = Entity<SpriteComponent>.op_Implicit(tuple);
				Color color = sprite.Color;
				sprites.SetColor(val, ((Color)(ref color)).WithAlpha(num));
			}
		}
	}

	private bool IsVisible(TransformComponent xform)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetTile(xform, out var gridUid, out var tile))
		{
			return false;
		}
		if (_vision.TryGetTileState(gridUid, tile, out var state))
		{
			return state == CivCommanderVisionTileState.Visible;
		}
		return false;
	}

	private bool TryGetTile(TransformComponent xform, out EntityUid gridUid, out Vector2i tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		gridUid = EntityUid.Invalid;
		tile = default(Vector2i);
		if (xform.MapID != _mapId || xform.MapID == MapId.Nullspace)
		{
			return false;
		}
		EntityUid? gridUid2 = xform.GridUid;
		if (gridUid2.HasValue)
		{
			EntityUid valueOrDefault = gridUid2.GetValueOrDefault();
			MapGridComponent val = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(valueOrDefault, ref val))
			{
				gridUid = valueOrDefault;
				tile = _maps.GetTileRef(valueOrDefault, val, xform.Coordinates).GridIndices;
				return true;
			}
		}
		MapCoordinates val2 = _xform.ToMapCoordinates(xform.Coordinates, true);
		EntityUid val3 = default(EntityUid);
		MapGridComponent val4 = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(val2, ref val3, ref val4) || val4 == null)
		{
			return false;
		}
		gridUid = val3;
		tile = _maps.GetTileRef(val3, val4, xform.Coordinates).GridIndices;
		return true;
	}
}
