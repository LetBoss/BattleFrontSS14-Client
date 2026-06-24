using System;
using Content.Shared._RMC14.Sentry;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Sentry;

public sealed class SentryAnimationVisualizer : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	private const string AnimationKey = "rmc_sentry_deploy";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SentrySpikesComponent, AppearanceChangeEvent>((EntityEventRefHandler<SentrySpikesComponent, AppearanceChangeEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
	}

	private void OnAfterAutoHandleState(Entity<SentrySpikesComponent> spikes, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a5: Expected O, but got Unknown
		SentryComponent sentryComponent = default(SentryComponent);
		if (((EntitySystem)this).TryComp<SentryComponent>(Entity<SentrySpikesComponent>.op_Implicit(spikes), ref sentryComponent) && sentryComponent.Mode == SentryMode.On && !_animation.HasRunningAnimation(Entity<SentrySpikesComponent>.op_Implicit(spikes), "rmc_sentry_deploy"))
		{
			_animation.Play(Entity<SentrySpikesComponent>.op_Implicit(spikes), new Animation
			{
				Length = spikes.Comp.AnimationTime,
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = spikes.Comp.Layer,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(spikes.Comp.AnimationState), 0f)
					}
				} }
			}, "rmc_sentry_deploy");
		}
	}
}
