// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Events.MassDataChangedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly record struct MassDataChangedEvent(
  Robust.Shared.GameObjects.Entity<PhysicsComponent, FixturesComponent> Entity,
  float OldMass,
  float OldInertia,
  Vector2 OldCenter)
{
  public float NewMass => this.Entity.Comp1._mass;

  public float NewInertia => this.Entity.Comp1._inertia;

  public Vector2 NewCenter => this.Entity.Comp1._localCenter;

  public bool MassChanged => (double) this.NewMass != (double) this.OldMass;

  public bool InertiaChanged => (double) this.NewInertia != (double) this.OldInertia;

  public bool CenterChanged => this.NewCenter != this.OldCenter;
}
