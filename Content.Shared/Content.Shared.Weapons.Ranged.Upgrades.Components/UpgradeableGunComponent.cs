using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Weapons.Ranged.Upgrades.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(GunUpgradeSystem) })]
public sealed class UpgradeableGunComponent : Component, ISerializationGenerated<UpgradeableGunComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string UpgradesContainerId = "upgrades";

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist Whitelist = new EntityWhitelist();

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? InsertSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/thunk.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public int MaxUpgradeCount = 2;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UpgradeableGunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (UpgradeableGunComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<UpgradeableGunComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string UpgradesContainerIdTemp = null;
		if (UpgradesContainerId == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(UpgradesContainerId, ref UpgradesContainerIdTemp, hookCtx, false, context))
		{
			UpgradesContainerIdTemp = UpgradesContainerId;
		}
		target.UpgradesContainerId = UpgradesContainerIdTemp;
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
		SoundSpecifier InsertSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(InsertSound, ref InsertSoundTemp, hookCtx, true, context))
		{
			InsertSoundTemp = serialization.CreateCopy<SoundSpecifier>(InsertSound, hookCtx, context, false);
		}
		target.InsertSound = InsertSoundTemp;
		int MaxUpgradeCountTemp = 0;
		if (!serialization.TryCustomCopy<int>(MaxUpgradeCount, ref MaxUpgradeCountTemp, hookCtx, false, context))
		{
			MaxUpgradeCountTemp = MaxUpgradeCount;
		}
		target.MaxUpgradeCount = MaxUpgradeCountTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UpgradeableGunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UpgradeableGunComponent cast = (UpgradeableGunComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UpgradeableGunComponent cast = (UpgradeableGunComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UpgradeableGunComponent def = (UpgradeableGunComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UpgradeableGunComponent Instantiate()
	{
		return new UpgradeableGunComponent();
	}
}
