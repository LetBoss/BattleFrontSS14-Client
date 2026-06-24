using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Eye.Blinding;

public sealed class BlindingSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	[Dependency]
	private ILightManager _lightManager;

	private BlindOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, ComponentInit>((ComponentEventHandler<BlindableComponent, ComponentInit>)OnBlindInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, ComponentShutdown>((ComponentEventHandler<BlindableComponent, ComponentShutdown>)OnBlindShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<BlindableComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<BlindableComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)RoundRestartCleanup, (Type[])null, (Type[])null);
		_overlay = new BlindOverlay();
	}

	private void OnPlayerAttached(EntityUid uid, BlindableComponent component, LocalPlayerAttachedEvent args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnPlayerDetached(EntityUid uid, BlindableComponent component, LocalPlayerDetachedEvent args)
	{
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		_lightManager.Enabled = true;
	}

	private void OnBlindInit(EntityUid uid, BlindableComponent component, ComponentInit args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			_overlayMan.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnBlindShutdown(EntityUid uid, BlindableComponent component, ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}

	private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_lightManager.Enabled = true;
	}
}
