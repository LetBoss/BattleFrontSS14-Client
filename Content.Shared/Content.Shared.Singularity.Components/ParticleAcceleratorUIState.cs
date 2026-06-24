using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public sealed class ParticleAcceleratorUIState : BoundUserInterfaceState
{
	public bool Assembled;

	public bool Enabled;

	public ParticleAcceleratorPowerState State;

	public int PowerDraw;

	public int PowerReceive;

	public bool EmitterStarboardExists;

	public bool EmitterForeExists;

	public bool EmitterPortExists;

	public bool PowerBoxExists;

	public bool FuelChamberExists;

	public bool EndCapExists;

	public bool InterfaceBlock;

	public ParticleAcceleratorPowerState MaxLevel;

	public bool WirePowerBlock;

	public ParticleAcceleratorUIState(bool assembled, bool enabled, ParticleAcceleratorPowerState state, int powerReceive, int powerDraw, bool emitterStarboardExists, bool emitterForeExists, bool emitterPortExists, bool powerBoxExists, bool fuelChamberExists, bool endCapExists, bool interfaceBlock, ParticleAcceleratorPowerState maxLevel, bool wirePowerBlock)
	{
		Assembled = assembled;
		Enabled = enabled;
		State = state;
		PowerDraw = powerDraw;
		PowerReceive = powerReceive;
		EmitterStarboardExists = emitterStarboardExists;
		EmitterForeExists = emitterForeExists;
		EmitterPortExists = emitterPortExists;
		PowerBoxExists = powerBoxExists;
		FuelChamberExists = fuelChamberExists;
		EndCapExists = endCapExists;
		InterfaceBlock = interfaceBlock;
		MaxLevel = maxLevel;
		WirePowerBlock = wirePowerBlock;
	}
}
