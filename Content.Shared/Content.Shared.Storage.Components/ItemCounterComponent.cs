using System;
using System.Collections.Generic;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Storage.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedItemCounterSystem) })]
public sealed class ItemCounterComponent : Component, ISerializationGenerated<ItemCounterComponent>, ISerializationGenerated
{
	[DataField("baseLayer", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string BaseLayer = "";

	[DataField("composite", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool IsComposite;

	[DataField("layerStates", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<string> LayerStates = new List<string>();

	[DataField("count", false, 1, true, false, null)]
	public EntityWhitelist Count { get; set; }

	[DataField("amount", false, 1, false, false, null)]
	public int? MaxAmount { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemCounterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemCounterComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ItemCounterComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist CountTemp = null;
		if (Count == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Count, ref CountTemp, hookCtx, false, context))
		{
			if (Count == null)
			{
				CountTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Count, ref CountTemp, hookCtx, context, true);
			}
		}
		target.Count = CountTemp;
		int? MaxAmountTemp = null;
		if (!serialization.TryCustomCopy<int?>(MaxAmount, ref MaxAmountTemp, hookCtx, false, context))
		{
			MaxAmountTemp = MaxAmount;
		}
		target.MaxAmount = MaxAmountTemp;
		string BaseLayerTemp = null;
		if (BaseLayer == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(BaseLayer, ref BaseLayerTemp, hookCtx, false, context))
		{
			BaseLayerTemp = BaseLayer;
		}
		target.BaseLayer = BaseLayerTemp;
		bool IsCompositeTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsComposite, ref IsCompositeTemp, hookCtx, false, context))
		{
			IsCompositeTemp = IsComposite;
		}
		target.IsComposite = IsCompositeTemp;
		List<string> LayerStatesTemp = null;
		if (LayerStates == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(LayerStates, ref LayerStatesTemp, hookCtx, true, context))
		{
			LayerStatesTemp = serialization.CreateCopy<List<string>>(LayerStates, hookCtx, context, false);
		}
		target.LayerStates = LayerStatesTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemCounterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemCounterComponent cast = (ItemCounterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemCounterComponent cast = (ItemCounterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemCounterComponent def = (ItemCounterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemCounterComponent Instantiate()
	{
		return new ItemCounterComponent();
	}
}
