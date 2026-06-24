using System;
using System.Collections.Generic;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Implants.Components;

[RegisterComponent]
public sealed class RadioImplantComponent : Component, ISerializationGenerated<RadioImplantComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<RadioChannelPrototype>> RadioChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<RadioChannelPrototype>> ActiveAddedChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<RadioChannelPrototype>> TransmitterAddedChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RadioImplantComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RadioImplantComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RadioImplantComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<RadioChannelPrototype>> RadioChannelsTemp = null;
			if (RadioChannels == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(RadioChannels, ref RadioChannelsTemp, hookCtx, true, context))
			{
				RadioChannelsTemp = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(RadioChannels, hookCtx, context, false);
			}
			target.RadioChannels = RadioChannelsTemp;
			HashSet<ProtoId<RadioChannelPrototype>> ActiveAddedChannelsTemp = null;
			if (ActiveAddedChannels == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(ActiveAddedChannels, ref ActiveAddedChannelsTemp, hookCtx, true, context))
			{
				ActiveAddedChannelsTemp = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(ActiveAddedChannels, hookCtx, context, false);
			}
			target.ActiveAddedChannels = ActiveAddedChannelsTemp;
			HashSet<ProtoId<RadioChannelPrototype>> TransmitterAddedChannelsTemp = null;
			if (TransmitterAddedChannels == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(TransmitterAddedChannels, ref TransmitterAddedChannelsTemp, hookCtx, true, context))
			{
				TransmitterAddedChannelsTemp = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(TransmitterAddedChannels, hookCtx, context, false);
			}
			target.TransmitterAddedChannels = TransmitterAddedChannelsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RadioImplantComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadioImplantComponent cast = (RadioImplantComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadioImplantComponent cast = (RadioImplantComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadioImplantComponent def = (RadioImplantComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RadioImplantComponent Instantiate()
	{
		return new RadioImplantComponent();
	}
}
