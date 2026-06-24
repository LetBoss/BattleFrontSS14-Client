using System;
using Robust.Shared.Noise;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Parallax.Biomes.Layers;

[ImplicitDataDefinitionForInheritors]
public interface IBiomeLayer : ISerializationGenerated<IBiomeLayer>, ISerializationGenerated
{
	FastNoiseLite Noise { get; }

	float Threshold { get; }

	bool Invert { get; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IBiomeLayer>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IBiomeLayer cast = (IBiomeLayer)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IBiomeLayer Instantiate()
	{
		throw new NotImplementedException();
	}
}
