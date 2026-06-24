using System.Collections.Generic;

namespace Content.Client._CIV14merka.Squads;

public readonly record struct CivSquadRadialMenuState(int TeamId, string Title, string Description, IReadOnlyList<CivSquadRadialOption> Options);
