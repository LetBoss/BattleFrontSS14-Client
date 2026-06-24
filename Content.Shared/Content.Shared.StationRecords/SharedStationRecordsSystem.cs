using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.StationRecords;

public abstract class SharedStationRecordsSystem : EntitySystem
{
	public StationRecordKey? Convert((NetEntity, uint)? input)
	{
		if (input.HasValue)
		{
			return Convert(input.Value);
		}
		return null;
	}

	public (NetEntity, uint)? Convert(StationRecordKey? input)
	{
		if (input.HasValue)
		{
			return Convert(input.Value);
		}
		return null;
	}

	public StationRecordKey Convert((NetEntity, uint) input)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new StationRecordKey(input.Item2, ((EntitySystem)this).GetEntity(input.Item1));
	}

	public (NetEntity, uint) Convert(StationRecordKey input)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return (((EntitySystem)this).GetNetEntity(input.OriginStation, (MetaDataComponent)null), input.Id);
	}

	public List<(NetEntity, uint)> Convert(ICollection<StationRecordKey> input)
	{
		List<(NetEntity, uint)> result = new List<(NetEntity, uint)>(input.Count);
		foreach (StationRecordKey entry in input)
		{
			result.Add(Convert(entry));
		}
		return result;
	}

	public List<StationRecordKey> Convert(ICollection<(NetEntity, uint)> input)
	{
		List<StationRecordKey> result = new List<StationRecordKey>(input.Count);
		foreach (var entry in input)
		{
			result.Add(Convert(entry));
		}
		return result;
	}
}
