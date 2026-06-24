using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Machines.Events;

[ByRefEvent]
public record struct MultipartMachineAssemblyStateChanged(EntityUid Entity, bool IsAssembled, EntityUid? User, Dictionary<Enum, EntityUid> PartsAdded, Dictionary<Enum, EntityUid> PartsRemoved);
