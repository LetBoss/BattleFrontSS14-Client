using System;
using Content.Shared._RMC14.Doors;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Doors;

public sealed class RMCDoorVisualsSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private SpriteSystem _sprite;

	private const string ButtonAnimationKey = "rmc_pod_door_button_animation";

	private readonly TimeSpan _buttonAnimationLength = TimeSpan.FromSeconds(1.25);

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<RMCPodDoorButtonPressedEvent>((EntityEventHandler<RMCPodDoorButtonPressedEvent>)OnPodDoorButtonPressed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDoorButtonComponent, AnimationCompletedEvent>((EntityEventRefHandler<RMCDoorButtonComponent, AnimationCompletedEvent>)OnDoorButtonAnimationCompleted, (Type[])null, (Type[])null);
	}

	private void OnPodDoorButtonPressed(RMCPodDoorButtonPressedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00f2: Expected O, but got Unknown
		EntityUid? val = default(EntityUid?);
		RMCDoorButtonComponent rMCDoorButtonComponent = default(RMCDoorButtonComponent);
		if (((EntitySystem)this).TryGetEntity(ev.Button, ref val) && ((EntitySystem)this).TryComp<RMCDoorButtonComponent>(val, ref rMCDoorButtonComponent) && !_animation.HasRunningAnimation(val.Value, "rmc_pod_door_button_animation"))
		{
			string animationState = ev.AnimationState;
			_animation.Play(val.Value, new Animation
			{
				Length = _buttonAnimationLength,
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = RMCPodDoorButtonLayers.Animation,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(animationState), 0f)
					},
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(animationState), 0.5f)
					},
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(animationState), 1f)
					},
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(rMCDoorButtonComponent.OffState), 1.25f)
					}
				} }
			}, "rmc_pod_door_button_animation");
		}
	}

	private void OnDoorButtonAnimationCompleted(Entity<RMCDoorButtonComponent> ent, ref AnimationCompletedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!(args.Key != "rmc_pod_door_button_animation") && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCDoorButtonComponent>.op_Implicit(ent), ref item))
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), (Enum)RMCPodDoorButtonLayers.Animation, StateId.op_Implicit(ent.Comp.OffState));
		}
	}
}
