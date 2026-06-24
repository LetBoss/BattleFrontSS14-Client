using System;
using Content.Shared.Cargo;
using Content.Shared.Cargo.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Cargo.Systems;

public sealed class CargoSystem : SharedCargoSystem
{
	private enum CargoTelepadLayers : byte
	{
		Base,
		Beam
	}

	[Dependency]
	private AnimationPlayerSystem _player;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	private static readonly Animation CargoTelepadBeamAnimation = new Animation
	{
		Length = TimeSpan.FromSeconds(0.5),
		AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
		{
			LayerKey = CargoTelepadLayers.Beam,
			KeyFrames = 
			{
				new KeyFrame(new StateId("beam"), 0f)
			}
		} }
	};

	private static readonly Animation CargoTelepadIdleAnimation = new Animation
	{
		Length = TimeSpan.FromSeconds(0.8),
		AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
		{
			LayerKey = CargoTelepadLayers.Beam,
			KeyFrames = 
			{
				new KeyFrame(new StateId("idle"), 0f)
			}
		} }
	};

	private const string TelepadBeamKey = "cargo-telepad-beam";

	private const string TelepadIdleKey = "cargo-telepad-idle";

	public override void Initialize()
	{
		base.Initialize();
		InitializeCargoTelepad();
	}

	private void InitializeCargoTelepad()
	{
		((EntitySystem)this).SubscribeLocalEvent<CargoTelepadComponent, AppearanceChangeEvent>((ComponentEventRefHandler<CargoTelepadComponent, AppearanceChangeEvent>)OnCargoAppChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CargoTelepadComponent, AnimationCompletedEvent>((ComponentEventHandler<CargoTelepadComponent, AnimationCompletedEvent>)OnCargoAnimComplete, (Type[])null, (Type[])null);
	}

	private void OnCargoAppChange(EntityUid uid, CargoTelepadComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnChangeData(uid, args.Sprite);
	}

	private void OnCargoAnimComplete(EntityUid uid, CargoTelepadComponent component, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnChangeData(uid);
	}

	private void OnChangeData(EntityUid uid, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, true) || !((EntitySystem)this).TryComp<AnimationPlayerComponent>(uid, ref val))
		{
			return;
		}
		CargoTelepadState? cargoTelepadState = default(CargoTelepadState?);
		_appearance.TryGetData<CargoTelepadState?>(uid, (Enum)CargoTelepadVisuals.State, ref cargoTelepadState, (AppearanceComponent)null);
		switch (cargoTelepadState)
		{
		case CargoTelepadState.Teleporting:
			_player.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), "cargo-telepad-idle");
			if (!_player.HasRunningAnimation(uid, "cargo-telepad-beam"))
			{
				_player.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), CargoTelepadBeamAnimation, "cargo-telepad-beam");
			}
			break;
		case CargoTelepadState.Unpowered:
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)CargoTelepadLayers.Beam, false);
			_player.Stop(uid, val, "cargo-telepad-beam");
			_player.Stop(uid, val, "cargo-telepad-idle");
			break;
		default:
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)CargoTelepadLayers.Beam, true);
			if (!_player.HasRunningAnimation(uid, val, "cargo-telepad-idle") && !_player.HasRunningAnimation(uid, val, "cargo-telepad-beam"))
			{
				_player.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, val)), CargoTelepadIdleAnimation, "cargo-telepad-idle");
			}
			break;
		}
	}
}
