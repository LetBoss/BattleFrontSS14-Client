// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Events.PreventCollideEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;

#nullable enable
namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public struct PreventCollideEvent(
  EntityUid ourEntity,
  EntityUid otherEntity,
  PhysicsComponent ourBody,
  PhysicsComponent otherBody,
  Fixture ourFixture,
  Fixture otherFixture)
{
  public readonly EntityUid OurEntity = ourEntity;
  public readonly EntityUid OtherEntity = otherEntity;
  public readonly PhysicsComponent OurBody = ourBody;
  public readonly PhysicsComponent OtherBody = otherBody;
  public readonly Fixture OurFixture = ourFixture;
  public readonly Fixture OtherFixture = otherFixture;
  public bool Cancelled = false;
}
