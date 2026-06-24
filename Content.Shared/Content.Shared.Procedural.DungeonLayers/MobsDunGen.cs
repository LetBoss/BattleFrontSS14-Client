using System;
using Content.Shared.EntityTable;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.DungeonLayers;

public sealed class MobsDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<MobsDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int MinCount = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxCount = 1;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<EntityTablePrototype> Contents;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MobsDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MobsDunGen>(this, ref target, hookCtx, false, context))
		{
			int MinCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinCount, ref MinCountTemp, hookCtx, false, context))
			{
				MinCountTemp = MinCount;
			}
			target.MinCount = MinCountTemp;
			int MaxCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxCount, ref MaxCountTemp, hookCtx, false, context))
			{
				MaxCountTemp = MaxCount;
			}
			target.MaxCount = MaxCountTemp;
			ProtoId<EntityTablePrototype> ContentsTemp = default(ProtoId<EntityTablePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(Contents, ref ContentsTemp, hookCtx, false, context))
			{
				ContentsTemp = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(Contents, hookCtx, context, false);
			}
			target.Contents = ContentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MobsDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobsDunGen cast = (MobsDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobsDunGen def = (MobsDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MobsDunGen Instantiate()
	{
		return new MobsDunGen();
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
