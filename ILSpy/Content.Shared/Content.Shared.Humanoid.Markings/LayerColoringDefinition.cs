using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
public sealed class LayerColoringDefinition : ISerializationGenerated<LayerColoringDefinition>, ISerializationGenerated
{
	[DataField("type", false, 1, false, false, null)]
	public LayerColoringType? Type = new SkinColoring();

	[DataField("fallbackTypes", false, 1, false, false, null)]
	public List<LayerColoringType> FallbackTypes = new List<LayerColoringType>();

	[DataField("fallbackColor", false, 1, false, false, null)]
	public Color FallbackColor = Color.White;

	public Color GetColor(Color? skin, Color? eyes, MarkingSet markingSet)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Color? color = null;
		if (Type != null)
		{
			color = Type.GetColor(skin, eyes, markingSet);
		}
		if (!color.HasValue)
		{
			foreach (LayerColoringType fallbackType in FallbackTypes)
			{
				color = fallbackType.GetColor(skin, eyes, markingSet);
				if (color.HasValue)
				{
					break;
				}
			}
		}
		return (Color)(((_003F?)color) ?? FallbackColor);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LayerColoringDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<LayerColoringDefinition>(this, ref target, hookCtx, false, context))
		{
			LayerColoringType TypeTemp = null;
			if (!serialization.TryCustomCopy<LayerColoringType>(Type, ref TypeTemp, hookCtx, true, context))
			{
				TypeTemp = serialization.CreateCopy<LayerColoringType>(Type, hookCtx, context, false);
			}
			target.Type = TypeTemp;
			List<LayerColoringType> FallbackTypesTemp = null;
			if (FallbackTypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<LayerColoringType>>(FallbackTypes, ref FallbackTypesTemp, hookCtx, true, context))
			{
				FallbackTypesTemp = serialization.CreateCopy<List<LayerColoringType>>(FallbackTypes, hookCtx, context, false);
			}
			target.FallbackTypes = FallbackTypesTemp;
			Color FallbackColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(FallbackColor, ref FallbackColorTemp, hookCtx, false, context))
			{
				FallbackColorTemp = serialization.CreateCopy<Color>(FallbackColor, hookCtx, context, false);
			}
			target.FallbackColor = FallbackColorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LayerColoringDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerColoringDefinition cast = (LayerColoringDefinition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public LayerColoringDefinition Instantiate()
	{
		return new LayerColoringDefinition();
	}
}
