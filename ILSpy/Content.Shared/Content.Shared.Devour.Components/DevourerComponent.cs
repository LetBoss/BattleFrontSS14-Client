using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Devour.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedDevourSystem) })]
public sealed class DevourerComponent : Component, ISerializationGenerated<DevourerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string? DevourAction = "ActionDevour";

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? DevourActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SoundDevour = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg", (AudioParams?)null)
	{
		Params = ((AudioParams)(ref AudioParams.Default)).WithVolume(-3f)
	};

	[DataField(null, false, 1, false, false, null)]
	public float DevourTime = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float StructureDevourTime = 10f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? SoundStructureDevour = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Machines/airlock_creaking.ogg", (AudioParams?)null)
	{
		Params = ((AudioParams)(ref AudioParams.Default)).WithVolume(-3f)
	};

	public Container Stomach;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist = new EntityWhitelist
	{
		Components = new string[1] { "MobState" }
	};

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? StomachStorageWhitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? FoodPreferenceWhitelist;

	[DataField(null, false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string Chemical = "Ichor";

	[DataField(null, false, 1, false, false, null)]
	public float HealRate = 15f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DevourerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DevourerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DevourerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string DevourActionTemp = null;
		if (!serialization.TryCustomCopy<string>(DevourAction, ref DevourActionTemp, hookCtx, false, context))
		{
			DevourActionTemp = DevourAction;
		}
		target.DevourAction = DevourActionTemp;
		EntityUid? DevourActionEntityTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(DevourActionEntity, ref DevourActionEntityTemp, hookCtx, false, context))
		{
			DevourActionEntityTemp = serialization.CreateCopy<EntityUid?>(DevourActionEntity, hookCtx, context, false);
		}
		target.DevourActionEntity = DevourActionEntityTemp;
		SoundSpecifier SoundDevourTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(SoundDevour, ref SoundDevourTemp, hookCtx, true, context))
		{
			SoundDevourTemp = serialization.CreateCopy<SoundSpecifier>(SoundDevour, hookCtx, context, false);
		}
		target.SoundDevour = SoundDevourTemp;
		float DevourTimeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(DevourTime, ref DevourTimeTemp, hookCtx, false, context))
		{
			DevourTimeTemp = DevourTime;
		}
		target.DevourTime = DevourTimeTemp;
		float StructureDevourTimeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(StructureDevourTime, ref StructureDevourTimeTemp, hookCtx, false, context))
		{
			StructureDevourTimeTemp = StructureDevourTime;
		}
		target.StructureDevourTime = StructureDevourTimeTemp;
		SoundSpecifier SoundStructureDevourTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(SoundStructureDevour, ref SoundStructureDevourTemp, hookCtx, true, context))
		{
			SoundStructureDevourTemp = serialization.CreateCopy<SoundSpecifier>(SoundStructureDevour, hookCtx, context, false);
		}
		target.SoundStructureDevour = SoundStructureDevourTemp;
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
		EntityWhitelist StomachStorageWhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(StomachStorageWhitelist, ref StomachStorageWhitelistTemp, hookCtx, false, context))
		{
			if (StomachStorageWhitelist == null)
			{
				StomachStorageWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(StomachStorageWhitelist, ref StomachStorageWhitelistTemp, hookCtx, context, false);
			}
		}
		target.StomachStorageWhitelist = StomachStorageWhitelistTemp;
		EntityWhitelist FoodPreferenceWhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(FoodPreferenceWhitelist, ref FoodPreferenceWhitelistTemp, hookCtx, false, context))
		{
			if (FoodPreferenceWhitelist == null)
			{
				FoodPreferenceWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(FoodPreferenceWhitelist, ref FoodPreferenceWhitelistTemp, hookCtx, context, false);
			}
		}
		target.FoodPreferenceWhitelist = FoodPreferenceWhitelistTemp;
		string ChemicalTemp = null;
		if (Chemical == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Chemical, ref ChemicalTemp, hookCtx, false, context))
		{
			ChemicalTemp = Chemical;
		}
		target.Chemical = ChemicalTemp;
		float HealRateTemp = 0f;
		if (!serialization.TryCustomCopy<float>(HealRate, ref HealRateTemp, hookCtx, false, context))
		{
			HealRateTemp = HealRate;
		}
		target.HealRate = HealRateTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DevourerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DevourerComponent cast = (DevourerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DevourerComponent cast = (DevourerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DevourerComponent def = (DevourerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DevourerComponent Instantiate()
	{
		return new DevourerComponent();
	}
}
