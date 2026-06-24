using System;
using Content.Shared.EntityTable;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class MiddleConnectionDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<MiddleConnectionDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int OverlapCount = -1;

	[DataField(null, false, 1, false, false, null)]
	public int Count = 1;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ContentTileDefinition> Tile;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<EntityTablePrototype> Contents;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<EntityTablePrototype>? Flank;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MiddleConnectionDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MiddleConnectionDunGen>(this, ref target, hookCtx, false, context))
		{
			int OverlapCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(OverlapCount, ref OverlapCountTemp, hookCtx, false, context))
			{
				OverlapCountTemp = OverlapCount;
			}
			target.OverlapCount = OverlapCountTemp;
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
			ProtoId<EntityTablePrototype>? FlankTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>?>(Flank, ref FlankTemp, hookCtx, false, context))
			{
				FlankTemp = serialization.CreateCopy<ProtoId<EntityTablePrototype>?>(Flank, hookCtx, context, false);
			}
			target.Flank = FlankTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MiddleConnectionDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MiddleConnectionDunGen cast = (MiddleConnectionDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MiddleConnectionDunGen def = (MiddleConnectionDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MiddleConnectionDunGen Instantiate()
	{
		return new MiddleConnectionDunGen();
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
