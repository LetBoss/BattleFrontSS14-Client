using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.APC;

[Serializable]
[NetSerializable]
public sealed class ApcBoundInterfaceState : BoundUserInterfaceState, IEquatable<ApcBoundInterfaceState>
{
	public readonly bool MainBreaker;

	public readonly int Power;

	public readonly ApcExternalPowerState ApcExternalPower;

	public readonly float Charge;

	public ApcBoundInterfaceState(bool mainBreaker, int power, ApcExternalPowerState apcExternalPower, float charge)
	{
		MainBreaker = mainBreaker;
		Power = power;
		ApcExternalPower = apcExternalPower;
		Charge = charge;
	}

	public bool Equals(ApcBoundInterfaceState? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (MainBreaker == other.MainBreaker && Power == other.Power && ApcExternalPower == other.ApcExternalPower)
		{
			return MathHelper.CloseTo(Charge, other.Charge, 1E-07f);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is ApcBoundInterfaceState other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(MainBreaker, Power, (int)ApcExternalPower, Charge);
	}
}
