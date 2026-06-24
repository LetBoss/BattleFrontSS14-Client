using System;
using System.Collections.Generic;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class HideLayerClothingComponent : Component, ISerializationGenerated<HideLayerClothingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[Obsolete("This attribute is deprecated, please use Layers instead.")]
	public HashSet<HumanoidVisualLayers>? Slots = new HashSet<HumanoidVisualLayers>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, SlotFlags> Layers = new Dictionary<HumanoidVisualLayers, SlotFlags>();

	[DataField(null, false, 1, false, false, null)]
	public bool HideOnToggle;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HideLayerClothingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HideLayerClothingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HideLayerClothingComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<HumanoidVisualLayers> SlotsTemp = null;
			if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(Slots, ref SlotsTemp, hookCtx, true, context))
			{
				SlotsTemp = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(Slots, hookCtx, context, false);
			}
			target.Slots = SlotsTemp;
			Dictionary<HumanoidVisualLayers, SlotFlags> LayersTemp = null;
			if (Layers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(Layers, ref LayersTemp, hookCtx, true, context))
			{
				LayersTemp = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(Layers, hookCtx, context, false);
			}
			target.Layers = LayersTemp;
			bool HideOnToggleTemp = false;
			if (!serialization.TryCustomCopy<bool>(HideOnToggle, ref HideOnToggleTemp, hookCtx, false, context))
			{
				HideOnToggleTemp = HideOnToggle;
			}
			target.HideOnToggle = HideOnToggleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HideLayerClothingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HideLayerClothingComponent cast = (HideLayerClothingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HideLayerClothingComponent cast = (HideLayerClothingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HideLayerClothingComponent def = (HideLayerClothingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HideLayerClothingComponent Instantiate()
	{
		return new HideLayerClothingComponent();
	}
}
