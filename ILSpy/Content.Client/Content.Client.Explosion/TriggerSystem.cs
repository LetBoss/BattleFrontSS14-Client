using System;
using Content.Shared.Trigger;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Explosion;

public sealed class TriggerSystem : EntitySystem
{
	public enum ProximityTriggerVisualLayers : byte
	{
		Base
	}

	[Dependency]
	private AnimationPlayerSystem _player;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	private const string AnimKey = "proximity";

	private static readonly Animation _flasherAnimation = new Animation
	{
		Length = TimeSpan.FromSeconds(0.6000000238418579),
		AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
		{
			LayerKey = ProximityTriggerVisualLayers.Base,
			KeyFrames = 
			{
				new KeyFrame(StateId.op_Implicit("flashing"), 0f)
			}
		} },
		AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
		{
			ComponentType = typeof(PointLightComponent),
			InterpolationMode = (AnimationInterpolationMode)2,
			Property = "AnimatedRadius",
			KeyFrames = 
			{
				new KeyFrame((object)0.1f, 0f, (Func<float, float>)null)
			},
			KeyFrames = 
			{
				new KeyFrame((object)3f, 0.1f, (Func<float, float>)null)
			},
			KeyFrames = 
			{
				new KeyFrame((object)0.1f, 0.5f, (Func<float, float>)null)
			}
		} }
	};

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeProximity();
	}

	private void InitializeProximity()
	{
		((EntitySystem)this).SubscribeLocalEvent<TriggerOnProximityComponent, ComponentInit>((ComponentEventHandler<TriggerOnProximityComponent, ComponentInit>)OnProximityInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TriggerOnProximityComponent, AppearanceChangeEvent>((ComponentEventRefHandler<TriggerOnProximityComponent, AppearanceChangeEvent>)OnProxAppChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TriggerOnProximityComponent, AnimationCompletedEvent>((ComponentEventHandler<TriggerOnProximityComponent, AnimationCompletedEvent>)OnProxAnimation, (Type[])null, (Type[])null);
	}

	private void OnProxAnimation(EntityUid uid, TriggerOnProximityComponent component, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent val = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val))
		{
			_appearance.SetData(uid, (Enum)ProximityTriggerVisualState.State, (object)ProximityTriggerVisuals.Inactive, val);
			OnChangeData(uid, component, val);
		}
	}

	private void OnProximityInit(EntityUid uid, TriggerOnProximityComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(uid);
	}

	private void OnProxAppChange(EntityUid uid, TriggerOnProximityComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnChangeData(uid, component, args.Component, args.Sprite);
	}

	private void OnChangeData(EntityUid uid, TriggerOnProximityComponent component, AppearanceComponent appearance, SpriteComponent? spriteComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		ProximityTriggerVisuals proximityTriggerVisuals = default(ProximityTriggerVisuals);
		int num = default(int);
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref spriteComponent, true) || !((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val) || !_appearance.TryGetData<ProximityTriggerVisuals>(uid, (Enum)ProximityTriggerVisualState.State, ref proximityTriggerVisuals, appearance) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum)ProximityTriggerVisualLayers.Base, ref num, false))
		{
			return;
		}
		switch (proximityTriggerVisuals)
		{
		case ProximityTriggerVisuals.Inactive:
			if (!_player.HasRunningAnimation(uid, val, "proximity"))
			{
				_player.Stop(uid, val, "proximity");
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, StateId.op_Implicit("on"));
			}
			break;
		case ProximityTriggerVisuals.Active:
			if (!_player.HasRunningAnimation(uid, val, "proximity"))
			{
				_player.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), _flasherAnimation, "proximity");
			}
			break;
		default:
			_player.Stop(uid, val, "proximity");
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, StateId.op_Implicit("off"));
			break;
		}
	}
}
