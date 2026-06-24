using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class BiomeDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<BiomeDunGen>
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<BiomeTemplatePrototype> BiomeTemplate;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<ContentTileDefinition>>? TileMask;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<BiomeDunGen>(this, ref target, hookCtx, false, context))
		{
			ProtoId<BiomeTemplatePrototype> BiomeTemplateTemp = default(ProtoId<BiomeTemplatePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<BiomeTemplatePrototype>>(BiomeTemplate, ref BiomeTemplateTemp, hookCtx, false, context))
			{
				BiomeTemplateTemp = serialization.CreateCopy<ProtoId<BiomeTemplatePrototype>>(BiomeTemplate, hookCtx, context, false);
			}
			target.BiomeTemplate = BiomeTemplateTemp;
			HashSet<ProtoId<ContentTileDefinition>> TileMaskTemp = null;
			if (!serialization.TryCustomCopy<HashSet<ProtoId<ContentTileDefinition>>>(TileMask, ref TileMaskTemp, hookCtx, true, context))
			{
				TileMaskTemp = serialization.CreateCopy<HashSet<ProtoId<ContentTileDefinition>>>(TileMask, hookCtx, context, false);
			}
			target.TileMask = TileMaskTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeDunGen cast = (BiomeDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeDunGen def = (BiomeDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeDunGen Instantiate()
	{
		return new BiomeDunGen();
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
