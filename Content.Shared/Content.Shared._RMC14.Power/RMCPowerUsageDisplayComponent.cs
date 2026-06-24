using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Power;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedRMCPowerSystem) })]
public sealed class RMCPowerUsageDisplayComponent : Component, ISerializationGenerated<RMCPowerUsageDisplayComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string PowerText = "rmc-power-usage-display-defib";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCPowerUsageDisplayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCPowerUsageDisplayComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCPowerUsageDisplayComponent>(this, ref target, hookCtx, false, context))
		{
			string PowerTextTemp = null;
			if (PowerText == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PowerText, ref PowerTextTemp, hookCtx, false, context))
			{
				PowerTextTemp = PowerText;
			}
			target.PowerText = PowerTextTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCPowerUsageDisplayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCPowerUsageDisplayComponent cast = (RMCPowerUsageDisplayComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCPowerUsageDisplayComponent cast = (RMCPowerUsageDisplayComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCPowerUsageDisplayComponent def = (RMCPowerUsageDisplayComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCPowerUsageDisplayComponent Instantiate()
	{
		return new RMCPowerUsageDisplayComponent();
	}
}
