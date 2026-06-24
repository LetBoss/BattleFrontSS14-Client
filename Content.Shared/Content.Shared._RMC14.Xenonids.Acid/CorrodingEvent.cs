using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Acid;

[ByRefEvent]
public record struct CorrodingEvent(EntityUid Acid, float Dps, float ExpendableLightDps, XenoAcidStrength AcidStrength, bool Cancelled = false);
