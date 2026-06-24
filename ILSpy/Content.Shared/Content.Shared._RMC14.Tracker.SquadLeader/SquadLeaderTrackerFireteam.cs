using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

[Serializable]
[DataDefinition]
[NetSerializable]
public record SquadLeaderTrackerFireteam : ISerializationGenerated<SquadLeaderTrackerFireteam>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SquadLeaderTrackerMarine? Leader;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<NetEntity, SquadLeaderTrackerMarine>? Members;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SquadLeaderTrackerFireteam target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<SquadLeaderTrackerFireteam>(this, ref target, hookCtx, false, context))
		{
			SquadLeaderTrackerMarine? LeaderTemp = null;
			if (!serialization.TryCustomCopy<SquadLeaderTrackerMarine?>(Leader, ref LeaderTemp, hookCtx, false, context))
			{
				LeaderTemp = serialization.CreateCopy<SquadLeaderTrackerMarine?>(Leader, hookCtx, context, false);
			}
			target.Leader = LeaderTemp;
			Dictionary<NetEntity, SquadLeaderTrackerMarine> MembersTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(Members, ref MembersTemp, hookCtx, true, context))
			{
				MembersTemp = serialization.CreateCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(Members, hookCtx, context, false);
			}
			target.Members = MembersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SquadLeaderTrackerFireteam target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SquadLeaderTrackerFireteam cast = (SquadLeaderTrackerFireteam)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual SquadLeaderTrackerFireteam Instantiate()
	{
		return new SquadLeaderTrackerFireteam();
	}
}
