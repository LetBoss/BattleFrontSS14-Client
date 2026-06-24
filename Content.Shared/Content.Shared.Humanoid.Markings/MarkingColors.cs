using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
public sealed class MarkingColors : ISerializationGenerated<MarkingColors>, ISerializationGenerated
{
	[DataField("default", true, 1, false, false, null)]
	public LayerColoringDefinition Default = new LayerColoringDefinition();

	[DataField("layers", true, 1, false, false, null)]
	public Dictionary<string, LayerColoringDefinition>? Layers;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MarkingColors target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<MarkingColors>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		LayerColoringDefinition DefaultTemp = null;
		if (Default == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<LayerColoringDefinition>(Default, ref DefaultTemp, hookCtx, false, context))
		{
			if (Default == null)
			{
				DefaultTemp = null;
			}
			else
			{
				serialization.CopyTo<LayerColoringDefinition>(Default, ref DefaultTemp, hookCtx, context, true);
			}
		}
		target.Default = DefaultTemp;
		Dictionary<string, LayerColoringDefinition> LayersTemp = null;
		if (!serialization.TryCustomCopy<Dictionary<string, LayerColoringDefinition>>(Layers, ref LayersTemp, hookCtx, true, context))
		{
			LayersTemp = serialization.CreateCopy<Dictionary<string, LayerColoringDefinition>>(Layers, hookCtx, context, false);
		}
		target.Layers = LayersTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MarkingColors target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MarkingColors cast = (MarkingColors)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MarkingColors Instantiate()
	{
		return new MarkingColors();
	}
}
