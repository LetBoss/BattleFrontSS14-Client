using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Abilities.Goliath;

public sealed class GoliathSummonTentacleAction : WorldTargetActionEvent, ISerializationGenerated<GoliathSummonTentacleAction>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId EntityId = EntProtoId.op_Implicit("EffectGoliathTentacleSpawn");

	[DataField(null, false, 1, false, false, null)]
	public List<Direction> OffsetDirections = new List<Direction>
	{
		(Direction)4,
		(Direction)0,
		(Direction)2,
		(Direction)6
	};

	[DataField(null, false, 1, false, false, null)]
	public int ExtraSpawns = 3;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GoliathSummonTentacleAction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GoliathSummonTentacleAction)definitionCast;
		if (!serialization.TryCustomCopy<GoliathSummonTentacleAction>(this, ref target, hookCtx, false, context))
		{
			EntProtoId EntityIdTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(EntityId, ref EntityIdTemp, hookCtx, false, context))
			{
				EntityIdTemp = serialization.CreateCopy<EntProtoId>(EntityId, hookCtx, context, false);
			}
			target.EntityId = EntityIdTemp;
			List<Direction> OffsetDirectionsTemp = null;
			if (OffsetDirections == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<Direction>>(OffsetDirections, ref OffsetDirectionsTemp, hookCtx, true, context))
			{
				OffsetDirectionsTemp = serialization.CreateCopy<List<Direction>>(OffsetDirections, hookCtx, context, false);
			}
			target.OffsetDirections = OffsetDirectionsTemp;
			int ExtraSpawnsTemp = 0;
			if (!serialization.TryCustomCopy<int>(ExtraSpawns, ref ExtraSpawnsTemp, hookCtx, false, context))
			{
				ExtraSpawnsTemp = ExtraSpawns;
			}
			target.ExtraSpawns = ExtraSpawnsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GoliathSummonTentacleAction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GoliathSummonTentacleAction cast = (GoliathSummonTentacleAction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GoliathSummonTentacleAction cast = (GoliathSummonTentacleAction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GoliathSummonTentacleAction Instantiate()
	{
		return new GoliathSummonTentacleAction();
	}
}
