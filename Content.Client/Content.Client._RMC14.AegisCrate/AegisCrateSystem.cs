using System;
using Content.Shared._RMC14.AegisCrate;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.AegisCrate;

public sealed class AegisCrateSystem : SharedAegisCrateSystem
{
	private const string AnimationKey = "AegisCrateOpenAnim";

	private Animation? _openingAnimation;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private AnimationPlayerSystem _animation;

	public override void Initialize()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009f: Expected O, but got Unknown
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AegisCrateComponent, AegisCrateStateChangedEvent>((EntityEventRefHandler<AegisCrateComponent, AegisCrateStateChangedEvent>)OnStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AegisCrateComponent, AnimationCompletedEvent>((EntityEventRefHandler<AegisCrateComponent, AnimationCompletedEvent>)OnAegisAnimationFinished, (Type[])null, (Type[])null);
		_openingAnimation = new Animation
		{
			Length = OpeningSpeed,
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = AegisCrateVisualLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit("aegis_crate_opening"), 0f)
				},
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit("aegis_crate_open"), 1.46f)
				}
			} }
		};
	}

	protected override void OnStartup(Entity<AegisCrateComponent> crate, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		base.OnStartup(crate, ref args);
		SetVisuals(crate);
	}

	private void SetVisuals(Entity<AegisCrateComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<AegisCrateComponent>.op_Implicit(ent), ref item) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), item)), (Enum)AegisCrateVisualLayers.Base, ref num, true))
		{
			return;
		}
		switch (ent.Comp.State)
		{
		case AegisCrateState.Closed:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), item)), num, StateId.op_Implicit("aegis_crate"));
			break;
		case AegisCrateState.Opening:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), item)), num, StateId.op_Implicit("aegis_crate_opening"));
			if (!_animation.HasRunningAnimation(Entity<AegisCrateComponent>.op_Implicit(ent), "AegisCrateOpenAnim"))
			{
				_animation.Play(Entity<AegisCrateComponent>.op_Implicit(ent), _openingAnimation, "AegisCrateOpenAnim");
			}
			break;
		case AegisCrateState.Open:
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), item)), num, StateId.op_Implicit("aegis_crate_open"));
			break;
		}
	}

	private void OnStateChanged(Entity<AegisCrateComponent> ent, ref AegisCrateStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetVisuals(ent);
	}

	private void OnAegisAnimationFinished(Entity<AegisCrateComponent> ent, ref AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<AegisCrateComponent>.op_Implicit(ent), ref val))
		{
			_animation.Stop(Entity<AegisCrateComponent>.op_Implicit(ent), val, "AegisCrateOpenAnim");
		}
	}
}
