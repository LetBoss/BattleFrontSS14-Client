using System;
using Content.Shared.Emag.Systems;
using Content.Shared.Ninja.Systems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(EmagProviderSystem) })]
public sealed class EmagProviderComponent : Component, ISerializationGenerated<EmagProviderComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<TagPrototype> AccessBreakerImmuneTag = ProtoId<TagPrototype>.op_Implicit("AccessBreakerImmune");

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EmagType EmagType = EmagType.Access;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier EmagSound = (SoundSpecifier)new SoundCollectionSpecifier("sparks", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmagProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EmagProviderComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<EmagProviderComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ProtoId<TagPrototype> AccessBreakerImmuneTagTemp = default(ProtoId<TagPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(AccessBreakerImmuneTag, ref AccessBreakerImmuneTagTemp, hookCtx, false, context))
		{
			AccessBreakerImmuneTagTemp = serialization.CreateCopy<ProtoId<TagPrototype>>(AccessBreakerImmuneTag, hookCtx, context, false);
		}
		target.AccessBreakerImmuneTag = AccessBreakerImmuneTagTemp;
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
		EmagType EmagTypeTemp = EmagType.None;
		if (!serialization.TryCustomCopy<EmagType>(EmagType, ref EmagTypeTemp, hookCtx, false, context))
		{
			EmagTypeTemp = EmagType;
		}
		target.EmagType = EmagTypeTemp;
		SoundSpecifier EmagSoundTemp = null;
		if (EmagSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(EmagSound, ref EmagSoundTemp, hookCtx, true, context))
		{
			EmagSoundTemp = serialization.CreateCopy<SoundSpecifier>(EmagSound, hookCtx, context, false);
		}
		target.EmagSound = EmagSoundTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmagProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmagProviderComponent cast = (EmagProviderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmagProviderComponent cast = (EmagProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmagProviderComponent def = (EmagProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EmagProviderComponent Instantiate()
	{
		return new EmagProviderComponent();
	}
}
