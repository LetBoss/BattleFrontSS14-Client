using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Requisitions;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RequisitionsEntry : ISerializationGenerated<RequisitionsEntry>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? Name;

	[DataField(null, false, 1, true, false, null)]
	public int Cost;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Crate;

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> Entities = new List<EntProtoId>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RequisitionsEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RequisitionsEntry>(this, ref target, hookCtx, false, context))
		{
			string NameTemp = null;
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			int CostTemp = 0;
			if (!serialization.TryCustomCopy<int>(Cost, ref CostTemp, hookCtx, false, context))
			{
				CostTemp = Cost;
			}
			target.Cost = CostTemp;
			EntProtoId CrateTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Crate, ref CrateTemp, hookCtx, false, context))
			{
				CrateTemp = serialization.CreateCopy<EntProtoId>(Crate, hookCtx, context, false);
			}
			target.Crate = CrateTemp;
			List<EntProtoId> EntitiesTemp = null;
			if (Entities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntProtoId>>(Entities, ref EntitiesTemp, hookCtx, true, context))
			{
				EntitiesTemp = serialization.CreateCopy<List<EntProtoId>>(Entities, hookCtx, context, false);
			}
			target.Entities = EntitiesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RequisitionsEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RequisitionsEntry cast = (RequisitionsEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RequisitionsEntry Instantiate()
	{
		return new RequisitionsEntry();
	}
}
