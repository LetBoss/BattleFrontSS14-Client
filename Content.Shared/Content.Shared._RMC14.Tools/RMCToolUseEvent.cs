using System;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Tools;

[ByRefEvent]
public record struct RMCToolUseEvent(EntityUid User, TimeSpan Delay, bool Handled = false);
