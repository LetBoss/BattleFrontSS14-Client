using System;
using Content.Shared.Trigger;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Trigger;

public sealed class TimerTriggerVisualizerSystem : VisualizerSystem<TimerTriggerVisualsComponent>
{
	[Dependency]
	private SharedAudioSystem _audioSystem;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TimerTriggerVisualsComponent, ComponentInit>((ComponentEventHandler<TimerTriggerVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, TimerTriggerVisualsComponent comp, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0052: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		comp.PrimingAnimation = new Animation
		{
			Length = TimeSpan.MaxValue,
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = TriggerVisualLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.PrimingSprite), 0f)
				}
			} }
		};
		if (comp.PrimingSound != null)
		{
			comp.PrimingAnimation.AnimationTracks.Add((AnimationTrack)new AnimationTrackPlaySound
			{
				KeyFrames = 
				{
					new KeyFrame(_audioSystem.ResolveSound(comp.PrimingSound), 0f, (Func<AudioParams>)null)
				}
			});
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, TimerTriggerVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (args.Sprite == null || !((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val))
		{
			return;
		}
		TriggerVisualState triggerVisualState = default(TriggerVisualState);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<TriggerVisualState>(uid, (Enum)TriggerVisuals.VisualState, ref triggerVisualState, args.Component))
		{
			triggerVisualState = TriggerVisualState.Unprimed;
		}
		switch (triggerVisualState)
		{
		case TriggerVisualState.Primed:
			if (!base.AnimationSystem.HasRunningAnimation(uid, val, "priming_animation"))
			{
				base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), comp.PrimingAnimation, "priming_animation");
			}
			break;
		case TriggerVisualState.Unprimed:
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)TriggerVisualLayers.Base, StateId.op_Implicit(comp.UnprimedSprite));
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
