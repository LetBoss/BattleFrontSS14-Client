using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.ControlComputer;

[Serializable]
[ByRefEvent]
[NetSerializable]
public sealed record MarineControlComputerMedalMarineEvent(NetEntity Actor, NetEntity? Marine, string? LastPlayerId = null);
