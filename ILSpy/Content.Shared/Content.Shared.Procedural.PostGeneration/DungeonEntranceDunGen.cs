using System;
using Content.Shared.EntityTable;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class DungeonEntranceDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<DungeonEntranceDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int Count = 1;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ContentTileDefinition> Tile;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<EntityTablePrototype> Contents;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DungeonEntranceDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DungeonEntranceDunGen>(this, ref target, hookCtx, false, context))
		{
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			ProtoId<ContentTileDefinition> TileTemp = default(ProtoId<ContentTileDefinition>);
			if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(Tile, ref TileTemp, hookCtx, false, context))
			{
				TileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(Tile, hookCtx, context, false);
			}
			target.Tile = TileTemp;
			ProtoId<EntityTablePrototype> ContentsTemp = default(ProtoId<EntityTablePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(Contents, ref ContentsTemp, hookCtx, false, context))
			{
				ContentsTemp = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(Contents, hookCtx, context, false);
			}
			target.Contents = ContentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DungeonEntranceDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DungeonEntranceDunGen cast = (DungeonEntranceDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DungeonEntranceDunGen def = (DungeonEntranceDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DungeonEntranceDunGen Instantiate()
	{
		return new DungeonEntranceDunGen();
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
