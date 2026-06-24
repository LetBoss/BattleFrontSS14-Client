// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SpeedModifierContactsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Water;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Slippery;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class SpeedModifierContactsSystem : EntitySystem
{
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private MovementSpeedModifierSystem _speedModifierSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private RMCWaterSystem _rmcWater;
  private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _toRemove = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SpeedModifierContactsComponent, StartCollideEvent>(new ComponentEventRefHandler<SpeedModifierContactsComponent, StartCollideEvent>(this.OnEntityEnter));
    this.SubscribeLocalEvent<SpeedModifierContactsComponent, EndCollideEvent>(new ComponentEventRefHandler<SpeedModifierContactsComponent, EndCollideEvent>(this.OnEntityExit));
    this.SubscribeLocalEvent<SpeedModifiedByContactComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SpeedModifiedByContactComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovementSpeedModifiers));
    this.SubscribeLocalEvent<SpeedModifierContactsComponent, ComponentShutdown>(new ComponentEventHandler<SpeedModifierContactsComponent, ComponentShutdown>(this.OnShutdown));
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this._toRemove.Clear();
    foreach (EntityUid uid in this._toUpdate)
      this._speedModifierSystem.RefreshMovementSpeedModifiers(uid);
    foreach (EntityUid uid in this._toRemove)
      this.RemComp<SpeedModifiedByContactComponent>(uid);
    this._toUpdate.Clear();
  }

  public void ChangeSpeedModifiers(
    EntityUid uid,
    float speed,
    SpeedModifierContactsComponent? component = null)
  {
    this.ChangeSpeedModifiers(uid, speed, speed, component);
  }

  public void ChangeSpeedModifiers(
    EntityUid uid,
    float walkSpeed,
    float sprintSpeed,
    SpeedModifierContactsComponent? component = null)
  {
    if (!this.Resolve<SpeedModifierContactsComponent>(uid, ref component))
      return;
    component.WalkSpeedModifier = walkSpeed;
    component.SprintSpeedModifier = sprintSpeed;
    this.Dirty(uid, (IComponent) component);
    this._toUpdate.UnionWith((IEnumerable<EntityUid>) this._physics.GetContactingEntities(uid));
  }

  private void OnShutdown(
    EntityUid uid,
    SpeedModifierContactsComponent component,
    ComponentShutdown args)
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>(uid, out comp))
      return;
    this._toUpdate.UnionWith((IEnumerable<EntityUid>) this._physics.GetContactingEntities(uid, comp));
  }

  private void OnRefreshMovementSpeedModifiers(
    EntityUid uid,
    SpeedModifiedByContactComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    PhysicsComponent comp1;
    if (!this.TryComp<PhysicsComponent>(uid, out comp1))
      return;
    float num1 = 0.0f;
    float num2 = 0.0f;
    bool flag1 = comp1.BodyStatus == BodyStatus.InAir || this._gravity.IsWeightless(uid, comp1);
    bool flag2 = true;
    int num3 = 0;
    foreach (EntityUid contactingEntity in this._physics.GetContactingEntities(uid, comp1))
    {
      bool flag3 = false;
      SpeedModifierContactsComponent comp2;
      if (this.TryComp<SpeedModifierContactsComponent>(contactingEntity, out comp2))
      {
        if (!this._whitelistSystem.IsWhitelistPass(comp2.IgnoreWhitelist, uid) && (!flag1 || comp2.AffectAirborne))
        {
          num1 += comp2.WalkSpeedModifier;
          num2 += comp2.SprintSpeedModifier;
          flag3 = true;
        }
        else
          continue;
      }
      if (this.HasComp<SlipperyComponent>(contactingEntity) && !flag3)
      {
        GetSlowedOverSlipperyModifierEvent args1 = new GetSlowedOverSlipperyModifierEvent();
        this.RaiseLocalEvent<GetSlowedOverSlipperyModifierEvent>(uid, ref args1);
        if (!MathHelper.CloseTo(args1.SlowdownModifier, 1f, 1E-07f))
        {
          num1 += args1.SlowdownModifier;
          num2 += args1.SlowdownModifier;
          flag3 = true;
        }
      }
      if (flag3)
      {
        flag2 = false;
        ++num3;
      }
    }
    if (num3 > 0 && (!MathHelper.CloseTo(num1, (float) num3, 1E-07f) || !MathHelper.CloseTo(num2, (float) num3, 1E-07f)))
    {
      float x1 = num1 / (float) num3;
      float x2 = num2 / (float) num3;
      GetSpeedModifierContactCapEvent args2 = new GetSpeedModifierContactCapEvent();
      this.RaiseLocalEvent<GetSpeedModifierContactCapEvent>(uid, ref args2);
      float walk = MathF.Max(x1, args2.MaxWalkSlowdown);
      float sprint = MathF.Max(x2, args2.MaxSprintSlowdown);
      args.ModifySpeed(walk, sprint);
    }
    if (!flag2)
      return;
    this._toRemove.Add(uid);
  }

  private void OnEntityExit(
    EntityUid uid,
    SpeedModifierContactsComponent component,
    ref EndCollideEvent args)
  {
    this._toUpdate.Add(args.OtherEntity);
  }

  private void OnEntityEnter(
    EntityUid uid,
    SpeedModifierContactsComponent component,
    ref StartCollideEvent args)
  {
    if (!this._rmcWater.CanCollide((Entity<RMCWaterComponent>) uid, args.OtherEntity))
      return;
    this.AddModifiedEntity(args.OtherEntity);
  }

  public void AddModifiedEntity(EntityUid uid)
  {
    if (!this.HasComp<MovementSpeedModifierComponent>(uid))
      return;
    this.EnsureComp<SpeedModifiedByContactComponent>(uid);
    this._toUpdate.Add(uid);
  }
}
