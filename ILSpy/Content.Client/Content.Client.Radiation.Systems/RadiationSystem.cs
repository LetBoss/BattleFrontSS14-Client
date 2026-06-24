using System;
using System.Collections.Generic;
using Content.Client.Radiation.Overlays;
using Content.Shared.Radiation.Events;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Radiation.Systems;

public sealed class RadiationSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayMan;

	public List<DebugRadiationRay>? Rays;

	public Dictionary<NetEntity, Dictionary<Vector2i, float>>? ResistanceGrids;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<OnRadiationOverlayToggledEvent>((EntityEventHandler<OnRadiationOverlayToggledEvent>)OnOverlayToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<OnRadiationOverlayUpdateEvent>((EntityEventHandler<OnRadiationOverlayUpdateEvent>)OnOverlayUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<OnRadiationOverlayResistanceUpdateEvent>((EntityEventHandler<OnRadiationOverlayResistanceUpdateEvent>)OnResistanceUpdate, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayMan.RemoveOverlay<RadiationDebugOverlay>();
	}

	private void OnOverlayToggled(OnRadiationOverlayToggledEvent ev)
	{
		if (ev.IsEnabled)
		{
			_overlayMan.AddOverlay((Overlay)(object)new RadiationDebugOverlay());
		}
		else
		{
			_overlayMan.RemoveOverlay<RadiationDebugOverlay>();
		}
	}

	private void OnOverlayUpdate(OnRadiationOverlayUpdateEvent ev)
	{
		RadiationDebugOverlay radiationDebugOverlay = default(RadiationDebugOverlay);
		if (_overlayMan.TryGetOverlay<RadiationDebugOverlay>(ref radiationDebugOverlay))
		{
			string text = $"Radiation update: {ev.ElapsedTimeMs}ms with. Receivers: {ev.ReceiversCount}, Sources: {ev.SourcesCount}, Rays: {ev.Rays.Count}";
			((EntitySystem)this).Log.Info(text);
			Rays = ev.Rays;
		}
	}

	private void OnResistanceUpdate(OnRadiationOverlayResistanceUpdateEvent ev)
	{
		ResistanceGrids = ev.Grids;
	}
}
