using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Mech;

[RegisterComponent]
public sealed class MechAssemblyVisualsComponent : Component, ISerializationGenerated<MechAssemblyVisualsComponent>, ISerializationGenerated
{
	[DataField("statePrefix", false, 1, true, false, null)]
	public string StatePrefix = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MechAssemblyVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (MechAssemblyVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<MechAssemblyVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string statePrefix = null;
			if (StatePrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StatePrefix, ref statePrefix, hookCtx, false, context))
			{
				statePrefix = StatePrefix;
			}
			target.StatePrefix = statePrefix;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MechAssemblyVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechAssemblyVisualsComponent target2 = (MechAssemblyVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechAssemblyVisualsComponent target2 = (MechAssemblyVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechAssemblyVisualsComponent target2 = (MechAssemblyVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MechAssemblyVisualsComponent Instantiate()
	{
		return new MechAssemblyVisualsComponent();
	}
}
