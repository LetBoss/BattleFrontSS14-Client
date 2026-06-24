using System;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Stun;

[ByRefEvent]
public record struct DazedEvent(TimeSpan Duration);
