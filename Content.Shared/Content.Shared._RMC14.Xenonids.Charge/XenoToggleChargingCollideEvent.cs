using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Charge;

[ByRefEvent]
public record struct XenoToggleChargingCollideEvent(Entity<ActiveXenoToggleChargingComponent> Charger, bool Handled = false);
