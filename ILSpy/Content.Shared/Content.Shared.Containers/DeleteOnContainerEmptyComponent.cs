using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Containers;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(DeleteOnContainerEmptySystem) })]
public sealed class DeleteOnContainerEmptyComponent : Component, ISerializationGenerated<DeleteOnContainerEmptyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string ContainerId = "storagebase";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DeleteOnContainerEmptyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DeleteOnContainerEmptyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DeleteOnContainerEmptyComponent>(this, ref target, hookCtx, false, context))
		{
			string ContainerIdTemp = null;
			if (ContainerId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ContainerId, ref ContainerIdTemp, hookCtx, false, context))
			{
				ContainerIdTemp = ContainerId;
			}
			target.ContainerId = ContainerIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DeleteOnContainerEmptyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeleteOnContainerEmptyComponent cast = (DeleteOnContainerEmptyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeleteOnContainerEmptyComponent cast = (DeleteOnContainerEmptyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeleteOnContainerEmptyComponent def = (DeleteOnContainerEmptyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DeleteOnContainerEmptyComponent Instantiate()
	{
		return new DeleteOnContainerEmptyComponent();
	}
}
