using System;
using Content.Shared.Audio.Jukebox;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Audio.Jukebox;

public sealed class JukeboxSystem : SharedJukeboxSystem
{
	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private AnimationPlayerSystem _animationPlayer;

	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<JukeboxComponent, AppearanceChangeEvent>((ComponentEventRefHandler<JukeboxComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JukeboxComponent, AnimationCompletedEvent>((ComponentEventHandler<JukeboxComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JukeboxComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<JukeboxComponent, AfterAutoHandleStateEvent>)OnJukeboxAfterState, (Type[])null, (Type[])null);
		_protoManager.PrototypesReloaded += OnProtoReload;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_protoManager.PrototypesReloaded -= OnProtoReload;
	}

	private void OnProtoReload(PrototypesReloadedEventArgs obj)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!obj.WasModified<JukeboxPrototype>())
		{
			return;
		}
		AllEntityQueryEnumerator<JukeboxComponent, UserInterfaceComponent> val = ((EntitySystem)this).AllEntityQuery<JukeboxComponent, UserInterfaceComponent>();
		EntityUid item = default(EntityUid);
		JukeboxComponent jukeboxComponent = default(JukeboxComponent);
		UserInterfaceComponent item2 = default(UserInterfaceComponent);
		JukeboxBoundUserInterface jukeboxBoundUserInterface = default(JukeboxBoundUserInterface);
		while (val.MoveNext(ref item, ref jukeboxComponent, ref item2))
		{
			if (_uiSystem.TryGetOpenUi<JukeboxBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit((item, item2)), (Enum)JukeboxUiKey.Key, ref jukeboxBoundUserInterface))
			{
				jukeboxBoundUserInterface.PopulateMusic();
			}
		}
	}

	private void OnJukeboxAfterState(Entity<JukeboxComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		JukeboxBoundUserInterface jukeboxBoundUserInterface = default(JukeboxBoundUserInterface);
		if (_uiSystem.TryGetOpenUi<JukeboxBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)JukeboxUiKey.Key, ref jukeboxBoundUserInterface))
		{
			jukeboxBoundUserInterface.Reload();
		}
	}

	private void OnAnimationCompleted(EntityUid uid, JukeboxComponent component, AnimationCompletedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			AppearanceComponent val = default(AppearanceComponent);
			JukeboxVisualState visualState = default(JukeboxVisualState);
			if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val) || !_appearanceSystem.TryGetData<JukeboxVisualState>(uid, (Enum)JukeboxVisuals.VisualState, ref visualState, val))
			{
				visualState = JukeboxVisualState.On;
			}
			UpdateAppearance(Entity<SpriteComponent>.op_Implicit((uid, item)), visualState, component);
		}
	}

	private void OnAppearanceChange(EntityUid uid, JukeboxComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateAppearance(visualState: (args.AppearanceData.TryGetValue(JukeboxVisuals.VisualState, out var value) && value is JukeboxVisualState) ? ((JukeboxVisualState)value) : JukeboxVisualState.On, entity: Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component: component);
		}
	}

	private void UpdateAppearance(Entity<SpriteComponent> entity, JukeboxVisualState visualState, JukeboxComponent component)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		SetLayerState(JukeboxVisualLayers.Base, component.OffState, entity);
		switch (visualState)
		{
		case JukeboxVisualState.On:
			SetLayerState(JukeboxVisualLayers.Base, component.OnState, entity);
			break;
		case JukeboxVisualState.Off:
			SetLayerState(JukeboxVisualLayers.Base, component.OffState, entity);
			break;
		case JukeboxVisualState.Select:
			PlayAnimation(entity.Owner, JukeboxVisualLayers.Base, component.SelectState, 1f, Entity<SpriteComponent>.op_Implicit(entity));
			break;
		}
	}

	private void PlayAnimation(EntityUid uid, JukeboxVisualLayers layer, string? state, float animationTime, SpriteComponent sprite)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(state) && !_animationPlayer.HasRunningAnimation(uid, state))
		{
			Animation animation = GetAnimation(layer, state, animationTime);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)layer, true);
			_animationPlayer.Play(uid, animation, state);
		}
	}

	private static Animation GetAnimation(JukeboxVisualLayers layer, string state, float animationTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Expected O, but got Unknown
		return new Animation
		{
			Length = TimeSpan.FromSeconds(animationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = layer,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(state), 0f)
				}
			} }
		};
	}

	private void SetLayerState(JukeboxVisualLayers layer, string? state, Entity<SpriteComponent> sprite)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(state))
		{
			_sprite.LayerSetVisible(sprite.AsNullable(), (Enum)layer, true);
			_sprite.LayerSetAutoAnimated(sprite.AsNullable(), (Enum)layer, true);
			_sprite.LayerSetRsiState(sprite.AsNullable(), (Enum)layer, StateId.op_Implicit(state));
		}
	}
}
