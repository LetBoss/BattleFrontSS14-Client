// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Slipify
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Slippery;
using Content.Shared.StepTrigger.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class Slipify : EntityEffect, ISerializationGenerated<Slipify>, ISerializationGenerated
{
  public override void Effect(EntityEffectBaseArgs args)
  {
    FixtureSystem fixtureSystem = args.EntityManager.System<FixtureSystem>();
    CollisionWakeSystem collisionWakeSystem = args.EntityManager.System<CollisionWakeSystem>();
    SlipperyComponent slipperyComponent = args.EntityManager.EnsureComponent<SlipperyComponent>(args.TargetEntity);
    args.EntityManager.Dirty(args.TargetEntity, (IComponent) slipperyComponent);
    args.EntityManager.EnsureComponent<StepTriggerComponent>(args.TargetEntity);
    FixturesComponent fixturesComponent = args.EntityManager.EnsureComponent<FixturesComponent>(args.TargetEntity);
    PhysicsComponent physicsComponent = args.EntityManager.EnsureComponent<PhysicsComponent>(args.TargetEntity);
    IPhysShape shape1 = fixturesComponent.Fixtures["fix1"].Shape;
    EntityUid targetEntity = args.TargetEntity;
    IPhysShape shape2 = shape1;
    FixturesComponent manager = fixturesComponent;
    PhysicsComponent body = physicsComponent;
    fixtureSystem.TryCreateFixture(targetEntity, shape2, "slips", hard: false, collisionLayer: 20, manager: manager, body: body);
    CollisionWakeComponent component = args.EntityManager.EnsureComponent<CollisionWakeComponent>(args.TargetEntity);
    collisionWakeSystem.SetEnabled(args.TargetEntity, false, component);
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    throw new NotImplementedException();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Slipify target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Slipify) target1;
    serialization.TryCustomCopy<Slipify>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Slipify target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Slipify target1 = (Slipify) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Slipify target1 = (Slipify) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Slipify EntityEffect.Instantiate() => new Slipify();
}
