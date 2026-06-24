using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.Surgery;

[ByRefEvent]
public record struct CMSurgeryStepEvent(EntityUid User, EntityUid Body, EntityUid Part, List<EntityUid> Tools);
