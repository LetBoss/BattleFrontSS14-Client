using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[Access(new Type[] { typeof(VehicleDeploySystem) })]
public sealed class VehicleDeployGatedHardpointsComponent : Component, ISerializationGenerated<VehicleDeployGatedHardpointsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<string> BlockedHardpoints = new List<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleDeployGatedHardpointsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleDeployGatedHardpointsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleDeployGatedHardpointsComponent>(this, ref target, hookCtx, false, context))
		{
			List<string> BlockedHardpointsTemp = null;
			if (BlockedHardpoints == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(BlockedHardpoints, ref BlockedHardpointsTemp, hookCtx, true, context))
			{
				BlockedHardpointsTemp = serialization.CreateCopy<List<string>>(BlockedHardpoints, hookCtx, context, false);
			}
			target.BlockedHardpoints = BlockedHardpointsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleDeployGatedHardpointsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleDeployGatedHardpointsComponent cast = (VehicleDeployGatedHardpointsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleDeployGatedHardpointsComponent cast = (VehicleDeployGatedHardpointsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleDeployGatedHardpointsComponent def = (VehicleDeployGatedHardpointsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleDeployGatedHardpointsComponent Instantiate()
	{
		return new VehicleDeployGatedHardpointsComponent();
	}
}
