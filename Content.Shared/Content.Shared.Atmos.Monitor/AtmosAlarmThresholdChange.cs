namespace Content.Shared.Atmos.Monitor;

public readonly struct AtmosAlarmThresholdChange(AtmosMonitorLimitType type, AlarmThresholdSetting? previous, AlarmThresholdSetting current)
{
	public readonly AtmosMonitorLimitType Type = type;

	public readonly AlarmThresholdSetting? Previous = previous;

	public readonly AlarmThresholdSetting Current = current;
}
