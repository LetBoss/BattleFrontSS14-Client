using System;
using Content.Shared.Singularity.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Singularity.Visualizers;

public sealed class RadiationCollectorSystem : VisualizerSystem<RadiationCollectorComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RadiationCollectorComponent, ComponentInit>((ComponentEventHandler<RadiationCollectorComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RadiationCollectorComponent, AnimationCompletedEvent>((ComponentEventHandler<RadiationCollectorComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, RadiationCollectorComponent comp, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_005b: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b6: Expected O, but got Unknown
		comp.ActivateAnimation = new Animation
		{
			Length = TimeSpan.FromSeconds(0.800000011920929),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = RadiationCollectorVisualLayers.Main,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.ActivatingState), 0f)
				}
			} }
		};
		comp.DeactiveAnimation = new Animation
		{
			Length = TimeSpan.FromSeconds(0.800000011920929),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = RadiationCollectorVisualLayers.Main,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.DeactivatingState), 0f)
				}
			} }
		};
	}

	private void UpdateVisuals(EntityUid uid, RadiationCollectorVisualState state, RadiationCollectorComponent comp, SpriteComponent sprite, AnimationPlayerComponent? animPlayer = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (state != comp.CurrentState && ((EntitySystem)this).Resolve<AnimationPlayerComponent>(uid, ref animPlayer, true) && !base.AnimationSystem.HasRunningAnimation(uid, animPlayer, "radiationcollector_animation"))
		{
			RadiationCollectorVisualState radiationCollectorVisualState = state & RadiationCollectorVisualState.Active;
			RadiationCollectorVisualState radiationCollectorVisualState2 = comp.CurrentState & RadiationCollectorVisualState.Active;
			if (radiationCollectorVisualState != radiationCollectorVisualState2)
			{
				radiationCollectorVisualState |= RadiationCollectorVisualState.Deactivating;
			}
			comp.CurrentState = state;
			switch (radiationCollectorVisualState)
			{
			case RadiationCollectorVisualState.Activating:
				base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animPlayer)), comp.ActivateAnimation, "radiationcollector_animation");
				break;
			case RadiationCollectorVisualState.Deactivating:
				base.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animPlayer)), comp.DeactiveAnimation, "radiationcollector_animation");
				break;
			case RadiationCollectorVisualState.Active:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)RadiationCollectorVisualLayers.Main, StateId.op_Implicit(comp.ActiveState));
				break;
			case RadiationCollectorVisualState.Deactive:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)RadiationCollectorVisualLayers.Main, StateId.op_Implicit(comp.InactiveState));
				break;
			}
		}
	}

	private void OnAnimationCompleted(EntityUid uid, RadiationCollectorComponent comp, AnimationCompletedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		AnimationPlayerComponent animPlayer = default(AnimationPlayerComponent);
		if (!(args.Key != "radiationcollector_animation") && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite) && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref animPlayer))
		{
			RadiationCollectorVisualState currentState = default(RadiationCollectorVisualState);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<RadiationCollectorVisualState>(uid, (Enum)RadiationCollectorVisuals.VisualState, ref currentState, (AppearanceComponent)null))
			{
				currentState = comp.CurrentState;
			}
			RadiationCollectorVisualState state = currentState & RadiationCollectorVisualState.Active;
			UpdateVisuals(uid, state, comp, sprite, animPlayer);
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, RadiationCollectorComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent animPlayer = default(AnimationPlayerComponent);
		if (args.Sprite != null && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref animPlayer))
		{
			RadiationCollectorVisualState state = default(RadiationCollectorVisualState);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<RadiationCollectorVisualState>(uid, (Enum)RadiationCollectorVisuals.VisualState, ref state, args.Component))
			{
				state = RadiationCollectorVisualState.Deactive;
			}
			UpdateVisuals(uid, state, comp, args.Sprite, animPlayer);
		}
	}
}
