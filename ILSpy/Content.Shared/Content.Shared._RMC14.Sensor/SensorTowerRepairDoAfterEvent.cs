using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Sensor;

[Serializable]
[NetSerializable]
public sealed class SensorTowerRepairDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<SensorTowerRepairDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SensorTowerState State;

	public SensorTowerRepairDoAfterEvent(SensorTowerState state)
	{
		State = state;
	}

	public SensorTowerRepairDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SensorTowerRepairDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SensorTowerRepairDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<SensorTowerRepairDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			SensorTowerState StateTemp = SensorTowerState.Weld;
			if (!serialization.TryCustomCopy<SensorTowerState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SensorTowerRepairDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SensorTowerRepairDoAfterEvent cast = (SensorTowerRepairDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SensorTowerRepairDoAfterEvent cast = (SensorTowerRepairDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SensorTowerRepairDoAfterEvent Instantiate()
	{
		return new SensorTowerRepairDoAfterEvent();
	}
}
