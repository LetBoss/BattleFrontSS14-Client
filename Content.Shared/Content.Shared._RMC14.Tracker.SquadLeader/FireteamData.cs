using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

[Serializable]
[DataDefinition]
[NetSerializable]
public record FireteamData : ISerializationGenerated<FireteamData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SquadLeaderTrackerFireteam?[] Fireteams = new SquadLeaderTrackerFireteam[3];

	[DataField(null, false, 1, false, false, null)]
	public string? SquadLeader;

	[DataField(null, false, 1, false, false, null)]
	public NetEntity? SquadLeaderId;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<NetEntity, SquadLeaderTrackerMarine> Unassigned = new Dictionary<NetEntity, SquadLeaderTrackerMarine>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref FireteamData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<FireteamData>(this, ref target, hookCtx, false, context))
		{
			SquadLeaderTrackerFireteam[] FireteamsTemp = null;
			if (Fireteams == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SquadLeaderTrackerFireteam[]>(Fireteams, ref FireteamsTemp, hookCtx, true, context))
			{
				FireteamsTemp = serialization.CreateCopy<SquadLeaderTrackerFireteam[]>(Fireteams, hookCtx, context, false);
			}
			target.Fireteams = FireteamsTemp;
			string SquadLeaderTemp = null;
			if (!serialization.TryCustomCopy<string>(SquadLeader, ref SquadLeaderTemp, hookCtx, false, context))
			{
				SquadLeaderTemp = SquadLeader;
			}
			target.SquadLeader = SquadLeaderTemp;
			NetEntity? SquadLeaderIdTemp = null;
			if (!serialization.TryCustomCopy<NetEntity?>(SquadLeaderId, ref SquadLeaderIdTemp, hookCtx, false, context))
			{
				SquadLeaderIdTemp = serialization.CreateCopy<NetEntity?>(SquadLeaderId, hookCtx, context, false);
			}
			target.SquadLeaderId = SquadLeaderIdTemp;
			Dictionary<NetEntity, SquadLeaderTrackerMarine> UnassignedTemp = null;
			if (Unassigned == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(Unassigned, ref UnassignedTemp, hookCtx, true, context))
			{
				UnassignedTemp = serialization.CreateCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(Unassigned, hookCtx, context, false);
			}
			target.Unassigned = UnassignedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref FireteamData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireteamData cast = (FireteamData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual FireteamData Instantiate()
	{
		return new FireteamData();
	}
}
