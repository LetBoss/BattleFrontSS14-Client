using System;
using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class BiomeMarkerLayerDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<BiomeMarkerLayerDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int Count = 6;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<WeightedRandomPrototype> MarkerTemplate;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BiomeMarkerLayerDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<BiomeMarkerLayerDunGen>(this, ref target, hookCtx, false, context))
		{
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			ProtoId<WeightedRandomPrototype> MarkerTemplateTemp = default(ProtoId<WeightedRandomPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<WeightedRandomPrototype>>(MarkerTemplate, ref MarkerTemplateTemp, hookCtx, false, context))
			{
				MarkerTemplateTemp = serialization.CreateCopy<ProtoId<WeightedRandomPrototype>>(MarkerTemplate, hookCtx, context, false);
			}
			target.MarkerTemplate = MarkerTemplateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BiomeMarkerLayerDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeMarkerLayerDunGen cast = (BiomeMarkerLayerDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BiomeMarkerLayerDunGen def = (BiomeMarkerLayerDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BiomeMarkerLayerDunGen Instantiate()
	{
		return new BiomeMarkerLayerDunGen();
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
