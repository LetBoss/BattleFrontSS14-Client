using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Robust.Shared.GameObjects;

public abstract class SharedGridFixtureSystem : EntitySystem
{
	[Robust.Shared.IoC.Dependency]
	private readonly FixtureSystem _fixtures;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedMapSystem _map;

	[Robust.Shared.IoC.Dependency]
	private readonly IConfigurationManager _cfg;

	private bool _enabled;

	private float _fixtureEnlargement;

	internal const string ShowGridNodesCommand = "showgridnodes";

	public override void Initialize()
	{
		base.Initialize();
		base.UpdatesBefore.Add(typeof(SharedBroadphaseSystem));
		base.Subs.CVar(_cfg, CVars.GenerateGridFixtures, SetEnabled, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.GridFixtureEnlargement, SetEnlargement, invokeImmediately: true);
		SubscribeLocalEvent<GridInitializeEvent>(OnGridInit);
		SubscribeLocalEvent<RegenerateGridBoundsEvent>(OnGridBoundsRegenerate);
	}

	private void OnGridBoundsRegenerate(ref RegenerateGridBoundsEvent ev)
	{
		RegenerateCollision(ev.Entity, ev.ChunkRectangles, ev.RemovedChunks);
	}

	protected virtual void OnGridInit(GridInitializeEvent ev)
	{
		if (!HasComp<MapComponent>(ev.EntityUid))
		{
			MapGridComponent grid = Comp<MapGridComponent>(ev.EntityUid);
			_map.RegenerateCollision(ev.EntityUid, grid, _map.GetMapChunks(ev.EntityUid, grid).Values.ToHashSet());
		}
	}

	private void SetEnabled(bool value)
	{
		_enabled = value;
	}

	private void SetEnlargement(float value)
	{
		_fixtureEnlargement = value;
	}

	internal void RegenerateCollision(EntityUid uid, Dictionary<MapChunk, List<Box2i>> mapChunks, List<MapChunk> removedChunks)
	{
		if (!_enabled)
		{
			return;
		}
		if (!TryComp(uid, out PhysicsComponent comp))
		{
			base.Log.Error($"Trying to regenerate collision for {uid} that doesn't have {"body"}");
			return;
		}
		if (!TryComp(uid, out FixturesComponent comp2))
		{
			base.Log.Error($"Trying to regenerate collision for {uid} that doesn't have {"manager"}");
			return;
		}
		if (!TryComp(uid, out TransformComponent comp3))
		{
			base.Log.Error($"Trying to regenerate collision for {uid} that doesn't have {"TransformComponent"}");
			return;
		}
		Dictionary<string, Fixture> dictionary = new Dictionary<string, Fixture>(mapChunks.Count);
		foreach (var (mapChunk2, rectangles) in mapChunks)
		{
			UpdateFixture(uid, mapChunk2, rectangles, comp, comp2, comp3);
			foreach (string fixture in mapChunk2.Fixtures)
			{
				dictionary[fixture] = comp2.Fixtures[fixture];
			}
		}
		EntityManager.EventBus.RaiseLocalEvent(uid, new GridFixtureChangeEvent
		{
			NewFixtures = dictionary
		}, broadcast: true);
		_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, comp2, comp);
		CheckSplit(uid, mapChunks, removedChunks);
	}

	internal virtual void CheckSplit(EntityUid gridEuid, Dictionary<MapChunk, List<Box2i>> mapChunks, List<MapChunk> removedChunks)
	{
	}

	internal virtual void CheckSplit(EntityUid gridEuid, MapChunk chunk, List<Box2i> rectangles)
	{
	}

	private bool UpdateFixture(EntityUid uid, MapChunk chunk, List<Box2i> rectangles, PhysicsComponent body, FixturesComponent manager, TransformComponent xform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = chunk.Indices * (int)chunk.ChunkSize;
		ValueList<(string, Fixture)> valueList = default(ValueList<(string, Fixture)>);
		Span<Vector2> span = stackalloc Vector2[4];
		foreach (Box2i rectangle in rectangles)
		{
			Box2i current = rectangle;
			Box2 val2 = Box2i.op_Implicit(((Box2i)(ref current)).Translated(val));
			Box2 val3 = ((Box2)(ref val2)).Enlarged(_fixtureEnlargement);
			PolygonShape polygonShape = new PolygonShape();
			span[0] = val3.BottomLeft;
			span[1] = ((Box2)(ref val3)).BottomRight;
			span[2] = val3.TopRight;
			span[3] = ((Box2)(ref val3)).TopLeft;
			polygonShape.Set(span, 4);
			Fixture item = new Fixture(polygonShape, int.MinValue, int.MinValue, hard: true)
			{
				Owner = uid
			};
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(12, 2, invariantCulture);
			handler.AppendLiteral("grid_chunk-");
			handler.AppendFormatted(val3.Left);
			handler.AppendLiteral("-");
			handler.AppendFormatted(val3.Bottom);
			string item2 = string.Create(invariantCulture, ref handler);
			valueList.Add((item2, item));
		}
		bool result = false;
		foreach (string fixture3 in chunk.Fixtures)
		{
			Fixture fixture = manager.Fixtures[fixture3];
			bool flag = false;
			for (int num = valueList.Count - 1; num >= 0; num--)
			{
				Fixture item3 = valueList[num].Item2;
				if (fixture.Equals(item3))
				{
					flag = true;
					valueList.RemoveSwap(num);
					break;
				}
			}
			if (!flag)
			{
				chunk.Fixtures.Remove(fixture3);
				_fixtures.DestroyFixture(uid, fixture3, fixture, updates: false, body, manager, xform);
				result = true;
			}
		}
		if (valueList.Count > 0)
		{
			result = true;
		}
		Span<(string, Fixture)> span2 = valueList.Span;
		for (int i = 0; i < span2.Length; i++)
		{
			var (text, fixture2) = span2[i];
			chunk.Fixtures.Add(text);
			if (!(_fixtures.GetFixtureOrNull(uid, text, manager)?.Shape is PolygonShape polygonShape2) || !polygonShape2.EqualsApprox((PolygonShape)fixture2.Shape))
			{
				_fixtures.CreateFixture(uid, text, fixture2, updates: false, manager, body, xform);
			}
		}
		return result;
	}
}
