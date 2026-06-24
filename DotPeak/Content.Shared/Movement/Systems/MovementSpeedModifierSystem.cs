// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.MovementSpeedModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Standing;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class MovementSpeedModifierSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IConfigurationManager _configManager;
  private float _frictionModifier;
  private float _airDamping;
  private float _offGridDamping;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MovementSpeedModifierComponent, MapInitEvent>(new EntityEventRefHandler<MovementSpeedModifierComponent, MapInitEvent>(this.OnModMapInit));
    this.Subs.CVar<float>(this._configManager, CCVars.TileFrictionModifier, (Action<float>) (value => this._frictionModifier = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.AirFriction, (Action<float>) (value => this._airDamping = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.OffgridFriction, (Action<float>) (value => this._offGridDamping = value), true);
  }

  private void OnModMapInit(Entity<MovementSpeedModifierComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.WeightlessAcceleration = ent.Comp.BaseWeightlessAcceleration;
    ent.Comp.WeightlessModifier = ent.Comp.BaseWeightlessModifier;
    ent.Comp.WeightlessFriction = this._airDamping * ent.Comp.BaseWeightlessFriction;
    ent.Comp.WeightlessFrictionNoInput = this._airDamping * ent.Comp.BaseWeightlessFriction;
    ent.Comp.OffGridFriction = new float?(this._offGridDamping * ent.Comp.BaseWeightlessFriction);
    ent.Comp.Acceleration = ent.Comp.BaseAcceleration;
    ent.Comp.Friction = this._frictionModifier * 2.5f;
    ent.Comp.FrictionNoInput = this._frictionModifier * 2.5f;
    this.Dirty<MovementSpeedModifierComponent>(ent);
  }

  public void RefreshWeightlessModifiers(EntityUid uid, MovementSpeedModifierComponent? move = null)
  {
    if (!this.Resolve<MovementSpeedModifierComponent>(uid, ref move, false) || this._timing.ApplyingState)
      return;
    RefreshWeightlessModifiersEvent args = new RefreshWeightlessModifiersEvent()
    {
      WeightlessAcceleration = move.BaseWeightlessAcceleration,
      WeightlessAccelerationMod = 1f,
      WeightlessModifier = move.BaseWeightlessModifier,
      WeightlessFriction = move.BaseWeightlessFriction,
      WeightlessFrictionMod = 1f,
      WeightlessFrictionNoInput = move.BaseWeightlessFriction,
      WeightlessFrictionNoInputMod = 1f
    };
    this.RaiseLocalEvent<RefreshWeightlessModifiersEvent>(uid, ref args);
    if (MathHelper.CloseTo(args.WeightlessAcceleration, move.WeightlessAcceleration, 1E-07f) && MathHelper.CloseTo(args.WeightlessModifier, move.WeightlessModifier, 1E-07f) && MathHelper.CloseTo(args.WeightlessFriction, move.WeightlessFriction, 1E-07f) && MathHelper.CloseTo(args.WeightlessFrictionNoInput, move.WeightlessFrictionNoInput, 1E-07f))
      return;
    move.WeightlessAcceleration = args.WeightlessAcceleration * args.WeightlessAccelerationMod;
    move.WeightlessModifier = args.WeightlessModifier;
    move.WeightlessFriction = this._airDamping * args.WeightlessFriction * args.WeightlessFrictionMod;
    move.WeightlessFrictionNoInput = this._airDamping * args.WeightlessFrictionNoInput * args.WeightlessFrictionNoInputMod;
    this.Dirty(uid, (IComponent) move);
  }

  public void RefreshMovementSpeedModifiers(EntityUid uid, MovementSpeedModifierComponent? move = null)
  {
    if (!this.Resolve<MovementSpeedModifierComponent>(uid, ref move, false) || this._timing.ApplyingState)
      return;
    RefreshMovementSpeedModifiersEvent args = new RefreshMovementSpeedModifiersEvent();
    this.RaiseLocalEvent<RefreshMovementSpeedModifiersEvent>(uid, args);
    RMCRestComponent comp;
    if (this.TryComp<RMCRestComponent>(uid, out comp) && comp.Resting)
    {
      float walk = comp.RestingSpeed;
      if ((double) args.WalkSpeedModifier != 0.0)
        walk = comp.RestingSpeed / args.WalkSpeedModifier;
      float sprint = comp.RestingSpeed;
      if ((double) args.SprintSpeedModifier != 0.0)
        sprint = comp.RestingSpeed / args.SprintSpeedModifier;
      args.ModifySpeed(walk, sprint);
    }
    if (MathHelper.CloseTo(args.WalkSpeedModifier, move.WalkSpeedModifier, 1E-07f) && MathHelper.CloseTo(args.SprintSpeedModifier, move.SprintSpeedModifier, 1E-07f))
      return;
    move.WalkSpeedModifier = args.WalkSpeedModifier;
    move.SprintSpeedModifier = args.SprintSpeedModifier;
    this.Dirty(uid, (IComponent) move);
  }

  public void ChangeBaseSpeed(
    EntityUid uid,
    float baseWalkSpeed,
    float baseSprintSpeed,
    float acceleration,
    MovementSpeedModifierComponent? move = null)
  {
    if (!this.Resolve<MovementSpeedModifierComponent>(uid, ref move, false))
      return;
    move.BaseWalkSpeed = baseWalkSpeed;
    move.BaseSprintSpeed = baseSprintSpeed;
    move.Acceleration = acceleration;
    this.Dirty(uid, (IComponent) move);
  }

  public void RefreshFrictionModifiers(EntityUid uid, MovementSpeedModifierComponent? move = null)
  {
    if (!this.Resolve<MovementSpeedModifierComponent>(uid, ref move, false) || this._timing.ApplyingState)
      return;
    RefreshFrictionModifiersEvent args = new RefreshFrictionModifiersEvent()
    {
      Friction = move.BaseFriction,
      FrictionNoInput = move.BaseFriction,
      Acceleration = move.BaseAcceleration
    };
    this.RaiseLocalEvent<RefreshFrictionModifiersEvent>(uid, ref args);
    if (MathHelper.CloseTo(args.Friction, move.Friction, 1E-07f) && MathHelper.CloseTo(args.FrictionNoInput, move.FrictionNoInput, 1E-07f) && MathHelper.CloseTo(args.Acceleration, move.Acceleration, 1E-07f))
      return;
    move.Friction = this._frictionModifier * args.Friction;
    move.FrictionNoInput = this._frictionModifier * args.FrictionNoInput;
    move.Acceleration = args.Acceleration;
    this.Dirty(uid, (IComponent) move);
  }

  public void ChangeBaseFriction(
    EntityUid uid,
    float friction,
    float frictionNoInput,
    float acceleration,
    MovementSpeedModifierComponent? move = null)
  {
    if (!this.Resolve<MovementSpeedModifierComponent>(uid, ref move, false))
      return;
    move.BaseFriction = friction;
    move.FrictionNoInput = frictionNoInput;
    move.BaseAcceleration = acceleration;
    this.Dirty(uid, (IComponent) move);
  }
}
