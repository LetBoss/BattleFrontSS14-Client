// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.EntitySystems.SharedEventHorizonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ghost;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.ViewVariables;

#nullable enable
namespace Content.Shared.Singularity.EntitySystems;

public abstract class SharedEventHorizonSystem : EntitySystem
{
  [Dependency]
  private FixtureSystem _fixtures;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected IViewVariablesManager Vvm;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EventHorizonComponent, ComponentStartup>(new ComponentEventHandler<EventHorizonComponent, ComponentStartup>(this.OnEventHorizonStartup));
    this.SubscribeLocalEvent<EventHorizonComponent, PreventCollideEvent>(new ComponentEventRefHandler<EventHorizonComponent, PreventCollideEvent>(this.OnPreventCollide));
    ViewVariablesTypeHandler<EventHorizonComponent> typeHandler = this.Vvm.GetTypeHandler<EventHorizonComponent>();
    typeHandler.AddPath<float>("Radius", (ComponentPropertyGetter<EventHorizonComponent, float>) ((_, comp) => comp.Radius), (ComponentPropertySetter<EventHorizonComponent, float>) ((uid, value, comp) => this.SetRadius(uid, value, eventHorizon: comp)));
    typeHandler.AddPath<bool>("CanBreachContainment", (ComponentPropertyGetter<EventHorizonComponent, bool>) ((_, comp) => comp.CanBreachContainment), (ComponentPropertySetter<EventHorizonComponent, bool>) ((uid, value, comp) => this.SetCanBreachContainment(uid, value, eventHorizon: comp)));
    typeHandler.AddPath<string>("ColliderFixtureId", (ComponentPropertyGetter<EventHorizonComponent, string>) ((_, comp) => comp.ColliderFixtureId), (ComponentPropertySetter<EventHorizonComponent, string>) ((uid, value, comp) => this.SetColliderFixtureId(uid, value, eventHorizon: comp)));
    typeHandler.AddPath<string>("ConsumerFixtureId", (ComponentPropertyGetter<EventHorizonComponent, string>) ((_, comp) => comp.ConsumerFixtureId), (ComponentPropertySetter<EventHorizonComponent, string>) ((uid, value, comp) => this.SetConsumerFixtureId(uid, value, eventHorizon: comp)));
  }

  public override void Shutdown()
  {
    ViewVariablesTypeHandler<EventHorizonComponent> typeHandler = this.Vvm.GetTypeHandler<EventHorizonComponent>();
    typeHandler.RemovePath("Radius");
    typeHandler.RemovePath("CanBreachContainment");
    typeHandler.RemovePath("ColliderFixtureId");
    typeHandler.RemovePath("ConsumerFixtureId");
    base.Shutdown();
  }

  public void SetRadius(
    EntityUid uid,
    float value,
    bool updateFixture = true,
    EventHorizonComponent? eventHorizon = null)
  {
    if (!this.Resolve<EventHorizonComponent>(uid, ref eventHorizon))
      return;
    float radius = eventHorizon.Radius;
    if ((double) value == (double) radius)
      return;
    eventHorizon.Radius = value;
    this.Dirty(uid, (IComponent) eventHorizon);
    if (!updateFixture)
      return;
    this.UpdateEventHorizonFixture(uid, eventHorizon: eventHorizon);
  }

  public void SetCanBreachContainment(
    EntityUid uid,
    bool value,
    bool updateFixture = true,
    EventHorizonComponent? eventHorizon = null)
  {
    if (!this.Resolve<EventHorizonComponent>(uid, ref eventHorizon))
      return;
    bool breachContainment = eventHorizon.CanBreachContainment;
    if (value == breachContainment)
      return;
    eventHorizon.CanBreachContainment = value;
    this.Dirty(uid, (IComponent) eventHorizon);
    if (!updateFixture)
      return;
    this.UpdateEventHorizonFixture(uid, eventHorizon: eventHorizon);
  }

  public void SetColliderFixtureId(
    EntityUid uid,
    string? value,
    bool updateFixture = true,
    EventHorizonComponent? eventHorizon = null)
  {
    if (!this.Resolve<EventHorizonComponent>(uid, ref eventHorizon))
      return;
    string colliderFixtureId = eventHorizon.ColliderFixtureId;
    if (value == colliderFixtureId)
      return;
    eventHorizon.ColliderFixtureId = value;
    this.Dirty(uid, (IComponent) eventHorizon);
    if (!updateFixture)
      return;
    this.UpdateEventHorizonFixture(uid, eventHorizon: eventHorizon);
  }

  public void SetConsumerFixtureId(
    EntityUid uid,
    string? value,
    bool updateFixture = true,
    EventHorizonComponent? eventHorizon = null)
  {
    if (!this.Resolve<EventHorizonComponent>(uid, ref eventHorizon))
      return;
    string consumerFixtureId = eventHorizon.ConsumerFixtureId;
    if (value == consumerFixtureId)
      return;
    eventHorizon.ConsumerFixtureId = value;
    this.Dirty(uid, (IComponent) eventHorizon);
    if (!updateFixture)
      return;
    this.UpdateEventHorizonFixture(uid, eventHorizon: eventHorizon);
  }

  public void UpdateEventHorizonFixture(
    EntityUid uid,
    FixturesComponent? fixtures = null,
    EventHorizonComponent? eventHorizon = null)
  {
    if (!this.Resolve<EventHorizonComponent>(uid, ref eventHorizon))
      return;
    string consumerFixtureId = eventHorizon.ConsumerFixtureId;
    string colliderFixtureId = eventHorizon.ColliderFixtureId;
    if (consumerFixtureId == null || colliderFixtureId == null || !this.Resolve<FixturesComponent>(uid, ref fixtures, false))
      return;
    Fixture fixtureOrNull1 = this._fixtures.GetFixtureOrNull(uid, consumerFixtureId, fixtures);
    if (fixtureOrNull1 != null)
    {
      this._physics.SetRadius(uid, consumerFixtureId, fixtureOrNull1, fixtureOrNull1.Shape, eventHorizon.Radius, fixtures);
      this._physics.SetHard(uid, fixtureOrNull1, false, fixtures);
    }
    Fixture fixtureOrNull2 = this._fixtures.GetFixtureOrNull(uid, colliderFixtureId, fixtures);
    if (fixtureOrNull2 != null)
    {
      this._physics.SetRadius(uid, colliderFixtureId, fixtureOrNull2, fixtureOrNull2.Shape, eventHorizon.Radius, fixtures);
      this._physics.SetHard(uid, fixtureOrNull2, true, fixtures);
    }
    this.Dirty(uid, (IComponent) fixtures);
  }

  private void OnEventHorizonStartup(
    EntityUid uid,
    EventHorizonComponent comp,
    ComponentStartup args)
  {
    this.UpdateEventHorizonFixture(uid, eventHorizon: comp);
  }

  private void OnPreventCollide(
    EntityUid uid,
    EventHorizonComponent comp,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled)
      return;
    this.PreventCollide(uid, comp, ref args);
  }

  protected virtual bool PreventCollide(
    EntityUid uid,
    EventHorizonComponent comp,
    ref PreventCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (this.HasComp<MapGridComponent>(otherEntity) || this.HasComp<GhostComponent>(otherEntity))
    {
      args.Cancelled = true;
      return true;
    }
    if (!this.HasComp<ContainmentFieldComponent>(otherEntity) && !this.HasComp<ContainmentFieldGeneratorComponent>(otherEntity))
      return false;
    if (comp.CanBreachContainment)
      args.Cancelled = true;
    return true;
  }
}
