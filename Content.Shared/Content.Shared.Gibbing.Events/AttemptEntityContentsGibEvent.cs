using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Gibbing.Events;

[ByRefEvent]
public record struct AttemptEntityContentsGibEvent(EntityUid Target, GibContentsOption GibType, List<string>? AllowedContainers, List<string>? ExcludedContainers);
