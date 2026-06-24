using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public readonly record struct BeingGibbedEvent(HashSet<EntityUid> GibbedParts);
