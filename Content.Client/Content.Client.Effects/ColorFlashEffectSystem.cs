using System;
using System.Collections.Generic;
using Content.Shared.Effects;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Effects;

public sealed class ColorFlashEffectSystem : SharedColorFlashEffectSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private SpriteSystem _sprite;

	private const float AnimationLength = 0.3f;

	private const string AnimationKey = "color-flash-effect";

	private ValueList<EntityUid> _toRemove;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeAllEvent<ColorFlashEffectEvent>((EntityEventHandler<ColorFlashEffectEvent>)OnColorFlashEffect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ColorFlashEffectComponent, AnimationCompletedEvent>((ComponentEventHandler<ColorFlashEffectComponent, AnimationCompletedEvent>)OnEffectAnimationCompleted, (Type[])null, (Type[])null);
	}

	public override void RaiseEffect(Color color, List<EntityUid> entities, Filter filter)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			OnColorFlashEffect(new ColorFlashEffectEvent(color, ((EntitySystem)this).GetNetEntityList(entities)));
		}
	}

	private void OnEffectAnimationCompleted(EntityUid uid, ColorFlashEffectComponent component, AnimationCompletedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!(args.Key != "color-flash-effect") && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), component.Color);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		AllEntityQueryEnumerator<ColorFlashEffectComponent> val = ((EntitySystem)this).AllEntityQuery<ColorFlashEffectComponent>();
		_toRemove.Clear();
		EntityUid val2 = default(EntityUid);
		ColorFlashEffectComponent colorFlashEffectComponent = default(ColorFlashEffectComponent);
		while (val.MoveNext(ref val2, ref colorFlashEffectComponent))
		{
			if (!_animation.HasRunningAnimation(val2, "color-flash-effect"))
			{
				_toRemove.Add(val2);
			}
		}
		Enumerator<EntityUid> enumerator = _toRemove.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				EntityUid current = enumerator.Current;
				((EntitySystem)this).RemComp<ColorFlashEffectComponent>(current);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private Animation? GetDamageAnimation(EntityUid uid, Color color, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Expected O, but got Unknown
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, false))
		{
			return null;
		}
		return new Animation
		{
			Length = TimeSpan.FromSeconds(0.30000001192092896),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Color",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)color, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)sprite.Color, 0.3f, (Func<float, float>)null)
				}
			} }
		};
	}

	private void OnColorFlashEffect(ColorFlashEffectEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Color color = ev.Color;
		SpriteComponent val = default(SpriteComponent);
		ColorFlashEffectComponent colorFlashEffectComponent = default(ColorFlashEffectComponent);
		foreach (NetEntity entity2 in ev.Entities)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(entity2);
			if (!((EntitySystem)this).Deleted(entity, (MetaDataComponent)null) && ((EntitySystem)this).TryComp<SpriteComponent>(entity, ref val))
			{
				((EntitySystem)this).TryComp<ColorFlashEffectComponent>(entity, ref colorFlashEffectComponent);
				_animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(entity), "color-flash-effect");
				Animation damageAnimation = GetDamageAnimation(entity, color, val);
				if (damageAnimation != null)
				{
					GetFlashEffectTargetEvent getFlashEffectTargetEvent = new GetFlashEffectTargetEvent(entity);
					((EntitySystem)this).RaiseLocalEvent<GetFlashEffectTargetEvent>(entity, ref getFlashEffectTargetEvent, false);
					entity = getFlashEffectTargetEvent.Target;
					((EntitySystem)this).EnsureComp<ColorFlashEffectComponent>(entity, ref colorFlashEffectComponent);
					((Component)colorFlashEffectComponent).NetSyncEnabled = false;
					colorFlashEffectComponent.Color = val.Color;
					_animation.Play(entity, damageAnimation, "color-flash-effect");
				}
			}
		}
	}
}
