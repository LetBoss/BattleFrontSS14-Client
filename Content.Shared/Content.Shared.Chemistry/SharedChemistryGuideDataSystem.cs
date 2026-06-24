using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry;

public abstract class SharedChemistryGuideDataSystem : EntitySystem
{
	[Dependency]
	protected IPrototypeManager PrototypeManager;

	protected readonly Dictionary<string, ReagentGuideEntry> Registry = new Dictionary<string, ReagentGuideEntry>();

	public IReadOnlyDictionary<string, ReagentGuideEntry> ReagentGuideRegistry => Registry;

	public abstract void ReloadAllReagentPrototypes();
}
