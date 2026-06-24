using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;

namespace Content.Shared.Robotics;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct CyborgControlData
{
	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier? ChassisSprite;

	[DataField(null, false, 1, true, false, null)]
	public string ChassisName;

	[DataField(null, false, 1, true, false, null)]
	public string Name;

	[DataField(null, false, 1, false, false, null)]
	public float Charge;

	[DataField(null, false, 1, false, false, null)]
	public int ModuleCount;

	[DataField(null, false, 1, false, false, null)]
	public bool HasBrain;

	[DataField(null, false, 1, false, false, null)]
	public bool CanDisable;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan Timeout;

	public CyborgControlData(SpriteSpecifier? chassisSprite, string chassisName, string name, float charge, int moduleCount, bool hasBrain, bool canDisable)
	{
		ChassisName = string.Empty;
		Name = string.Empty;
		Timeout = TimeSpan.Zero;
		ChassisSprite = chassisSprite;
		ChassisName = chassisName;
		Name = name;
		Charge = charge;
		ModuleCount = moduleCount;
		HasBrain = hasBrain;
		CanDisable = canDisable;
	}
}
