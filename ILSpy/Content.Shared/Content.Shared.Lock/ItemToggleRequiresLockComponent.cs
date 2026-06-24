using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Lock;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(LockSystem) })]
public sealed class ItemToggleRequiresLockComponent : Component, ISerializationGenerated<ItemToggleRequiresLockComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool RequireLocked;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemToggleRequiresLockComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemToggleRequiresLockComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemToggleRequiresLockComponent>(this, ref target, hookCtx, false, context))
		{
			bool RequireLockedTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireLocked, ref RequireLockedTemp, hookCtx, false, context))
			{
				RequireLockedTemp = RequireLocked;
			}
			target.RequireLocked = RequireLockedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemToggleRequiresLockComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemToggleRequiresLockComponent cast = (ItemToggleRequiresLockComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemToggleRequiresLockComponent cast = (ItemToggleRequiresLockComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemToggleRequiresLockComponent def = (ItemToggleRequiresLockComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemToggleRequiresLockComponent Instantiate()
	{
		return new ItemToggleRequiresLockComponent();
	}
}
