using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MedicalScanner;

[Serializable]
[NetSerializable]
public sealed class HealthAnalyzerScannedUserMessage : BoundUserInterfaceMessage
{
	public readonly NetEntity? TargetEntity;

	public float Temperature;

	public float BloodLevel;

	public bool? ScanMode;

	public bool? Bleeding;

	public bool? Unrevivable;

	public HealthAnalyzerScannedUserMessage(NetEntity? targetEntity, float temperature, float bloodLevel, bool? scanMode, bool? bleeding, bool? unrevivable)
	{
		TargetEntity = targetEntity;
		Temperature = temperature;
		BloodLevel = bloodLevel;
		ScanMode = scanMode;
		Bleeding = bleeding;
		Unrevivable = unrevivable;
	}
}
