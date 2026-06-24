using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Stylesheets;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.GlobalMap;

public sealed class CivGlobalPathOverlay : Overlay
{
	private static readonly Color SquadLeaderArrowColor = Color.FromHex((ReadOnlySpan<char>)"#ffd95a", (Color?)null);

	private const float HideDistanceThreshold = 17f;

	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly CivGlobalMapSystem _mapSystem;

	private readonly VehicleSystem _vehicles;

	private readonly IConfigurationManager _cfg;

	private readonly Font _font;

	private readonly Dictionary<int, string> _distanceTextCache = new Dictionary<int, string>();

	private readonly Dictionary<CivGlobalMapMarkerType, string> _markerLabelCache = new Dictionary<CivGlobalMapMarkerType, string>();

	public override OverlaySpace Space => (OverlaySpace)2;

	public CivGlobalPathOverlay(IPlayerManager player, SharedTransformSystem transform, CivGlobalMapSystem mapSystem, VehicleSystem vehicles, IResourceCache resourceCache, IConfigurationManager cfg)
	{
		_player = player;
		_transform = transform;
		_mapSystem = mapSystem;
		_vehicles = vehicles;
		_cfg = cfg;
		_font = resourceCache.NotoStack("Bold", 12);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && _vehicles.TryGetDisplayMapId(localEntity.Value, out var mapId) && !(mapId == MapId.Nullspace))
		{
			Vector2 worldPosition = _transform.GetWorldPosition(localEntity.Value);
			Vector2 center = new Vector2((float)((UIBox2i)(ref args.ViewportBounds)).Width / 2f, (float)((UIBox2i)(ref args.ViewportBounds)).Height / 2f);
			float radius = MathF.Min(((UIBox2i)(ref args.ViewportBounds)).Width, ((UIBox2i)(ref args.ViewportBounds)).Height) * 0.42f;
			if (_cfg.GetCVar<bool>(CCVars.Civ14ShowOrderArrows))
			{
				DrawSquadOrderArrows(in args, mapId, center, worldPosition, radius);
			}
			if (_cfg.GetCVar<bool>(CCVars.Civ14ShowLeaderArrow))
			{
				DrawSquadLeaderArrow(in args, mapId, center, worldPosition, radius);
			}
		}
	}

	private void DrawSquadOrderArrows(in OverlayDrawArgs args, MapId playerMapId, Vector2 center, Vector2 playerWorldPos, float radius)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		int viewerSquadId = _mapSystem.ViewerSquadId;
		if (viewerSquadId <= 0)
		{
			return;
		}
		int num = 0;
		foreach (CivGlobalMapMarkerState visibleMarker in _mapSystem.VisibleMarkers)
		{
			if (visibleMarker.IsObjective || visibleMarker.SquadId != viewerSquadId || !visibleMarker.PlacedBySquadLeader || !visibleMarker.Type.IsGlobal() || visibleMarker.MapId != playerMapId)
			{
				continue;
			}
			float num2 = (visibleMarker.Position - playerWorldPos).Length();
			if (num2 < 17f)
			{
				continue;
			}
			Vector2 vector = args.ViewportControl.WorldToScreen(visibleMarker.Position) - center;
			float num3 = vector.Length();
			if (!(num3 < 8f))
			{
				Vector2 vector2 = vector / num3;
				float num4 = MathF.Min(radius - 60f + (float)num * 40f, MathF.Max(36f, num3 - 20f));
				Vector2 center2 = center + vector2 * num4;
				Color color = CivGlobalMapColorResolver.GetColor(visibleMarker.Type);
				if (!_markerLabelCache.TryGetValue(visibleMarker.Type, out string value))
				{
					value = Loc.GetString("civ-gmap-marker-" + visibleMarker.Type.ToString().ToLowerInvariant());
					_markerLabelCache[visibleMarker.Type] = value;
				}
				DrawArrowWithDistance(in args, center2, vector2, color, num2, value);
				num++;
				if (num >= 3)
				{
					break;
				}
			}
		}
	}

	private static void DrawArrow(DrawingHandleScreen handle, Vector2 center, Vector2 direction, Color color)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = center + direction * 14f;
		Vector2 vector2 = new Vector2(0f - direction.Y, direction.X);
		Vector2 vector3 = center - direction * 6f + vector2 * 6f;
		Vector2 vector4 = center - direction * 6f - vector2 * 6f;
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector3, vector4 }, color);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector3, vector4, vector }, Color.Black);
		Vector2 vector5 = center - direction * 14f;
		Vector2 vector6 = center - direction * 2f;
		((DrawingHandleBase)handle).DrawLine(vector5, vector6, ((Color)(ref color)).WithAlpha(0.85f));
	}

	private void DrawArrowWithDistance(in OverlayDrawArgs args, Vector2 center, Vector2 direction, Color color, float worldDistance, string? label = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		DrawArrow(((OverlayDrawArgs)(ref args)).ScreenHandle, center, direction, color);
		int num = (int)worldDistance;
		if (!_distanceTextCache.TryGetValue(num, out string value))
		{
			value = Loc.GetString("civ-gmap-distance-meters", new(string, object)[1] { ("distance", num) });
			_distanceTextCache[num] = value;
		}
		Vector2 vector = center - direction * 24f;
		Vector2 dimensions = ((OverlayDrawArgs)(ref args)).ScreenHandle.GetDimensions(_font, (ReadOnlySpan<char>)value, 1f);
		Vector2 vector2 = vector - dimensions / 2f;
		if (!string.IsNullOrEmpty(label))
		{
			Vector2 dimensions2 = ((OverlayDrawArgs)(ref args)).ScreenHandle.GetDimensions(_font, (ReadOnlySpan<char>)label, 1f);
			Vector2 vector3 = new Vector2(vector.X - dimensions2.X / 2f, vector2.Y - dimensions2.Y);
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector3, (ReadOnlySpan<char>)label, 1f, color);
		}
		((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector2, (ReadOnlySpan<char>)value, 1f, color);
	}

	private void DrawSquadLeaderArrow(in OverlayDrawArgs args, MapId playerMapId, Vector2 center, Vector2 playerWorldPos, float radius)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null)
		{
			return;
		}
		int viewerSquadId = _mapSystem.ViewerSquadId;
		if (viewerSquadId <= 0)
		{
			return;
		}
		CivGlobalMapPlayerState civGlobalMapPlayerState = null;
		foreach (CivGlobalMapPlayerState visiblePlayer in _mapSystem.VisiblePlayers)
		{
			if (!visiblePlayer.IsSelf && visiblePlayer.IsSquadLeader && visiblePlayer.SquadId == viewerSquadId && visiblePlayer.MapId == playerMapId)
			{
				civGlobalMapPlayerState = visiblePlayer;
				break;
			}
		}
		if (civGlobalMapPlayerState == null)
		{
			return;
		}
		float num = (civGlobalMapPlayerState.Position - playerWorldPos).Length();
		if (!(num < 17f))
		{
			Vector2 vector = args.ViewportControl.WorldToScreen(civGlobalMapPlayerState.Position) - center;
			float num2 = vector.Length();
			if (!(num2 < 0.01f))
			{
				Vector2 vector2 = vector / num2;
				Vector2 center2 = center + vector2 * MathF.Max(48f, radius - 38f);
				DrawArrowWithDistance(in args, center2, vector2, SquadLeaderArrowColor, num, Loc.GetString("civ-gmap-leader-arrow"));
			}
		}
	}
}
