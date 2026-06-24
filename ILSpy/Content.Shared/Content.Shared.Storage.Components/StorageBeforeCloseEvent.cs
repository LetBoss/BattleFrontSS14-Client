using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components;

[ByRefEvent]
public readonly record struct StorageBeforeCloseEvent(HashSet<EntityUid> Contents, HashSet<EntityUid> BypassChecks);
