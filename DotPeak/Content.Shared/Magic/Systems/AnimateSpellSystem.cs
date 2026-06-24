// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Systems.AnimateSpellSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Magic.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Magic.Systems;

public sealed class AnimateSpellSystem : EntitySystem
{
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedContainerSystem _container;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AnimateComponent, MapInitEvent>(new EntityEventRefHandler<AnimateComponent, MapInitEvent>(this.OnAnimate));
  }

  private void OnAnimate(Entity<AnimateComponent> ent, ref MapInitEvent args)
  {
    FixturesComponent comp1;
    PhysicsComponent comp2;
    if (!this.TryComp<FixturesComponent>((EntityUid) ent, out comp1) || !this.TryComp<PhysicsComponent>((EntityUid) ent, out comp2))
      return;
    TransformComponent xform = this.Transform((EntityUid) ent);
    KeyValuePair<string, Fixture> keyValuePair = comp1.Fixtures.First<KeyValuePair<string, Fixture>>();
    this._transform.Unanchor((EntityUid) ent);
    this._physics.SetCanCollide((EntityUid) ent, true, manager: comp1, body: comp2);
    this._physics.SetCollisionMask((EntityUid) ent, keyValuePair.Key, keyValuePair.Value, 10, comp1, comp2);
    this._physics.SetCollisionLayer((EntityUid) ent, keyValuePair.Key, keyValuePair.Value, 65, comp1, comp2);
    this._physics.SetBodyType((EntityUid) ent, BodyType.KinematicController, comp1, comp2, xform);
    this._physics.SetBodyStatus((EntityUid) ent, comp2, BodyStatus.InAir);
    this._physics.SetFixedRotation((EntityUid) ent, false, manager: comp1, body: comp2);
    this._physics.SetHard((EntityUid) ent, keyValuePair.Value, true, comp1);
    this._container.AttachParentToContainerOrGrid((Entity<TransformComponent>) ((EntityUid) ent, xform));
    AnimateSpellEvent args1 = new AnimateSpellEvent();
    this.RaiseLocalEvent<AnimateSpellEvent>((EntityUid) ent, ref args1);
  }
}
