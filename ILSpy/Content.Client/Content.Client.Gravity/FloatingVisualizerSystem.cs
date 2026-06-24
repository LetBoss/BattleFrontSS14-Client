using System;
using System.Numerics;
using Content.Shared.Gravity;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Gravity;

public sealed class FloatingVisualizerSystem : SharedFloatingVisualizerSystem
{
	[Dependency]
	private AnimationPlayerSystem AnimationSystem;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FloatingVisualsComponent, AnimationCompletedEvent>((ComponentEventHandler<FloatingVisualsComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
	}

	public override void FloatAnimation(EntityUid uid, Vector2 offset, string animationKey, float animationTime, bool stop = false)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00b9: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (stop)
		{
			AnimationSystem.Stop(Entity<AnimationPlayerComponent>.op_Implicit(uid), animationKey);
			return;
		}
		Animation val = new Animation
		{
			Length = TimeSpan.FromSeconds(animationTime * 2f),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)offset, animationTime, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, animationTime, (Func<float, float>)null)
				}
			} }
		};
		if (!AnimationSystem.HasRunningAnimation(uid, animationKey))
		{
			AnimationSystem.Play(uid, val, animationKey);
		}
	}

	private void OnAnimationCompleted(EntityUid uid, FloatingVisualsComponent component, AnimationCompletedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Key != component.AnimationKey))
		{
			FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, !component.CanFloat);
		}
	}
}
