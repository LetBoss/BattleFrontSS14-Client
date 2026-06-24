using System;
using Content.Shared.Eye.Blinding.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Eye.Blinding;

public sealed class BlurryVisionSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	private BlurryVisionOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BlurryVisionComponent, ComponentInit>((ComponentEventHandler<BlurryVisionComponent, ComponentInit>)OnBlurryInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlurryVisionComponent, ComponentShutdown>((ComponentEventHandler<BlurryVisionComponent, ComponentShutdown>)OnBlurryShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlurryVisionComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<BlurryVisionComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlurryVisionComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<BlurryVisionComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		_overlay = new BlurryVisionOverlay();
	}

	private void OnPlayerAttached(EntityUid uid, BlurryVisionComponent component, LocalPlayerAttachedEvent args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnPlayerDetached(EntityUid uid, BlurryVisionComponent component, LocalPlayerDetachedEvent args)
	{
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}

	private void OnBlurryInit(EntityUid uid, BlurryVisionComponent component, ComponentInit args)
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

	private void OnBlurryShutdown(EntityUid uid, BlurryVisionComponent component, ComponentShutdown args)
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
}
