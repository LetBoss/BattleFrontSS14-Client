using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Timing;

namespace Content.Client.Vehicle;

public sealed class GridVehicleMoverOverlay : Overlay
{
	private sealed record FadingMovementDecision(Vector2 Start, Vector2 End, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind Kind, bool Success, MapId Map, TimeSpan Created);

	private const float MovementDecisionFadeSeconds = 2.5f;

	private static readonly Color[] EntryColors = (Color[])(object)new Color[7]
	{
		Color.Red,
		Color.Green,
		Color.Blue,
		Color.Yellow,
		Color.Magenta,
		Color.Cyan,
		Color.Orange
	};

	private static readonly Color ClearProbeFill = new Color(0.19f, 0.95f, 0.55f, 0.08f);

	private static readonly Color ClearProbeOutline = new Color(0.18f, 1f, 0.55f, 0.74f);

	private static readonly Color AppliedProbeFill = new Color(0.22f, 0.78f, 1f, 0.08f);

	private static readonly Color AppliedProbeOutline = new Color(0.25f, 0.84f, 1f, 0.72f);

	private static readonly Color BlockedProbeFill = new Color(1f, 0.16f, 0.22f, 0.18f);

	private static readonly Color BlockedProbeOutline = new Color(1f, 0.16f, 0.22f, 0.92f);

	private readonly IEntityManager _ents;

	private readonly IGameTiming _timing;

	private readonly SharedTransformSystem _transform;

	private readonly EntityLookupSystem _lookup;

	private readonly EntityQuery<MapGridComponent> _gridQuery;

	private readonly EntityQuery<FixturesComponent> _fixturesQuery;

	private readonly EntityQuery<PhysicsComponent> _physicsQuery;

	private readonly Dictionary<EntityUid, Vector2> _lastProbePositions = new Dictionary<EntityUid, Vector2>();

	private readonly List<FadingMovementDecision> _movementDecisions = new List<FadingMovementDecision>();

	public override OverlaySpace Space => (OverlaySpace)8;

	public bool DebugEnabled { get; set; }

	public bool CollisionsEnabled { get; set; } = true;

	public bool MovementEnabled { get; set; } = true;

	public GridVehicleMoverOverlay(IEntityManager ents)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		_ents = ents;
		_timing = IoCManager.Resolve<IGameTiming>();
		_transform = ents.System<SharedTransformSystem>();
		_lookup = ents.System<EntityLookupSystem>();
		_gridQuery = ents.GetEntityQuery<MapGridComponent>();
		_fixturesQuery = ents.GetEntityQuery<FixturesComponent>();
		_physicsQuery = ents.GetEntityQuery<PhysicsComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		if (DebugEnabled)
		{
			DrawVehicleDebug(worldHandle, args.MapId);
		}
		if (CollisionsEnabled)
		{
			DrawCollisionDebug(worldHandle, args.MapId);
		}
		if (MovementEnabled)
		{
			DrawMovementDecisions(worldHandle, args.MapId);
		}
		ClearFrameDebug();
	}

	private void DrawVehicleDebug(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<GridVehicleMoverComponent, TransformComponent> val = _ents.EntityQueryEnumerator<GridVehicleMoverComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		TransformComponent val2 = default(TransformComponent);
		while (val.MoveNext(ref uid, ref mover, ref val2))
		{
			EntityUid? gridUid = val2.GridUid;
			if (gridUid.HasValue)
			{
				EntityUid valueOrDefault = gridUid.GetValueOrDefault();
				if (_gridQuery.HasComp(valueOrDefault))
				{
					DrawMovementTarget(handle, valueOrDefault, mover);
					DrawFacing(handle, uid, mover);
					DrawFixtures(handle, uid);
					DrawPhysics(handle, uid);
				}
			}
		}
		DrawEntryAndExitPoints(handle, mapId);
		DrawTestedTiles(handle);
	}

	private void DrawMovementTarget(DrawingHandleWorld handle, EntityUid grid, GridVehicleMoverComponent mover)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ToMapPosition(grid, mover.Position);
		Vector2 vector2 = ToMapPosition(grid, new Vector2((float)mover.TargetTile.X + 0.5f, (float)mover.TargetTile.Y + 0.5f));
		Color val = (mover.IsCommittedToMove ? Color.White : Color.Red);
		((DrawingHandleBase)handle).DrawLine(vector, vector2, Color.Lime);
		((DrawingHandleBase)handle).DrawCircle(vector, 0.15f, val, true);
		((DrawingHandleBase)handle).DrawCircle(vector2, 0.15f, Color.Yellow, true);
	}

	private void DrawFacing(DrawingHandleWorld handle, EntityUid uid, GridVehicleMoverComponent mover)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!(mover.CurrentDirection == Vector2i.Zero))
		{
			EntityUid? gridUid = _ents.GetComponent<TransformComponent>(uid).GridUid;
			if (gridUid.HasValue)
			{
				EntityUid valueOrDefault = gridUid.GetValueOrDefault();
				Vector2 vector = ToMapPosition(valueOrDefault, mover.Position);
				Vector2 vector2 = Vector2.Normalize(new Vector2(mover.CurrentDirection.X, mover.CurrentDirection.Y));
				((DrawingHandleBase)handle).DrawLine(vector, vector + vector2 * 0.7f, Color.Orange);
			}
		}
	}

	private void DrawFixtures(DrawingHandleWorld handle, EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent val = default(FixturesComponent);
		if (!_physicsQuery.HasComp(uid) || !_fixturesQuery.TryComp(uid, ref val))
		{
			return;
		}
		int num = 0;
		foreach (Fixture value in val.Fixtures.Values)
		{
			Color val2 = EntryColors[num % EntryColors.Length];
			for (int i = 0; i < value.ProxyCount; i++)
			{
				handle.DrawRect(value.Proxies[i].AABB, val2, false);
			}
			num++;
		}
	}

	private void DrawPhysics(DrawingHandleWorld handle, EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Box2 worldAABB = _lookup.GetWorldAABB(uid, (TransformComponent)null);
		Vector2 worldPosition = _transform.GetWorldPosition(uid);
		handle.DrawRect(worldAABB, Color.Magenta, false);
		((DrawingHandleBase)handle).DrawCircle(worldPosition, 0.12f, Color.Magenta, true);
	}

	private void DrawEntryAndExitPoints(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? interiorGrid = FindFirstGrid(mapId);
		DrawEntryPoints(handle, mapId, interiorGrid);
		DrawExitPoints(handle, mapId);
	}

	private EntityUid? FindFirstGrid(MapId mapId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<MapGridComponent, TransformComponent> val = _ents.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
		EntityUid value = default(EntityUid);
		MapGridComponent val2 = default(MapGridComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref value, ref val2, ref val3))
		{
			if (val3.MapID == mapId)
			{
				return value;
			}
		}
		return null;
	}

	private void DrawEntryPoints(DrawingHandleWorld handle, MapId mapId, EntityUid? interiorGrid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<VehicleEnterComponent, TransformComponent> val = _ents.EntityQueryEnumerator<VehicleEnterComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		VehicleEnterComponent vehicleEnterComponent = default(VehicleEnterComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref vehicleEnterComponent, ref val3))
		{
			if (val3.MapID != mapId)
			{
				continue;
			}
			Vector2 worldPosition = _transform.GetWorldPosition(val3);
			Angle localRotation = val3.LocalRotation;
			for (int i = 0; i < vehicleEnterComponent.EntryPoints.Count; i++)
			{
				VehicleEntryPoint vehicleEntryPoint = vehicleEnterComponent.EntryPoints[i];
				Color entryColor = GetEntryColor(i);
				Vector2 position = worldPosition + ((Angle)(ref localRotation)).RotateVec(ref vehicleEntryPoint.Offset);
				DrawPoint(handle, position, Math.Max(0.05f, vehicleEntryPoint.Radius), entryColor);
				if (interiorGrid.HasValue)
				{
					EntityUid valueOrDefault = interiorGrid.GetValueOrDefault();
					Vector2? interiorCoords = vehicleEntryPoint.InteriorCoords;
					if (interiorCoords.HasValue)
					{
						Vector2 valueOrDefault2 = interiorCoords.GetValueOrDefault();
						DrawPoint(handle, ToMapPosition(valueOrDefault, valueOrDefault2), 0.15f, entryColor);
					}
				}
			}
		}
	}

	private void DrawExitPoints(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<VehicleExitComponent, TransformComponent> val = _ents.EntityQueryEnumerator<VehicleExitComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		VehicleExitComponent vehicleExitComponent = default(VehicleExitComponent);
		TransformComponent val3 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref vehicleExitComponent, ref val3))
		{
			if (!(val3.MapID != mapId))
			{
				DrawPoint(handle, _transform.GetWorldPosition(val3), 0.12f, GetEntryColor(vehicleExitComponent.EntryIndex));
			}
		}
	}

	private static void DrawPoint(DrawingHandleWorld handle, Vector2 position, float radius, Color color)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((DrawingHandleBase)handle).DrawCircle(position, radius, ((Color)(ref color)).WithAlpha(0.22f), true);
		((DrawingHandleBase)handle).DrawCircle(position, radius, ((Color)(ref color)).WithAlpha(0.85f), false);
	}

	private void DrawTestedTiles(DrawingHandleWorld handle)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (val, val2) in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugTestedTiles)
		{
			if (_gridQuery.HasComp(val))
			{
				Vector2 vector = ToMapPosition(val, new Vector2((float)val2.X + 0.5f, (float)val2.Y + 0.5f));
				Box2 val3 = ((Box2)(ref Box2.UnitCentered)).Translated(vector);
				Color black = Color.Black;
				handle.DrawRect(val3, ((Color)(ref black)).WithAlpha(0.4f), false);
			}
		}
	}

	private void DrawCollisionDebug(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		DrawCollisionProbes(handle, mapId);
		DrawBlockedCollisions(handle, mapId);
	}

	private void DrawCollisionProbes(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_lastProbePositions.Clear();
		foreach (Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe debugCollisionProbe in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbes)
		{
			if (!(debugCollisionProbe.Map != mapId))
			{
				DrawProbePath(handle, debugCollisionProbe);
				DrawProbeBounds(handle, debugCollisionProbe);
				DrawProbeFacing(handle, debugCollisionProbe);
			}
		}
	}

	private void DrawProbePath(DrawingHandleWorld handle, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe probe)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (_lastProbePositions.TryGetValue(probe.Tested, out var value))
		{
			((DrawingHandleBase)handle).DrawLine(value, probe.Position, new Color(0.55f, 0.9f, 1f, 0.38f));
		}
		_lastProbePositions[probe.Tested] = probe.Position;
	}

	private static void DrawProbeBounds(DrawingHandleWorld handle, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe probe)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Color val = (probe.Blocked ? BlockedProbeFill : (probe.ApplyEffects ? AppliedProbeFill : ClearProbeFill));
		Color val2 = (probe.Blocked ? BlockedProbeOutline : (probe.ApplyEffects ? AppliedProbeOutline : ClearProbeOutline));
		handle.DrawRect(probe.MovementAabb, val, true);
		handle.DrawRect(probe.MovementAabb, val2, false);
		Box2Rotated fixtureBounds = probe.FixtureBounds;
		handle.DrawRect(ref fixtureBounds, new Color(0.45f, 0.92f, 1f, 0.72f), false);
		fixtureBounds = probe.MovementBounds;
		handle.DrawRect(ref fixtureBounds, new Color(0.95f, 0.95f, 1f, 0.45f), false);
		handle.DrawRect(probe.TestedAabb, new Color(0.45f, 0.92f, 1f, 0.18f), false);
	}

	private static void DrawProbeFacing(DrawingHandleWorld handle, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisionProbe probe)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Color val = (probe.Blocked ? new Color(1f, 0.24f, 0.26f, 0.95f) : new Color(0.22f, 1f, 0.58f, 0.95f));
		Angle rotation = probe.Rotation;
		Vector2 unitX = Vector2.UnitX;
		Vector2 vector = ((Angle)(ref rotation)).RotateVec(ref unitX);
		((DrawingHandleBase)handle).DrawCircle(probe.Position, probe.Blocked ? 0.11f : 0.07f, val, true);
		DrawArrow(handle, probe.Position, probe.Position + vector * 0.85f, val);
	}

	private void DrawMovementDecisions(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		CaptureMovementDecisions();
		TimeSpan curTime = _timing.CurTime;
		for (int num = _movementDecisions.Count - 1; num >= 0; num--)
		{
			FadingMovementDecision fadingMovementDecision = _movementDecisions[num];
			float num2 = (float)(curTime - fadingMovementDecision.Created).TotalSeconds;
			if (num2 >= 2.5f)
			{
				_movementDecisions.RemoveAt(num);
			}
			else if (!(fadingMovementDecision.Map != mapId))
			{
				DrawMovementDecision(handle, fadingMovementDecision, 1f - num2 / 2.5f);
			}
		}
	}

	private void CaptureMovementDecisions()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = _timing.CurTime;
		foreach (Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecision debugMovementDecision in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisions)
		{
			if (_gridQuery.HasComp(debugMovementDecision.Grid))
			{
				MapCoordinates val = _transform.ToMapCoordinates(new EntityCoordinates(debugMovementDecision.Grid, debugMovementDecision.Start), true);
				MapCoordinates val2 = _transform.ToMapCoordinates(new EntityCoordinates(debugMovementDecision.Grid, debugMovementDecision.End), true);
				if (!(val.MapId != val2.MapId))
				{
					_movementDecisions.Add(new FadingMovementDecision(val.Position, val2.Position, debugMovementDecision.Kind, debugMovementDecision.Success, val.MapId, curTime));
				}
			}
		}
		Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisions.Clear();
	}

	private static void DrawMovementDecision(DrawingHandleWorld handle, FadingMovementDecision decision, float alpha)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		Color movementDecisionColor = GetMovementDecisionColor(decision.Kind, decision.Success);
		Color val = ((Color)(ref movementDecisionColor)).WithAlpha(0.85f * alpha);
		Color val2 = ((Color)(ref val)).WithAlpha(0.16f * alpha);
		Vector2 delta = decision.End - decision.Start;
		float num = (decision.Success ? 0.08f : 0.12f);
		if (delta.LengthSquared() > 0.0001f)
		{
			DrawArrow(handle, decision.Start, decision.End, val);
		}
		else
		{
			((DrawingHandleBase)handle).DrawCircle(decision.Start, 0.1f, val, true);
		}
		((DrawingHandleBase)handle).DrawCircle(decision.Start, num * 0.75f, val2, true);
		((DrawingHandleBase)handle).DrawCircle(decision.Start, num * 0.75f, val, false);
		((DrawingHandleBase)handle).DrawCircle(decision.End, num, val2, true);
		((DrawingHandleBase)handle).DrawCircle(decision.End, num, val, false);
		if (IsBlockedDecision(decision.Kind))
		{
			DrawBlockedDecisionMarker(handle, decision.End, delta, ((Color)(ref val)).WithAlpha(0.95f * alpha));
		}
	}

	private static bool IsBlockedDecision(Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind kind)
	{
		if (kind == Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.DirectBlocked || kind == Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardBlocked)
		{
			return true;
		}
		return false;
	}

	private static void DrawBlockedDecisionMarker(DrawingHandleWorld handle, Vector2 end, Vector2 delta, Color color)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ((delta.LengthSquared() > 0.0001f) ? (Vector2.Normalize(new Vector2(0f - delta.Y, delta.X)) * 0.14f) : new Vector2(0.14f, 0f));
		((DrawingHandleBase)handle).DrawLine(end - vector, end + vector, color);
		((DrawingHandleBase)handle).DrawLine(end - new Vector2(vector.X, 0f - vector.Y), end + new Vector2(vector.X, 0f - vector.Y), color);
	}

	private static Color GetMovementDecisionColor(Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind kind, bool success)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(kind switch
		{
			Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.DirectClear => new Color(0.2f, 1f, 0.45f, 1f), 
			Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.DirectBlocked => new Color(1f, 0.2f, 0.24f, 1f), 
			Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.LaneCorrection => new Color(0.3f, 0.72f, 1f, 1f), 
			Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.LaneCorrectionFailed => new Color(1f, 0.62f, 0.18f, 1f), 
			Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardAfterCorrection => new Color(0.78f, 0.42f, 1f, 1f), 
			Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardBlocked => new Color(1f, 0.1f, 0.12f, 1f), 
			_ => success ? Color.Lime : Color.Red, 
		});
	}

	private static void DrawBlockedCollisions(DrawingHandleWorld handle, MapId mapId)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision debugCollision in Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollisions)
		{
			if (!(debugCollision.Map != mapId))
			{
				DrawCollisionAabbs(handle, debugCollision);
				DrawCollisionDirection(handle, debugCollision);
				DrawCollisionGap(handle, debugCollision);
			}
		}
	}

	private static void DrawCollisionAabbs(DrawingHandleWorld handle, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision hit)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		handle.DrawRect(hit.TestedAabb, new Color(0.92f, 0.27f, 0.36f, 0.16f), true);
		handle.DrawRect(hit.TestedAabb, new Color(0.97f, 0.38f, 0.46f, 0.85f), false);
		handle.DrawRect(hit.BlockerAabb, new Color(1f, 0.8f, 0.32f, 0.14f), true);
		handle.DrawRect(hit.BlockerAabb, new Color(1f, 0.82f, 0.22f, 0.9f), false);
		Box2 testedAabb = hit.TestedAabb;
		Box2 blockerAabb = hit.BlockerAabb;
		Box2 val = ((Box2)(ref testedAabb)).Intersect(ref blockerAabb);
		if (!(((Box2)(ref val)).Width <= 0f) && !(((Box2)(ref val)).Height <= 0f))
		{
			handle.DrawRect(val, new Color(0.83f, 0.23f, 1f, 0.25f), true);
			handle.DrawRect(val, new Color(0.74f, 0.16f, 0.95f, 0.9f), false);
		}
	}

	private static void DrawCollisionDirection(DrawingHandleWorld handle, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision hit)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Box2 val = hit.TestedAabb;
		Vector2 center = ((Box2)(ref val)).Center;
		val = hit.BlockerAabb;
		Vector2 center2 = ((Box2)(ref val)).Center;
		Vector2 vector = center2 - center;
		float num = vector.Length();
		((DrawingHandleBase)handle).DrawCircle(center, 0.09f, new Color(0.97f, 0.38f, 0.46f, 0.85f), true);
		((DrawingHandleBase)handle).DrawCircle(center2, 0.09f, new Color(1f, 0.82f, 0.22f, 0.9f), true);
		if (!(num <= 0.01f))
		{
			Vector2 end = center + vector / num * MathF.Min(num, 0.9f);
			DrawArrow(handle, center, end, new Color(0.7f, 0.35f, 1f, 0.85f));
		}
	}

	private static void DrawCollisionGap(DrawingHandleWorld handle, Content.Shared.Vehicle.GridVehicleMoverSystem.DebugCollision hit)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		float num = hit.Distance - hit.Skin + hit.Clearance;
		Box2 val = hit.TestedAabb;
		Vector2 center = ((Box2)(ref val)).Center;
		val = hit.BlockerAabb;
		Vector2 vector = (center + ((Box2)(ref val)).Center) * 0.5f;
		Color val2 = ((num <= 0f) ? new Color(1f, 0.32f, 0.32f, 0.92f) : new Color(0.45f, 1f, 0.58f, 0.95f));
		float num2 = 0.07f + MathF.Min(MathF.Abs(num) * 0.03f, 0.09f);
		((DrawingHandleBase)handle).DrawCircle(vector, num2, val2, true);
		((DrawingHandleBase)handle).DrawCircle(vector, num2 * 0.55f, ((Color)(ref val2)).WithAlpha(0.55f), true);
	}

	private Vector2 ToMapPosition(EntityUid grid, Vector2 localPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return _transform.ToMapCoordinates(new EntityCoordinates(grid, localPosition), true).Position;
	}

	private static Color GetEntryColor(int entryIndex)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		int num = ((entryIndex < 0) ? Math.Abs(entryIndex) : entryIndex);
		return EntryColors[num % EntryColors.Length];
	}

	private static void DrawArrow(DrawingHandleWorld handle, Vector2 start, Vector2 end, Color color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		((DrawingHandleBase)handle).DrawLine(start, end, color);
		Vector2 vector = end - start;
		float num = vector.Length();
		if (!(num <= 0.01f))
		{
			Vector2 vector2 = vector / num;
			Vector2 vector3 = end - vector2 * 0.12f;
			Vector2 vector4 = new Vector2(0f - vector2.Y, vector2.X) * 0.06f;
			((DrawingHandleBase)handle).DrawLine(end, vector3 + vector4, color);
			((DrawingHandleBase)handle).DrawLine(end, vector3 - vector4, color);
		}
	}

	private static void ClearFrameDebug()
	{
		Content.Shared.Vehicle.GridVehicleMoverSystem.DebugTestedTiles.Clear();
		Content.Shared.Vehicle.GridVehicleMoverSystem.DebugMovementDecisions.Clear();
		GridVehicleMoverSystem.DebugCollisionPositions.Clear();
	}
}
