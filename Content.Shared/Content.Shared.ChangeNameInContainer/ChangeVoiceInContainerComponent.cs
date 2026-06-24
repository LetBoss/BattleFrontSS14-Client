using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.ChangeNameInContainer;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ChangeNameInContainerSystem) })]
public sealed class ChangeVoiceInContainerComponent : Component, ISerializationGenerated<ChangeVoiceInContainerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChangeVoiceInContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChangeVoiceInContainerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ChangeVoiceInContainerComponent>(this, ref target, hookCtx, false, context))
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
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChangeVoiceInContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChangeVoiceInContainerComponent cast = (ChangeVoiceInContainerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChangeVoiceInContainerComponent cast = (ChangeVoiceInContainerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChangeVoiceInContainerComponent def = (ChangeVoiceInContainerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChangeVoiceInContainerComponent Instantiate()
	{
		return new ChangeVoiceInContainerComponent();
	}
}
