using System;
using Content.Shared.Flash;
using Content.Shared.Flash.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Flash;

public sealed class FlashSystem : SharedFlashSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	private FlashOverlay _overlay;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FlashedComponent, ComponentInit>((ComponentEventHandler<FlashedComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlashedComponent, ComponentShutdown>((ComponentEventHandler<FlashedComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlashedComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<FlashedComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlashedComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<FlashedComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		_overlay = new FlashOverlay();
	}

	private void OnPlayerAttached(EntityUid uid, FlashedComponent component, LocalPlayerAttachedEvent args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnPlayerDetached(EntityUid uid, FlashedComponent component, LocalPlayerDetachedEvent args)
	{
		_overlay.ScreenshotTexture = null;
		((Overlay)_overlay).RequestScreenTexture = false;
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}

	private void OnInit(EntityUid uid, FlashedComponent component, ComponentInit args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			((Overlay)_overlay).RequestScreenTexture = true;
			_overlayMan.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnShutdown(EntityUid uid, FlashedComponent component, ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			_overlay.ScreenshotTexture = null;
			((Overlay)_overlay).RequestScreenTexture = false;
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}
}
