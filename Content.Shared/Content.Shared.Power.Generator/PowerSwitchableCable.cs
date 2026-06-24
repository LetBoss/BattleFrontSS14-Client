using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Power.Generator;

[DataDefinition]
public sealed class PowerSwitchableCable : ISerializationGenerated<PowerSwitchableCable>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public SwitchableVoltage Voltage;

	[DataField(null, false, 1, true, false, null)]
	public string Node = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PowerSwitchableCable target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PowerSwitchableCable>(this, ref target, hookCtx, false, context))
		{
			SwitchableVoltage VoltageTemp = SwitchableVoltage.HV;
			if (!serialization.TryCustomCopy<SwitchableVoltage>(Voltage, ref VoltageTemp, hookCtx, false, context))
			{
				VoltageTemp = Voltage;
			}
			target.Voltage = VoltageTemp;
			string NodeTemp = null;
			if (Node == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Node, ref NodeTemp, hookCtx, false, context))
			{
				NodeTemp = Node;
			}
			target.Node = NodeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PowerSwitchableCable target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerSwitchableCable cast = (PowerSwitchableCable)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PowerSwitchableCable Instantiate()
	{
		return new PowerSwitchableCable();
	}
}
