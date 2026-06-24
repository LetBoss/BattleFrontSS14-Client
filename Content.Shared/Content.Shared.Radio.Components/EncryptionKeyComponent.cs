using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Radio.Components;

[RegisterComponent]
public sealed class EncryptionKeyComponent : Component, ISerializationGenerated<EncryptionKeyComponent>, ISerializationGenerated
{
	[DataField("channels", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
	public HashSet<string> Channels = new HashSet<string>();

	[DataField("defaultChannel", false, 1, false, false, typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
	public string? DefaultChannel;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<RadioChannelPrototype>> ReadOnlyChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EncryptionKeyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EncryptionKeyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EncryptionKeyComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<string> ChannelsTemp = null;
			if (Channels == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(Channels, ref ChannelsTemp, hookCtx, true, context))
			{
				ChannelsTemp = serialization.CreateCopy<HashSet<string>>(Channels, hookCtx, context, false);
			}
			target.Channels = ChannelsTemp;
			string DefaultChannelTemp = null;
			if (!serialization.TryCustomCopy<string>(DefaultChannel, ref DefaultChannelTemp, hookCtx, false, context))
			{
				DefaultChannelTemp = DefaultChannel;
			}
			target.DefaultChannel = DefaultChannelTemp;
			HashSet<ProtoId<RadioChannelPrototype>> ReadOnlyChannelsTemp = null;
			if (ReadOnlyChannels == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(ReadOnlyChannels, ref ReadOnlyChannelsTemp, hookCtx, true, context))
			{
				ReadOnlyChannelsTemp = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(ReadOnlyChannels, hookCtx, context, false);
			}
			target.ReadOnlyChannels = ReadOnlyChannelsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EncryptionKeyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EncryptionKeyComponent cast = (EncryptionKeyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EncryptionKeyComponent cast = (EncryptionKeyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EncryptionKeyComponent def = (EncryptionKeyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EncryptionKeyComponent Instantiate()
	{
		return new EncryptionKeyComponent();
	}
}
