using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.ControlComputer;

[Serializable]
[ByRefEvent]
[NetSerializable]
public sealed record MarineControlComputerMedalNameEvent(NetEntity Actor, NetEntity? Marine, string Name, string? LastPlayerId = null);
