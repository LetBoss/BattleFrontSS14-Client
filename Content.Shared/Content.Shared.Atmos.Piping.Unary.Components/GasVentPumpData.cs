using System;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components;

[Serializable]
[NetSerializable]
public sealed class GasVentPumpData : IAtmosDeviceData
{
	public static GasVentPumpData FilterModePreset = new GasVentPumpData
	{
		Enabled = true,
		PumpDirection = VentPumpDirection.Releasing,
		PressureChecks = VentPressureBound.ExternalBound,
		ExternalPressureBound = 101.325f,
		InternalPressureBound = 0f,
		PressureLockoutOverride = false
	};

	public static GasVentPumpData FillModePreset = new GasVentPumpData
	{
		Enabled = true,
		Dirty = true,
		PumpDirection = VentPumpDirection.Releasing,
		PressureChecks = VentPressureBound.ExternalBound,
		ExternalPressureBound = 5066.25f,
		InternalPressureBound = 0f,
		PressureLockoutOverride = true
	};

	public static GasVentPumpData PanicModePreset = new GasVentPumpData
	{
		Enabled = false,
		Dirty = true,
		PumpDirection = VentPumpDirection.Releasing,
		PressureChecks = VentPressureBound.ExternalBound,
		ExternalPressureBound = 101.325f,
		InternalPressureBound = 0f,
		PressureLockoutOverride = false
	};

	public static GasVentPumpData ReplaceModePreset = new GasVentPumpData
	{
		Enabled = false,
		IgnoreAlarms = true,
		Dirty = true,
		PumpDirection = VentPumpDirection.Releasing,
		PressureChecks = VentPressureBound.ExternalBound,
		ExternalPressureBound = 101.325f,
		InternalPressureBound = 0f,
		PressureLockoutOverride = false
	};

	public bool Enabled { get; set; }

	public bool Dirty { get; set; }

	public bool IgnoreAlarms { get; set; }

	public VentPumpDirection PumpDirection { get; set; } = VentPumpDirection.Releasing;

	public VentPressureBound PressureChecks { get; set; } = VentPressureBound.ExternalBound;

	public float ExternalPressureBound { get; set; } = 101.325f;

	public float InternalPressureBound { get; set; }

	public bool PressureLockoutOverride { get; set; }
}
