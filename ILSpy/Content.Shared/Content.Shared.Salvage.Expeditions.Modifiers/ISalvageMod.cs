using Robust.Shared.Localization;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

public interface ISalvageMod
{
	LocId Description { get; }

	float Cost { get; }
}
