using System;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class BoundaryWallDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<BoundaryWallDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public BoundaryWallFlags Flags = BoundaryWallFlags.Rooms | BoundaryWallFlags.Corridors;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Wall;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? CornerWall;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ContentTileDefinition> Tile;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BoundaryWallDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<BoundaryWallDunGen>(this, ref target, hookCtx, false, context))
		{
			BoundaryWallFlags FlagsTemp = (BoundaryWallFlags)0;
			if (!serialization.TryCustomCopy<BoundaryWallFlags>(Flags, ref FlagsTemp, hookCtx, false, context))
			{
				FlagsTemp = Flags;
			}
			target.Flags = FlagsTemp;
			EntProtoId WallTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Wall, ref WallTemp, hookCtx, false, context))
			{
				WallTemp = serialization.CreateCopy<EntProtoId>(Wall, hookCtx, context, false);
			}
			target.Wall = WallTemp;
			EntProtoId? CornerWallTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(CornerWall, ref CornerWallTemp, hookCtx, false, context))
			{
				CornerWallTemp = serialization.CreateCopy<EntProtoId?>(CornerWall, hookCtx, context, false);
			}
			target.CornerWall = CornerWallTemp;
			ProtoId<ContentTileDefinition> TileTemp = default(ProtoId<ContentTileDefinition>);
			if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(Tile, ref TileTemp, hookCtx, false, context))
			{
				TileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(Tile, hookCtx, context, false);
			}
			target.Tile = TileTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BoundaryWallDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BoundaryWallDunGen cast = (BoundaryWallDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BoundaryWallDunGen def = (BoundaryWallDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BoundaryWallDunGen Instantiate()
	{
		return new BoundaryWallDunGen();
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
