using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Atmos.Monitor;

[Prototype("alarmThreshold", 1)]
public sealed class AtmosAlarmThresholdPrototype : IPrototype
{
	[DataField("ignore", false, 1, false, false, null)]
	public bool Ignore;

	[DataField("upperBound", false, 1, false, false, null)]
	public AlarmThresholdSetting UpperBound = AlarmThresholdSetting.Disabled;

	[DataField("lowerBound", false, 1, false, false, null)]
	public AlarmThresholdSetting LowerBound = AlarmThresholdSetting.Disabled;

	[DataField("upperWarnAround", false, 1, false, false, null)]
	public AlarmThresholdSetting UpperWarningPercentage = AlarmThresholdSetting.Disabled;

	[DataField("lowerWarnAround", false, 1, false, false, null)]
	public AlarmThresholdSetting LowerWarningPercentage = AlarmThresholdSetting.Disabled;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
