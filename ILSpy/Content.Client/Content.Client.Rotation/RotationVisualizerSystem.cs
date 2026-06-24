using System;
using Content.Shared.Rotation;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Rotation;

public sealed class RotationVisualizerSystem : SharedRotationVisualsSystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private AnimationPlayerSystem _animation;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RotationVisualsComponent, AppearanceChangeEvent>((ComponentEventRefHandler<RotationVisualsComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, RotationVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		RotationState rotationState = default(RotationState);
		if (args.Sprite != null && ((SharedAppearanceSystem)_appearance).TryGetData<RotationState>(uid, (Enum)RotationVisuals.RotationState, ref rotationState, args.Component))
		{
			switch (rotationState)
			{
			case RotationState.Vertical:
				AnimateSpriteRotation(uid, args.Sprite, component.VerticalRotation, component.AnimationTime);
				break;
			case RotationState.Horizontal:
				AnimateSpriteRotation(uid, args.Sprite, component.HorizontalRotation, component.AnimationTime);
				break;
			}
		}
	}

	public void AnimateSpriteRotation(EntityUid uid, SpriteComponent spriteComp, Angle rotation, float animationTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00c9: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Angle rotation2 = spriteComp.Rotation;
		if (!((Angle)(ref rotation2)).Equals(rotation))
		{
			AnimationPlayerComponent val = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
			if (_animation.HasRunningAnimation(val, "rotate"))
			{
				_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), "rotate");
			}
			Animation val2 = new Animation
			{
				Length = TimeSpan.FromSeconds(animationTime),
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					ComponentType = typeof(SpriteComponent),
					Property = "Rotation",
					InterpolationMode = (AnimationInterpolationMode)0,
					KeyFrames = 
					{
						new KeyFrame((object)spriteComp.Rotation, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)rotation, animationTime, (Func<float, float>)null)
					}
				} }
			};
			_animation.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), val2, "rotate");
		}
	}
}
