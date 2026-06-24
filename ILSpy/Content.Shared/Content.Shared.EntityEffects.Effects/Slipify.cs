using System;
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

namespace Content.Shared.EntityEffects.Effects;

public sealed class Slipify : EntityEffect, ISerializationGenerated<Slipify>, ISerializationGenerated
{
	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		FixtureSystem obj = args.EntityManager.System<FixtureSystem>();
		CollisionWakeSystem colWakeSystem = args.EntityManager.System<CollisionWakeSystem>();
		SlipperyComponent slippery = args.EntityManager.EnsureComponent<SlipperyComponent>(args.TargetEntity);
		args.EntityManager.Dirty(args.TargetEntity, (IComponent)(object)slippery, (MetaDataComponent)null);
		args.EntityManager.EnsureComponent<StepTriggerComponent>(args.TargetEntity);
		FixturesComponent fixtures = args.EntityManager.EnsureComponent<FixturesComponent>(args.TargetEntity);
		PhysicsComponent body = args.EntityManager.EnsureComponent<PhysicsComponent>(args.TargetEntity);
		IPhysShape shape = fixtures.Fixtures["fix1"].Shape;
		obj.TryCreateFixture(args.TargetEntity, shape, "slips", 1f, false, 20, 0, 0.4f, 0f, true, fixtures, body, (TransformComponent)null);
		CollisionWakeComponent collisionWake = args.EntityManager.EnsureComponent<CollisionWakeComponent>(args.TargetEntity);
		colWakeSystem.SetEnabled(args.TargetEntity, false, collisionWake);
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		throw new NotImplementedException();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Slipify target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Slipify)definitionCast;
		serialization.TryCustomCopy<Slipify>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Slipify target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Slipify cast = (Slipify)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Slipify cast = (Slipify)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Slipify Instantiate()
	{
		return new Slipify();
	}
}
