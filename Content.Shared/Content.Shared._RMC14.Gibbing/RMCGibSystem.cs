using System;
using System.Numerics;
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
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

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
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCSpawnEntitiesOnGibComponent, BeingGibbedEvent>((EntityEventRefHandler<RMCSpawnEntitiesOnGibComponent, BeingGibbedEvent>)OnGibbed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCGibOnDeathComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCGibOnDeathComponent, MobStateChangedEvent>)OnDeath, (Type[])null, (Type[])null);
	}

	private void OnGibbed(Entity<RMCSpawnEntitiesOnGibComponent> ent, ref BeingGibbedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		foreach (EntProtoId protoId in ent.Comp.Entities)
		{
			EntityCoordinates position = _transform.GetMoverCoordinates(Entity<RMCSpawnEntitiesOnGibComponent>.op_Implicit(ent));
			EntityUid newEntity = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(protoId), position);
			_transform.AttachToGridOrMap(newEntity, (TransformComponent)null);
		}
	}

	private void OnDeath(Entity<RMCGibOnDeathComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			float gibProbability = ent.Comp.GibChance;
			MobThresholdsComponent thresholds = default(MobThresholdsComponent);
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<RMCGibOnDeathComponent>.op_Implicit(ent), ref thresholds) && ((EntitySystem)this).TryComp<DamageableComponent>(Entity<RMCGibOnDeathComponent>.op_Implicit(ent), ref damageable))
			{
				FixedPoint2 damage = damageable.Damage.GetTotal();
				FixedPoint2 dead = _thresholds.GetThresholdForState(Entity<RMCGibOnDeathComponent>.op_Implicit(ent), MobState.Dead, thresholds);
				gibProbability += (float)(damage - dead) * ent.Comp.DamageGibMultiplier;
			}
			if (!(_random.NextFloat() > gibProbability) && !_net.IsClient)
			{
				_body.GibBody(Entity<RMCGibOnDeathComponent>.op_Implicit(ent), ent.Comp.DropOrgans);
			}
		}
	}

	public void ScatterInventoryItems(EntityUid target, float? launchImpulse = null, float? launchImpulseVariance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inventory = default(InventoryComponent);
		if (!((EntitySystem)this).TryComp<InventoryComponent>(target, ref inventory))
		{
			return;
		}
		float impulse = launchImpulse ?? 8f;
		float impulseVariance = launchImpulseVariance ?? 3f;
		TransformComponent targetTransform = ((EntitySystem)this).Transform(target);
		foreach (EntityUid item in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(target)))
		{
			_transform.DropNextTo(Entity<TransformComponent>.op_Implicit(item), Entity<TransformComponent>.op_Implicit((target, targetTransform)));
			Angle scatterAngle = _random.NextAngle();
			Vector2 scatterVector = ((Angle)(ref scatterAngle)).ToVec() * (impulse + _random.NextFloat(impulseVariance));
			_physics.ApplyLinearImpulse(item, scatterVector, (FixturesComponent)null, (PhysicsComponent)null);
			_transform.SetWorldRotation(item, _random.NextAngle());
		}
	}
}
