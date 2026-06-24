using System;
using System.Collections.Generic;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Toggleable;

[RegisterComponent]
public sealed class ToggleableVisualsComponent : Component, ISerializationGenerated<ToggleableVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string? SpriteLayer;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HandLocation, List<PrototypeLayerData>> InhandVisuals = new Dictionary<HandLocation, List<PrototypeLayerData>>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, List<PrototypeLayerData>> ClothingVisuals = new Dictionary<string, List<PrototypeLayerData>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToggleableVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ToggleableVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<ToggleableVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string spriteLayer = null;
			if (!serialization.TryCustomCopy<string>(SpriteLayer, ref spriteLayer, hookCtx, false, context))
			{
				spriteLayer = SpriteLayer;
			}
			target.SpriteLayer = spriteLayer;
			Dictionary<HandLocation, List<PrototypeLayerData>> inhandVisuals = null;
			if (InhandVisuals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<HandLocation, List<PrototypeLayerData>>>(InhandVisuals, ref inhandVisuals, hookCtx, true, context))
			{
				inhandVisuals = serialization.CreateCopy<Dictionary<HandLocation, List<PrototypeLayerData>>>(InhandVisuals, hookCtx, context, false);
			}
			target.InhandVisuals = inhandVisuals;
			Dictionary<string, List<PrototypeLayerData>> clothingVisuals = null;
			if (ClothingVisuals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, List<PrototypeLayerData>>>(ClothingVisuals, ref clothingVisuals, hookCtx, true, context))
			{
				clothingVisuals = serialization.CreateCopy<Dictionary<string, List<PrototypeLayerData>>>(ClothingVisuals, hookCtx, context, false);
			}
			target.ClothingVisuals = clothingVisuals;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToggleableVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleableVisualsComponent target2 = (ToggleableVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleableVisualsComponent target2 = (ToggleableVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToggleableVisualsComponent target2 = (ToggleableVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToggleableVisualsComponent Instantiate()
	{
		return new ToggleableVisualsComponent();
	}
}
