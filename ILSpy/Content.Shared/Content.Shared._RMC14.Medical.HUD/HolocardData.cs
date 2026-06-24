using Content.Shared.StatusIcon;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Medical.HUD;

public record struct HolocardData(ProtoId<HealthIconPrototype>? HolocardIcon, LocId Description);
