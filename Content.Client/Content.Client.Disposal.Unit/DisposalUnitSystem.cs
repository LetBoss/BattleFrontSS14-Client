using System;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Unit;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Disposal.Unit;

public sealed class DisposalUnitSystem : SharedDisposalUnitSystem
{
	[Dependency]
	private AppearanceSystem _appearanceSystem;

	[Dependency]
	private AnimationPlayerSystem _animationSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private SpriteSystem _sprite;

	private const string AnimationKey = "disposal_unit_animation";

	private const string DefaultFlushState = "disposal-flush";

	private const string DefaultChargeState = "disposal-charging";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<DisposalUnitComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, AppearanceChangeEvent>((EntityEventRefHandler<DisposalUnitComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, DisposalUnitComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
	}

	protected override void UpdateUI(Entity<DisposalUnitComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		DisposalUnitBoundUserInterface disposalUnitBoundUserInterface = default(DisposalUnitBoundUserInterface);
		if (_uiSystem.TryGetOpenUi<DisposalUnitBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum)DisposalUnitComponent.DisposalUnitUiKey.Key, ref disposalUnitBoundUserInterface))
		{
			disposalUnitBoundUserInterface.Refresh(entity);
		}
	}

	protected override void OnDisposalInit(Entity<DisposalUnitComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.OnDisposalInit(ent, ref args);
		SpriteComponent sprite = default(SpriteComponent);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<DisposalUnitComponent>.op_Implicit(ent), ref sprite) && ((EntitySystem)this).TryComp<AppearanceComponent>(Entity<DisposalUnitComponent>.op_Implicit(ent), ref appearance))
		{
			UpdateState(ent, sprite, appearance);
		}
	}

	private void OnAppearanceChange(Entity<DisposalUnitComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateState(ent, args.Sprite, args.Component);
		}
	}

	private void UpdateState(Entity<DisposalUnitComponent> ent, SpriteComponent sprite, AppearanceComponent appearance)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01c7: Expected O, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		DisposalUnitComponent.VisualState visualState = default(DisposalUnitComponent.VisualState);
		if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<DisposalUnitComponent.VisualState>(Entity<DisposalUnitComponent>.op_Implicit(ent), (Enum)DisposalUnitComponent.Visuals.VisualState, ref visualState, appearance))
		{
			return;
		}
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.Unanchored, visualState == DisposalUnitComponent.VisualState.UnAnchored);
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.Base, visualState == DisposalUnitComponent.VisualState.Anchored);
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.OverlayFlush, visualState == DisposalUnitComponent.VisualState.OverlayFlushing);
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.BaseCharging, visualState == DisposalUnitComponent.VisualState.OverlayCharging);
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.BaseCharging, ref num, false))
		{
			new StateId("disposal-charging");
		}
		else
		{
			_sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), num);
		}
		if (visualState == DisposalUnitComponent.VisualState.OverlayFlushing)
		{
			if (!_animationSystem.HasRunningAnimation(Entity<DisposalUnitComponent>.op_Implicit(ent), "disposal_unit_animation"))
			{
				int num2 = default(int);
				StateId val = (StateId)(_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.OverlayFlush, ref num2, false) ? _sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), num2) : new StateId("disposal-flush"));
				Animation val2 = new Animation
				{
					Length = ent.Comp.FlushDelay,
					AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
					{
						LayerKey = DisposalUnitVisualLayers.OverlayFlush,
						KeyFrames = 
						{
							new KeyFrame(val, 0f)
						}
					} }
				};
				if (ent.Comp.FlushSound != null)
				{
					val2.AnimationTracks.Add((AnimationTrack)new AnimationTrackPlaySound
					{
						KeyFrames = 
						{
							new KeyFrame(_audioSystem.ResolveSound(ent.Comp.FlushSound), 0f, (Func<AudioParams>)null)
						}
					});
				}
				_animationSystem.Play(Entity<DisposalUnitComponent>.op_Implicit(ent), val2, "disposal_unit_animation");
			}
		}
		else
		{
			_animationSystem.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "disposal_unit_animation");
		}
		DisposalUnitComponent.HandleState handleState = default(DisposalUnitComponent.HandleState);
		if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<DisposalUnitComponent.HandleState>(Entity<DisposalUnitComponent>.op_Implicit(ent), (Enum)DisposalUnitComponent.Visuals.Handle, ref handleState, appearance))
		{
			handleState = DisposalUnitComponent.HandleState.Normal;
		}
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.OverlayEngaged, handleState != DisposalUnitComponent.HandleState.Normal);
		DisposalUnitComponent.LightStates lightStates = default(DisposalUnitComponent.LightStates);
		if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<DisposalUnitComponent.LightStates>(Entity<DisposalUnitComponent>.op_Implicit(ent), (Enum)DisposalUnitComponent.Visuals.Light, ref lightStates, appearance))
		{
			lightStates = DisposalUnitComponent.LightStates.Off;
		}
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.OverlayCharging, (lightStates & DisposalUnitComponent.LightStates.Charging) != 0);
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.OverlayReady, (lightStates & DisposalUnitComponent.LightStates.Ready) != 0);
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum)DisposalUnitVisualLayers.OverlayFull, (lightStates & DisposalUnitComponent.LightStates.Full) != 0);
	}
}
