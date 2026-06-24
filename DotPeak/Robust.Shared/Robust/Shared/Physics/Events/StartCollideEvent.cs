// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Events.StartCollideEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Utility;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly struct StartCollideEvent
{
  public readonly EntityUid OurEntity;
  public readonly EntityUid OtherEntity;
  public readonly PhysicsComponent OurBody;
  public readonly PhysicsComponent OtherBody;
  public readonly string OurFixtureId;
  public readonly string OtherFixtureId;
  public readonly Fixture OurFixture;
  public readonly Fixture OtherFixture;
  internal readonly FixedArray2<Vector2> _worldPoints;
  public readonly int PointCount;
  public readonly Vector2 WorldNormal;

  public Vector2[] WorldPoints => this._worldPoints.AsSpan.Slice(0, this.PointCount).ToArray();

  internal StartCollideEvent(
    EntityUid ourEntity,
    EntityUid otherEntity,
    string ourFixtureId,
    string otherFixtureId,
    Fixture ourFixture,
    Fixture otherFixture,
    PhysicsComponent ourBody,
    PhysicsComponent otherBody,
    FixedArray2<Vector2> worldPoints,
    int pointCount,
    Vector2 worldNormal)
  {
    this.OurEntity = ourEntity;
    this.OtherEntity = otherEntity;
    this.OurFixtureId = ourFixtureId;
    this.OtherFixtureId = otherFixtureId;
    this.OurFixture = ourFixture;
    this.OtherFixture = otherFixture;
    this.OtherBody = otherBody;
    this.OurBody = ourBody;
    this._worldPoints = worldPoints;
    this.PointCount = pointCount;
    this.WorldNormal = worldNormal;
  }
}
