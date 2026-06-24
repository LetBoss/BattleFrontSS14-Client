using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Animations;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Animations;

public sealed class RMCAnimationSystem : SharedRMCAnimationSystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private SpriteSystem _sprite;

	private const string FlickId = "rmc_flick_animation";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCPlayAnimationEvent>((EntityEventHandler<RMCPlayAnimationEvent>)OnPlayAnimation, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RMCFlickEvent>((EntityEventHandler<RMCFlickEvent>)OnFlick, (Type[])null, (Type[])null);
	}

	private void OnPlayAnimation(RMCPlayAnimationEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.Entity);
		RMCAnimationComponent rMCAnimationComponent = default(RMCAnimationComponent);
		if (!((EntityUid)(ref entity)).Valid || _animation.HasRunningAnimation(entity, ev.Animation.Id) || !((EntitySystem)this).TryComp<RMCAnimationComponent>(entity, ref rMCAnimationComponent) || !rMCAnimationComponent.Animations.TryGetValue(ev.Animation, out var value))
		{
			return;
		}
		List<AnimationTrack> list = new List<AnimationTrack>();
		foreach (RMCAnimationTrack animationTrack in value.AnimationTracks)
		{
			List<KeyFrame> list2 = new List<KeyFrame>();
			foreach (RMCKeyFrame keyFrame in animationTrack.KeyFrames)
			{
				list2.Add(new KeyFrame(StateId.op_Implicit(keyFrame.State), keyFrame.KeyTime));
			}
			AnimationTrackSpriteFlick val = new AnimationTrackSpriteFlick
			{
				LayerKey = animationTrack.LayerKey
			};
			val.KeyFrames.AddRange(list2);
			list.Add((AnimationTrack)(object)val);
		}
		Animation val2 = new Animation
		{
			Length = value.Length
		};
		val2.AnimationTracks.AddRange(list);
		_animation.Play(entity, val2, ev.Animation.Id);
	}

	private void OnFlick(RMCFlickEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.Entity);
		if (((EntityUid)(ref entity)).Valid && !_animation.HasRunningAnimation(entity, "rmc_flick_animation"))
		{
			string text = ev.Layer ?? "rmc_flick_animation";
			if (!_sprite.LayerExists(Entity<SpriteComponent>.op_Implicit(entity), text))
			{
				_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit(entity), "rmc_flick_animation", 0);
			}
			float animationLength = _sprite.GetState(ev.AnimationState).AnimationLength;
			Animation val = new Animation
			{
				Length = TimeSpan.FromSeconds(animationLength),
				AnimationTracks = { (AnimationTrack)(object)new RMCAnimationTrackSpriteFlick
				{
					LayerKey = "rmc_flick_animation",
					KeyFrames = new List<RMCAnimationTrackSpriteFlick.KeyFrame>
					{
						new RMCAnimationTrackSpriteFlick.KeyFrame(ev.AnimationState, 0f),
						new RMCAnimationTrackSpriteFlick.KeyFrame(ev.DefaultState, animationLength)
					}
				} }
			};
			_animation.Play(entity, val, "rmc_flick_animation");
		}
	}
}
