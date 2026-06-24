using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.DungeonLayers;

[Virtual]
public class OreDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<OreDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? Replacement;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Entity;

	[DataField(null, false, 1, false, false, null)]
	public int Count = 10;

	[DataField(null, false, 1, false, false, null)]
	public int MinGroupSize = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxGroupSize = 1;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref OreDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<OreDunGen>(this, ref target, hookCtx, false, context))
		{
			EntProtoId? ReplacementTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(Replacement, ref ReplacementTemp, hookCtx, false, context))
			{
				ReplacementTemp = serialization.CreateCopy<EntProtoId?>(Replacement, hookCtx, context, false);
			}
			target.Replacement = ReplacementTemp;
			EntProtoId EntityTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = serialization.CreateCopy<EntProtoId>(Entity, hookCtx, context, false);
			}
			target.Entity = EntityTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			int MinGroupSizeTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinGroupSize, ref MinGroupSizeTemp, hookCtx, false, context))
			{
				MinGroupSizeTemp = MinGroupSize;
			}
			target.MinGroupSize = MinGroupSizeTemp;
			int MaxGroupSizeTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxGroupSize, ref MaxGroupSizeTemp, hookCtx, false, context))
			{
				MaxGroupSizeTemp = MaxGroupSize;
			}
			target.MaxGroupSize = MaxGroupSizeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref OreDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OreDunGen cast = (OreDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OreDunGen def = (OreDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual OreDunGen Instantiate()
	{
		return new OreDunGen();
	}

	IDunGenLayer IDunGenLayer.Instantiate()
	{
		return Instantiate();
	}

	IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
	{
		return Instantiate();
	}
}
