using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Follower.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Client.Orbit;

public sealed class OrbitVisualsSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _robustRandom;

	[Dependency]
	private AnimationPlayerSystem _animations;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SpriteSystem _sprite;

	private readonly string _orbitStopKey = "orbiting_stop";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<OrbitVisualsComponent, ComponentInit>((ComponentEventHandler<OrbitVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitVisualsComponent, ComponentRemove>((ComponentEventHandler<OrbitVisualsComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, OrbitVisualsComponent component, ComponentInit args)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		_robustRandom.SetSeed((int)_timing.CurTime.TotalMilliseconds);
		component.OrbitDistance = _robustRandom.NextFloat(0.75f * component.OrbitDistance, 1.25f * component.OrbitDistance);
		component.OrbitLength = _robustRandom.NextFloat(0.5f * component.OrbitLength, 1.5f * component.OrbitLength);
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			val.EnableDirectionOverride = true;
			val.DirectionOverride = (Direction)0;
		}
		AnimationPlayerComponent val2 = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
		if (_animations.HasRunningAnimation(uid, val2, _orbitStopKey))
		{
			_animations.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, val2)), _orbitStopKey);
		}
	}

	private void OnComponentRemove(EntityUid uid, OrbitVisualsComponent component, ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			val.EnableDirectionOverride = false;
			AnimationPlayerComponent val2 = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
			if (!_animations.HasRunningAnimation(uid, val2, _orbitStopKey))
			{
				_animations.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val2)), GetStopAnimation(component, val), _orbitStopKey);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityQueryEnumerator<OrbitVisualsComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<OrbitVisualsComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		OrbitVisualsComponent orbitVisualsComponent = default(OrbitVisualsComponent);
		SpriteComponent item2 = default(SpriteComponent);
		Angle val2 = default(Angle);
		while (val.MoveNext(ref item, ref orbitVisualsComponent, ref item2))
		{
			float num = (float)(_timing.CurTime.TotalSeconds / (double)orbitVisualsComponent.OrbitLength) % 1f;
			((Angle)(ref val2))._002Ector(Math.PI * 2.0 * (double)num);
			Vector2 vector = new Vector2(orbitVisualsComponent.OrbitDistance, 0f);
			Vector2 vector2 = ((Angle)(ref val2)).RotateVec(ref vector);
			_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((item, item2)), val2);
			_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((item, item2)), vector2);
		}
	}

	private Animation GetStopAnimation(OrbitVisualsComponent component, SpriteComponent sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0100: Expected O, but got Unknown
		float orbitStopLength = component.OrbitStopLength;
		Animation val = new Animation
		{
			Length = TimeSpan.FromSeconds(orbitStopLength),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Offset",
				KeyFrames = 
				{
					new KeyFrame((object)sprite.Offset, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Vector2.Zero, orbitStopLength, (Func<float, float>)null)
				},
				InterpolationMode = (AnimationInterpolationMode)0
			} }
		};
		List<AnimationTrack> animationTracks = val.AnimationTracks;
		AnimationTrackComponentProperty val2 = new AnimationTrackComponentProperty
		{
			ComponentType = typeof(SpriteComponent),
			Property = "Rotation"
		};
		List<KeyFrame> keyFrames = ((AnimationTrackProperty)val2).KeyFrames;
		Angle rotation = sprite.Rotation;
		keyFrames.Add(new KeyFrame((object)((Angle)(ref rotation)).Reduced(), 0f, (Func<float, float>)null));
		((AnimationTrackProperty)val2).KeyFrames.Add(new KeyFrame((object)Angle.Zero, orbitStopLength, (Func<float, float>)null));
		((AnimationTrackProperty)val2).InterpolationMode = (AnimationInterpolationMode)0;
		animationTracks.Add((AnimationTrack)val2);
		return val;
	}
}
