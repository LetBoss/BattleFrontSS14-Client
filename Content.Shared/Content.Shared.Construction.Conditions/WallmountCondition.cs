using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class WallmountCondition : IConstructionCondition, ISerializationGenerated<WallmountCondition>, ISerializationGenerated
{
	private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");

	public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
		SharedTransformSystem obj = entManager.System<SharedTransformSystem>();
		Vector2 userWorldPosition = obj.GetWorldPosition(user);
		Vector2 objWorldPosition = obj.ToMapCoordinates(location, true).Position;
		Vector2 userToObject = objWorldPosition - userWorldPosition;
		Angle gridRotation = obj.GetWorldRotation(location.EntityId);
		Vector2 vector = DirectionExtensions.ToVec(direction);
		Vector2 directionWithOffset = ((Angle)(ref gridRotation)).RotateVec(ref vector);
		if (Vector2.Dot(Vector2Helpers.Normalized(directionWithOffset), Vector2Helpers.Normalized(userToObject)) > 0f)
		{
			return false;
		}
		SharedPhysicsSystem physics = entManager.System<SharedPhysicsSystem>();
		CollisionRay rUserToObj = default(CollisionRay);
		((CollisionRay)(ref rUserToObj))._002Ector(userWorldPosition, Vector2Helpers.Normalized(userToObject), 2);
		float length = userToObject.Length();
		TagSystem tagSystem = entManager.System<TagSystem>();
		IEnumerable<RayCastResults> userToObjRaycastResults = physics.IntersectRayWithPredicate(entManager.GetComponent<TransformComponent>(user).MapID, rUserToObj, length, (Func<EntityUid, bool>)((EntityUid e) => !tagSystem.HasTag(e, WallTag)), true);
		RayCastResults? targetWall = Extensions.FirstOrNull<RayCastResults>(userToObjRaycastResults);
		if (!targetWall.HasValue)
		{
			return false;
		}
		CollisionRay rAdjWall = default(CollisionRay);
		((CollisionRay)(ref rAdjWall))._002Ector(objWorldPosition, Vector2Helpers.Normalized(directionWithOffset), 2);
		return !physics.IntersectRayWithPredicate(entManager.GetComponent<TransformComponent>(user).MapID, rAdjWall, 0.5f, (Func<EntityUid, bool>)delegate(EntityUid e)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			RayCastResults value = targetWall.Value;
			return e == ((RayCastResults)(ref value)).HitEntity || !tagSystem.HasTag(e, WallTag);
		}, true).Any();
	}

	public ConstructionGuideEntry GenerateGuideEntry()
	{
		return new ConstructionGuideEntry
		{
			Localization = "construction-step-condition-wallmount"
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WallmountCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<WallmountCondition>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WallmountCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WallmountCondition cast = (WallmountCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public WallmountCondition Instantiate()
	{
		return new WallmountCondition();
	}
}
