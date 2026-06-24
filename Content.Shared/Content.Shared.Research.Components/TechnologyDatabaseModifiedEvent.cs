using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components;

[ByRefEvent]
public readonly record struct TechnologyDatabaseModifiedEvent(List<string>? NewlyUnlockedRecipes);
