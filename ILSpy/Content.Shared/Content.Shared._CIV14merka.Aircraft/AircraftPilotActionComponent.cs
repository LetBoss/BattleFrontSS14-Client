using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Aircraft;

[RegisterComponent]
public sealed class AircraftPilotActionComponent : Component, ISerializationGenerated<AircraftPilotActionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Vehicle;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? AscendAction;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? DescendAction;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? BombAction;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AircraftPilotActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AircraftPilotActionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AircraftPilotActionComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? VehicleTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Vehicle, ref VehicleTemp, hookCtx, false, context))
			{
				VehicleTemp = serialization.CreateCopy<EntityUid?>(Vehicle, hookCtx, context, false);
			}
			target.Vehicle = VehicleTemp;
			EntityUid? AscendActionTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(AscendAction, ref AscendActionTemp, hookCtx, false, context))
			{
				AscendActionTemp = serialization.CreateCopy<EntityUid?>(AscendAction, hookCtx, context, false);
			}
			target.AscendAction = AscendActionTemp;
			EntityUid? DescendActionTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(DescendAction, ref DescendActionTemp, hookCtx, false, context))
			{
				DescendActionTemp = serialization.CreateCopy<EntityUid?>(DescendAction, hookCtx, context, false);
			}
			target.DescendAction = DescendActionTemp;
			EntityUid? BombActionTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(BombAction, ref BombActionTemp, hookCtx, false, context))
			{
				BombActionTemp = serialization.CreateCopy<EntityUid?>(BombAction, hookCtx, context, false);
			}
			target.BombAction = BombActionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AircraftPilotActionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AircraftPilotActionComponent cast = (AircraftPilotActionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AircraftPilotActionComponent cast = (AircraftPilotActionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AircraftPilotActionComponent def = (AircraftPilotActionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AircraftPilotActionComponent Instantiate()
	{
		return new AircraftPilotActionComponent();
	}
}
