using System;
using Content.Shared.Drunk;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Drunk;

public sealed class DrunkSystem : SharedDrunkSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	private DrunkOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DrunkComponent, ComponentInit>((ComponentEventHandler<DrunkComponent, ComponentInit>)OnDrunkInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrunkComponent, ComponentShutdown>((ComponentEventHandler<DrunkComponent, ComponentShutdown>)OnDrunkShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrunkComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<DrunkComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrunkComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<DrunkComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		_overlay = new DrunkOverlay();
	}

	private void OnPlayerAttached(EntityUid uid, DrunkComponent component, LocalPlayerAttachedEvent args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnPlayerDetached(EntityUid uid, DrunkComponent component, LocalPlayerDetachedEvent args)
	{
		_overlay.CurrentBoozePower = 0f;
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}

	private void OnDrunkInit(EntityUid uid, DrunkComponent component, ComponentInit args)
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

	private void OnDrunkShutdown(EntityUid uid, DrunkComponent component, ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			_overlay.CurrentBoozePower = 0f;
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}
}
