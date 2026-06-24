using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedAnomalyCoreSystem) })]
public sealed class CorePoweredThrowerComponent : Component, ISerializationGenerated<CorePoweredThrowerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string CoreSlotId = "core_slot";

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 StabilityPerThrow = new Vector2(0.1f, 0.2f);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CorePoweredThrowerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CorePoweredThrowerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CorePoweredThrowerComponent>(this, ref target, hookCtx, false, context))
		{
			string CoreSlotIdTemp = null;
			if (CoreSlotId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CoreSlotId, ref CoreSlotIdTemp, hookCtx, false, context))
			{
				CoreSlotIdTemp = CoreSlotId;
			}
			target.CoreSlotId = CoreSlotIdTemp;
			Vector2 StabilityPerThrowTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(StabilityPerThrow, ref StabilityPerThrowTemp, hookCtx, false, context))
			{
				StabilityPerThrowTemp = serialization.CreateCopy<Vector2>(StabilityPerThrow, hookCtx, context, false);
			}
			target.StabilityPerThrow = StabilityPerThrowTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CorePoweredThrowerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorePoweredThrowerComponent cast = (CorePoweredThrowerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorePoweredThrowerComponent cast = (CorePoweredThrowerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorePoweredThrowerComponent def = (CorePoweredThrowerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CorePoweredThrowerComponent Instantiate()
	{
		return new CorePoweredThrowerComponent();
	}
}
