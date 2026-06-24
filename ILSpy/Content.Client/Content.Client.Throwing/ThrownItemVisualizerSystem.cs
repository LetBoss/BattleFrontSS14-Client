using System;
using System.Numerics;
using Content.Shared.Throwing;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Throwing;

public sealed class ThrownItemVisualizerSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _anim;

	[Dependency]
	private SpriteSystem _sprite;

	private const string AnimationKey = "thrown-item";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<ThrownItemComponent, AfterAutoHandleStateEvent>)OnAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, ComponentShutdown>((ComponentEventHandler<ThrownItemComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnAutoHandleState(EntityUid uid, ThrownItemComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) || !component.Animate)
		{
			return;
		}
		AnimationPlayerComponent val2 = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
		if (!_anim.HasRunningAnimation(uid, val2, "thrown-item"))
		{
			Animation animation = GetAnimation(Entity<ThrownItemComponent, SpriteComponent>.op_Implicit((uid, component, val)));
			if (animation != null)
			{
				component.OriginalScale = val.Scale;
				_anim.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val2)), animation, "thrown-item");
			}
		}
	}

	private void OnShutdown(EntityUid uid, ThrownItemComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (_anim.HasRunningAnimation(uid, "thrown-item"))
		{
			SpriteComponent item = default(SpriteComponent);
			if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && component.OriginalScale.HasValue)
			{
				_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, item)), component.OriginalScale.Value);
			}
			_anim.Stop(Entity<AnimationPlayerComponent>.op_Implicit(uid), "thrown-item");
		}
	}

	private static Animation? GetAnimation(Entity<ThrownItemComponent, SpriteComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_012a: Expected O, but got Unknown
		TimeSpan? timeSpan = ent.Comp1.LandTime - ent.Comp1.ThrownTime;
		if (timeSpan.HasValue)
		{
			TimeSpan valueOrDefault = timeSpan.GetValueOrDefault();
			if (valueOrDefault <= TimeSpan.Zero)
			{
				return null;
			}
			Vector2 scale = ent.Comp2.Scale;
			float num = (float)valueOrDefault.TotalSeconds;
			return new Animation
			{
				Length = valueOrDefault,
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					ComponentType = typeof(SpriteComponent),
					Property = "Scale",
					KeyFrames = 
					{
						new KeyFrame((object)scale, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)(scale * 1.4f), num * 0.25f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)scale, num * 0.75f, (Func<float, float>)null)
					},
					InterpolationMode = (AnimationInterpolationMode)0
				} }
			};
		}
		return null;
	}
}
