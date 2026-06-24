using System;
using Content.Shared.Item;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.ParcelWrap.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { })]
public sealed class ParcelWrapComponent : Component, ISerializationGenerated<ParcelWrapComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId ParcelPrototype;

	[DataField(null, false, 1, false, false, null)]
	public bool WrappedItemsMaintainSize = true;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ItemSizePrototype> FallbackItemSize = ProtoId<ItemSizePrototype>.op_Implicit("Ginormous");

	[DataField(null, false, 1, false, false, null)]
	public bool WrappedItemsMaintainShape;

	[DataField(null, false, 1, true, false, null)]
	public TimeSpan WrapDelay = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? WrapSound;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ParcelWrapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ParcelWrapComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ParcelWrapComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntProtoId ParcelPrototypeTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(ParcelPrototype, ref ParcelPrototypeTemp, hookCtx, false, context))
		{
			ParcelPrototypeTemp = serialization.CreateCopy<EntProtoId>(ParcelPrototype, hookCtx, context, false);
		}
		target.ParcelPrototype = ParcelPrototypeTemp;
		bool WrappedItemsMaintainSizeTemp = false;
		if (!serialization.TryCustomCopy<bool>(WrappedItemsMaintainSize, ref WrappedItemsMaintainSizeTemp, hookCtx, false, context))
		{
			WrappedItemsMaintainSizeTemp = WrappedItemsMaintainSize;
		}
		target.WrappedItemsMaintainSize = WrappedItemsMaintainSizeTemp;
		ProtoId<ItemSizePrototype> FallbackItemSizeTemp = default(ProtoId<ItemSizePrototype>);
		if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(FallbackItemSize, ref FallbackItemSizeTemp, hookCtx, false, context))
		{
			FallbackItemSizeTemp = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(FallbackItemSize, hookCtx, context, false);
		}
		target.FallbackItemSize = FallbackItemSizeTemp;
		bool WrappedItemsMaintainShapeTemp = false;
		if (!serialization.TryCustomCopy<bool>(WrappedItemsMaintainShape, ref WrappedItemsMaintainShapeTemp, hookCtx, false, context))
		{
			WrappedItemsMaintainShapeTemp = WrappedItemsMaintainShape;
		}
		target.WrappedItemsMaintainShape = WrappedItemsMaintainShapeTemp;
		TimeSpan WrapDelayTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(WrapDelay, ref WrapDelayTemp, hookCtx, false, context))
		{
			WrapDelayTemp = serialization.CreateCopy<TimeSpan>(WrapDelay, hookCtx, context, false);
		}
		target.WrapDelay = WrapDelayTemp;
		SoundSpecifier WrapSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(WrapSound, ref WrapSoundTemp, hookCtx, true, context))
		{
			WrapSoundTemp = serialization.CreateCopy<SoundSpecifier>(WrapSound, hookCtx, context, false);
		}
		target.WrapSound = WrapSoundTemp;
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
	public void Copy(ref ParcelWrapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParcelWrapComponent cast = (ParcelWrapComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParcelWrapComponent cast = (ParcelWrapComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParcelWrapComponent def = (ParcelWrapComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ParcelWrapComponent Instantiate()
	{
		return new ParcelWrapComponent();
	}
}
