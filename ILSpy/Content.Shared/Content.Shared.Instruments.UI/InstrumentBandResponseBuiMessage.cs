using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments.UI;

[Serializable]
[NetSerializable]
public sealed class InstrumentBandResponseBuiMessage : BoundUserInterfaceMessage
{
	public (NetEntity, string)[] Nearby { get; set; }

	public InstrumentBandResponseBuiMessage((NetEntity, string)[] nearby)
	{
		Nearby = nearby;
	}
}
