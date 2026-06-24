namespace Content.Shared.Atmos.Monitor.Components;

public interface IAtmosDeviceData
{
	bool Enabled { get; set; }

	bool Dirty { get; set; }

	bool IgnoreAlarms { get; set; }
}
