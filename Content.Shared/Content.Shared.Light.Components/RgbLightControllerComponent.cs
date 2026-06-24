using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Light.Components;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] { typeof(SharedRgbLightControllerSystem) })]
public sealed class RgbLightControllerComponent : Component, ISerializationGenerated<RgbLightControllerComponent>, ISerializationGenerated
{
	[DataField("layers", false, 1, false, false, null)]
	public List<int>? Layers;

	public Color OriginalLightColor;

	public Dictionary<int, Color>? OriginalLayerColors;

	public EntityUid? Holder;

	public List<string>? HolderLayers;

	[DataField("cycleRate", false, 1, false, false, null)]
	public float CycleRate { get; set; } = 0.1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RgbLightControllerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RgbLightControllerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RgbLightControllerComponent>(this, ref target, hookCtx, false, context))
		{
			float CycleRateTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CycleRate, ref CycleRateTemp, hookCtx, false, context))
			{
				CycleRateTemp = CycleRate;
			}
			target.CycleRate = CycleRateTemp;
			List<int> LayersTemp = null;
			if (!serialization.TryCustomCopy<List<int>>(Layers, ref LayersTemp, hookCtx, true, context))
			{
				LayersTemp = serialization.CreateCopy<List<int>>(Layers, hookCtx, context, false);
			}
			target.Layers = LayersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RgbLightControllerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RgbLightControllerComponent cast = (RgbLightControllerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RgbLightControllerComponent cast = (RgbLightControllerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RgbLightControllerComponent def = (RgbLightControllerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RgbLightControllerComponent Instantiate()
	{
		return new RgbLightControllerComponent();
	}
}
