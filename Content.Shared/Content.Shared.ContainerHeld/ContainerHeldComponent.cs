using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.ContainerHeld;

[RegisterComponent]
[NetworkedComponent]
public sealed class ContainerHeldComponent : Component, ISerializationGenerated<ContainerHeldComponent>, ISerializationGenerated
{
	[DataField("threshold", false, 1, false, false, null)]
	public int Threshold { get; private set; } = 1;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerHeldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainerHeldComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ContainerHeldComponent>(this, ref target, hookCtx, false, context))
		{
			int ThresholdTemp = 0;
			if (!serialization.TryCustomCopy<int>(Threshold, ref ThresholdTemp, hookCtx, false, context))
			{
				ThresholdTemp = Threshold;
			}
			target.Threshold = ThresholdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerHeldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerHeldComponent cast = (ContainerHeldComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerHeldComponent cast = (ContainerHeldComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerHeldComponent def = (ContainerHeldComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerHeldComponent Instantiate()
	{
		return new ContainerHeldComponent();
	}
}
