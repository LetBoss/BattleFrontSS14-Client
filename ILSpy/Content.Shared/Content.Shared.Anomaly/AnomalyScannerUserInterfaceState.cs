using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Anomaly;

[Serializable]
[NetSerializable]
public sealed class AnomalyScannerUserInterfaceState : BoundUserInterfaceState
{
	public FormattedMessage Message;

	public TimeSpan? NextPulseTime;

	public AnomalyScannerUserInterfaceState(FormattedMessage message, TimeSpan? nextPulseTime)
	{
		Message = message;
		NextPulseTime = nextPulseTime;
	}
}
