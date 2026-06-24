using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class PrefabDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<PrefabDunGen>
{
	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<DungeonPresetPrototype>> Presets = new List<ProtoId<DungeonPresetPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? RoomWhitelist;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ContentTileDefinition>? FallbackTile;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PrefabDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<PrefabDunGen>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		List<ProtoId<DungeonPresetPrototype>> PresetsTemp = null;
		if (Presets == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<ProtoId<DungeonPresetPrototype>>>(Presets, ref PresetsTemp, hookCtx, true, context))
		{
			PresetsTemp = serialization.CreateCopy<List<ProtoId<DungeonPresetPrototype>>>(Presets, hookCtx, context, false);
		}
		target.Presets = PresetsTemp;
		EntityWhitelist RoomWhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(RoomWhitelist, ref RoomWhitelistTemp, hookCtx, false, context))
		{
			if (RoomWhitelist == null)
			{
				RoomWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(RoomWhitelist, ref RoomWhitelistTemp, hookCtx, context, false);
			}
		}
		target.RoomWhitelist = RoomWhitelistTemp;
		ProtoId<ContentTileDefinition>? FallbackTileTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(FallbackTile, ref FallbackTileTemp, hookCtx, false, context))
		{
			FallbackTileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(FallbackTile, hookCtx, context, false);
		}
		target.FallbackTile = FallbackTileTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PrefabDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrefabDunGen cast = (PrefabDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrefabDunGen def = (PrefabDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PrefabDunGen Instantiate()
	{
		return new PrefabDunGen();
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
