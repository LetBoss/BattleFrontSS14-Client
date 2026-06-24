using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Lock;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(LockingWhitelistSystem) })]
public sealed class LockingWhitelistComponent : Component, ISerializationGenerated<LockingWhitelistComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LockingWhitelistComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LockingWhitelistComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<LockingWhitelistComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		target.Blacklist = BlacklistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LockingWhitelistComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LockingWhitelistComponent cast = (LockingWhitelistComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LockingWhitelistComponent cast = (LockingWhitelistComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LockingWhitelistComponent def = (LockingWhitelistComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LockingWhitelistComponent Instantiate()
	{
		return new LockingWhitelistComponent();
	}
}
