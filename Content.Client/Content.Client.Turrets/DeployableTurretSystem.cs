using System;
using Content.Client.Power;
using Content.Shared.Turrets;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Turrets;

public sealed class DeployableTurretSystem : SharedDeployableTurretSystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretComponent, ComponentInit>((EntityEventRefHandler<DeployableTurretComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretComponent, AnimationCompletedEvent>((EntityEventRefHandler<DeployableTurretComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableTurretComponent, AppearanceChangeEvent>((EntityEventRefHandler<DeployableTurretComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(Entity<DeployableTurretComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0068: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00d0: Expected O, but got Unknown
		ent.Comp.DeploymentAnimation = (object)new Animation
		{
			Length = TimeSpan.FromSeconds(ent.Comp.DeploymentLength),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DeployableTurretVisuals.Turret,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(ent.Comp.DeployingState), 0f)
				}
			} }
		};
		ent.Comp.RetractionAnimation = (object)new Animation
		{
			Length = TimeSpan.FromSeconds(ent.Comp.RetractionLength),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DeployableTurretVisuals.Turret,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(ent.Comp.RetractingState), 0f)
				}
			} }
		};
	}

	private void OnAnimationCompleted(Entity<DeployableTurretComponent> ent, ref AnimationCompletedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (!(args.Key != "deployable_turret_animation") && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref sprite))
		{
			DeployableTurretState visualState = default(DeployableTurretState);
			if (!((SharedAppearanceSystem)_appearance).TryGetData<DeployableTurretState>(Entity<DeployableTurretComponent>.op_Implicit(ent), (Enum)DeployableTurretVisuals.Turret, ref visualState, (AppearanceComponent)null))
			{
				visualState = ent.Comp.VisualState;
			}
			DeployableTurretState state = visualState & DeployableTurretState.Deployed;
			UpdateVisuals(ent, state, sprite, args.AnimationPlayer);
		}
	}

	private void OnAppearanceChange(Entity<DeployableTurretComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent animPlayer = default(AnimationPlayerComponent);
		if (args.Sprite != null && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref animPlayer))
		{
			DeployableTurretState state = default(DeployableTurretState);
			if (!((SharedAppearanceSystem)_appearance).TryGetData<DeployableTurretState>(Entity<DeployableTurretComponent>.op_Implicit(ent), (Enum)DeployableTurretVisuals.Turret, ref state, args.Component))
			{
				state = DeployableTurretState.Retracted;
			}
			UpdateVisuals(ent, state, args.Sprite, animPlayer);
		}
	}

	private void UpdateVisuals(Entity<DeployableTurretComponent> ent, DeployableTurretState state, SpriteComponent sprite, AnimationPlayerComponent? animPlayer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		if (((EntitySystem)this).Resolve<AnimationPlayerComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref animPlayer, true) && !_animation.HasRunningAnimation(Entity<DeployableTurretComponent>.op_Implicit(ent), animPlayer, "deployable_turret_animation"))
		{
			DeployableTurretState deployableTurretState = state & DeployableTurretState.Deployed;
			DeployableTurretState deployableTurretState2 = ent.Comp.VisualState & DeployableTurretState.Deployed;
			if (deployableTurretState != deployableTurretState2)
			{
				deployableTurretState |= DeployableTurretState.Retracting;
			}
			ent.Comp.VisualState = state;
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum)DeployableTurretVisuals.Weapon, (int)(deployableTurretState & DeployableTurretState.Deployed) > 0);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum)PowerDeviceVisualLayers.Powered, HasAmmo(ent) && deployableTurretState == DeployableTurretState.Retracted);
			switch (deployableTurretState)
			{
			case DeployableTurretState.Deploying:
				_animation.Play(Entity<AnimationPlayerComponent>.op_Implicit((Entity<DeployableTurretComponent>.op_Implicit(ent), animPlayer)), (Animation)ent.Comp.DeploymentAnimation, "deployable_turret_animation");
				break;
			case DeployableTurretState.Retracting:
				_animation.Play(Entity<AnimationPlayerComponent>.op_Implicit((Entity<DeployableTurretComponent>.op_Implicit(ent), animPlayer)), (Animation)ent.Comp.RetractionAnimation, "deployable_turret_animation");
				break;
			case DeployableTurretState.Deployed:
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum)DeployableTurretVisuals.Turret, StateId.op_Implicit(ent.Comp.DeployedState));
				break;
			case DeployableTurretState.Retracted:
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum)DeployableTurretVisuals.Turret, StateId.op_Implicit(ent.Comp.RetractedState));
				break;
			}
		}
	}
}
