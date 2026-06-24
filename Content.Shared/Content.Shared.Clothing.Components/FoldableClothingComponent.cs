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
public sealed class FoldableClothingComponent : Component, ISerializationGenerated<FoldableClothingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SlotFlags? FoldedSlots;

	[DataField(null, false, 1, false, false, null)]
	public SlotFlags? UnfoldedSlots;

	[DataField(null, false, 1, false, false, null)]
	public string? FoldedEquippedPrefix;

	[DataField(null, false, 1, false, false, null)]
	public string? FoldedHeldPrefix;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<HumanoidVisualLayers> UnfoldedHideLayers = new HashSet<HumanoidVisualLayers>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<HumanoidVisualLayers> FoldedHideLayers = new HashSet<HumanoidVisualLayers>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoldableClothingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FoldableClothingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FoldableClothingComponent>(this, ref target, hookCtx, false, context))
		{
			SlotFlags? FoldedSlotsTemp = null;
			if (!serialization.TryCustomCopy<SlotFlags?>(FoldedSlots, ref FoldedSlotsTemp, hookCtx, false, context))
			{
				FoldedSlotsTemp = FoldedSlots;
			}
			target.FoldedSlots = FoldedSlotsTemp;
			SlotFlags? UnfoldedSlotsTemp = null;
			if (!serialization.TryCustomCopy<SlotFlags?>(UnfoldedSlots, ref UnfoldedSlotsTemp, hookCtx, false, context))
			{
				UnfoldedSlotsTemp = UnfoldedSlots;
			}
			target.UnfoldedSlots = UnfoldedSlotsTemp;
			string FoldedEquippedPrefixTemp = null;
			if (!serialization.TryCustomCopy<string>(FoldedEquippedPrefix, ref FoldedEquippedPrefixTemp, hookCtx, false, context))
			{
				FoldedEquippedPrefixTemp = FoldedEquippedPrefix;
			}
			target.FoldedEquippedPrefix = FoldedEquippedPrefixTemp;
			string FoldedHeldPrefixTemp = null;
			if (!serialization.TryCustomCopy<string>(FoldedHeldPrefix, ref FoldedHeldPrefixTemp, hookCtx, false, context))
			{
				FoldedHeldPrefixTemp = FoldedHeldPrefix;
			}
			target.FoldedHeldPrefix = FoldedHeldPrefixTemp;
			HashSet<HumanoidVisualLayers> UnfoldedHideLayersTemp = null;
			if (UnfoldedHideLayers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(UnfoldedHideLayers, ref UnfoldedHideLayersTemp, hookCtx, true, context))
			{
				UnfoldedHideLayersTemp = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(UnfoldedHideLayers, hookCtx, context, false);
			}
			target.UnfoldedHideLayers = UnfoldedHideLayersTemp;
			HashSet<HumanoidVisualLayers> FoldedHideLayersTemp = null;
			if (FoldedHideLayers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(FoldedHideLayers, ref FoldedHideLayersTemp, hookCtx, true, context))
			{
				FoldedHideLayersTemp = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(FoldedHideLayers, hookCtx, context, false);
			}
			target.FoldedHideLayers = FoldedHideLayersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoldableClothingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoldableClothingComponent cast = (FoldableClothingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoldableClothingComponent cast = (FoldableClothingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoldableClothingComponent def = (FoldableClothingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoldableClothingComponent Instantiate()
	{
		return new FoldableClothingComponent();
	}
}
