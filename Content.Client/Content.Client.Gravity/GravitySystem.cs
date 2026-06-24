using System;
using System.Numerics;
using Content.Shared.Camera;
using Content.Shared.Gravity;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Client.Gravity;

public sealed class GravitySystem : SharedGravitySystem
{
	[Dependency]
	private AppearanceSystem _appearanceSystem;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedCameraRecoilSystem _sharedCameraRecoil;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SharedGravityGeneratorComponent, AppearanceChangeEvent>((ComponentEventRefHandler<SharedGravityGeneratorComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		InitializeShake();
	}

	private void OnAppearanceChange(EntityUid uid, SharedGravityGeneratorComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		PowerChargeStatus key = default(PowerChargeStatus);
		if (((SharedAppearanceSystem)_appearanceSystem).TryGetData<PowerChargeStatus>(uid, (Enum)PowerChargeVisuals.State, ref key, args.Component) && comp.SpriteMap.TryGetValue(key, out string value))
		{
			int num = _sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)GravityGeneratorVisualLayers.Base);
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(value));
		}
		float num2 = default(float);
		if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<float>(uid, (Enum)PowerChargeVisuals.Charge, ref num2, args.Component))
		{
			return;
		}
		int num3 = _sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)GravityGeneratorVisualLayers.Core);
		if (!(num2 < 0.2f))
		{
			if (num2 >= 0.2f)
			{
				if (num2 < 0.4f)
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, true);
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit(comp.CoreStartupState));
					return;
				}
				if (num2 < 0.6f)
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, true);
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit(comp.CoreIdleState));
					return;
				}
				if (num2 < 0.8f)
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, true);
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit(comp.CoreActivatingState));
					return;
				}
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, true);
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit(comp.CoreActivatedState));
		}
		else
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, false);
		}
	}

	private void InitializeShake()
	{
		((EntitySystem)this).SubscribeLocalEvent<GravityShakeComponent, ComponentInit>((ComponentEventHandler<GravityShakeComponent, ComponentInit>)OnShakeInit, (Type[])null, (Type[])null);
	}

	private void OnShakeInit(EntityUid uid, GravityShakeComponent component, ComponentInit args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		TransformComponent val = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(localEntity, ref val))
		{
			return;
		}
		EntityUid? gridUid = val.GridUid;
		EntityUid val2 = uid;
		if (!gridUid.HasValue || gridUid.GetValueOrDefault() != val2)
		{
			gridUid = val.MapUid;
			val2 = uid;
			if (!gridUid.HasValue || gridUid.GetValueOrDefault() != val2)
			{
				return;
			}
		}
		GravityComponent gravityComponent = default(GravityComponent);
		if (Timing.IsFirstTimePredicted && ((EntitySystem)this).TryComp<GravityComponent>(uid, ref gravityComponent))
		{
			_audio.PlayGlobal(gravityComponent.GravityShakeSound, Filter.Local(), true, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-2f));
		}
	}

	protected override void ShakeGrid(EntityUid uid, GravityComponent? gravity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		base.ShakeGrid(uid, gravity);
		if (!((EntitySystem)this).Resolve<GravityComponent>(uid, ref gravity, true) || !Timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		TransformComponent val = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(localEntity, ref val))
		{
			return;
		}
		EntityUid? gridUid = val.GridUid;
		EntityUid val2 = uid;
		if (!gridUid.HasValue || gridUid.GetValueOrDefault() != val2)
		{
			return;
		}
		if (!val.GridUid.HasValue)
		{
			gridUid = val.MapUid;
			val2 = uid;
			if (!gridUid.HasValue || gridUid.GetValueOrDefault() != val2)
			{
				return;
			}
		}
		Vector2 kickback = new Vector2(_random.NextFloat(), _random.NextFloat()) * 100f;
		_sharedCameraRecoil.KickCamera(localEntity.Value, kickback);
	}
}
