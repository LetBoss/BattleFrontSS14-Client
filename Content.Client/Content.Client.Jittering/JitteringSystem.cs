using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Jittering;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Client.Jittering;

public sealed class JitteringSystem : SharedJitteringSystem
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private AnimationPlayerSystem _animationPlayer;

	[Dependency]
	private SpriteSystem _sprite;

	private readonly float[] _sign = new float[2] { -1f, 1f };

	private readonly string _jitterAnimationKey = "jittering";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<JitteringComponent, ComponentStartup>((ComponentEventHandler<JitteringComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JitteringComponent, ComponentShutdown>((ComponentEventHandler<JitteringComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JitteringComponent, AnimationCompletedEvent>((ComponentEventHandler<JitteringComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, JitteringComponent jittering, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			AnimationPlayerComponent item = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
			jittering.StartOffset = val.Offset;
			_animationPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, item)), GetAnimation(jittering, val), _jitterAnimationKey);
		}
	}

	private void OnShutdown(EntityUid uid, JitteringComponent jittering, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val))
		{
			_animationPlayer.Stop(uid, val, _jitterAnimationKey);
		}
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), jittering.StartOffset);
		}
	}

	private void OnAnimationCompleted(EntityUid uid, JitteringComponent jittering, AnimationCompletedEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent item = default(AnimationPlayerComponent);
		SpriteComponent sprite = default(SpriteComponent);
		if (!(args.Key != _jitterAnimationKey) && args.Finished && ((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref item) && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite))
		{
			_animationPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, item)), GetAnimation(jittering, sprite), _jitterAnimationKey);
		}
	}

	private Animation GetAnimation(JitteringComponent jittering, SpriteComponent sprite)
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_01a5: Expected O, but got Unknown
		float num = MathF.Min(4f, jittering.Amplitude / 100f + 1f) / 10f;
		Vector2 vector = new Vector2(_random.NextFloat(num / 4f, num), _random.NextFloat(num / 4f, num / 3f));
		vector.X *= RandomExtensions.Pick<float>(_random, (IReadOnlyList<float>)_sign);
		vector.Y *= RandomExtensions.Pick<float>(_random, (IReadOnlyList<float>)_sign);
		if (Math.Sign(vector.X) == Math.Sign(jittering.LastJitter.X) || Math.Sign(vector.Y) == Math.Sign(jittering.LastJitter.Y))
		{
			if (RandomExtensions.Prob(_random, 0.5f))
			{
				vector.X *= -1f;
			}
			else
			{
				vector.Y *= -1f;
			}
		}
		float num2 = 0f;
		if (jittering.Frequency > 0f)
		{
			num2 = 1f / jittering.Frequency;
		}
		jittering.LastJitter = vector;
		return new Animation
		{
			Length = TimeSpan.FromSeconds(num2),
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
					new KeyFrame((object)(jittering.StartOffset + vector), num2, (Func<float, float>)null)
				}
			} }
		};
	}
}
