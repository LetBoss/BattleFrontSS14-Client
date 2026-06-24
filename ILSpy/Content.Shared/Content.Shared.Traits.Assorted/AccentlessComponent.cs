using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Traits.Assorted;

[RegisterComponent]
[NetworkedComponent]
public sealed class AccentlessComponent : Component, ISerializationGenerated<AccentlessComponent>, ISerializationGenerated
{
	[DataField("removes", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ComponentRegistry RemovedAccents = new ComponentRegistry();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AccentlessComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AccentlessComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AccentlessComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry RemovedAccentsTemp = null;
			if (RemovedAccents == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(RemovedAccents, ref RemovedAccentsTemp, hookCtx, false, context))
			{
				RemovedAccentsTemp = serialization.CreateCopy<ComponentRegistry>(RemovedAccents, hookCtx, context, false);
			}
			target.RemovedAccents = RemovedAccentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AccentlessComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccentlessComponent cast = (AccentlessComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccentlessComponent cast = (AccentlessComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccentlessComponent def = (AccentlessComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AccentlessComponent Instantiate()
	{
		return new AccentlessComponent();
	}
}
