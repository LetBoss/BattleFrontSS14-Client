using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.ViewVariables;

namespace Content.Client.Inventory;

[RegisterComponent]
[Access(new Type[] { typeof(ClientInventorySystem) })]
public sealed class InventorySlotsComponent : Component, ISerializationGenerated<InventorySlotsComponent>, ISerializationGenerated
{
	[ViewVariables]
	public readonly Dictionary<string, ClientInventorySystem.SlotData> SlotData = new Dictionary<string, ClientInventorySystem.SlotData>();

	[ViewVariables]
	[Access(/*Could not decode attribute arguments.*/)]
	public readonly Dictionary<string, HashSet<string>> VisualLayerKeys = new Dictionary<string, HashSet<string>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InventorySlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (InventorySlotsComponent)(object)val;
		serialization.TryCustomCopy<InventorySlotsComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InventorySlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InventorySlotsComponent target2 = (InventorySlotsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InventorySlotsComponent target2 = (InventorySlotsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InventorySlotsComponent target2 = (InventorySlotsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InventorySlotsComponent Instantiate()
	{
		return new InventorySlotsComponent();
	}
}
