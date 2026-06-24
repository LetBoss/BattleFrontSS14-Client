using System;
using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class AtmosMonitoringConsoleDeviceComponent : Component, ISerializationGenerated<AtmosMonitoringConsoleDeviceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<NavMapBlipPrototype>? NavMapBlip;

	[DataField(null, false, 1, false, false, null)]
	public bool ShowAbsentConnections = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AtmosMonitoringConsoleDeviceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AtmosMonitoringConsoleDeviceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AtmosMonitoringConsoleDeviceComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<NavMapBlipPrototype>? NavMapBlipTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<NavMapBlipPrototype>?>(NavMapBlip, ref NavMapBlipTemp, hookCtx, false, context))
			{
				NavMapBlipTemp = serialization.CreateCopy<ProtoId<NavMapBlipPrototype>?>(NavMapBlip, hookCtx, context, false);
			}
			target.NavMapBlip = NavMapBlipTemp;
			bool ShowAbsentConnectionsTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShowAbsentConnections, ref ShowAbsentConnectionsTemp, hookCtx, false, context))
			{
				ShowAbsentConnectionsTemp = ShowAbsentConnections;
			}
			target.ShowAbsentConnections = ShowAbsentConnectionsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AtmosMonitoringConsoleDeviceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosMonitoringConsoleDeviceComponent cast = (AtmosMonitoringConsoleDeviceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosMonitoringConsoleDeviceComponent cast = (AtmosMonitoringConsoleDeviceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosMonitoringConsoleDeviceComponent def = (AtmosMonitoringConsoleDeviceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AtmosMonitoringConsoleDeviceComponent Instantiate()
	{
		return new AtmosMonitoringConsoleDeviceComponent();
	}
}
