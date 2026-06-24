using System;
using System.Numerics;
using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.Sprite;
using Content.Shared.ParaDrop;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;

namespace Content.Client._RMC14.ParaDrop;

public sealed class ParaDropSystem : SharedParaDropSystem
{
	[Dependency]
	private AnimationPlayerSystem _animPlayer;

	[Dependency]
	private TransformSystem _transform;

	[Dependency]
	private RMCSpriteSystem _rmcSprite;

	[Dependency]
	private SpriteSystem _sprite;

	private const string DroppingAnimationKey = "dropping-animation";

	private const string SkyFallingAnimationKey = "sky-falling-animation";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SkyFallingComponent, ComponentInit>((EntityEventRefHandler<SkyFallingComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkyFallingComponent, ComponentRemove>((EntityEventRefHandler<SkyFallingComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, ComponentRemove>((EntityEventRefHandler<ParaDroppingComponent, ComponentRemove>)OnParaDroppingRemove, (Type[])null, (Type[])null);
	}

	public Animation ReturnFallAnimation(float fallDuration, float fallHeight)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Expected O, but got Unknown
		return new Animation
		{
			Length = TimeSpan.FromSeconds(fallDuration),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				KeyFrames = 
				{
					new KeyFrame((object)new Vector2(0f, fallHeight), 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)new Vector2(0f, 0f), fallDuration, (Func<float, float>)null)
				}
			} }
		};
	}

	private Animation GetFallingDisappearingAnimation(float duration, Vector2 originalScale, Vector2 endScale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Expected O, but got Unknown
		return new Animation
		{
			Length = TimeSpan.FromSeconds(duration),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Scale",
				KeyFrames = 
				{
					new KeyFrame((object)originalScale, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)endScale, duration, (Func<float, float>)null)
				},
				InterpolationMode = (AnimationInterpolationMode)1
			} },
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				KeyFrames = 
				{
					new KeyFrame((object)new Vector2(0f, 0f), 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)new Vector2(0f, -1f), duration, (Func<float, float>)null)
				}
			} }
		};
	}

	private void OnComponentInit(Entity<SkyFallingComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref val) && !((EntitySystem)this).TerminatingOrDeleted(Entity<SkyFallingComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			ent.Comp.OriginalScale = val.Scale;
			AnimationPlayerComponent val2 = default(AnimationPlayerComponent);
			if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref val2) && !_animPlayer.HasRunningAnimation(val2, "sky-falling-animation"))
			{
				_animPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((Entity<SkyFallingComponent>.op_Implicit(ent), val2)), GetFallingDisappearingAnimation(ent.Comp.RemainingTime, ent.Comp.OriginalScale, ent.Comp.AnimationScale), "sky-falling-animation");
			}
		}
	}

	private void OnComponentRemove(Entity<SkyFallingComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref item) && !((EntitySystem)this).TerminatingOrDeleted(Entity<SkyFallingComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			AnimationPlayerComponent item2 = default(AnimationPlayerComponent);
			if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref item2))
			{
				_animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<SkyFallingComponent>.op_Implicit(ent), item2)), "sky-falling-animation");
			}
			_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), ent.Comp.OriginalScale);
		}
	}

	private void OnParaDroppingRemove(Entity<ParaDroppingComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent item = default(AnimationPlayerComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<ParaDroppingComponent>.op_Implicit(ent), (MetaDataComponent)null) && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref item))
		{
			_animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<ParaDroppingComponent>.op_Implicit(ent), item)), "dropping-animation");
			SpriteComponent item2 = default(SpriteComponent);
			if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref item2))
			{
				_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((ent.Owner, item2)), default(Vector2));
			}
		}
	}

	private void SpawnParachute(float fallDuration, EntityCoordinates coordinates, ParaDroppableComponent paraDroppable, float multiplier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(paraDroppable.ParachutePrototype), coordinates);
		((EntitySystem)this).EnsureComp<TimedDespawnComponent>(val).Lifetime = fallDuration;
		((EntitySystem)this).AddComp<RMCUpdateClientLocationComponent>(val);
		((EntitySystem)this).EnsureComp<ParaDroppingComponent>(val).RemainingTime = fallDuration;
		_animPlayer.Play(val, ReturnFallAnimation(fallDuration, paraDroppable.FallHeight * multiplier), "dropping-animation");
	}

	public void PlayFallAnimation(EntityUid fallingUid, float fallDuration, float timeRemaining, float fallHeight, string animationKey, ParaDroppableComponent? paraDroppable = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float num = timeRemaining / fallDuration;
		float fallDuration2 = fallDuration * num;
		float fallHeight2 = fallHeight * num;
		if (timeRemaining > 0f && num > 0f && num < 1f)
		{
			_animPlayer.Play(fallingUid, ReturnFallAnimation(fallDuration2, fallHeight2), animationKey);
			if (paraDroppable != null)
			{
				SpawnParachute(fallDuration2, ((SharedTransformSystem)_transform).GetMoverCoordinates(fallingUid), paraDroppable, num);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityQueryEnumerator<ParaDroppableComponent, ParaDroppingComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ParaDroppableComponent, ParaDroppingComponent>();
		EntityUid val2 = default(EntityUid);
		ParaDroppableComponent paraDroppableComponent = default(ParaDroppableComponent);
		ParaDroppingComponent paraDroppingComponent = default(ParaDroppingComponent);
		while (val.MoveNext(ref val2, ref paraDroppableComponent, ref paraDroppingComponent))
		{
			if (!((EntitySystem)this).HasComp<SkyFallingComponent>(val2))
			{
				if (!_animPlayer.HasRunningAnimation(val2, "dropping-animation") && paraDroppableComponent.LastParaDrop.HasValue && ((EntitySystem)this).Transform(val2).MapID != MapId.Nullspace)
				{
					PlayFallAnimation(val2, paraDroppableComponent.DropDuration, paraDroppingComponent.RemainingTime, paraDroppableComponent.FallHeight, "dropping-animation", paraDroppableComponent);
				}
				_rmcSprite.UpdatePosition(val2);
			}
		}
	}
}
