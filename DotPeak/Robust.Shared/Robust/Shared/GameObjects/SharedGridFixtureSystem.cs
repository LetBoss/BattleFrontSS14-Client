// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedGridFixtureSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
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
    this.UpdatesBefore.Add(typeof (SharedBroadphaseSystem));
    this.Subs.CVar<bool>(this._cfg, CVars.GenerateGridFixtures, new Action<bool>(this.SetEnabled), true);
    this.Subs.CVar<float>(this._cfg, CVars.GridFixtureEnlargement, new Action<float>(this.SetEnlargement), true);
    this.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInit));
    this.SubscribeLocalEvent<RegenerateGridBoundsEvent>(new EntityEventRefHandler<RegenerateGridBoundsEvent>(this.OnGridBoundsRegenerate));
  }

  private void OnGridBoundsRegenerate(ref RegenerateGridBoundsEvent ev)
  {
    this.RegenerateCollision(ev.Entity, ev.ChunkRectangles, ev.RemovedChunks);
  }

  protected virtual void OnGridInit(GridInitializeEvent ev)
  {
    if (this.HasComp<MapComponent>(ev.EntityUid))
      return;
    MapGridComponent grid = this.Comp<MapGridComponent>(ev.EntityUid);
    this._map.RegenerateCollision(ev.EntityUid, grid, (IReadOnlySet<MapChunk>) this._map.GetMapChunks(ev.EntityUid, grid).Values.ToHashSet<MapChunk>());
  }

  private void SetEnabled(bool value) => this._enabled = value;

  private void SetEnlargement(float value) => this._fixtureEnlargement = value;

  internal void RegenerateCollision(
    EntityUid uid,
    Dictionary<MapChunk, List<Box2i>> mapChunks,
    List<MapChunk> removedChunks)
  {
    if (!this._enabled)
      return;
    PhysicsComponent comp1;
    if (!this.TryComp<PhysicsComponent>(uid, out comp1))
    {
      this.Log.Error($"Trying to regenerate collision for {uid} that doesn't have {"body"}");
    }
    else
    {
      FixturesComponent comp2;
      if (!this.TryComp<FixturesComponent>(uid, out comp2))
      {
        this.Log.Error($"Trying to regenerate collision for {uid} that doesn't have {"manager"}");
      }
      else
      {
        TransformComponent comp3;
        if (!this.TryComp(uid, out comp3))
        {
          this.Log.Error($"Trying to regenerate collision for {uid} that doesn't have {"TransformComponent"}");
        }
        else
        {
          Dictionary<string, Fixture> dictionary = new Dictionary<string, Fixture>(mapChunks.Count);
          foreach ((MapChunk mapChunk, List<Box2i> rectangles) in mapChunks)
          {
            this.UpdateFixture(uid, mapChunk, rectangles, comp1, comp2, comp3);
            foreach (string fixture in mapChunk.Fixtures)
              dictionary[fixture] = comp2.Fixtures[fixture];
          }
          this.EntityManager.EventBus.RaiseLocalEvent<GridFixtureChangeEvent>(uid, new GridFixtureChangeEvent()
          {
            NewFixtures = dictionary
          }, true);
          this._fixtures.FixtureUpdate(uid, manager: comp2, body: comp1);
          this.CheckSplit(uid, mapChunks, removedChunks);
        }
      }
    }
  }

  internal virtual void CheckSplit(
    EntityUid gridEuid,
    Dictionary<MapChunk, List<Box2i>> mapChunks,
    List<MapChunk> removedChunks)
  {
  }

  internal virtual void CheckSplit(EntityUid gridEuid, MapChunk chunk, List<Box2i> rectangles)
  {
  }

  private unsafe bool UpdateFixture(
    EntityUid uid,
    MapChunk chunk,
    List<Box2i> rectangles,
    PhysicsComponent body,
    FixturesComponent manager,
    TransformComponent xform)
  {
    Vector2i vector2i = Vector2i.op_Multiply(chunk.Indices, (int) chunk.ChunkSize);
    ValueList<(string, Fixture)> valueList = new ValueList<(string, Fixture)>();
    ; // Unable to render the statement
    Span<Vector2> vertices = new Span<Vector2>((void*) pointer, 4);
    foreach (Box2i rectangle in rectangles)
    {
      Box2 box2_1 = Box2i.op_Implicit(((Box2i) ref rectangle).Translated(vector2i));
      Box2 box2_2 = ((Box2) ref box2_1).Enlarged(this._fixtureEnlargement);
      PolygonShape shape = new PolygonShape();
      vertices[0] = box2_2.BottomLeft;
      vertices[1] = ((Box2) ref box2_2).BottomRight;
      vertices[2] = box2_2.TopRight;
      vertices[3] = ((Box2) ref box2_2).TopLeft;
      shape.Set((ReadOnlySpan<Vector2>) vertices, 4);
      Fixture fixture = new Fixture((IPhysShape) shape, int.MinValue, int.MinValue, true)
      {
        Owner = uid
      };
      IFormatProvider invariantCulture = (IFormatProvider) CultureInfo.InvariantCulture;
      IFormatProvider provider = invariantCulture;
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2, invariantCulture);
      interpolatedStringHandler.AppendLiteral("grid_chunk-");
      interpolatedStringHandler.AppendFormatted<float>(box2_2.Left);
      interpolatedStringHandler.AppendLiteral("-");
      interpolatedStringHandler.AppendFormatted<float>(box2_2.Bottom);
      ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
      string str = string.Create(provider, ref local);
      valueList.Add((str, fixture));
    }
    bool flag1 = false;
    foreach (string fixture1 in chunk.Fixtures)
    {
      Fixture fixture2 = manager.Fixtures[fixture1];
      bool flag2 = false;
      for (int index = valueList.Count - 1; index >= 0; --index)
      {
        Fixture other = valueList[index].Item2;
        if (fixture2.Equals(other))
        {
          flag2 = true;
          valueList.RemoveSwap(index);
          break;
        }
      }
      if (!flag2)
      {
        chunk.Fixtures.Remove(fixture1);
        this._fixtures.DestroyFixture(uid, fixture1, fixture2, false, body, manager, xform);
        flag1 = true;
      }
    }
    if (valueList.Count > 0)
      flag1 = true;
    Span<(string, Fixture)> span = valueList.Span;
    for (int index = 0; index < span.Length; ++index)
    {
      (string str, Fixture fixture) = span[index];
      chunk.Fixtures.Add(str);
      if (!(this._fixtures.GetFixtureOrNull(uid, str, manager)?.Shape is PolygonShape shape) || !shape.EqualsApprox((PolygonShape) fixture.Shape))
        this._fixtures.CreateFixture(uid, str, fixture, false, manager, body, xform);
    }
    return flag1;
  }
}
