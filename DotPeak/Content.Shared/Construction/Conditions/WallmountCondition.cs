// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.WallmountCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class WallmountCondition : 
  IConstructionCondition,
  ISerializationGenerated<WallmountCondition>,
  ISerializationGenerated
{
  private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
    SharedTransformSystem sharedTransformSystem = ientityManager.System<SharedTransformSystem>();
    Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(user);
    Vector2 position = sharedTransformSystem.ToMapCoordinates(location, true).Position;
    Vector2 vector2_1 = position - worldPosition;
    Angle worldRotation = sharedTransformSystem.GetWorldRotation(location.EntityId);
    ref Angle local1 = ref worldRotation;
    Vector2 vec = DirectionExtensions.ToVec(direction);
    ref Vector2 local2 = ref vec;
    Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
    if ((double) Vector2.Dot(Vector2Helpers.Normalized(vector2_2), Vector2Helpers.Normalized(vector2_1)) > 0.0)
      return false;
    SharedPhysicsSystem sharedPhysicsSystem = ientityManager.System<SharedPhysicsSystem>();
    CollisionRay collisionRay1;
    // ISSUE: explicit constructor call
    ((CollisionRay) ref collisionRay1).\u002Ector(worldPosition, Vector2Helpers.Normalized(vector2_1), 2);
    float num = vector2_1.Length();
    TagSystem tagSystem = ientityManager.System<TagSystem>();
    RayCastResults? targetWall = Extensions.FirstOrNull<RayCastResults>(sharedPhysicsSystem.IntersectRayWithPredicate(ientityManager.GetComponent<TransformComponent>(user).MapID, collisionRay1, num, (Func<EntityUid, bool>) (e => !tagSystem.HasTag(e, WallmountCondition.WallTag)), true));
    if (!targetWall.HasValue)
      return false;
    CollisionRay collisionRay2;
    // ISSUE: explicit constructor call
    ((CollisionRay) ref collisionRay2).\u002Ector(position, Vector2Helpers.Normalized(vector2_2), 2);
    return !sharedPhysicsSystem.IntersectRayWithPredicate(ientityManager.GetComponent<TransformComponent>(user).MapID, collisionRay2, 0.5f, (Func<EntityUid, bool>) (e =>
    {
      EntityUid entityUid = e;
      RayCastResults rayCastResults = targetWall.Value;
      EntityUid hitEntity = ((RayCastResults) ref rayCastResults).HitEntity;
      return EntityUid.op_Equality(entityUid, hitEntity) || !tagSystem.HasTag(e, WallmountCondition.WallTag);
    }), true).Any<RayCastResults>();
  }

  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = "construction-step-condition-wallmount"
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WallmountCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<WallmountCondition>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WallmountCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WallmountCondition target1 = (WallmountCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public WallmountCondition Instantiate() => new WallmountCondition();
}
