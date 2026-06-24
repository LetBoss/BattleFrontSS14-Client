using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Stacks;

[RegisterComponent]
[NetworkedComponent]
public sealed class StackLayerThresholdComponent : Component, ISerializationGenerated<StackLayerThresholdComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<int> Thresholds = new List<int>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StackLayerThresholdComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StackLayerThresholdComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StackLayerThresholdComponent>(this, ref target, hookCtx, false, context))
		{
			List<int> ThresholdsTemp = null;
			if (Thresholds == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<int>>(Thresholds, ref ThresholdsTemp, hookCtx, true, context))
			{
				ThresholdsTemp = serialization.CreateCopy<List<int>>(Thresholds, hookCtx, context, false);
			}
			target.Thresholds = ThresholdsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StackLayerThresholdComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StackLayerThresholdComponent cast = (StackLayerThresholdComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StackLayerThresholdComponent cast = (StackLayerThresholdComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StackLayerThresholdComponent def = (StackLayerThresholdComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StackLayerThresholdComponent Instantiate()
	{
		return new StackLayerThresholdComponent();
	}
}
