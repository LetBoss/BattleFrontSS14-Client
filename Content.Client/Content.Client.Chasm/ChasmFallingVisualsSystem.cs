using System;
using Content.Shared.Chasm;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Chasm;

public sealed class ChasmFallingVisualsSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _anim;

	[Dependency]
	private SpriteSystem _sprite;

	private readonly string _chasmFallAnimationKey = "chasm_fall";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChasmFallingComponent, ComponentInit>((ComponentEventHandler<ChasmFallingComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChasmFallingComponent, ComponentRemove>((ComponentEventHandler<ChasmFallingComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, ChasmFallingComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
		{
			component.OriginalScale = val.Scale;
			AnimationPlayerComponent val2 = default(AnimationPlayerComponent);
			if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val2) && !_anim.HasRunningAnimation(val2, _chasmFallAnimationKey))
			{
				_anim.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val2)), GetFallingAnimation(component), _chasmFallAnimationKey);
			}
		}
	}

	private void OnComponentRemove(EntityUid uid, ChasmFallingComponent component, ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
		{
			_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, item)), component.OriginalScale);
			AnimationPlayerComponent val = default(AnimationPlayerComponent);
			if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val) && _anim.HasRunningAnimation(val, _chasmFallAnimationKey))
			{
				_anim.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), _chasmFallAnimationKey);
			}
		}
	}

	private Animation GetFallingAnimation(ChasmFallingComponent component)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Expected O, but got Unknown
		TimeSpan animationTime = component.AnimationTime;
		return new Animation
		{
			Length = animationTime,
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Scale",
				KeyFrames = 
				{
					new KeyFrame((object)component.OriginalScale, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)component.AnimationScale, (float)animationTime.Seconds, (Func<float, float>)null)
				},
				InterpolationMode = (AnimationInterpolationMode)1
			} }
		};
	}
}
