// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.FixturesChangeSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public sealed class FixturesChangeSystem : EntitySystem
{
  [Dependency]
  private readonly FixtureSystem _fixtures;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixturesQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.SubscribeLocalEvent<FixturesChangeComponent, ComponentStartup>(new EntityEventRefHandler<FixturesChangeComponent, ComponentStartup>(this.OnChangeStartup));
    this.SubscribeLocalEvent<FixturesChangeComponent, ComponentShutdown>(new EntityEventRefHandler<FixturesChangeComponent, ComponentShutdown>(this.OnChangeShutdown));
  }

  private void OnChangeStartup(Entity<FixturesChangeComponent> ent, ref ComponentStartup args)
  {
    PhysicsComponent component1;
    FixturesComponent component2;
    if (!this._physicsQuery.TryComp((EntityUid) ent, out component1) || !this._fixturesQuery.TryComp((EntityUid) ent, out component2))
      return;
    foreach ((string str, Fixture fixture) in ent.Comp.Fixtures)
      this._fixtures.TryCreateFixture(ent.Owner, fixture.Shape, str, fixture.Density, fixture.Hard, fixture.CollisionLayer, fixture.CollisionMask, fixture.Friction, fixture.Restitution, manager: component2, body: component1);
    this._physics.WakeBody(ent.Owner, manager: component2, body: component1);
  }

  private void OnChangeShutdown(Entity<FixturesChangeComponent> ent, ref ComponentShutdown args)
  {
    foreach (string key in ent.Comp.Fixtures.Keys)
      this._fixtures.DestroyFixture(ent.Owner, key);
  }
}
