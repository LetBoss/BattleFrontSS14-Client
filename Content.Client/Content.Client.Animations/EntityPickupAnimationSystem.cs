using System;
using System.Numerics;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Spawners;

namespace Content.Client.Animations;

public sealed class EntityPickupAnimationSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _animations;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private TransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EntityPickupAnimationComponent, AnimationCompletedEvent>((ComponentEventHandler<EntityPickupAnimationComponent, AnimationCompletedEvent>)OnEntityPickupAnimationCompleted, (Type[])null, (Type[])null);
	}

	private void OnEntityPickupAnimationCompleted(EntityUid uid, EntityPickupAnimationComponent component, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Del((EntityUid?)uid);
	}

	public void AnimateEntityPickup(EntityUid uid, EntityCoordinates initial, Vector2 final, Angle initialAngle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0199: Expected O, but got Unknown
		if (((EntitySystem)this).Deleted(uid, (MetaDataComponent)null) || !((EntityCoordinates)(ref initial)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return;
		}
		MetaDataComponent val = ((EntitySystem)this).MetaData(uid);
		if (!((EntitySystem)this).IsPaused((EntityUid?)uid, val))
		{
			EntityUid val2 = ((EntitySystem)this).Spawn("clientsideclone", initial);
			((EntitySystem)this).EnsureComp<EntityPickupAnimationComponent>(val2);
			string entityName = val.EntityName;
			_metaData.SetEntityName(val2, entityName, (MetaDataComponent)null, true);
			SpriteComponent item = default(SpriteComponent);
			if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
			{
				((EntitySystem)this).Log.Error("Entity ({0}) couldn't be animated for pickup since it doesn't have a {1}!", new object[2] { val.EntityName, "SpriteComponent" });
				return;
			}
			SpriteComponent item2 = ((EntitySystem)this).Comp<SpriteComponent>(val2);
			_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((uid, item)), Entity<SpriteComponent>.op_Implicit((val2, item2)));
			_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((val2, item2)), true);
			AnimationPlayerComponent val3 = ((EntitySystem)this).Comp<AnimationPlayerComponent>(val2);
			((EntitySystem)this).EnsureComp<TimedDespawnComponent>(val2).Lifetime = 0.25f;
			((SharedTransformSystem)_transform).SetLocalRotationNoLerp(val2, initialAngle, (TransformComponent)null);
			_animations.Play(new Entity<AnimationPlayerComponent>(val2, val3), new Animation
			{
				Length = TimeSpan.FromMilliseconds(125L),
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					ComponentType = typeof(TransformComponent),
					Property = "LocalPosition",
					InterpolationMode = (AnimationInterpolationMode)0,
					KeyFrames = 
					{
						new KeyFrame((object)initial.Position, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)final, 0.125f, (Func<float, float>)null)
					}
				} }
			}, "fancy_pickup_anim");
		}
	}
}
