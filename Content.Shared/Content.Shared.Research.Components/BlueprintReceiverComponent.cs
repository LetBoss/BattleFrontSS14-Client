using System;
using Content.Shared.Research.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Research.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(BlueprintSystem) })]
public sealed class BlueprintReceiverComponent : Component, ISerializationGenerated<BlueprintReceiverComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string ContainerId = "blueprint";

	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist Whitelist = new EntityWhitelist();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BlueprintReceiverComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BlueprintReceiverComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<BlueprintReceiverComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
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
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BlueprintReceiverComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlueprintReceiverComponent cast = (BlueprintReceiverComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlueprintReceiverComponent cast = (BlueprintReceiverComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlueprintReceiverComponent def = (BlueprintReceiverComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BlueprintReceiverComponent Instantiate()
	{
		return new BlueprintReceiverComponent();
	}
}
