using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class NoiseDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<NoiseDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int Iterations = int.MaxValue;

	[DataField(null, false, 1, false, false, null)]
	public int TileCap = 128;

	[DataField(null, false, 1, false, false, null)]
	public float CapStd = 8f;

	[DataField(null, false, 1, true, false, null)]
	public List<NoiseDunGenLayer> Layers = new List<NoiseDunGenLayer>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NoiseDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<NoiseDunGen>(this, ref target, hookCtx, false, context))
		{
			int IterationsTemp = 0;
			if (!serialization.TryCustomCopy<int>(Iterations, ref IterationsTemp, hookCtx, false, context))
			{
				IterationsTemp = Iterations;
			}
			target.Iterations = IterationsTemp;
			int TileCapTemp = 0;
			if (!serialization.TryCustomCopy<int>(TileCap, ref TileCapTemp, hookCtx, false, context))
			{
				TileCapTemp = TileCap;
			}
			target.TileCap = TileCapTemp;
			float CapStdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CapStd, ref CapStdTemp, hookCtx, false, context))
			{
				CapStdTemp = CapStd;
			}
			target.CapStd = CapStdTemp;
			List<NoiseDunGenLayer> LayersTemp = null;
			if (Layers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<NoiseDunGenLayer>>(Layers, ref LayersTemp, hookCtx, true, context))
			{
				LayersTemp = serialization.CreateCopy<List<NoiseDunGenLayer>>(Layers, hookCtx, context, false);
			}
			target.Layers = LayersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NoiseDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NoiseDunGen cast = (NoiseDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NoiseDunGen def = (NoiseDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public NoiseDunGen Instantiate()
	{
		return new NoiseDunGen();
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
