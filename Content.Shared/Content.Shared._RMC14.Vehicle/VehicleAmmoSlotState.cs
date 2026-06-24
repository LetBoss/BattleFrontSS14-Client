namespace Content.Shared._RMC14.Vehicle;

public readonly record struct VehicleAmmoSlotState(int SlotIndex, int Rounds, int Capacity, bool IsReadySlot);
