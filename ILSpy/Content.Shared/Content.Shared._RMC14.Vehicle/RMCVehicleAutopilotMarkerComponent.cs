using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class RMCVehicleAutopilotMarkerComponent : Component, ISerializationGenerated<RMCVehicleAutopilotMarkerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string MarkerName = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public List<string> Aliases = new List<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCVehicleAutopilotMarkerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCVehicleAutopilotMarkerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCVehicleAutopilotMarkerComponent>(this, ref target, hookCtx, false, context))
		{
			string MarkerNameTemp = null;
			if (MarkerName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(MarkerName, ref MarkerNameTemp, hookCtx, false, context))
			{
				MarkerNameTemp = MarkerName;
			}
			target.MarkerName = MarkerNameTemp;
			List<string> AliasesTemp = null;
			if (Aliases == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Aliases, ref AliasesTemp, hookCtx, true, context))
			{
				AliasesTemp = serialization.CreateCopy<List<string>>(Aliases, hookCtx, context, false);
			}
			target.Aliases = AliasesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCVehicleAutopilotMarkerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleAutopilotMarkerComponent cast = (RMCVehicleAutopilotMarkerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleAutopilotMarkerComponent cast = (RMCVehicleAutopilotMarkerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleAutopilotMarkerComponent def = (RMCVehicleAutopilotMarkerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCVehicleAutopilotMarkerComponent Instantiate()
	{
		return new RMCVehicleAutopilotMarkerComponent();
	}
}
