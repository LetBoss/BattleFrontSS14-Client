using System;
using Content.Shared._RMC14.Stun;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Stun;

public sealed class DazedUiController : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEntityManager _entityManager;

	private DazedOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new DazedOverlay();
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerAttachedEvent>((EntityEventHandler<LocalPlayerAttachedEvent>)OnPlayerAttach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerDetachedEvent>((EntityEventHandler<LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDazedComponent, ComponentStartup>((EntityEventRefHandler<RMCDazedComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<DazedComponentShutdownEvent>((EntityEventHandler<DazedComponentShutdownEvent>)OnLocalPlayerDazedShutdown, (Type[])null, (Type[])null);
	}

	private void OnPlayerAttach(LocalPlayerAttachedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_overlay.IsEnabled = _entityManager.HasComponent<RMCDazedComponent>(args.Entity);
		if (!_overlayManager.HasOverlay<DazedOverlay>())
		{
			_overlayManager.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnPlayerDetached(LocalPlayerDetachedEvent args)
	{
		if (_overlayManager.HasOverlay<DazedOverlay>())
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		}
		_overlay.IsEnabled = false;
	}

	private void OnStartup(Entity<RMCDazedComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = Entity<RMCDazedComponent>.op_Implicit(ent);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && val == localEntity.GetValueOrDefault())
		{
			_overlay.IsEnabled = true;
			if (!_overlayManager.HasOverlay<DazedOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
		}
	}

	private void OnLocalPlayerDazedShutdown(DazedComponentShutdownEvent args)
	{
		_overlay.IsEnabled = false;
	}
}
