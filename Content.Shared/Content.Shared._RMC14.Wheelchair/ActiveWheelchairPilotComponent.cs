using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Wheelchair;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(WheelchairSystem) })]
public sealed class ActiveWheelchairPilotComponent : Component, ISerializationGenerated<ActiveWheelchairPilotComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? BellActionEntity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActiveWheelchairPilotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActiveWheelchairPilotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActiveWheelchairPilotComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? BellActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(BellActionEntity, ref BellActionEntityTemp, hookCtx, false, context))
			{
				BellActionEntityTemp = serialization.CreateCopy<EntityUid?>(BellActionEntity, hookCtx, context, false);
			}
			target.BellActionEntity = BellActionEntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActiveWheelchairPilotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveWheelchairPilotComponent cast = (ActiveWheelchairPilotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveWheelchairPilotComponent cast = (ActiveWheelchairPilotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveWheelchairPilotComponent def = (ActiveWheelchairPilotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActiveWheelchairPilotComponent Instantiate()
	{
		return new ActiveWheelchairPilotComponent();
	}
}
