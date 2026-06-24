using System;
using Content.Shared.Parallax.Biomes.Markers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.Loot;

public sealed class BiomeMarkerLoot : IDungeonLoot, ISerializationGenerated<IDungeonLoot>, ISerializationGenerated, ISerializationGenerated<BiomeMarkerLoot>
{
	[DataField("proto", false, 1, true, false, null)]
	public ProtoId<BiomeMarkerLayerPrototype> Prototype;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeMarkerLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<BiomeMarkerLoot>(this, ref target, hookCtx, false, context))
		{
			ProtoId<BiomeMarkerLayerPrototype> PrototypeTemp = default(ProtoId<BiomeMarkerLayerPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<BiomeMarkerLayerPrototype>>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<ProtoId<BiomeMarkerLayerPrototype>>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeMarkerLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeMarkerLoot cast = (BiomeMarkerLoot)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDungeonLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeMarkerLoot def = (BiomeMarkerLoot)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDungeonLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeMarkerLoot Instantiate()
	{
		return new BiomeMarkerLoot();
	}

	IDungeonLoot IDungeonLoot.Instantiate()
	{
		return Instantiate();
	}

	IDungeonLoot ISerializationGenerated<IDungeonLoot>.Instantiate()
	{
		return Instantiate();
	}
}
