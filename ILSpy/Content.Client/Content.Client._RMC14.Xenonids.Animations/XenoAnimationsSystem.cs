using System;
using System.Numerics;
using Content.Shared._RMC14.Xenonids.Animation;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Xenonids.Animations;

public sealed class XenoAnimationsSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private IPlayerManager _player;

	private const string MeleeLungeKey = "melee-lunge";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeAllEvent<PlayLungeAnimationEvent>((EntityEventHandler<PlayLungeAnimationEvent>)OnPlayLungeAnimation, (Type[])null, (Type[])null);
	}

	private void OnPlayLungeAnimation(PlayLungeAnimationEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(ev.EntityUid, ref val))
		{
			return;
		}
		if (!ev.Client)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			EntityUid? val2 = val;
			if (localEntity.HasValue == val2.HasValue && (!localEntity.HasValue || localEntity.GetValueOrDefault() == val2.GetValueOrDefault()))
			{
				return;
			}
		}
		Vector2 direction = ev.Direction;
		Animation lungeAnimation = GetLungeAnimation(direction);
		_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(val.Value), "melee-lunge");
		_animation.Play(val.Value, lungeAnimation, "melee-lunge");
	}

	private Animation GetLungeAnimation(Vector2 direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Expected O, but got Unknown
		return new Animation
		{
			Length = TimeSpan.FromSeconds(0.4000000059604645),
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
					new KeyFrame((object)(Vector2Helpers.Normalized(direction) * 0.6f), 0.1f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, 0.3f, (Func<float, float>)null)
				}
			} }
		};
	}
}
