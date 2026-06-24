using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class PubgWeaponModuleSlotDefinition : ISerializationGenerated<PubgWeaponModuleSlotDefinition>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public PubgModuleSlotType Slot;

	[DataField(null, false, 1, false, false, null)]
	public PubgModuleUiAnchor UiAnchor = PubgModuleUiAnchor.Top;

	[DataField(null, false, 1, true, false, null)]
	public string DisplayNameLocKey = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> AllowedModules = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public string? ContainerId;

	[DataField(null, false, 1, false, false, null)]
	public bool RequiredForReloading;

	[DataField(null, false, 1, false, false, null)]
	public bool StoresAmmo;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? DefaultModule;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgWeaponModuleSlotDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PubgWeaponModuleSlotDefinition>(this, ref target, hookCtx, false, context))
		{
			PubgModuleSlotType SlotTemp = PubgModuleSlotType.Optic;
			if (!serialization.TryCustomCopy<PubgModuleSlotType>(Slot, ref SlotTemp, hookCtx, false, context))
			{
				SlotTemp = Slot;
			}
			target.Slot = SlotTemp;
			PubgModuleUiAnchor UiAnchorTemp = PubgModuleUiAnchor.TopLeft;
			if (!serialization.TryCustomCopy<PubgModuleUiAnchor>(UiAnchor, ref UiAnchorTemp, hookCtx, false, context))
			{
				UiAnchorTemp = UiAnchor;
			}
			target.UiAnchor = UiAnchorTemp;
			string DisplayNameLocKeyTemp = null;
			if (DisplayNameLocKey == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DisplayNameLocKey, ref DisplayNameLocKeyTemp, hookCtx, false, context))
			{
				DisplayNameLocKeyTemp = DisplayNameLocKey;
			}
			target.DisplayNameLocKey = DisplayNameLocKeyTemp;
			List<EntProtoId> AllowedModulesTemp = null;
			if (AllowedModules == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntProtoId>>(AllowedModules, ref AllowedModulesTemp, hookCtx, true, context))
			{
				AllowedModulesTemp = serialization.CreateCopy<List<EntProtoId>>(AllowedModules, hookCtx, context, false);
			}
			target.AllowedModules = AllowedModulesTemp;
			string ContainerIdTemp = null;
			if (!serialization.TryCustomCopy<string>(ContainerId, ref ContainerIdTemp, hookCtx, false, context))
			{
				ContainerIdTemp = ContainerId;
			}
			target.ContainerId = ContainerIdTemp;
			bool RequiredForReloadingTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiredForReloading, ref RequiredForReloadingTemp, hookCtx, false, context))
			{
				RequiredForReloadingTemp = RequiredForReloading;
			}
			target.RequiredForReloading = RequiredForReloadingTemp;
			bool StoresAmmoTemp = false;
			if (!serialization.TryCustomCopy<bool>(StoresAmmo, ref StoresAmmoTemp, hookCtx, false, context))
			{
				StoresAmmoTemp = StoresAmmo;
			}
			target.StoresAmmo = StoresAmmoTemp;
			EntProtoId? DefaultModuleTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(DefaultModule, ref DefaultModuleTemp, hookCtx, false, context))
			{
				DefaultModuleTemp = serialization.CreateCopy<EntProtoId?>(DefaultModule, hookCtx, context, false);
			}
			target.DefaultModule = DefaultModuleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgWeaponModuleSlotDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModuleSlotDefinition cast = (PubgWeaponModuleSlotDefinition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PubgWeaponModuleSlotDefinition Instantiate()
	{
		return new PubgWeaponModuleSlotDefinition();
	}
}
