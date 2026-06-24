using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Marines.Squads;

[ByRefEvent]
public record struct GetMarineSquadNameEvent(string SquadName, string RoleName);
