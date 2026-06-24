using System;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Light.EntitySystems;

public sealed class RotatingLightSystem : SharedRotatingLightSystem
{
	[Dependency]
	private AnimationPlayerSystem _animations;

	private const string AnimKey = "rotating_light";

	private Animation GetAnimation(float speed)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e2: Expected O, but got Unknown
		float num = 120f / speed;
		return new Animation
		{
			Length = TimeSpan.FromSeconds(360f / speed),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(PointLightComponent),
				InterpolationMode = (AnimationInterpolationMode)0,
				Property = "Rotation",
				KeyFrames = 
				{
					new KeyFrame((object)Angle.Zero, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Angle.FromDegrees(120.0), num, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Angle.FromDegrees(240.0), num, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Angle.FromDegrees(360.0), num, (Func<float, float>)null)
				}
			} }
		};
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RotatingLightComponent, ComponentStartup>((ComponentEventHandler<RotatingLightComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotatingLightComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<RotatingLightComponent, AfterAutoHandleStateEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RotatingLightComponent, AnimationCompletedEvent>((ComponentEventHandler<RotatingLightComponent, AnimationCompletedEvent>)OnAnimationComplete, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, RotatingLightComponent comp, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent player = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
		PlayAnimation(uid, comp, player);
	}

	private void OnAfterAutoHandleState(EntityUid uid, RotatingLightComponent comp, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val))
		{
			if (comp.Enabled)
			{
				PlayAnimation(uid, comp, val);
			}
			else
			{
				_animations.Stop(uid, val, "rotating_light");
			}
		}
	}

	private void OnAnimationComplete(EntityUid uid, RotatingLightComponent comp, AnimationCompletedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Finished)
		{
			PlayAnimation(uid, comp);
		}
	}

	public void PlayAnimation(EntityUid uid, RotatingLightComponent? comp = null, AnimationPlayerComponent? player = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RotatingLightComponent, AnimationPlayerComponent>(uid, ref comp, ref player, true) && comp.Enabled && !_animations.HasRunningAnimation(uid, player, "rotating_light"))
		{
			_animations.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, player)), GetAnimation(comp.Speed), "rotating_light");
		}
	}
}
