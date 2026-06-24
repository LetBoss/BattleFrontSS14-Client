using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class AtmosAlertsComputerDeviceSilencedMessage : BoundUserInterfaceMessage
{
	public NetEntity AtmosDevice;

	public bool SilenceDevice = true;

	public AtmosAlertsComputerDeviceSilencedMessage(NetEntity atmosDevice, bool silenceDevice = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		AtmosDevice = atmosDevice;
		SilenceDevice = silenceDevice;
	}
}
