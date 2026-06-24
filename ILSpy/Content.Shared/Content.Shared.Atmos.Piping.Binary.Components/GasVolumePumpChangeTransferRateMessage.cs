using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasVolumePumpChangeTransferRateMessage : BoundUserInterfaceMessage
{
	public float TransferRate { get; }

	public GasVolumePumpChangeTransferRateMessage(float transferRate)
	{
		TransferRate = transferRate;
	}
}
