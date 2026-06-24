using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
public sealed class UserInterfaceUserComponent : Component, ISerializationGenerated<UserInterfaceUserComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<EntityUid, List<Enum>> OpenInterfaces = new Dictionary<EntityUid, List<Enum>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UserInterfaceUserComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (UserInterfaceUserComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Dictionary<EntityUid, List<Enum>> target3 = null;
			if (OpenInterfaces == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(OpenInterfaces, ref target3, hookCtx, hasHooks: true, context))
			{
				target3 = serialization.CreateCopy(OpenInterfaces, hookCtx, context);
			}
			target.OpenInterfaces = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UserInterfaceUserComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceUserComponent target2 = (UserInterfaceUserComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceUserComponent target2 = (UserInterfaceUserComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UserInterfaceUserComponent target2 = (UserInterfaceUserComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UserInterfaceUserComponent Instantiate()
	{
		return new UserInterfaceUserComponent();
	}
}
