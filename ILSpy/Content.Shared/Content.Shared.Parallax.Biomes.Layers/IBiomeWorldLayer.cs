using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Parallax.Biomes.Layers;

public interface IBiomeWorldLayer : IBiomeLayer, ISerializationGenerated<IBiomeLayer>, ISerializationGenerated, ISerializationGenerated<IBiomeWorldLayer>
{
	List<string> AllowedTiles { get; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IBiomeWorldLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IBiomeWorldLayer>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IBiomeWorldLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IBiomeWorldLayer cast = (IBiomeWorldLayer)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void InternalCopy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IBiomeWorldLayer def = (IBiomeWorldLayer)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref IBiomeLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	new IBiomeWorldLayer Instantiate()
	{
		throw new NotImplementedException();
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
