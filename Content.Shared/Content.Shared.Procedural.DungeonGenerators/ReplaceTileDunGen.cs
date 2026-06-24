using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class ReplaceTileDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<ReplaceTileDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public float VariantWeight = 0.1f;

	[DataField(null, false, 1, true, false, null)]
	public List<ReplaceTileLayer> Layers = new List<ReplaceTileLayer>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReplaceTileDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ReplaceTileDunGen>(this, ref target, hookCtx, false, context))
		{
			float VariantWeightTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VariantWeight, ref VariantWeightTemp, hookCtx, false, context))
			{
				VariantWeightTemp = VariantWeight;
			}
			target.VariantWeight = VariantWeightTemp;
			List<ReplaceTileLayer> LayersTemp = null;
			if (Layers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ReplaceTileLayer>>(Layers, ref LayersTemp, hookCtx, true, context))
			{
				LayersTemp = serialization.CreateCopy<List<ReplaceTileLayer>>(Layers, hookCtx, context, false);
			}
			target.Layers = LayersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReplaceTileDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReplaceTileDunGen cast = (ReplaceTileDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReplaceTileDunGen def = (ReplaceTileDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ReplaceTileDunGen Instantiate()
	{
		return new ReplaceTileDunGen();
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
