using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedBorgSystem) })]
public sealed class ItemBorgModuleComponent : Component, ISerializationGenerated<ItemBorgModuleComponent>, ISerializationGenerated
{
	[DataField("items", false, 1, true, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
	public List<string> Items = new List<string>();

	[DataField("providedItems", false, 1, false, false, null)]
	public SortedDictionary<string, EntityUid> ProvidedItems = new SortedDictionary<string, EntityUid>();

	[DataField("handCounter", false, 1, false, false, null)]
	public int HandCounter;

	[DataField("itemsCrated", false, 1, false, false, null)]
	public bool ItemsCreated;

	[ViewVariables]
	public Container ProvidedContainer;

	[DataField("providedContainerId", false, 1, false, false, null)]
	public string ProvidedContainerId = "provided_container";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemBorgModuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemBorgModuleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemBorgModuleComponent>(this, ref target, hookCtx, false, context))
		{
			List<string> ItemsTemp = null;
			if (Items == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Items, ref ItemsTemp, hookCtx, true, context))
			{
				ItemsTemp = serialization.CreateCopy<List<string>>(Items, hookCtx, context, false);
			}
			target.Items = ItemsTemp;
			SortedDictionary<string, EntityUid> ProvidedItemsTemp = null;
			if (ProvidedItems == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SortedDictionary<string, EntityUid>>(ProvidedItems, ref ProvidedItemsTemp, hookCtx, true, context))
			{
				ProvidedItemsTemp = serialization.CreateCopy<SortedDictionary<string, EntityUid>>(ProvidedItems, hookCtx, context, false);
			}
			target.ProvidedItems = ProvidedItemsTemp;
			int HandCounterTemp = 0;
			if (!serialization.TryCustomCopy<int>(HandCounter, ref HandCounterTemp, hookCtx, false, context))
			{
				HandCounterTemp = HandCounter;
			}
			target.HandCounter = HandCounterTemp;
			bool ItemsCreatedTemp = false;
			if (!serialization.TryCustomCopy<bool>(ItemsCreated, ref ItemsCreatedTemp, hookCtx, false, context))
			{
				ItemsCreatedTemp = ItemsCreated;
			}
			target.ItemsCreated = ItemsCreatedTemp;
			string ProvidedContainerIdTemp = null;
			if (ProvidedContainerId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ProvidedContainerId, ref ProvidedContainerIdTemp, hookCtx, false, context))
			{
				ProvidedContainerIdTemp = ProvidedContainerId;
			}
			target.ProvidedContainerId = ProvidedContainerIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemBorgModuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemBorgModuleComponent cast = (ItemBorgModuleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemBorgModuleComponent cast = (ItemBorgModuleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemBorgModuleComponent def = (ItemBorgModuleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemBorgModuleComponent Instantiate()
	{
		return new ItemBorgModuleComponent();
	}
}
