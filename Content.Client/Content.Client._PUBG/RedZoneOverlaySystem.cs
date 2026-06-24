using System;
using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG;

public sealed class RedZoneOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	private RedZoneOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RedZoneStateEvent>((EntitySessionEventHandler<RedZoneStateEvent>)OnRedZoneStateUpdate, (Type[])null, (Type[])null);
		_overlay = new RedZoneOverlay();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
	}

	private void OnRedZoneStateUpdate(RedZoneStateEvent ev, EntitySessionEventArgs args)
	{
		if (_overlay != null)
		{
			_overlay.BombActive = ev.HasActiveBomb;
			_overlay.BombCenter = ev.BombCenter;
			_overlay.BombRadius = ev.BombRadius;
			if (ev.ZoneActive && ev.HasActiveBomb && ev.BombRadius > 0f && !_overlayManager.HasOverlay<RedZoneOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
			else if ((!ev.ZoneActive || !ev.HasActiveBomb || ev.BombRadius <= 0f) && _overlayManager.HasOverlay<RedZoneOverlay>())
			{
				_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			}
		}
	}
}
