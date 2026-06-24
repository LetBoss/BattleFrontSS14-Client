using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.StationRecords;

public readonly struct StationRecordKey(uint id, EntityUid originStation) : IEquatable<StationRecordKey>
{
	[DataField(null, false, 1, false, false, null)]
	public readonly uint Id = id;

	[DataField("station", false, 1, false, false, null)]
	public readonly EntityUid OriginStation = originStation;

	public static StationRecordKey Invalid;

	public bool Equals(StationRecordKey other)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (Id == other.Id)
		{
			return OriginStation.Id == other.OriginStation.Id;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is StationRecordKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return HashCode.Combine<uint, EntityUid>(Id, OriginStation);
	}

	public bool IsValid()
	{
		return ((EntityUid)(ref OriginStation)).IsValid();
	}
}
