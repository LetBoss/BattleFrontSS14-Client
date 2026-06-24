using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class PowerChargeState : BoundUserInterfaceState
{
	public bool On;

	public byte Charge;

	public PowerChargePowerStatus PowerStatus;

	public short PowerDraw;

	public short PowerDrawMax;

	public short EtaSeconds;

	public PowerChargeState(bool on, byte charge, PowerChargePowerStatus powerStatus, short powerDraw, short powerDrawMax, short etaSeconds)
	{
		On = on;
		Charge = charge;
		PowerStatus = powerStatus;
		PowerDraw = powerDraw;
		PowerDrawMax = powerDrawMax;
		EtaSeconds = etaSeconds;
	}
}
