using System;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class MapAtmosphereComponentState : ComponentState
{
	public SharedGasTileOverlaySystem.GasOverlayData Overlay;

	public MapAtmosphereComponentState(SharedGasTileOverlaySystem.GasOverlayData overlay)
	{
		Overlay = overlay;
	}
}
