using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.DungeonLayers;

public sealed class FillGridDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<FillGridDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<ContentTileDefinition>>? AllowedTiles;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Entity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FillGridDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<FillGridDunGen>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<ContentTileDefinition>> AllowedTilesTemp = null;
			if (!serialization.TryCustomCopy<HashSet<ProtoId<ContentTileDefinition>>>(AllowedTiles, ref AllowedTilesTemp, hookCtx, true, context))
			{
				AllowedTilesTemp = serialization.CreateCopy<HashSet<ProtoId<ContentTileDefinition>>>(AllowedTiles, hookCtx, context, false);
			}
			target.AllowedTiles = AllowedTilesTemp;
			EntProtoId EntityTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = serialization.CreateCopy<EntProtoId>(Entity, hookCtx, context, false);
			}
			target.Entity = EntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FillGridDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FillGridDunGen cast = (FillGridDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FillGridDunGen def = (FillGridDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public FillGridDunGen Instantiate()
	{
		return new FillGridDunGen();
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
