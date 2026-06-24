using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Vehicle.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(VehicleSystem) })]
public sealed class ContainerVehicleComponent : Component, ISerializationGenerated<ContainerVehicleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string ContainerId;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerVehicleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainerVehicleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ContainerVehicleComponent>(this, ref target, hookCtx, false, context))
		{
			string ContainerIdTemp = null;
			if (ContainerId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ContainerId, ref ContainerIdTemp, hookCtx, false, context))
			{
				ContainerIdTemp = ContainerId;
			}
			target.ContainerId = ContainerIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerVehicleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerVehicleComponent cast = (ContainerVehicleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerVehicleComponent cast = (ContainerVehicleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerVehicleComponent def = (ContainerVehicleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerVehicleComponent Instantiate()
	{
		return new ContainerVehicleComponent();
	}
}
