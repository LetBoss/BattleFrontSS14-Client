using System;
using Content.Shared.Whitelist;
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
public sealed class GenericKeyedVehicleComponent : Component, ISerializationGenerated<GenericKeyedVehicleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string ContainerId;

	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist KeyWhitelist = new EntityWhitelist();

	[DataField(null, false, 1, false, false, null)]
	public bool PreventInvalidInsertion = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GenericKeyedVehicleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GenericKeyedVehicleComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<GenericKeyedVehicleComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
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
		EntityWhitelist KeyWhitelistTemp = null;
		if (KeyWhitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(KeyWhitelist, ref KeyWhitelistTemp, hookCtx, false, context))
		{
			if (KeyWhitelist == null)
			{
				KeyWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(KeyWhitelist, ref KeyWhitelistTemp, hookCtx, context, true);
			}
		}
		target.KeyWhitelist = KeyWhitelistTemp;
		bool PreventInvalidInsertionTemp = false;
		if (!serialization.TryCustomCopy<bool>(PreventInvalidInsertion, ref PreventInvalidInsertionTemp, hookCtx, false, context))
		{
			PreventInvalidInsertionTemp = PreventInvalidInsertion;
		}
		target.PreventInvalidInsertion = PreventInvalidInsertionTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GenericKeyedVehicleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericKeyedVehicleComponent cast = (GenericKeyedVehicleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericKeyedVehicleComponent cast = (GenericKeyedVehicleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericKeyedVehicleComponent def = (GenericKeyedVehicleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GenericKeyedVehicleComponent Instantiate()
	{
		return new GenericKeyedVehicleComponent();
	}
}
