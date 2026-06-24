using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Prototypes;

[DataRecord]
public sealed record BodyPrototypeSlot(EntProtoId? Part, HashSet<string> Connections, Dictionary<string, string> Organs);
