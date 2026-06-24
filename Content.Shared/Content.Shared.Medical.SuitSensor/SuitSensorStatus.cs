using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Medical.SuitSensor;

[Serializable]
[NetSerializable]
public sealed class SuitSensorStatus
{
	public TimeSpan Timestamp;

	public NetEntity SuitSensorUid;

	public NetEntity OwnerUid;

	public string Name;

	public string Job;

	public string JobIcon;

	public List<string> JobDepartments;

	public bool IsAlive;

	public int? TotalDamage;

	public int? TotalDamageThreshold;

	public NetCoordinates? Coordinates;

	public float? DamagePercentage
	{
		get
		{
			if (TotalDamageThreshold.HasValue && TotalDamage.HasValue)
			{
				return (float?)TotalDamage / (float)TotalDamageThreshold.Value;
			}
			return null;
		}
	}

	public SuitSensorStatus(NetEntity ownerUid, NetEntity suitSensorUid, string name, string job, string jobIcon, List<string> jobDepartments)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		OwnerUid = ownerUid;
		SuitSensorUid = suitSensorUid;
		Name = name;
		Job = job;
		JobIcon = jobIcon;
		JobDepartments = jobDepartments;
	}
}
