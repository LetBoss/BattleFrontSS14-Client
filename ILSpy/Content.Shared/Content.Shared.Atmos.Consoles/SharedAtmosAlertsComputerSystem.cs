using System;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Atmos.Consoles;

public abstract class SharedAtmosAlertsComputerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AtmosAlertsComputerComponent, AtmosAlertsComputerDeviceSilencedMessage>((ComponentEventHandler<AtmosAlertsComputerComponent, AtmosAlertsComputerDeviceSilencedMessage>)OnDeviceSilencedMessage, (Type[])null, (Type[])null);
	}

	private void OnDeviceSilencedMessage(EntityUid uid, AtmosAlertsComputerComponent component, AtmosAlertsComputerDeviceSilencedMessage args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.SilenceDevice)
		{
			component.SilencedDevices.Add(args.AtmosDevice);
		}
		else
		{
			component.SilencedDevices.Remove(args.AtmosDevice);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
