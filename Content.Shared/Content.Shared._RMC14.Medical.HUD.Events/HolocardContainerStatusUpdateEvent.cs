using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.HUD.Events;

[ByRefEvent]
public record struct HolocardContainerStatusUpdateEvent(HolocardStatus NewStatus);
