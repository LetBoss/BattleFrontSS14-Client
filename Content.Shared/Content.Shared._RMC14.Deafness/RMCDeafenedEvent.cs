using System;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Deafness;

[ByRefEvent]
public record struct RMCDeafenedEvent(TimeSpan Duration);
