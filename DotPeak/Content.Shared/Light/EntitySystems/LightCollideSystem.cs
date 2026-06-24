// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.LightCollideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public sealed class LightCollideSystem : EntitySystem
{
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SlimPoweredLightSystem _lights;
  private Robust.Shared.GameObjects.EntityQuery<LightOnCollideComponent> _lightQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._lightQuery = this.GetEntityQuery<LightOnCollideComponent>();
    this.SubscribeLocalEvent<LightOnCollideColliderComponent, PreventCollideEvent>(new EntityEventRefHandler<LightOnCollideColliderComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<LightOnCollideColliderComponent, StartCollideEvent>(new EntityEventRefHandler<LightOnCollideColliderComponent, StartCollideEvent>(this.OnStart));
    this.SubscribeLocalEvent<LightOnCollideColliderComponent, EndCollideEvent>(new EntityEventRefHandler<LightOnCollideColliderComponent, EndCollideEvent>(this.OnEnd));
    this.SubscribeLocalEvent<LightOnCollideColliderComponent, ComponentShutdown>(new EntityEventRefHandler<LightOnCollideColliderComponent, ComponentShutdown>(this.OnCollideShutdown));
  }

  private void OnCollideShutdown(
    Entity<LightOnCollideColliderComponent> ent,
    ref ComponentShutdown args)
  {
    if (this.TerminatingOrDeleted(ent.Owner))
      return;
    ContactEnumerator contacts = this._physics.GetContacts((Entity<FixturesComponent>) ent.Owner);
    Contact contact;
    while (contacts.MoveNext(out contact))
    {
      if (contact.IsTouching)
      {
        EntityUid uid = contact.OtherEnt(ent.Owner);
        if (this._lightQuery.HasComp(uid))
          this._physics.RegenerateContacts((Entity<PhysicsComponent>) uid);
      }
    }
  }

  private void OnPreventCollide(
    Entity<LightOnCollideColliderComponent> ent,
    ref PreventCollideEvent args)
  {
    if (this._lightQuery.HasComp(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  private void OnEnd(Entity<LightOnCollideColliderComponent> ent, ref EndCollideEvent args)
  {
    if (args.OurFixtureId != ent.Comp.FixtureId || !this._lightQuery.HasComp(args.OtherEntity) || this._physics.GetTouchingContacts((Entity<FixturesComponent>) args.OtherEntity) - 1 > 0)
      return;
    this._lights.SetEnabled((Entity<SlimPoweredLightComponent>) args.OtherEntity, false);
  }

  private void OnStart(Entity<LightOnCollideColliderComponent> ent, ref StartCollideEvent args)
  {
    if (args.OurFixtureId != ent.Comp.FixtureId || !this._lightQuery.HasComp(args.OtherEntity))
      return;
    this._lights.SetEnabled((Entity<SlimPoweredLightComponent>) args.OtherEntity, true);
  }
}
