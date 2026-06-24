using System;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.DungeonLayers;

public sealed class EntityTableDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<EntityTableDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int MinCount = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxCount = 1;

	[DataField(null, false, 1, true, false, null)]
	public EntityTableSelector Table;

	[DataField(null, false, 1, false, false, null)]
	public bool PerDungeon;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntityTableDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<EntityTableDunGen>(this, ref target, hookCtx, false, context))
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
			EntityTableSelector TableTemp = null;
			if (Table == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<EntityTableSelector>(Table, ref TableTemp, hookCtx, true, context))
			{
				TableTemp = serialization.CreateCopy<EntityTableSelector>(Table, hookCtx, context, false);
			}
			target.Table = TableTemp;
			bool PerDungeonTemp = false;
			if (!serialization.TryCustomCopy<bool>(PerDungeon, ref PerDungeonTemp, hookCtx, false, context))
			{
				PerDungeonTemp = PerDungeon;
			}
			target.PerDungeon = PerDungeonTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntityTableDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTableDunGen cast = (EntityTableDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTableDunGen def = (EntityTableDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public EntityTableDunGen Instantiate()
	{
		return new EntityTableDunGen();
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
