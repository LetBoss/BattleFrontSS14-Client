using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public sealed class PubgLoadoutWeaponSlotState
{
	public string SlotId { get; }

	public PubgModuleSlotType SlotType { get; }

	public PubgModuleUiAnchor UiAnchor { get; }

	public string DisplayNameLocKey { get; }

	public List<string> AllowedModulePrototypeIds { get; }

	public NetEntity AttachedModule { get; }

	public string? AttachedModuleName { get; }

	public PubgLoadoutWeaponSlotState(string slotId, PubgModuleSlotType slotType, PubgModuleUiAnchor uiAnchor, string displayNameLocKey, List<string> allowedModulePrototypeIds, NetEntity attachedModule, string? attachedModuleName)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		SlotId = slotId;
		SlotType = slotType;
		UiAnchor = uiAnchor;
		DisplayNameLocKey = displayNameLocKey;
		AllowedModulePrototypeIds = allowedModulePrototypeIds;
		AttachedModule = attachedModule;
		AttachedModuleName = attachedModuleName;
	}
}
