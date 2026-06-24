using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Hands;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(RMCHandsSystem) })]
public sealed class WhitelistPickupComponent : Component, ISerializationGenerated<WhitelistPickupComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist Whitelist = new EntityWhitelist();

	[DataField(null, false, 1, false, false, null)]
	public bool AllowDead;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WhitelistPickupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WhitelistPickupComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<WhitelistPickupComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (Whitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, true);
			}
		}
		target.Whitelist = WhitelistTemp;
		bool AllowDeadTemp = false;
		if (!serialization.TryCustomCopy<bool>(AllowDead, ref AllowDeadTemp, hookCtx, false, context))
		{
			AllowDeadTemp = AllowDead;
		}
		target.AllowDead = AllowDeadTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WhitelistPickupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WhitelistPickupComponent cast = (WhitelistPickupComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WhitelistPickupComponent cast = (WhitelistPickupComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WhitelistPickupComponent def = (WhitelistPickupComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WhitelistPickupComponent Instantiate()
	{
		return new WhitelistPickupComponent();
	}
}
