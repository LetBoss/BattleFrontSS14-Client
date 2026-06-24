// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Gibbing.RMCGibSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Events;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Gibbing;

public sealed class RMCGibSystem : EntitySystem
{
  private const float ItemLaunchImpulse = 8f;
  private const float ItemLaunchImpulseVariance = 3f;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private MobThresholdSystem _thresholds;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCSpawnEntitiesOnGibComponent, BeingGibbedEvent>(new EntityEventRefHandler<RMCSpawnEntitiesOnGibComponent, BeingGibbedEvent>(this.OnGibbed));
    this.SubscribeLocalEvent<RMCGibOnDeathComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCGibOnDeathComponent, MobStateChangedEvent>(this.OnDeath));
  }

  private void OnGibbed(Entity<RMCSpawnEntitiesOnGibComponent> ent, ref BeingGibbedEvent args)
  {
    if (this._net.IsClient)
      return;
    foreach (EntProtoId entity in ent.Comp.Entities)
      this._transform.AttachToGridOrMap(this.Spawn((string) entity, this._transform.GetMoverCoordinates((EntityUid) ent)));
  }

  private void OnDeath(Entity<RMCGibOnDeathComponent> ent, ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    float gibChance = ent.Comp.GibChance;
    MobThresholdsComponent comp1;
    DamageableComponent comp2;
    if (this.TryComp<MobThresholdsComponent>((EntityUid) ent, out comp1) && this.TryComp<DamageableComponent>((EntityUid) ent, out comp2))
    {
      FixedPoint2 total = comp2.Damage.GetTotal();
      FixedPoint2 thresholdForState = this._thresholds.GetThresholdForState((EntityUid) ent, MobState.Dead, comp1);
      gibChance += (float) (total - thresholdForState) * ent.Comp.DamageGibMultiplier;
    }
    if ((double) this._random.NextFloat() > (double) gibChance || this._net.IsClient)
      return;
    this._body.GibBody((EntityUid) ent, ent.Comp.DropOrgans, splatCone: new Angle());
  }

  public void ScatterInventoryItems(
    EntityUid target,
    float? launchImpulse = null,
    float? launchImpulseVariance = null)
  {
    if (!this.TryComp<InventoryComponent>(target, out InventoryComponent _))
      return;
    float num = launchImpulse ?? 8f;
    float maxValue = launchImpulseVariance ?? 3f;
    TransformComponent transformComponent = this.Transform(target);
    foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities((Entity<HandsComponent, InventoryComponent>) target))
    {
      this._transform.DropNextTo((Entity<TransformComponent>) orInventoryEntity, (Entity<TransformComponent>) (target, transformComponent));
      Angle angle = this._random.NextAngle();
      Vector2 impulse = ((Angle) ref angle).ToVec() * (num + this._random.NextFloat(maxValue));
      this._physics.ApplyLinearImpulse(orInventoryEntity, impulse);
      this._transform.SetWorldRotation(orInventoryEntity, this._random.NextAngle());
    }
  }
}
