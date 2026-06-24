using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleDeployDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<VehicleDeployDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public bool Deploy;

	public override DoAfterEvent Clone()
	{
		return new VehicleDeployDoAfterEvent
		{
			Deploy = Deploy
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleDeployDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleDeployDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<VehicleDeployDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			bool DeployTemp = false;
			if (!serialization.TryCustomCopy<bool>(Deploy, ref DeployTemp, hookCtx, false, context))
			{
				DeployTemp = Deploy;
			}
			target.Deploy = DeployTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleDeployDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleDeployDoAfterEvent cast = (VehicleDeployDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleDeployDoAfterEvent cast = (VehicleDeployDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleDeployDoAfterEvent Instantiate()
	{
		return new VehicleDeployDoAfterEvent();
	}
}
