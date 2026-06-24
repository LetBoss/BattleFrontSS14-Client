using System;
using Content.Client.Atmos.Components;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Atmos.EntitySystems;

public sealed class AtmosphereSystem : SharedAtmosphereSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MapAtmosphereComponent, ComponentHandleState>((ComponentEventRefHandler<MapAtmosphereComponent, ComponentHandleState>)OnMapHandleState, (Type[])null, (Type[])null);
	}

	private void OnMapHandleState(EntityUid uid, MapAtmosphereComponent component, ref ComponentHandleState args)
	{
		if (((ComponentHandleState)(ref args)).Current is MapAtmosphereComponentState mapAtmosphereComponentState)
		{
			component.OverlayData = mapAtmosphereComponentState.Overlay;
		}
	}
}
