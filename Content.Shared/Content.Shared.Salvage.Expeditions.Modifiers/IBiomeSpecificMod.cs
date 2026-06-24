using System.Collections.Generic;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

public interface IBiomeSpecificMod : ISalvageMod
{
	List<string>? Biomes { get; }
}
