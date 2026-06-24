using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Materials;

[ByRefEvent]
public readonly record struct ConsumeStoredMaterialsEvent(Entity<MaterialStorageComponent> Entity, Dictionary<ProtoId<MaterialPrototype>, int> Materials, bool LocalOnly);
