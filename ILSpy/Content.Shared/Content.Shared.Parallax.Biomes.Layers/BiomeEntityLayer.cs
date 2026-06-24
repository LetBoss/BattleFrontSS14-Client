using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable]
[NetSerializable]
public sealed class BiomeEntityLayer : IBiomeWorldLayer, IBiomeLayer, ISerializationGenerated<IBiomeLayer>, ISerializationGenerated, ISerializationGenerated<IBiomeWorldLayer>, ISerializationGenerated<BiomeEntityLayer>
{
	[DataField("entities", false, 1, true, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
	public List<string> Entities = new List<string>();

	[DataField("allowedTiles", false, 1, false, false, typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
	public List<string> AllowedTiles { get; private set; } = new List<string>();

	[DataField("noise", false, 1, false, false, null)]
	public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

	[DataField("threshold", false, 1, false, false, null)]
	public float Threshold { get; private set; } = 0.5f;

	[DataField("invert", false, 1, false, false, null)]
	public bool Invert { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeEntityLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<BiomeEntityLayer>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		List<string> AllowedTilesTemp = null;
		if (AllowedTiles == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(AllowedTiles, ref AllowedTilesTemp, hookCtx, true, context))
		{
			AllowedTilesTemp = serialization.CreateCopy<List<string>>(AllowedTiles, hookCtx, context, false);
		}
		target.AllowedTiles = AllowedTilesTemp;
		FastNoiseLite NoiseTemp = null;
		if (Noise == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<FastNoiseLite>(Noise, ref NoiseTemp, hookCtx, true, context))
		{
			if (Noise == null)
			{
				NoiseTemp = null;
			}
			else
			{
				serialization.CopyTo<FastNoiseLite>(Noise, ref NoiseTemp, hookCtx, context, true);
			}
		}
		target.Noise = NoiseTemp;
		float ThresholdTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Threshold, ref ThresholdTemp, hookCtx, false, context))
		{
			ThresholdTemp = Threshold;
		}
		target.Threshold = ThresholdTemp;
		bool InvertTemp = false;
		if (!serialization.TryCustomCopy<bool>(Invert, ref InvertTemp, hookCtx, false, context))
		{
			InvertTemp = Invert;
		}
		target.Invert = InvertTemp;
		List<string> EntitiesTemp = null;
		if (Entities == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(Entities, ref EntitiesTemp, hookCtx, true, context))
		{
			EntitiesTemp = serialization.CreateCopy<List<string>>(Entities, hookCtx, context, false);
		}
		target.Entities = EntitiesTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeEntityLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeEntityLayer cast = (BiomeEntityLayer)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IBiomeWorldLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeEntityLayer def = (BiomeEntityLayer)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IBiomeWorldLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeEntityLayer def = (BiomeEntityLayer)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeEntityLayer Instantiate()
	{
		return new BiomeEntityLayer();
	}

	IBiomeWorldLayer IBiomeWorldLayer.Instantiate()
	{
		return Instantiate();
	}

	IBiomeWorldLayer ISerializationGenerated<IBiomeWorldLayer>.Instantiate()
	{
		return Instantiate();
	}
}
