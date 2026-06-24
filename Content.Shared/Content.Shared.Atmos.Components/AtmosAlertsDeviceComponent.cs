using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { })]
public sealed class AtmosAlertsDeviceComponent : Component, ISerializationGenerated<AtmosAlertsDeviceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables]
	public AtmosAlertsComputerGroup Group;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AtmosAlertsDeviceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AtmosAlertsDeviceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AtmosAlertsDeviceComponent>(this, ref target, hookCtx, false, context))
		{
			AtmosAlertsComputerGroup GroupTemp = AtmosAlertsComputerGroup.Invalid;
			if (!serialization.TryCustomCopy<AtmosAlertsComputerGroup>(Group, ref GroupTemp, hookCtx, false, context))
			{
				GroupTemp = Group;
			}
			target.Group = GroupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AtmosAlertsDeviceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlertsDeviceComponent cast = (AtmosAlertsDeviceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlertsDeviceComponent cast = (AtmosAlertsDeviceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlertsDeviceComponent def = (AtmosAlertsDeviceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AtmosAlertsDeviceComponent Instantiate()
	{
		return new AtmosAlertsDeviceComponent();
	}
}
