using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.PowerLoader;

[ByRefEvent]
public record struct PowerLoaderInteractEvent(EntityUid PowerLoader, EntityUid Target, EntityUid Used, List<EntityUid> Buckled, bool Handled = false);
