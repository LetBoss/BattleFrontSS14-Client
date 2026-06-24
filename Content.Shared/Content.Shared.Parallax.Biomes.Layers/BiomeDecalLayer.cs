using System;
using System.Collections.Generic;
using Content.Shared.Decals;
using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable]
[NetSerializable]
public sealed class BiomeDecalLayer : IBiomeWorldLayer, IBiomeLayer, ISerializationGenerated<IBiomeLayer>, ISerializationGenerated, ISerializationGenerated<IBiomeWorldLayer>, ISerializationGenerated<BiomeDecalLayer>
{
	[DataField("divisions", false, 1, false, false, null)]
	public float Divisions = 1f;

	[DataField("decals", false, 1, true, false, typeof(PrototypeIdListSerializer<DecalPrototype>))]
	public List<string> Decals = new List<string>();

	[DataField("allowedTiles", false, 1, false, false, typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
	public List<string> AllowedTiles { get; private set; } = new List<string>();

	[DataField("noise", false, 1, false, false, null)]
	public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

	[DataField("threshold", false, 1, false, false, null)]
	public float Threshold { get; private set; } = 0.8f;

	[DataField("invert", false, 1, false, false, null)]
	public bool Invert { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeDecalLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<BiomeDecalLayer>(this, ref target, hookCtx, false, context))
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
		float DivisionsTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Divisions, ref DivisionsTemp, hookCtx, false, context))
		{
			DivisionsTemp = Divisions;
		}
		target.Divisions = DivisionsTemp;
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
		List<string> DecalsTemp = null;
		if (Decals == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(Decals, ref DecalsTemp, hookCtx, true, context))
		{
			DecalsTemp = serialization.CreateCopy<List<string>>(Decals, hookCtx, context, false);
		}
		target.Decals = DecalsTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeDecalLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeDecalLayer cast = (BiomeDecalLayer)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IBiomeWorldLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeDecalLayer def = (BiomeDecalLayer)target;
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
		BiomeDecalLayer def = (BiomeDecalLayer)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeDecalLayer Instantiate()
	{
		return new BiomeDecalLayer();
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
