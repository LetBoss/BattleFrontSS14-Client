using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stacks;

[RegisterComponent]
[NetworkedComponent]
public sealed class StackComponent : Component, ISerializationGenerated<StackComponent>, ISerializationGenerated
{
	[DataField("lingering", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Lingering;

	[DataField("baseLayer", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string BaseLayer = "";

	[DataField("composite", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool IsComposite;

	[DataField("layerStates", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<string> LayerStates = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public StackLayerFunction LayerFunction;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("stackType", false, 1, true, false, typeof(PrototypeIdSerializer<StackPrototype>))]
	public string StackTypeId { get; private set; }

	[DataField("count", false, 1, false, false, null)]
	public int Count { get; set; } = 30;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("maxCountOverride", false, 1, false, false, null)]
	public int? MaxCountOverride { get; set; }

	[DataField("unlimited", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Unlimited { get; set; }

	[DataField("throwIndividually", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool ThrowIndividually { get; set; }

	[ViewVariables]
	public bool UiUpdateNeeded { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StackComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StackComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StackComponent>(this, ref target, hookCtx, false, context))
		{
			string StackTypeIdTemp = null;
			if (StackTypeId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StackTypeId, ref StackTypeIdTemp, hookCtx, false, context))
			{
				StackTypeIdTemp = StackTypeId;
			}
			target.StackTypeId = StackTypeIdTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			int? MaxCountOverrideTemp = null;
			if (!serialization.TryCustomCopy<int?>(MaxCountOverride, ref MaxCountOverrideTemp, hookCtx, false, context))
			{
				MaxCountOverrideTemp = MaxCountOverride;
			}
			target.MaxCountOverride = MaxCountOverrideTemp;
			bool UnlimitedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Unlimited, ref UnlimitedTemp, hookCtx, false, context))
			{
				UnlimitedTemp = Unlimited;
			}
			target.Unlimited = UnlimitedTemp;
			bool LingeringTemp = false;
			if (!serialization.TryCustomCopy<bool>(Lingering, ref LingeringTemp, hookCtx, false, context))
			{
				LingeringTemp = Lingering;
			}
			target.Lingering = LingeringTemp;
			bool ThrowIndividuallyTemp = false;
			if (!serialization.TryCustomCopy<bool>(ThrowIndividually, ref ThrowIndividuallyTemp, hookCtx, false, context))
			{
				ThrowIndividuallyTemp = ThrowIndividually;
			}
			target.ThrowIndividually = ThrowIndividuallyTemp;
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
			StackLayerFunction LayerFunctionTemp = StackLayerFunction.None;
			if (!serialization.TryCustomCopy<StackLayerFunction>(LayerFunction, ref LayerFunctionTemp, hookCtx, false, context))
			{
				LayerFunctionTemp = LayerFunction;
			}
			target.LayerFunction = LayerFunctionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StackComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StackComponent cast = (StackComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StackComponent cast = (StackComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StackComponent def = (StackComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StackComponent Instantiate()
	{
		return new StackComponent();
	}
}
