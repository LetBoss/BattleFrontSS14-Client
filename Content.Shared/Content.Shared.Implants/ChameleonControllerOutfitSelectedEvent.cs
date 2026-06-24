using Content.Shared.Inventory;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;

namespace Content.Shared.Implants;

[ByRefEvent]
public record struct ChameleonControllerOutfitSelectedEvent(ChameleonOutfitPrototype ChameleonOutfit, RoleLoadout? CustomRoleLoadout, RoleLoadout? DefaultRoleLoadout, StartingGearPrototype? JobStartingGearPrototype, StartingGearPrototype? StartingGearPrototype) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;
}
