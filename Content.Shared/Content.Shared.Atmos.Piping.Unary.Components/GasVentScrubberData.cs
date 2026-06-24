using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components;

[Serializable]
[NetSerializable]
public sealed class GasVentScrubberData : IAtmosDeviceData
{
	public static HashSet<Gas> DefaultFilterGases = new HashSet<Gas>
	{
		Gas.CarbonDioxide,
		Gas.Plasma,
		Gas.Tritium,
		Gas.WaterVapor,
		Gas.Ammonia,
		Gas.NitrousOxide,
		Gas.Frezon
	};

	public static GasVentScrubberData FilterModePreset = new GasVentScrubberData
	{
		Enabled = true,
		FilterGases = new HashSet<Gas>(DefaultFilterGases),
		PumpDirection = ScrubberPumpDirection.Scrubbing,
		VolumeRate = 200f,
		WideNet = false
	};

	public static GasVentScrubberData WideFilterModePreset = new GasVentScrubberData
	{
		Enabled = true,
		FilterGases = new HashSet<Gas>(DefaultFilterGases),
		PumpDirection = ScrubberPumpDirection.Scrubbing,
		VolumeRate = 200f,
		WideNet = true
	};

	public static GasVentScrubberData FillModePreset = new GasVentScrubberData
	{
		Enabled = false,
		Dirty = true,
		FilterGases = new HashSet<Gas>(DefaultFilterGases),
		PumpDirection = ScrubberPumpDirection.Scrubbing,
		VolumeRate = 200f,
		WideNet = false
	};

	public static GasVentScrubberData PanicModePreset = new GasVentScrubberData
	{
		Enabled = true,
		Dirty = true,
		FilterGases = new HashSet<Gas>(DefaultFilterGases),
		PumpDirection = ScrubberPumpDirection.Siphoning,
		VolumeRate = 200f,
		WideNet = true
	};

	public static GasVentScrubberData ReplaceModePreset = new GasVentScrubberData
	{
		Enabled = true,
		IgnoreAlarms = true,
		Dirty = true,
		FilterGases = new HashSet<Gas>(DefaultFilterGases),
		PumpDirection = ScrubberPumpDirection.Siphoning,
		VolumeRate = 200f,
		WideNet = false
	};

	public bool Enabled { get; set; }

	public bool Dirty { get; set; }

	public bool IgnoreAlarms { get; set; }

	public HashSet<Gas> FilterGases { get; set; } = new HashSet<Gas>(DefaultFilterGases);

	public ScrubberPumpDirection PumpDirection { get; set; } = ScrubberPumpDirection.Scrubbing;

	public float VolumeRate { get; set; } = 200f;

	public bool WideNet { get; set; }

	public bool AirAlarmPanicWireCut { get; set; }
}
