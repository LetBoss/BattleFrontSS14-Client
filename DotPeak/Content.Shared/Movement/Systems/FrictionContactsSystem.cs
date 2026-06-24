// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.FrictionContactsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class FrictionContactsSystem : EntitySystem
{
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private MovementSpeedModifierSystem _speedModifierSystem;
  private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _toRemove = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FrictionContactsComponent, StartCollideEvent>(new ComponentEventRefHandler<FrictionContactsComponent, StartCollideEvent>(this.OnEntityEnter));
    this.SubscribeLocalEvent<FrictionContactsComponent, EndCollideEvent>(new ComponentEventRefHandler<FrictionContactsComponent, EndCollideEvent>(this.OnEntityExit));
    this.SubscribeLocalEvent<FrictionModifiedByContactComponent, RefreshFrictionModifiersEvent>(new EntityEventRefHandler<FrictionModifiedByContactComponent, RefreshFrictionModifiersEvent>(this.OnRefreshFrictionModifiers));
    this.SubscribeLocalEvent<FrictionContactsComponent, ComponentShutdown>(new ComponentEventHandler<FrictionContactsComponent, ComponentShutdown>(this.OnShutdown));
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this._toRemove.Clear();
    foreach (EntityUid uid in this._toUpdate)
      this._speedModifierSystem.RefreshFrictionModifiers(uid);
    foreach (EntityUid uid in this._toRemove)
      this.RemComp<FrictionModifiedByContactComponent>(uid);
    this._toUpdate.Clear();
  }

  public void ChangeFrictionModifiers(
    EntityUid uid,
    float friction,
    FrictionContactsComponent? component = null)
  {
    this.ChangeFrictionModifiers(uid, friction, new float?(), new float?(), component);
  }

  public void ChangeFrictionModifiers(
    EntityUid uid,
    float mobFriction,
    float? mobFrictionNoInput,
    float? acceleration,
    FrictionContactsComponent? component = null)
  {
    if (!this.Resolve<FrictionContactsComponent>(uid, ref component))
      return;
    component.MobFriction = mobFriction;
    component.MobFrictionNoInput = mobFrictionNoInput;
    if (acceleration.HasValue)
      component.MobAcceleration = acceleration.Value;
    this.Dirty(uid, (IComponent) component);
    this._toUpdate.UnionWith((IEnumerable<EntityUid>) this._physics.GetContactingEntities(uid));
  }

  private void OnShutdown(
    EntityUid uid,
    FrictionContactsComponent component,
    ComponentShutdown args)
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>(uid, out comp))
      return;
    this._toUpdate.UnionWith((IEnumerable<EntityUid>) this._physics.GetContactingEntities(uid, comp));
  }

  private void OnRefreshFrictionModifiers(
    Entity<FrictionModifiedByContactComponent> entity,
    ref RefreshFrictionModifiersEvent args)
  {
    PhysicsComponent comp1;
    if (!this.TryComp<PhysicsComponent>((EntityUid) entity, out comp1))
      return;
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    bool flag1 = comp1.BodyStatus == BodyStatus.InAir || this._gravity.IsWeightless((EntityUid) entity, comp1);
    bool flag2 = true;
    int num4 = 0;
    foreach (EntityUid contactingEntity in this._physics.GetContactingEntities((EntityUid) entity, comp1))
    {
      FrictionContactsComponent comp2;
      if (this.TryComp<FrictionContactsComponent>(contactingEntity, out comp2) && (!flag1 || comp2.AffectAirborne))
      {
        num1 += comp2.MobFriction;
        num2 += (float) ((double) comp2.MobFrictionNoInput ?? (double) comp2.MobFriction);
        num3 += comp2.MobAcceleration;
        flag2 = false;
        ++num4;
      }
    }
    if (num4 > 0)
    {
      if (!MathHelper.CloseTo(num1, (float) num4, 1E-07f) || !MathHelper.CloseTo(num2, (float) num4, 1E-07f))
      {
        float friction = num1 / (float) num4;
        float noInput = num2 / (float) num4;
        args.ModifyFriction(friction, noInput);
      }
      if (!MathHelper.CloseTo(num3, (float) num4, 1E-07f))
      {
        float acceleration = num3 / (float) num4;
        args.ModifyAcceleration(acceleration);
      }
    }
    if (!flag2)
      return;
    this._toRemove.Add((EntityUid) entity);
  }

  private void OnEntityExit(
    EntityUid uid,
    FrictionContactsComponent component,
    ref EndCollideEvent args)
  {
    this._toUpdate.Add(args.OtherEntity);
  }

  private void OnEntityEnter(
    EntityUid uid,
    FrictionContactsComponent component,
    ref StartCollideEvent args)
  {
    this.AddModifiedEntity(args.OtherEntity);
  }

  public void AddModifiedEntity(EntityUid uid)
  {
    if (!this.HasComp<MovementSpeedModifierComponent>(uid))
      return;
    this.EnsureComp<FrictionModifiedByContactComponent>(uid);
    this._toUpdate.Add(uid);
  }
}
