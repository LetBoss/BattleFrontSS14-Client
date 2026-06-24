using System;
using System.Collections.Generic;
using Content.Shared.Procedural.Distance;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class NoiseDistanceDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<NoiseDistanceDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public IDunGenDistance? DistanceConfig;

	[DataField(null, false, 1, false, false, null)]
	public Vector2i Size;

	[DataField(null, false, 1, true, false, null)]
	public List<NoiseDunGenLayer> Layers = new List<NoiseDunGenLayer>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NoiseDistanceDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<NoiseDistanceDunGen>(this, ref target, hookCtx, false, context))
		{
			IDunGenDistance DistanceConfigTemp = null;
			if (!serialization.TryCustomCopy<IDunGenDistance>(DistanceConfig, ref DistanceConfigTemp, hookCtx, true, context))
			{
				DistanceConfigTemp = serialization.CreateCopy<IDunGenDistance>(DistanceConfig, hookCtx, context, false);
			}
			target.DistanceConfig = DistanceConfigTemp;
			Vector2i SizeTemp = default(Vector2i);
			if (!serialization.TryCustomCopy<Vector2i>(Size, ref SizeTemp, hookCtx, false, context))
			{
				SizeTemp = serialization.CreateCopy<Vector2i>(Size, hookCtx, context, false);
			}
			target.Size = SizeTemp;
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
	public void Copy(ref NoiseDistanceDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NoiseDistanceDunGen cast = (NoiseDistanceDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NoiseDistanceDunGen def = (NoiseDistanceDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public NoiseDistanceDunGen Instantiate()
	{
		return new NoiseDistanceDunGen();
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
