using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Armor;

[ByRefEvent]
public record struct CMGetArmorEvent(SlotFlags TargetSlots, int XenoArmor = 0, int Melee = 0, int Bullet = 0, int Bio = 0, int FrontalArmor = 0, int SideArmor = 0, double ArmorModifier = 1.0, int ExplosionArmor = 0) : IInventoryRelayEvent;
