using System;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.CriticalGrace;

[ByRefEvent]
public record struct GetCriticalGraceTimeEvent(TimeSpan Time);
