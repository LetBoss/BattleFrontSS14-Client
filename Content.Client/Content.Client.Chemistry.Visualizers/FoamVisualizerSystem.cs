using System;
using Content.Shared.Chemistry.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Chemistry.Visualizers;

public sealed class FoamVisualizerSystem : VisualizerSystem<FoamVisualsComponent>
{
	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FoamVisualsComponent, ComponentInit>((ComponentEventHandler<FoamVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoamVisualsComponent, AnimationCompletedEvent>((ComponentEventHandler<FoamVisualsComponent, AnimationCompletedEvent>)OnAnimationComplete, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<FoamVisualsComponent, SmokeComponent> val = ((EntitySystem)this).EntityQueryEnumerator<FoamVisualsComponent, SmokeComponent>();
		EntityUid val2 = default(EntityUid);
		FoamVisualsComponent foamVisualsComponent = default(FoamVisualsComponent);
		SmokeComponent smokeComponent = default(SmokeComponent);
		AnimationPlayerComponent val3 = default(AnimationPlayerComponent);
		while (val.MoveNext(ref val2, ref foamVisualsComponent, ref smokeComponent))
		{
			if (!(_timing.CurTime < foamVisualsComponent.StartTime + TimeSpan.FromSeconds(smokeComponent.Duration) - TimeSpan.FromSeconds(foamVisualsComponent.AnimationTime)) && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(val2, ref val3) && !base.AnimationSystem.HasRunningAnimation(val2, val3, "foamdissolve_animation"))
			{
				base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((val2, val3)), foamVisualsComponent.Animation, "foamdissolve_animation");
			}
		}
	}

	private void OnComponentInit(EntityUid uid, FoamVisualsComponent comp, ComponentInit args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_006a: Expected O, but got Unknown
		comp.StartTime = _timing.CurTime;
		comp.Animation = new Animation
		{
			Length = TimeSpan.FromSeconds(comp.AnimationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = FoamVisualLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.AnimationState), 0f)
				}
			} }
		};
	}

	private void OnAnimationComplete(EntityUid uid, FoamVisualsComponent component, AnimationCompletedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!(args.Key != "foamdissolve_animation") && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			base.SpriteSystem.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), false);
		}
	}
}
