using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Containers;

[RegisterComponent]
public sealed class ContainerFillComponent : Component, ISerializationGenerated<ContainerFillComponent>, ISerializationGenerated
{
	[DataField("containers", false, 1, false, false, typeof(ContainerFillSerializer))]
	public Dictionary<string, List<string>> Containers = new Dictionary<string, List<string>>();

	[DataField("ignoreConstructionSpawn", false, 1, false, false, null)]
	public bool IgnoreConstructionSpawn = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerFillComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainerFillComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ContainerFillComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, List<string>> ContainersTemp = null;
			if (Containers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(Containers, ref ContainersTemp, hookCtx, true, context))
			{
				ContainersTemp = serialization.CreateCopy<Dictionary<string, List<string>>>(Containers, hookCtx, context, false);
			}
			target.Containers = ContainersTemp;
			bool IgnoreConstructionSpawnTemp = false;
			if (!serialization.TryCustomCopy<bool>(IgnoreConstructionSpawn, ref IgnoreConstructionSpawnTemp, hookCtx, false, context))
			{
				IgnoreConstructionSpawnTemp = IgnoreConstructionSpawn;
			}
			target.IgnoreConstructionSpawn = IgnoreConstructionSpawnTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerFillComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerFillComponent cast = (ContainerFillComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerFillComponent cast = (ContainerFillComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerFillComponent def = (ContainerFillComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerFillComponent Instantiate()
	{
		return new ContainerFillComponent();
	}
}
