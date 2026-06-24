using System;
using Robust.Shared.Noise;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable]
[NetSerializable]
public sealed class BiomeMetaLayer : IBiomeLayer, ISerializationGenerated<IBiomeLayer>, ISerializationGenerated, ISerializationGenerated<BiomeMetaLayer>
{
	[DataField("template", false, 1, true, false, typeof(PrototypeIdSerializer<BiomeTemplatePrototype>))]
	public string Template = string.Empty;

	[DataField("noise", false, 1, false, false, null)]
	public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

	[DataField("threshold", false, 1, false, false, null)]
	public float Threshold { get; private set; } = -1f;

	[DataField("invert", false, 1, false, false, null)]
	public bool Invert { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeMetaLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<BiomeMetaLayer>(this, ref target, hookCtx, false, context))
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
		string TemplateTemp = null;
		if (Template == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Template, ref TemplateTemp, hookCtx, false, context))
		{
			TemplateTemp = Template;
		}
		target.Template = TemplateTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeMetaLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeMetaLayer cast = (BiomeMetaLayer)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeMetaLayer def = (BiomeMetaLayer)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeMetaLayer Instantiate()
	{
		return new BiomeMetaLayer();
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
