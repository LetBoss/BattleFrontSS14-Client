using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable]
[NetSerializable]
public sealed class BiomeTileLayer : IBiomeLayer, ISerializationGenerated<IBiomeLayer>, ISerializationGenerated, ISerializationGenerated<BiomeTileLayer>
{
	[DataField(null, false, 1, false, false, null)]
	public List<byte>? Variants;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ContentTileDefinition> Tile = ProtoId<ContentTileDefinition>.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

	[DataField(null, false, 1, false, false, null)]
	public float Threshold { get; private set; } = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public bool Invert { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeTileLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<BiomeTileLayer>(this, ref target, hookCtx, false, context))
		{
			return;
		}
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
		List<byte> VariantsTemp = null;
		if (!serialization.TryCustomCopy<List<byte>>(Variants, ref VariantsTemp, hookCtx, true, context))
		{
			VariantsTemp = serialization.CreateCopy<List<byte>>(Variants, hookCtx, context, false);
		}
		target.Variants = VariantsTemp;
		ProtoId<ContentTileDefinition> TileTemp = default(ProtoId<ContentTileDefinition>);
		if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(Tile, ref TileTemp, hookCtx, false, context))
		{
			TileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(Tile, hookCtx, context, false);
		}
		target.Tile = TileTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeTileLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeTileLayer cast = (BiomeTileLayer)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeTileLayer def = (BiomeTileLayer)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeTileLayer Instantiate()
	{
		return new BiomeTileLayer();
	}

	IBiomeLayer IBiomeLayer.Instantiate()
	{
		return Instantiate();
	}

	IBiomeLayer ISerializationGenerated<IBiomeLayer>.Instantiate()
	{
		return Instantiate();
	}
}
