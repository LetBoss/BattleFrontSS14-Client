using System;
using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Client._PUBG;

public sealed class PubgZoneSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IResourceCache _resourceCache;

	private PubgZoneOverlay? _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgZoneStateEvent>((EntitySessionEventHandler<PubgZoneStateEvent>)OnZoneStateUpdate, (Type[])null, (Type[])null);
		_overlay = new PubgZoneOverlay(_resourceCache);
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

	private void OnZoneStateUpdate(PubgZoneStateEvent ev, EntitySessionEventArgs args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (_overlay != null)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(ev.ZoneMapEntity);
			MapId zoneMapId = MapId.Nullspace;
			MapComponent val = default(MapComponent);
			if (((EntitySystem)this).TryComp<MapComponent>(entity, ref val))
			{
				zoneMapId = val.MapId;
			}
			_overlay.CurrentCenter = ev.CurrentCenter;
			_overlay.CurrentRadius = ev.CurrentRadius;
			_overlay.NextCenter = ev.NextCenter;
			_overlay.NextRadius = ev.NextRadius;
			_overlay.State = ev.State;
			_overlay.Active = ev.Active;
			_overlay.Visible = ev.Visible;
			_overlay.ZoneMapId = zoneMapId;
			if (ev.Active && !_overlayManager.HasOverlay<PubgZoneOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
			else if (!ev.Active && _overlayManager.HasOverlay<PubgZoneOverlay>())
			{
				_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			}
		}
	}
}
