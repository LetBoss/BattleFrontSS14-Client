using System;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class SplineDungeonConnectorDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<SplineDungeonConnectorDunGen>
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ContentTileDefinition> Tile;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ContentTileDefinition>? WidenTile;

	[DataField(null, false, 1, false, false, null)]
	public int DivisionDistance = 10;

	[DataField(null, false, 1, false, false, null)]
	public float VarianceMax = 0.35f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SplineDungeonConnectorDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<SplineDungeonConnectorDunGen>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ContentTileDefinition> TileTemp = default(ProtoId<ContentTileDefinition>);
			if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(Tile, ref TileTemp, hookCtx, false, context))
			{
				TileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(Tile, hookCtx, context, false);
			}
			target.Tile = TileTemp;
			ProtoId<ContentTileDefinition>? WidenTileTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(WidenTile, ref WidenTileTemp, hookCtx, false, context))
			{
				WidenTileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(WidenTile, hookCtx, context, false);
			}
			target.WidenTile = WidenTileTemp;
			int DivisionDistanceTemp = 0;
			if (!serialization.TryCustomCopy<int>(DivisionDistance, ref DivisionDistanceTemp, hookCtx, false, context))
			{
				DivisionDistanceTemp = DivisionDistance;
			}
			target.DivisionDistance = DivisionDistanceTemp;
			float VarianceMaxTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VarianceMax, ref VarianceMaxTemp, hookCtx, false, context))
			{
				VarianceMaxTemp = VarianceMax;
			}
			target.VarianceMax = VarianceMaxTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SplineDungeonConnectorDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SplineDungeonConnectorDunGen cast = (SplineDungeonConnectorDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SplineDungeonConnectorDunGen def = (SplineDungeonConnectorDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SplineDungeonConnectorDunGen Instantiate()
	{
		return new SplineDungeonConnectorDunGen();
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
